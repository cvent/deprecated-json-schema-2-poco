using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Mono.Options;
using NLog;
using NLog.Config;
using NLog.Targets;
using ThinkBinary.SchemaToPoco.Core;
using ThinkBinary.SchemaToPoco.Core.CodeToLanguage;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json;

namespace ThinkBinary.SchemaToPoco.Console
{
	class Program
	{
		private static Logger _log;
		private static OptionSet _options;

		static Int32 Main(string[] args)
		{
			try
			{
				ConfigureLogging();
				var settings = ConfigureCommandLineOptions(args);
				if (settings.ShowHelp)
				{
					var description = new StringBuilder("JSON schema to POCO\nhttps://github.com/codedemonuk/json-schema-to-poco\n\n");
					_options.WriteOptionDescriptions(new StringWriter(description));
					System.Console.WriteLine(description.ToString());

					return (int)ExitCodes.Ok;
				}

                var jsonSchemaToCodeUnit = new JsonSchemaToCodeUnit(/*settings.Schema*/"C:\\Users\\SLiu\\Projects\\raml-to-dropwizard-csharp\\schema\\data-set.json", settings.Namespace);
				var codeUnit = jsonSchemaToCodeUnit.Execute();
				var csharpGenerator = new CodeCompileUnitToCSharp(codeUnit);
				System.Console.WriteLine(csharpGenerator.Execute());

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

		private static CommandLineSettings ConfigureCommandLineOptions(IEnumerable<string> arguements)
		{
			var settings = new CommandLineSettings();

			_options = new OptionSet
			{
				{"n|namespace","Namespace contaning all of the generated classes", ns => settings.Namespace = ns},
				{"r|rootclass", "Name of the root class in the schema",rc => settings.RootClass = rc},
				{"s|schema", "File path to the schema file", s => settings.Schema = s},
				{"o|output", "Generated output file", fn => settings.OutputFilename = fn},
				{"?|help","Show this help message", h => settings.ShowHelp = !string.IsNullOrWhiteSpace(h)}
			};

			_options.Parse(arguements);

			return settings;
		}
	}
}
