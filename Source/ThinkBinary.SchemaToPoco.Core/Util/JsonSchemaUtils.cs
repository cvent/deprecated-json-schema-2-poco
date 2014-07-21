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
    public class JsonSchemaUtils
    {
        /// <summary>
        /// Default type to set to if not specified elsewhere.
        /// </summary>
        private const string DefaultType = "Object";

        /// <summary>
        /// Check if the schema is an integer type.
        /// </summary>
        /// <param name="schema">The JSON shema.</param>
        /// <returns>True if it is an integer.</returns>
        public static bool IsInteger(JsonSchema schema)
        {
            return schema.Type != null && schema.Type.Value.ToString().Equals("Integer");
        }

        /// <summary>
        /// Check if the schema is an string type.
        /// </summary>
        /// <param name="schema">The JSON shema.</param>
        /// <returns>True if it is an string.</returns>
        public static bool IsString(JsonSchema schema)
        {
            return schema.Type != null && schema.Type.Value.ToString().Equals("String");
        }

        /// <summary>
        /// Check if the schema is an array type.
        /// </summary>
        /// <param name="schema">The JSON shema.</param>
        /// <returns>True if it is an array.</returns>
        public static bool IsArray(JsonSchema schema)
        {
            return schema.Type != null && schema.Type.Value.ToString().Equals("Array");
        }

        /// <summary>
        /// Get the type of the schema as a string.
        /// </summary>
        /// <param name="schema">The JSON schema.</param>
        /// <returns>The type of the schema as a string.</returns>
        public static string GetTypeString(JsonSchema schema)
        {
            string toRet = DefaultType;

            // Set the type to the type if it is not an array
            if (!IsArray(schema)) {
                if (schema.Title != null)
                    toRet = schema.Title;
                else if (schema.Type != null)
                    toRet = schema.Type.ToString();
            }
            else {
                toRet = schema.UniqueItems ? "HashSet<" : "List<";

                // Set the type to the title if it exists
                if (schema.Title != null)
                    toRet += schema.Title;
                else if (schema.Items.Count > 0)
                {
                    // Set the type to the title of the items
                    if (schema.Items[0].Title != null)
                        toRet += schema.Items[0].Title;
                    // Set the type to the type of the items
                    else if (schema.Items[0].Type != null)
                        toRet += schema.Items[0].Type;
                    else
                        toRet += DefaultType;
                }
                else
                    toRet += DefaultType;

                toRet += ">";
            }

            return toRet;
        }
    }
}
