using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkBinary.SchemaToPoco.Core.Types;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            /*string schemaJson;

            schemaJson = @"{
  'id': 'person',
  'type': 'object',
  'properties': {
    'name': {'type':'string'},
    'age': {'type':'integer'}
  }
}";
            var definition = new { id = "" };
            var personSchema = JsonConvert.DeserializeAnonymousType(schemaJson, definition);
            Console.WriteLine(personSchema.id);
            // true*/
            //Type t = new TypeBuilderHelper("com.cvent").GetCustomType("DataSet");
            //Console.WriteLine(t.Name);
        }
    }
}
