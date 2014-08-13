using System.IO;
using Cvent.SchemaToPoco.Core;

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
            Config = new JsonSchemaToPocoConfiguration();
            ShowHelp = false;
        }

        /// <summary>
        ///     Configuration for JsonSchemaToPoco.
        /// </summary>
        public JsonSchemaToPocoConfiguration Config { get; set; }

        /// <summary>
        ///     Show command line help.
        /// </summary>
        public bool ShowHelp { get; set; }
    }
}
