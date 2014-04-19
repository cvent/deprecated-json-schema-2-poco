namespace ThinkBinary.SchemaToPoco.Console
{
	internal class CommandLineSettings
	{
		public CommandLineSettings()
		{
			// Meaningful defaults
			Namespace = "";
			OutputFilename = "output.cs";
			RootClass = "RootClass";
			ShowHelp = false;
		}

		public string Namespace { get; set; }
		public string RootClass { get; set; }
		public string Schema { get; set; }
		public string OutputFilename { get; set; }

		public bool ShowHelp { get; set; }
	}
}
