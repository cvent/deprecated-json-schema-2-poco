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
using ThinkBinary.SchemaToPoco.Core.Util;

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
                JsonSchemaResolverUtil resolver = new JsonSchemaResolverUtil(_settings.Namespace, !_settings.Verbose);
                _schemas = resolver.ResolveSchemas(file, reader.ReadToEnd());
            }
        }
	}
}
