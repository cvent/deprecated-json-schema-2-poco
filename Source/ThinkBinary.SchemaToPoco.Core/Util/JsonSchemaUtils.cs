using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkBinary.SchemaToPoco.Core.Util
{
    /// <summary>
    /// Common utilities for a JsonSchema.
    /// </summary>
    class JsonSchemaUtils
    {
        /// <summary>
        /// Check if the schema is an integer type.
        /// </summary>
        /// <param name="schema">The JSON shema.</param>
        /// <returns>True if it is an integer.</returns>
        public static bool isInteger(JsonSchema schema)
        {
            return schema.Type != null && schema.Type.Value.ToString().Equals("Integer");
        }

        /// <summary>
        /// Check if the schema is an string type.
        /// </summary>
        /// <param name="schema">The JSON shema.</param>
        /// <returns>True if it is an string.</returns>
        public static bool isString(JsonSchema schema)
        {
            return schema.Type != null && schema.Type.Value.ToString().Equals("String");
        }

        /// <summary>
        /// Check if the schema is an array type.
        /// </summary>
        /// <param name="schema">The JSON shema.</param>
        /// <returns>True if it is an array.</returns>
        public static bool isArray(JsonSchema schema)
        {
            return schema.Type != null && schema.Type.Value.ToString().Equals("Array");
        }

        /// <summary>
        /// Get the type of the schema as a string.
        /// </summary>
        /// <param name="schema">The JSON schema.</param>
        /// <returns>The type of the schema as a string.</returns>
        public static string getTypeString(JsonSchema schema)
        {
            // Set the type to the type if it is not an array
            if (!JsonSchemaUtils.isArray(schema)) {
                if (schema.Title != null)
                    return schema.Title;
                if (schema.Type != null)
                    return schema.Type.ToString();
            }

            if (JsonSchemaUtils.isArray(schema)) {
                var listWrap = schema.UniqueItems ? "HashSet<" : "List<";

                // Set the type to the title if it exists
                if(schema.Title != null)
                    return listWrap + schema.Title + ">";

                if(schema.Items.Count > 0 && schema.Items[0].Title != null)
                    return listWrap + schema.Items[0].Title + ">";
            }

            // Default type
            return "object";
        }
    }
}
