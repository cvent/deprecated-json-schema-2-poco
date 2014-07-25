using Newtonsoft.Json.Schema;

namespace Test
{
    internal class Program
    {
        private static void Main(string[] args)
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
