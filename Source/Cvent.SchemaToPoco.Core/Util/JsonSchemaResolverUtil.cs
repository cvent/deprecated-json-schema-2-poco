using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using Cvent.SchemaToPoco.Core.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;

namespace Cvent.SchemaToPoco.Core.Util
{
    /// <summary>
    ///     Resolve JSON schema $ref attributes
    /// </summary>
    public class JsonSchemaResolverUtil
    {
        /// <summary>
        ///     The absolute path to the base generated directory.
        /// </summary>
        private readonly string _baseDir;

        /// <summary>
        ///     Whether or not to create directories.
        /// </summary>
        private readonly bool _createDirs;

        /// <summary>
        ///     The namespace.
        /// </summary>
        private readonly string _ns;

        /// <summary>
        ///     Resolving schemas so that they can be parsed.
        /// </summary>
        private readonly JsonSchemaResolver _resolver = new JsonSchemaResolver();

        /// <summary>
        ///     Keeps track of the found schemas.
        /// </summary>
        private readonly Dictionary<Uri, JsonSchemaWrapper> _schemas = new Dictionary<Uri, JsonSchemaWrapper>();

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="ns">settings.Namespace</param>
        /// <param name="createDirs">settings.Verbose</param>
        /// <param name="baseDir">The base directory of the generated files.</param>
        public JsonSchemaResolverUtil(string ns, bool createDirs, string baseDir)
        {
            _ns = ns;
            _createDirs = createDirs;
            _baseDir = baseDir;
        }

        /// <summary>
        ///     Resolve all schemas.
        /// </summary>
        /// <param name="filePath">Path to the current file.</param>
        /// <returns>A Dictionary containing all resolved schemas.</returns>
        public Dictionary<Uri, JsonSchemaWrapper> ResolveSchemas(string filePath)
        {
            var uri = GetAbsoluteUri(new Uri(Directory.GetCurrentDirectory()), new Uri(filePath, UriKind.RelativeOrAbsolute), false);
            JsonSchemaWrapper schema = ResolveSchemaHelper(uri, uri);
            _schemas.Add(uri, schema);
            return _schemas;
        }

