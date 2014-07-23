using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NDesk.Options;
using NLog;
using NLog.Config;
using NLog.Targets;
using ThinkBinary.SchemaToPoco.Core;
using ThinkBinary.SchemaToPoco.Core.CodeToLanguage;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json;
using System.Web;
using System.Text.RegularExpressions;
using ThinkBinary.SchemaToPoco.Core.Types;

namespace ThinkBinary.SchemaToPoco.Console
{
    /// <summary>
    /// Main executor class.
    /// </summary>
	class Program
	{
        /// <summary>
        /// Log information.
        /// </summary>
		private static Logger _log;

        /// <summary>
        /// Arguments from command line, raw format.
        /// </summary>
		private static OptionSet _options;

        /// <summary>
        /// Arguments from command line.
        /// </summary>
        private static CommandLineSettings _settings;

        /// <summary>
        /// Base directory.
        /// </summary>
        private static string _baseDir;

        /// <summary>
        /// Keeps track of the found schemas.
        /// </summary>
        private static Dictionary<string, JsonSchemaWrapper> _schemas = new Dictionary<string, JsonSchemaWrapper>();

        /// <summary>
        /// Resolving schemas so that they can be parsed.
        /// </summary>
        private static JsonSchemaResolver _resolver = new JsonSchemaResolver();

		public static Int32 Main(string[] args)
		{
			try
			{
				ConfigureLogging();

                // Initialize _settings, _baseDir
				_settings = ConfigureCommandLineOptions(args);

                CreateDirectories(_settings.Namespace);

				if (_settings.ShowHelp)
				{
                    var description = new StringBuilder("JSON schema to POCO\nhttps://github.com/cvent/json-schema-2-poco\n\n");
					_options.WriteOptionDescriptions(new StringWriter(description));
					System.Console.WriteLine(description.ToString());

					return (int)ExitCodes.Ok;
				}

                // Load schemas given a json file or directory
                LoadSchemas(_settings.Schema);

                foreach(JsonSchemaWrapper s in _schemas.Values) {
                    if (s.ToCreate)
                    {
                        var jsonSchemaToCodeUnit = new JsonSchemaToCodeUnit(s, s.Namespace);
                        var codeUnit = jsonSchemaToCodeUnit.Execute();
                        var csharpGenerator = new CodeCompileUnitToCSharp(codeUnit);

                        if (_settings.Verbose)
                            System.Console.WriteLine(csharpGenerator.Execute());
                        else
                        {
                            string saveLoc = _baseDir + @"\" + s.Namespace.Replace('.', '\\') + @"\" + s.Schema.Title + ".cs";
                            GenerateFile(csharpGenerator.Execute(), saveLoc);
                            System.Console.WriteLine("Wrote " + saveLoc);
                        }
                    }
                }

				return (int)ExitCodes.Ok;
			}
			catch (Exception e)
			{
				_log.Fatal(e);
				return (int)ExitCodes.AbnormalExit;
			}
		}

        /// <summary>
        /// Configuring the logger.
        /// </summary>
		private static void ConfigureLogging()
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
        /// Configure command line options.
        /// </summary>
        /// <param name="arguements">Arguments from the command line.</param>
        /// <returns>The command line options.</returns>
		private static CommandLineSettings ConfigureCommandLineOptions(string[] arguements)
		{
			var settings = new CommandLineSettings();

			_options = new OptionSet
			{
				{"n=|namespace=","Namespace contaning all of the generated classes", ns => settings.Namespace = ns},
				{"s=|schema=", "File path to the schema file", s => settings.Schema = s},
				{"o=|output=", "Directory to save files", fn => settings.OutputFiledir = fn},
                {"v|verbose","Print out files in console without generating", v => settings.Verbose = !string.IsNullOrWhiteSpace(v)},
				{"?|help","Show this help message", h => settings.ShowHelp = !string.IsNullOrWhiteSpace(h)}
			};

			_options.Parse(arguements);
            _baseDir = settings.OutputFiledir;

			return settings;
		}

        /// <summary>
        /// Write to a file.
        /// </summary>
        /// <param name="data">Data to write to the file.</param>
        /// <param name="path">Path to the file.</param>
        private static void GenerateFile(string data, string path)
        {
            StreamWriter sw = new StreamWriter(File.Open(path, FileMode.Create));
            sw.Write(data);
            sw.Close();
        }

        /// <summary>
        /// Generate all the directories to the path if they do not exist.
        /// </summary>
        /// <param name="ns">Namespace ie. com.cvent</param>
        private static void CreateDirectories(string ns)
        {
            var nsDir = ns.Replace('.', '\\');
            Directory.CreateDirectory(_baseDir + @"\" + nsDir);
        }

        /// <summary>
        /// Load all the schemas from a file.
        /// </summary>
        /// <param name="file">File path.</param>
        private static void LoadSchemas(string file)
        {
            using (TextReader reader = File.OpenText(file))
            {
                JsonSchemaWrapper schema = ResolveSchemas(file, reader);
                _schemas.Add(file, schema);
            }
        }

        /// <summary>
        /// Recursively resolve all schemas.
        /// </summary>
        /// <param name="prevPath">Path to the current file.</param>
        /// <param name="reader">TextReader for the file.</param>
        /// <returns>An extended wrapper for the JsonSchema.</returns>
        private static JsonSchemaWrapper ResolveSchemas(string filePath, TextReader reader)
        {
            string data = reader.ReadToEnd();
            var definition = new { csharpType = string.Empty, csharpInterfaces = new string[]{} };
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
                        schema = ResolveSchemas(currPath, reader2);
                        _schemas.Add(currPath, schema);
                    }
                }
                else
                    schema = _schemas[currPath];

                // Add schema to dependencies
                dependencies.Add(schema);
            }

            // Set up schema and wrapper to return
            JsonSchema parsed = JsonSchema.Parse(data, _resolver);
            parsed.Id = Path.GetFileName(filePath);
            JsonSchemaWrapper toReturn = new JsonSchemaWrapper(parsed);
            toReturn.Namespace = _settings.Namespace;
            toReturn.Dependencies = dependencies;

            // If csharpType is specified
            if (!String.IsNullOrEmpty(deserialized.csharpType))
            {
                // Create directories and set namespace
                int lastIndex = deserialized.csharpType.LastIndexOf('.');
                string cType = deserialized.csharpType.Substring(lastIndex == -1 ? 0 : lastIndex + 1);

                toReturn.Namespace = deserialized.csharpType.Substring(0, lastIndex);
                toReturn.Schema.Title = cType;

                if(!_settings.Verbose)
                    CreateDirectories(toReturn.Namespace);
            }

            // If csharpInterfaces is specified
            if(deserialized.csharpInterfaces != null)
                foreach (string s in deserialized.csharpInterfaces)
                {
                    // Try to resolve the type
                    Type t = Type.GetType(s, false);

                    // If type cannot be found, create a new type
                    if (t == null)
                    {
                        TypeBuilderHelper builder = new TypeBuilderHelper(toReturn.Namespace);
                        t = builder.GetCustomType(s, !s.Contains("."));
                    }

                    toReturn.Interfaces.Add(t);
                }

            return toReturn;
        }
	}
}
