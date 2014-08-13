using System;
using System.Text.RegularExpressions;
using Cvent.SchemaToPoco.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;

namespace Test
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string json = @"{
                'fOo': [
                    'User',
                    'Admin'
                ],
                'str': 'asdfaasdfasdfasdf'
            }";

            var def = JsonConvert.DeserializeObject<DefaultClass>(json);

            foreach(var s in def.Foo)
                Console.WriteLine(s);

            Console.WriteLine(def.Str);

            Console.ReadLine();
        }
    }
}