        /// <summary>
        ///     Recursively resolve all schemas. All references to external schemas must have .json extension.
        /// </summary>
        /// <param name="parent">Path to the parent file.</param>
        /// <param name="current">Path to the current file.</param>
        /// <returns>An extended wrapper for the JsonSchema.</returns>
        /// TODO check if parent is needed - right now it assumes parent for all children
        private JsonSchemaWrapper ResolveSchemaHelper(Uri parent, Uri current)
        {
            var uri = GetAbsoluteUri(parent, current, true);
            var data = ReadFromPath(uri);
            var definition = new {csharpType = string.Empty, csharpInterfaces = new string[] {}};
            var deserialized = JsonConvert.DeserializeAnonymousType(data, definition);
            var dependencies = new List<JsonSchemaWrapper>();

            MatchCollection matches = Regex.Matches(data, @"\""\$ref\""\s*:\s*\""(.*.json)\""");
            foreach (Match match in matches)
            {
                // Get the full path to the file, and change the reference to match
                var currPath = new Uri(match.Groups[1].Value, UriKind.RelativeOrAbsolute);
                var currUri = GetAbsoluteUri(parent, currPath, true);

                JsonSchemaWrapper schema;

                if (!_schemas.ContainsKey(currUri))
                {
                    schema = ResolveSchemaHelper(parent, currUri);
                    _schemas.Add(currUri, schema);
                }
                else
                {
                    schema = _schemas[currUri];
                }

                // Add schema to dependencies
                dependencies.Add(schema);
            }

            // Set up schema and wrapper to return
            JsonSchema parsed = JsonSchema.Parse(StandardizeReferences(parent, data), _resolver);
            parsed.Id = uri.ToString();
            var toReturn = new JsonSchemaWrapper(parsed) {Namespace = _ns, Dependencies = dependencies};

            // If csharpType is specified
            if (!String.IsNullOrEmpty(deserialized.csharpType))
            {
                // Create directories and set namespace
                int lastIndex = deserialized.csharpType.LastIndexOf('.');
                string cType = deserialized.csharpType.Substring(lastIndex == -1 ? 0 : lastIndex + 1);

                toReturn.Namespace = deserialized.csharpType.Substring(0, lastIndex);
                toReturn.Schema.Title = cType;

                if (_createDirs)
                {
                    IoUtils.CreateDirectoryFromNamespace(_baseDir, toReturn.Namespace);
                }
            }

            // If csharpInterfaces is specified
            if (deserialized.csharpInterfaces != null)
            {
                foreach (string s in deserialized.csharpInterfaces)
                {
                    // Try to resolve the type
                    Type t = Type.GetType(s, false);

                    // If type cannot be found, create a new type
                    if (t == null)
                    {
                        var builder = new TypeBuilderHelper(toReturn.Namespace);
                        t = builder.GetCustomType(s, !s.Contains("."));
                    }

                    toReturn.Interfaces.Add(t);
                }
            }

            return toReturn;
        }

        /// <summary>
        ///     Read data from a Uri path.
        /// </summary>
        /// <param name="path">The path to read from.</param>
        /// <returns>The string contents of that path.</returns>
        private string ReadFromPath(Uri path)
        {
            // file://, relative, and absolute paths
            if(path.IsFile)
            {
                return File.ReadAllText(path.AbsolutePath);
            }

            // http://, https://
            if (path.ToString().StartsWith("http"))
            {
                using (var client = new WebClient())
                {
                    return client.DownloadString(path);
                }
            }

            throw new ArgumentException("Cannot read from the path: " + path);
        }

        /// <summary>
        ///     Get an absolute Uri given a relative Uri. If the relative Uri is absolute, then return that.
        /// </summary>
        /// <param name="baseUri">The base, or parent Uri.</param>
        /// <param name="relativeUri">The relative Uri to combine.</param>
        /// <param name="preserveSlashes">Leave the baseUri and relativeUri slashes untouched. If false, the relativeUri is guaranteed to be relative to the baseUri.</param>
        /// <returns>An absolute Uri.</returns>
        private Uri GetAbsoluteUri(Uri baseUri, Uri relativeUri, bool preserveSlashes)
        {
            if (relativeUri.IsAbsoluteUri)
            {
                return relativeUri;
            }

            if (!preserveSlashes)
            {
                if (!baseUri.ToString().EndsWith(@"\"))
                {
                    baseUri = new Uri(baseUri + @"\");
                }
            }

            return new Uri(baseUri, relativeUri);
        }

        /// <summary>
        ///     Convert all $ref attributes to absolute paths.
        /// </summary>
        /// <param name="parentUri">The parent Uri to resolve relative paths against.</param>
        /// <param name="data">The JSON schema.</param>
        /// <returns>The JSON schema with standardized $ref attributes.</returns>
        private string StandardizeReferences(Uri parentUri, string data)
        {
            var lines = new List<string>(data.Split('\n'));
            var pattern = new Regex(@"(\""\$ref\""\s*:\s*\"")(.*.json)(\"")");

            for (int i = lines.Count - 1; i >= 0; i--)
            {
                if (pattern.IsMatch(lines[i]))
                {
                    var matched = pattern.Match(lines[i]);
                    var matchedPath = matched.Groups[2].Value;
                    var absPath = GetAbsoluteUri(parentUri, new Uri(matchedPath, UriKind.RelativeOrAbsolute), true);
                    lines[i] = matched.Groups[1].Value + absPath + matched.Groups[3].Value + ",";
                }
            }

            return string.Join("\n", lines);
        }

        /// <summary>
        ///     Convert a schema with no references to a JsonSchemaWrapper.
        /// </summary>
        /// <param name="data">The JSON schema.</param>
        public static JsonSchemaWrapper ConvertToWrapper(string data)
        {
            return new JsonSchemaWrapper(JsonSchema.Parse(data));
        }
    }
}
