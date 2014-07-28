using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Media;
using System.Text.RegularExpressions;

namespace Cvent.SchemaToPoco.Core.Util
{
    /// <summary>
    ///     Common string utilities.
    /// </summary>
    public static class StringUtils
    {
        private static char[] _escapeChars = new[]
        {
            'w',
            'W',
            'd',
            'D',
            's',
            'S'
        };

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
        public static string SanitizeRegex(string s, bool literal)
        {
            return literal ? ToLiteral(s, true).Replace("\"", "\"\"") : Regex.Escape(s);
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

        /// <summary>
        ///     Convert a string to a literal string.
        /// </summary>
        /// <param name="input">The string.</param>
        /// <param name="preserveEscapes">Whether or not to preserve regex escape sequences.</param>
        /// <returns>An escaped string.</returns>
        public static string ToLiteral(string input, bool preserveEscapes)
        {
            using (var writer = new StringWriter())
            {
                using (var provider = CodeDomProvider.CreateProvider("CSharp"))
                {
                    provider.GenerateCodeFromExpression(new CodePrimitiveExpression(input), writer, null);
                    string s = writer.ToString();

                    // Remove quotes from beginning and end
                    s = s.TrimStart(new[] { '"' }).TrimEnd(new[] { '"' });

                    // Preserve escape sequences
                    if (preserveEscapes)
                    {
                        foreach (char c in _escapeChars)
                        {
                            s = s.Replace(@"\\" + c, @"\" + c);
                        }
                    }

                    return s;
                }
            }
        }
    }
}
