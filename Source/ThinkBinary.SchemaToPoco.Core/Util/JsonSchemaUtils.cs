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
        public static bool IsNumber(JsonSchema schema)
        {
            return schema.Type != null && (schema.Type.Value.ToString().Equals("Integer") || schema.Type.Value.ToString().Equals("Float"));
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
        /// Get the type of the schema.
        /// </summary>
        /// <param name="schema">The JSON schema.</param>
        /// <returns>The type of the schema.</returns>
        public static Type GetType(JsonSchema schema)
        {
            string toRet = DefaultType;

            // Set the type to the type if it is not an array
            if (!IsArray(schema)) {
                if (schema.Title != null)
                    toRet = schema.Title;
                else if (schema.Type != null)
                    toRet = GetPrimitiveType(schema.Type);
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
                        toRet += GetPrimitiveType(schema.Items[0].Type);
                    else
                        toRet += DefaultType;
                }
                else
                    toRet += DefaultType;

                toRet += ">";
            }

            return Type.GetType(toRet, true);
        }

        /// <summary>
        /// Get the primitive name of a type if it exists.
        /// </summary>
        /// <param name="s">The type name.</param>
        /// <returns>The primitive type, if it exists.</returns>
        private static string GetPrimitiveType(JsonSchemaType? type)
        {
            string sType = type.ToString();

            var primitives = new Dictionary<string, string>() {
                {"String", "System.String"},
                {"Float", "System.Single"},
                {"Integer", "System.Int32"},
                {"Boolean", "System.Boolean"},
                {"Object", "System.Object"}
            };

            if(primitives.ContainsKey(sType))
                return primitives[sType];

            return sType;
        }
    }
}
