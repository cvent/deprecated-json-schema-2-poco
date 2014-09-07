using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NDesk.Options;

namespace Cvent.SchemaToPoco
{
    /// <summary>
    ///     Main entry point for schema to POCO generation.
    /// </summary>
    public class Program
    {
        /// <summary>
        ///     Main method.
        /// </summary>
        /// <param name="args">Arguments from command line.</param>
        /// <returns>An exit code.</returns>
        public static void Main(string[] args)
        {
            new Program().Execute(args);
        }

        private void Execute(IEnumerable<string> args)
        {
            CommandLineSettings settings = ParseCommandLineParameters(args);
            var schema = new Cvent.SchemaToPoco.JsonSchema.JsonSchema();
            schema.ParseFromFile(settings.JsonSchemaFileLocation);
            Environment.Exit((int)ExitCodes.Success);
        }

        private CommandLineSettings ParseCommandLineParameters(IEnumerable<string> args)
        {
            var settings = new CommandLineSettings();

            var options = new OptionSet
            {
                {"n=|namespace=", "Namespace containing all of the generated classes", ns => settings.Namespace = ns},
                {"s=|schema=", "File path to the schema file", s => settings.JsonSchemaFileLocation = s},
                {"o=|output=", "Directory to save files", fn => settings.OutputDirectory = fn},
                {
                    "v|verbose", "Print out files in console without generating",
                    v => settings.Verbose = !string.IsNullOrWhiteSpace(v)
                },
                {"?|help", "Show this help message", h => settings.ShowHelp = !string.IsNullOrWhiteSpace(h)}
            };

            try
            {
                options.Parse(args);
            }
            catch (OptionException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Try --help for more information.");
                Environment.Exit((int)ExitCodes.InvalidInputParameters);
            }

            if (settings.ShowHelp)
            {
                var description =
                    new StringBuilder("JSON schema to POCO\nhttps://github.com/cvent/json-schema-2-poco\n\n");
                options.WriteOptionDescriptions(new StringWriter(description));
                Console.WriteLine(description.ToString());
                Environment.Exit((int)ExitCodes.Success);
            }

            return settings;
        }
    }
}
