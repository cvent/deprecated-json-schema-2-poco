using System;
using System.IO;

namespace Cvent.SchemaToPoco.Core.Util
{
    /// <summary>
    ///     Common string utilities.
    /// </summary>
    public static class StringUtils
    {
        /// <summary>
        ///     Capitalize the first letter in a string.
        /// </summary>
        /// <param name="s">The string.</param>
        /// <returns>A capitalized string.</returns>
        public static string Capitalize(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return s;
            }

            char[] arr = s.ToCharArray();
            arr[0] = Char.ToUpper(arr[0]);
            return new string(arr);
        }

        /// <summary>
        ///     Sanitize a string for use as an identifier by capitalizing all words and removing whitespace.
        /// </summary>
        /// <param name="s">The string.</param>
        /// <returns>A sanitized string.</returns>
        public static string SanitizeIdentifier(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return s;
            }

            // Capitalize all words
            string[] arr = s.Split(null);

            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = Capitalize(arr[i]);
            }

            // Remove whitespace
            string ret = string.Join(null, arr);

            // Make sure it begins with a letter or underscore
            if (!Char.IsLetter(ret[0]) && ret[0] != '_')
            {
                ret = "_" + ret;
            }

            return ret;
        }

        /// <summary>
        ///     Sanitize a regular expression
        /// </summary>
        /// <param name="s">The regex.</param>
        /// <param name="literal">Whether or not to sanitize for use as a string literal.</param>
        /// <returns>A sanitized regular expression</returns>
        /// TODO
        public static string SanitizeRegex(string s, bool literal)
        {
            return s;
        }

        /// <summary>
        ///     Load data from a Uri.
        /// </summary>
        /// <param name="uri">The Uri.</param>
        /// <returns>A string with all the data.</returns>
        public static string LoadUri(Uri uri)
        {
            if (uri.IsFile)
            {
                using (TextReader reader = File.OpenText(uri.ToString()))
                {
                    return reader.ReadToEnd();
                }
            }
            return null;
        }
    }
}
