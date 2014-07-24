using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Cvent.SchemaToPoco.Core.Types;

namespace Cvent.SchemaToPoco.Core.Util
{
    /// <summary>
    /// Abstraction for the different schema array types.
    /// </summary>
    public enum ArrayType { List, HashSet };

    /// <summary>
    /// Common utilities for a JsonSchema.
    /// </summary>
    public class JsonSchemaUtils
    {
        /// <summary>
        /// Default type to set to if not specified elsewhere.
        /// </summary>
        private const string DefaultType = "System.Object";

        /// <summary>
        /// What primitive objects map to in C#.
        /// </summary>
        private static Dictionary<string, string> Primitives = new Dictionary<string, string>() {
            {"String", "System.String"},
            {"Float", "System.Single"},
            {"Integer", "System.Int32"},
            {"Boolean", "System.Boolean"},
            {"Object", "System.Object"}
        };

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
        /// Get the array type for the given schema.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <exception cref="System.NotSupportedException">Thrown when given schema is not an array type.</exception>
        /// <returns>The array type of the schema.</returns>
        public static ArrayType GetArrayType(JsonSchema schema)
        {
            if (!IsArray(schema))
                throw new NotSupportedException();

            return schema.UniqueItems ? ArrayType.HashSet : ArrayType.List;
        }

        /// <summary>
        /// Get the type of the schema. If it is an array, get the array type.
        /// </summary>
        /// <param name="schema">The JSON schema.</param>
        /// <param name="ns">The namespace.</param>
        /// <returns>The type of the schema.</returns>
        public static Type GetType(JsonSchema schema, string ns)
        {
            string toRet = DefaultType;
            TypeBuilderHelper builder = new TypeBuilderHelper(ns);

            // Set the type to the type if it is not an array
            if (!IsArray(schema)) {
                if (schema.Title != null)
                    return builder.GetCustomType(schema.Title, true);
                else if (schema.Type != null)
                    toRet = GetPrimitiveType(schema.Type);
            }
            else {
                // Set the type to the title if it exists
                if (schema.Title != null)
                    return builder.GetCustomType(schema.Title, true);
                else if (schema.Items.Count > 0)
                {
                    // Set the type to the title of the items
                    if (schema.Items[0].Title != null)
                        return builder.GetCustomType(schema.Items[0].Title, true);
                    // Set the type to the type of the items
                    else if (schema.Items[0].Type != null)
                        toRet = GetPrimitiveType(schema.Items[0].Type);
                }
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

            if(Primitives.ContainsKey(sType))
                return Primitives[sType];

            return sType;
        }
    }
}
