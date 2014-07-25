using System.IO;

namespace Cvent.SchemaToPoco.Core.Util
{
    /// <summary>
    ///     General IO Utilities.
    /// </summary>
    public static class IoUtils
    {
        /// <summary>
        ///     Generate all the directories to the path if they do not exist.
        /// </summary>
        /// <param name="baseDir">Absolute path to the base generated directory.</param>
        /// <param name="ns">Namespace ie. com.cvent</param>
        public static void CreateDirectoryFromNamespace(string baseDir, string ns)
        {
            string nsDir = ns.Replace('.', '\\');
            Directory.CreateDirectory(baseDir + @"\" + nsDir);
        }

        /// <summary>
        ///     Write to a file.
        /// </summary>
        /// <param name="data">Data to write to the file.</param>
        /// <param name="path">Path to the file.</param>
        public static void GenerateFile(string data, string path)
        {
            var sw = new StreamWriter(File.Open(path, FileMode.Create));
            sw.Write(data);
            sw.Close();
        }
    }
}
