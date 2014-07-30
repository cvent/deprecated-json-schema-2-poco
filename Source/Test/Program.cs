using System;
using System.Text.RegularExpressions;
using Cvent.SchemaToPoco.Core;
using Newtonsoft.Json.Schema;

namespace Test
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            /*Regex r = new Regex(@"^\\\""dev\""\'[a-c]\s$");
            string text = @"\""dev""'a ";
            MatchCollection col = r.Matches(text);
            Console.WriteLine(text);
            Console.WriteLine("{0} matches found", col.Count);*/
            var uri = new Uri("file:///C:/Users/SLiu/Projects/json-schema-2-poco/Examples/Schemas");
            System.Console.WriteLine(uri.IsFile);
        }
    }
}
