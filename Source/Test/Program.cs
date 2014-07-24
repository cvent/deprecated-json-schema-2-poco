using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cvent.SchemaToPoco.Core.Types;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            string schemaJson = @"{
              '$id' : 'http://jsonschema.net/examples/',
              'properties': {
                'A': {'$ref':'#B.json'}
              }
            }";

            JsonSchema parsed = JsonSchema.Parse(schemaJson);
        }
    }
}
