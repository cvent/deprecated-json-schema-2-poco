using System;
using System.Collections.Generic;
using System.IO;
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
        private readonly Dictionary<string, JsonSchemaWrapper> _schemas = new Dictionary<string, JsonSchemaWrapper>();

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
        /// <param name="data">String data for the file.</param>
        /// <returns>A Dictionary containing all resolved schemas.</returns>
        public Dictionary<string, JsonSchemaWrapper> ResolveSchemas(string filePath, string data)
        {
            JsonSchemaWrapper schema = ResolveSchemaHelper(filePath, data);
            _schemas.Add(filePath, schema);
            return _schemas;
        }

        /// <summary>
        ///     Recursively resolve all schemas.
        /// </summary>
        /// <param name="filePath">Path to the current file.</param>
        /// <param name="data">String data for the file.</param>
        /// <returns>An extended wrapper for the JsonSchema.</returns>
        private JsonSchemaWrapper ResolveSchemaHelper(string filePath, string data)
        {
            var definition = new {csharpType = string.Empty, csharpInterfaces = new string[] {}};
            var deserialized = JsonConvert.DeserializeAnonymousType(data, definition);
            var dependencies = new List<JsonSchemaWrapper>();

            MatchCollection matches = Regex.Matches(data, @"\""\$ref\""\s*:\s*\""(.*.json)\""");
            foreach (Match match in matches)
            {
                // Get the full path to the file
                string currPath = Path.GetDirectoryName(filePath) + @"\" + match.Groups[1].Value;
                JsonSchemaWrapper schema;

                if (!_schemas.ContainsKey(currPath))
                {
                    using (TextReader reader2 = File.OpenText(currPath))
                    {
                        schema = ResolveSchemaHelper(currPath, reader2.ReadToEnd());
                        _schemas.Add(currPath, schema);
                    }
                }
                else
                {
                    schema = _schemas[currPath];
                }

                // Add schema to dependencies
                dependencies.Add(schema);
            }

            // Set up schema and wrapper to return
            JsonSchema parsed = JsonSchema.Parse(data, _resolver);
            parsed.Id = Path.GetFileName(filePath);
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
        ///     Convert a schema with no references to a JsonSchemaWrapper.
        /// </summary>
        /// <param name="data">The JSON schema.</param>
        public static JsonSchemaWrapper ConvertToWrapper(string data)
        {
            return new JsonSchemaWrapper(JsonSchema.Parse(data));
        }
    }
}
