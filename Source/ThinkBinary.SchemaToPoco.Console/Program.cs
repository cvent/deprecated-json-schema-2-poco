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
        /// Arguments from command line.
        /// </summary>
		private static OptionSet _options;

        /// <summary>
        /// Base directory.
        /// </summary>
        private static string _baseDir;

        /// <summary>
        /// Namespace directory. ie. com.cvent would become com\cvent
        /// </summary>
        private static string _nsDir;

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
				var settings = ConfigureCommandLineOptions(args);

                CreateDirectories(settings.Namespace);

				if (settings.ShowHelp)
				{
					var description = new StringBuilder("JSON schema to POCO\nhttps://github.com/sbl03/json-schema-to-poco\n\n");
					_options.WriteOptionDescriptions(new StringWriter(description));
					System.Console.WriteLine(description.ToString());

					return (int)ExitCodes.Ok;
				}

                // Load schemas given a json file or directory
                LoadSchemas(/*settings.Schema*/"C:\\Users\\SLiu\\Projects\\raml-to-dropwizard-csharp\\schema\\data-set.json");

                foreach(JsonSchemaWrapper s in _schemas.Values) {
                    if (s.ToCreate)
                    {
                        var jsonSchemaToCodeUnit = new JsonSchemaToCodeUnit(s, settings.Namespace);
                        var codeUnit = jsonSchemaToCodeUnit.Execute();
                        var csharpGenerator = new CodeCompileUnitToCSharp(codeUnit);
System.Console.WriteLine(csharpGenerator.Execute());
                        string saveLoc = _baseDir + @"\" + _nsDir + @"\" + s.Schema.Title + ".cs";
                        GenerateFile(csharpGenerator.Execute(), saveLoc);
                        System.Console.WriteLine("Wrote " + saveLoc);
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
				{"r=|rootclass=", "Name of the root class in the schema",rc => settings.RootClass = rc},
				{"s=|schema=", "File path to the schema file", s => settings.Schema = s},
				{"o=|output=", "Directory to save files", fn => settings.OutputFiledir = fn},
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
            _nsDir = ns.Replace('.', '\\');
            Directory.CreateDirectory(_baseDir + @"\" + _nsDir);
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
        private static JsonSchemaWrapper ResolveSchemas(string prevPath, TextReader reader)
        {
            string data = reader.ReadToEnd();
            var definition = new { csharpType = string.Empty, csharpInterfaces = new string[]{} };
            var deserialized = JsonConvert.DeserializeAnonymousType(data, definition);

            MatchCollection matches = Regex.Matches(data, @"\""\$ref\""\s*:\s*\""(.*.json)\""");
            foreach (Match match in matches)
            {
                string currPath = Path.GetDirectoryName(prevPath) + @"\" + match.Groups[1].Value;

                if (!_schemas.ContainsKey(currPath))
                {
                    using (TextReader reader2 = File.OpenText(currPath))
                    {
                        JsonSchemaWrapper schema = ResolveSchemas(currPath, reader2);
                        _schemas.Add(currPath, schema);
                    }
                }
            }

            JsonSchema parsed = JsonSchema.Parse(data, _resolver);
            parsed.Id = Path.GetFileName(prevPath);
            JsonSchemaWrapper toReturn = new JsonSchemaWrapper(parsed);

            // If csharpType is specified
            if (!String.IsNullOrEmpty(deserialized.csharpType))
            {
                int lastIndex = deserialized.csharpType.LastIndexOf('.');
                string cType = deserialized.csharpType.Substring(lastIndex == -1 ? 0 : lastIndex + 1);

                toReturn.CClass = deserialized.csharpType;
                toReturn.Schema.Title = cType;
                toReturn.ToCreate = false;
            }

            // If csharpInterfaces is specified
            if(deserialized.csharpInterfaces != null)
                foreach (string s in deserialized.csharpInterfaces)
                    if (!String.IsNullOrEmpty(s))
                        toReturn.Interfaces.Add(s);

            return toReturn;
        }
	}
}
