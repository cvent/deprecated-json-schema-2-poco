using System;
using System.IO;
using System.Net;

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

        /// <summary>
        ///     Read data from a Uri path.
        /// </summary>
        /// <param name="path">The path to read from.</param>
        /// <returns>The string contents of that path.</returns>
        public static string ReadFromPath(Uri path)
        {
            // file://, relative, and absolute paths
            if (path.IsFile)
            {
                return File.ReadAllText(path.AbsolutePath);
            }

            // http://, https://
            if (path.ToString().StartsWith("http"))
            {
                using (var client = new WebClient())
                {
                    return client.DownloadString(path);
                }
            }

            throw new ArgumentException("Cannot read from the path: " + path);
        }

        /// <summary>
        ///     Get an absolute Uri given a relative Uri. If the relative Uri is absolute, then return that.
        /// </summary>
        /// <param name="baseUri">The base, or parent Uri.</param>
        /// <param name="relativeUri">The relative Uri to combine.</param>
        /// <param name="preserveSlashes">Leave the baseUri and relativeUri slashes untouched. If false, the relativeUri is guaranteed to be relative to the baseUri.</param>
        /// <returns>An absolute Uri.</returns>
        public static Uri GetAbsoluteUri(Uri baseUri, Uri relativeUri, bool preserveSlashes = true)
        {
            if (relativeUri.IsAbsoluteUri)
            {
                return relativeUri;
            }

            if (!preserveSlashes)
            {
                if (!baseUri.ToString().EndsWith(@"\"))
                {
                    baseUri = new Uri(baseUri + @"\");
                }
            }

            return new Uri(baseUri, relativeUri);
        }
    }
}
