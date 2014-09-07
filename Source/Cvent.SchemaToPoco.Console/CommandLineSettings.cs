using System.IO;

namespace Cvent.SchemaToPoco
{
    /// <summary>
    ///     Command line settings.
    /// </summary>
    internal class CommandLineSettings
    {
        /// <summary>
        ///     Location of root JSON schema file.
        /// </summary>
        public string JsonSchemaFileLocation { get; set; }

        /// <summary>
        ///     Namespace to set for generated files.
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        ///     Directory to output generated files.
        /// </summary>
        public string OutputDirectory { get; set; }

        /// <summary>
        ///     Whether or not to just print out files in console without generating.
        /// </summary>
        public bool Verbose { get; set; }

        /// <summary>
        ///     Show command line help.
        /// </summary>
        public bool ShowHelp { get; set; }

        public CommandLineSettings()
        {
            Namespace = "generated";
            OutputDirectory = Directory.GetCurrentDirectory();
            Verbose = false;
            ShowHelp = false;
        }
    }
}
