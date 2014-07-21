using System.IO;
namespace ThinkBinary.SchemaToPoco.Console
{
    /// <summary>
    /// Command line settings.
    /// </summary>
	internal class CommandLineSettings
	{
		public CommandLineSettings()
		{
			// Meaningful defaults
			Namespace = "generated";
			OutputFiledir = Directory.GetCurrentDirectory();
			RootClass = "RootClass";
			ShowHelp = false;
		}

        /// <summary>
        /// Namespace for all of the generated schemas.
        /// </summary>
		public string Namespace { get; set; }

        /// <summary>
        /// The root class name. Not used.
        /// </summary>
		public string RootClass { get; set; }

        /// <summary>
        /// The schema file path.
        /// </summary>
		public string Schema { get; set; }

        /// <summary>
        /// Output file directory.
        /// </summary>
		public string OutputFiledir { get; set; }

        /// <summary>
        /// Show command line help.
        /// </summary>
		public bool ShowHelp { get; set; }
	}
}
