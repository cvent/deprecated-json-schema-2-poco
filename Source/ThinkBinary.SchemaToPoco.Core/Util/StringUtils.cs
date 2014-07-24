using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkBinary.SchemaToPoco.Util
{
    /// <summary>
    /// Common string utilities.
    /// </summary>
    public class StringUtils
    {
        /// <summary>
        /// Capitalize the first letter in a string.
        /// </summary>
        /// <param name="s">The string.</param>
        /// <returns>A capitalized string.</returns>
        public static string Capitalize(string s)
        {
            if (s == null || s.Length == 0)
                return s;

            var arr = s.ToCharArray();
            arr[0] = Char.ToUpper(arr[0]);
            return new string(arr);
        }

        /// <summary>
        /// Sanitize a string for use as an identifier by capitalizing all words and removing whitespace.
        /// </summary>
        /// <param name="s">The string.</param>
        /// <returns>A sanitized string.</returns>
        public static string Sanitize(string s)
        {
            if (s == null || s.Length == 0)
                return s;

            // Capitalize all words
            var arr = s.Split(null);
            
            for (int i = 0; i < arr.Length; i++)
                arr[i] = Capitalize(arr[i]);

            // Remove whitespace
            string ret = string.Join(null, arr);

            // Make sure it begins with a letter or underscore
            if (!Char.IsLetter(ret[0]) && ret[0] != '_')
                ret = "_" + ret;

            return ret;
        }

        /// <summary>
        /// Load data from a Uri.
        /// </summary>
        /// <param name="uri">The Uri.</param>
        /// <returns>A string with all the data.</returns>
        public static string LoadURI(Uri uri)
        {
            if (uri.IsFile)
                using (TextReader reader = File.OpenText(uri.ToString()))
                {
                    return reader.ReadToEnd();
                }
            else return null;
        }
    }
}
