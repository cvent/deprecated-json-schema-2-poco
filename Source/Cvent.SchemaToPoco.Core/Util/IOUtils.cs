using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cvent.SchemaToPoco.Core.Util
{
    /// <summary>
    /// General IO Utilities.
    /// </summary>
    public class IOUtils
    {
        /// <summary>
        /// Generate all the directories to the path if they do not exist.
        /// </summary>
        /// <param name="baseDir">Absolute path to the base generated directory.</param>
        /// <param name="ns">Namespace ie. com.cvent</param>
        public static void CreateDirectoryFromNamespace(string baseDir, string ns)
        {
            var nsDir = ns.Replace('.', '\\');
            Directory.CreateDirectory(baseDir + @"\" + nsDir);
        }
    }
}
