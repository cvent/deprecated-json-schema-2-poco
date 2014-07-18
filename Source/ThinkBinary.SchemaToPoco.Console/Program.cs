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

namespace ThinkBinary.SchemaToPoco.Console
{
	class Program
	{
		private static Logger _log;
		private static OptionSet _options;
        private static string _baseDir;
        private static string _nsDir;
        private static Dictionary<string, JsonSchema> _schemas = new Dictionary<string, JsonSchema>();
        private static JsonSchemaResolver _resolver = new JsonSchemaResolver();

		static Int32 Main(string[] args)
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

                foreach(JsonSchema s in _schemas.Values) {
                    if (s != null)
                    {
                        var jsonSchemaToCodeUnit = new JsonSchemaToCodeUnit(s, settings.Namespace);
                        var codeUnit = jsonSchemaToCodeUnit.Execute();
                        var csharpGenerator = new CodeCompileUnitToCSharp(codeUnit);
System.Console.WriteLine(csharpGenerator.Execute());
                        string saveLoc = _baseDir + @"\" + _nsDir + @"\" + s.Title + ".cs";
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

        private static void GenerateFile(string data, string path)
        {
            StreamWriter sw = new StreamWriter(File.Open(path, FileMode.Create));
            sw.Write(data);
            sw.Close();
        }

        private static void CreateDirectories(string ns)
        {
            _nsDir = ns.Replace('.', '\\');
            Directory.CreateDirectory(_baseDir + @"\" + _nsDir);
        }

        private static void LoadSchemas(string file)
        {
            using (TextReader reader = File.OpenText(file))
            {
                JsonSchema schema = ResolveSchemas(file, reader);
                _schemas.Add(file, schema);
            }
        }

        private static JsonSchema ResolveSchemas(string prevPath, TextReader reader)
        {
            string data = reader.ReadToEnd();

            MatchCollection matches = Regex.Matches(data, @"\""\$ref\""\s*:\s*\""(.*.json)\""");
            foreach (Match match in matches)
            {
                string currPath = Path.GetDirectoryName(prevPath) + @"\" + match.Groups[1].Value;

                if (!_schemas.ContainsKey(currPath))
                {
                    using (TextReader reader2 = File.OpenText(currPath))
                    {
                        JsonSchema schema = ResolveSchemas(currPath, reader2);
                        _schemas.Add(currPath, schema);
                    }
                }
            }

            JsonSchema ret = JsonSchema.Parse(data, _resolver);
            ret.Id = Path.GetFileName(prevPath);

            // If csharpType is specified, return null
            Match match2 = Regex.Match(data, @"\""csharpType\""\s*:\s*\""(.*)\""");

            if (match2 != null && match2.Groups[1].Length > 0)
            {
                int lastIndex = match2.Groups[1].Value.LastIndexOf('.');
                string cType = match2.Groups[1].Value.Substring(lastIndex == -1 ? 0 : lastIndex + 1);

                ret.Title = cType;
                return null;
            }

            return ret;
        }
	}
}
