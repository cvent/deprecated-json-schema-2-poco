using System.IO;

namespace Cvent.SchemaToPoco.Console
{
    /// <summary>
    ///     Command line settings.
    /// </summary>
    internal class CommandLineSettings
    {
        public CommandLineSettings()
        {
            // Meaningful defaults
            Namespace = "generated";
            OutputFiledir = Directory.GetCurrentDirectory();
            ShowHelp = false;
            Verbose = false;
        }

        /// <summary>
        ///     Namespace for all of the generated schemas.
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        ///     The schema file path.
        /// </summary>
        public string Schema { get; set; }

        /// <summary>
        ///     Output file directory.
        /// </summary>
        public string OutputFiledir { get; set; }

        /// <summary>
        ///     Show command line help.
        /// </summary>
        public bool ShowHelp { get; set; }

        /// <summary>
        ///     Print out generated code without saving files.
        /// </summary>
        public bool Verbose { get; set; }
    }
}
