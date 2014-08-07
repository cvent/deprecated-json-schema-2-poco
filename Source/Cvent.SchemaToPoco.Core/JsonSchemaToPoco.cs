using System;
using System.CodeDom;
using System.Collections.Generic;
using Cvent.SchemaToPoco.Core.CodeToLanguage;
using Cvent.SchemaToPoco.Core.Types;
using Cvent.SchemaToPoco.Core.Util;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace Cvent.SchemaToPoco.Core
{
    /// <summary>
    ///     Main controller.
    /// </summary>
    public class JsonSchemaToPoco
    {
        /// <summary>
        ///     Logger.
        /// </summary>
        private Logger _log;

        /// <summary>
        ///     Location of the JSON schema.
        /// </summary>
        private readonly string _schemaLocation;

        /// <summary>
        ///     Name of the namespace.
        /// </summary>
        private readonly string _nsName;

        /// <summary>
        ///     Base directory to store generated files.
        /// </summary>
        private readonly string _baseDir;

        /// <summary>
        ///     Whether or not to store the files or print them out.
        /// </summary>
        private readonly bool _verbose;

        /// <summary>
        ///     Keeps track of the found schemas.
        /// </summary>
        private Dictionary<Uri, JsonSchemaWrapper> _schemas = new Dictionary<Uri, JsonSchemaWrapper>();

        /// <summary>
        ///     Initialize settings.
        /// </summary>
        /// <param name="schemaFileLocation">Location of the JSON schema file.</param>
        /// <param name="ns">Namespace to set the schema file.</param>
        /// <param name="outputDirectory">Base directory to save the generated files.</param>
        /// <param name="verbose">Whether or not to save the files or print them out.</param>
        public JsonSchemaToPoco(string schemaFileLocation, string ns = "generated", string outputDirectory = "", bool verbose = false)
        {
            _schemaLocation = schemaFileLocation;
            _nsName = ns;
            _baseDir = outputDirectory;
            _verbose = verbose;
        }

        /// <summary>
        ///     Main executor method.
        /// </summary>
        /// <returns>A status code.</returns>
        public int Execute()
        {
            try
            {
                ConfigureLogging();

                // Load schemas given a json file or directory
                LoadSchemas();

                // Generate code
                Generate();

                return (int)ExitCodes.Ok;
            }
            catch (Exception e)
            {
                _log.Fatal(e);
                return (int)ExitCodes.AbnormalExit;
            }
        }

        /// <summary>
        ///     Configuring the logger.
        /// </summary>
        private void ConfigureLogging()
        {
            var coloredConsoleTarget = new ColoredConsoleTarget
            {
                Layout = "${date:format=yyyy-MM-dd} ${time:format=hh:mm:ss} [${level}] ${message}"
            };
            var loggingRule = new LoggingRule("*", LogLevel.Debug, coloredConsoleTarget);
            LogManager.Configuration = new LoggingConfiguration();
            LogManager.Configuration.AddTarget("Console", coloredConsoleTarget);
            LogManager.Configuration.LoggingRules.Add(loggingRule);
            LogManager.ReconfigExistingLoggers();

            _log = LogManager.GetCurrentClassLogger();
        }

        /// <summary>
        ///     Load all the schemas from a file.
        /// </summary>
        private void LoadSchemas()
        {
            var resolver = new JsonSchemaResolverUtil(_nsName, !_verbose, _baseDir);
            _schemas = resolver.ResolveSchemas(_schemaLocation);
        }

        /// <summary>
        ///     Generate C# code.
        /// </summary>
        private void Generate()
        {
            var generatedCode = GenerateHelper();

            // Create directory to generate files
            if (!_verbose)
            {
                IoUtils.CreateDirectoryFromNamespace(_baseDir, _nsName);
            }

            foreach (var entry in generatedCode)
            {
                if (!_verbose)
                {
                    string saveLoc = _baseDir + @"\" + entry.Key.Namespace.Replace('.', '\\') + @"\" + entry.Key.Schema.Title +
                                     ".cs";
                    IoUtils.GenerateFile(entry.Value, saveLoc);
                    Console.WriteLine("Wrote " + saveLoc);
                }
                else
                {
                    Console.WriteLine(entry.Value);
                }
            }
        }

        private Dictionary<JsonSchemaWrapper, string> GenerateHelper()
        {
            var generatedCode = new Dictionary<JsonSchemaWrapper, string>();

            foreach (JsonSchemaWrapper s in _schemas.Values)
            {
                if (s.ToCreate)
                {
                    var jsonSchemaToCodeUnit = new JsonSchemaToCodeUnit(s, s.Namespace);
                    CodeCompileUnit codeUnit = jsonSchemaToCodeUnit.Execute();
                    var csharpGenerator = new CodeCompileUnitToCSharp(codeUnit);

                    generatedCode.Add(s, csharpGenerator.Execute());
                }
            }

            return generatedCode;
        }

        /// <summary>
        ///     Static method to return a Dictionary of JsonSchemaWrapper and its corresponding C# generated code.
        /// </summary>
        /// <param name="schemaLoc">Location of JSON schema.</param>
        /// <returns>A mapping of all the JSON schemas and the generated code.</returns>
        public static Dictionary<JsonSchemaWrapper, string> GenerateFromFile(string schemaLoc)
        {
            var controller = new JsonSchemaToPoco(schemaLoc);
            controller.LoadSchemas();
            return controller.GenerateHelper();
        }

        /// <summary>
        ///     Static method to return generated code for a single JSON schema with no references.
        /// </summary>
        /// <param name="schema">JSON schema as a string.</param>
        /// <returns>The generated code.</returns>
        public static string Generate(string schema)
        {
            var jsonSchemaToCodeUnit = new JsonSchemaToCodeUnit(JsonSchemaResolverUtil.ConvertToWrapper(schema), "generated");
            CodeCompileUnit codeUnit = jsonSchemaToCodeUnit.Execute();
            var csharpGenerator = new CodeCompileUnitToCSharp(codeUnit);
            return csharpGenerator.Execute();
        }
    }
}
