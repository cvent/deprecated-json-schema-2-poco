using System.IO;
namespace ThinkBinary.SchemaToPoco.Console
{
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

		public string Namespace { get; set; }
		public string RootClass { get; set; }
		public string Schema { get; set; }
		public string OutputFiledir { get; set; }

		public bool ShowHelp { get; set; }
	}
}
