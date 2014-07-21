using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkBinary.SchemaToPoco.Core.Types
{
    /// <summary>
    /// Wrapper for a JsonSchema.
    /// </summary>
    public class JsonSchemaWrapper
    {
        /// <summary>
        /// The JsonSchema.
        /// </summary>
        public JsonSchema Schema { get; set; }
        
        /// <summary>
        /// Fully qualified name of the class that this schema uses, if any
        /// </summary>
        public string CClass { get; set; }

        /// <summary>
        /// Whether or not this schema should be generated or just referenced.
        /// </summary>
        public bool ToCreate { get; set; }

        /// <summary>
        /// List of interfaces.
        /// </summary>
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
