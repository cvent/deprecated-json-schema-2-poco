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
        /// Set defaults for required fields.
        /// </summary>
        public const string DefaultClassName = "DefaultClassName";

        /// <summary>
        /// The JsonSchema.
        /// </summary>
        public JsonSchema Schema { get; set; }
        
        /// <summary>
        /// Namespace for this JSON schema to use.
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// Full path to this schema. Note that the namespace does not affect this field.
        /// </summary>
        public string FullPath { get; set; }

        /// <summary>
        /// Whether or not this schema should be generated or just referenced.
        /// </summary>
        public bool ToCreate { get; set; }

        /// <summary>
        /// List of interfaces.
        /// </summary>
        public List<Type> Interfaces { get; set; }

        /// <summary>
        /// List of other JsonSchemaWrappers this wrapper depends on.
        /// </summary>
        public List<JsonSchemaWrapper> Dependencies { get; set; }

        public JsonSchemaWrapper(JsonSchema schema)
        {
            Schema = schema;

            // Initialize defaults
            ToCreate = true;
            Interfaces = new List<Type>();
            Dependencies = new List<JsonSchemaWrapper>();

            Schema.Title = Schema.Title ?? DefaultClassName;
        }

        public JsonSchemaWrapper GetDependencyFromSchema(JsonSchema schema)
        {
            foreach (JsonSchemaWrapper w in Dependencies)
                if (w.Schema.Equals(schema))
                    return w;

            return null;
        }
    }
}
