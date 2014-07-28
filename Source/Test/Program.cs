using System;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Schema;

namespace Test
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Regex r = new Regex(@"^\\\""dev\""\'[a-c]\s$");
            string text = @"\""dev""'a ";
            MatchCollection col = r.Matches(text);
            Console.WriteLine(text);
            Console.WriteLine("{0} matches found", col.Count);
        }
    }
}
