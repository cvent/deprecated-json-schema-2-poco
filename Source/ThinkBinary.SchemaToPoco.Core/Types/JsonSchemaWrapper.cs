using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkBinary.SchemaToPoco.Core.Types
{
    public class JsonSchemaWrapper
    {
        public JsonSchema Schema { get; set; }
        
        // Fully qualified name of the class that this schema uses, if any
        public string CClass { get; set; }

        public bool ToCreate { get; set; }

        public List<string> Interfaces { get; set; }

        public JsonSchemaWrapper(JsonSchema schema)
        {
            Schema = schema;

            // Initialize defaults
            ToCreate = true;
            Interfaces = new List<string>();
        }
    }
}
