using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Cvent.SchemaToPoco.Core.UnitTests
{
    /// <summary>
    ///     Handy utilities for unit tests.
    /// </summary>
    internal static class TestUtils
    {
        /// <summary>
        ///     Standardize the generated C# code by removing comments and whitespace.
        /// </summary>
        /// <param name="code">The C# code to sanitize.</param>
        /// <returns>Sanitized C# code.</returns>
        internal static string SanitizeCSharpCode(string code)
        {
            var lines = new List<string>(code.Split('\n'));
            for (int i = lines.Count - 1; i >= 0; i--)
            {
                if (lines[i].StartsWith("//"))
                {
                    lines.RemoveAt(i);
                }
            }
            return RemoveAllWhitespace(string.Join("\n", lines));
        }

        /// <summary>
        ///     Remove all whitespace from a string to more easily test code. This includes newlines.
        /// </summary>
        /// <param name="s">The string to sanitize.</param>
        /// <returns>The string with all whitespace removed.</returns>
        private static string RemoveAllWhitespace(string s)
        {
            return Regex.Replace(s, @"\s+", "");
        }
    }
}
