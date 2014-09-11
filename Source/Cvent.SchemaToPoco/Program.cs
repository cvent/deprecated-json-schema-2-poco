using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Cvent.SchemaToPoco.Json.Class;
using Microsoft.CSharp;
using NDesk.Options;
using Cvent.SchemaToPoco.Json.Schema;

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
            var settings = ParseCommandLineParameters(args);
            var schema = JsonSchemaParser.Parse(new Uri(settings.JsonSchemaFileLocation));
            var jsonClass = JsonClass.CreateFromSchema(schema, settings.Namespace);
            var uniqueClassesById = new Dictionary<string, JsonClass>();
            var uniqueClassesByName = new Dictionary<string, JsonClass>();
            GetUniqueJsonClasses(jsonClass, uniqueClassesById, uniqueClassesByName);

            foreach (var jClass in uniqueClassesById.Values)
            {
                File.WriteAllText(Path.Combine(settings.OutputDirectory, jClass.Name + ".cs"), GetCSharpClassText(jClass.GetClassRepresentation()));
            }

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

        private string GetCSharpClassText(CodeCompileUnit classRepresentation)
        {
            var stringBuilder = new StringBuilder();
            var stringWriter = new StringWriter(stringBuilder);
            var writer = new IndentedTextWriter(stringWriter, "    ");
            var codeProvider = new CSharpCodeProvider();
            codeProvider.GenerateCodeFromCompileUnit(classRepresentation, writer, new CodeGeneratorOptions
            {
                BlankLinesBetweenMembers = true,
                VerbatimOrder = false,
                BracingStyle = "C"
            });

            string output = stringBuilder.ToString();
            return output;
        }

        private void GetUniqueJsonClasses(JsonClass jsonClass, Dictionary<string, JsonClass> uniqueClassesById, Dictionary<string, JsonClass> uniqueClassesByName)
        {
            foreach (var dependentClass in jsonClass.DependentClasses)
            {
                GetUniqueJsonClasses(dependentClass, uniqueClassesById, uniqueClassesByName);
            }

            if (uniqueClassesById.ContainsKey(jsonClass.Id))
            {
                jsonClass.Name = uniqueClassesById[jsonClass.Id].Id;
                return;
            }

            while (uniqueClassesByName.ContainsKey(jsonClass.Name))
            {
                jsonClass.Name = jsonClass.Name + "_";
            }

            uniqueClassesById.Add(jsonClass.Id, jsonClass);
            uniqueClassesByName.Add(jsonClass.Name, jsonClass);
        }
    }
}
