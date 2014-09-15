using System;
using System.Collections.Generic;

namespace Cvent.JsonSchema2Poco
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
            var commandLineSettings = CommandLineSettings.ParseCommandLineParameters(args);
            JsonSchema2Poco.Generate(commandLineSettings);
            Environment.Exit((int)ExitCodes.Success);
        }
    }
}
