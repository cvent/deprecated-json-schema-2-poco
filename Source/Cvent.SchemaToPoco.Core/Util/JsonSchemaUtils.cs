using System;
using Cvent.SchemaToPoco.Core.Types;
using Newtonsoft.Json.Schema;

namespace Cvent.SchemaToPoco.Core.Util
{
    /// <summary>
    ///     Abstraction for the different schema array types.
    /// </summary>
    public enum ArrayType
    {
        List,
        HashSet
    };

    /// <summary>
    ///     Common utilities for a JsonSchema.
    /// </summary>
    public static class JsonSchemaUtils
    {
        /// <summary>
        ///     Default type to set to if not specified elsewhere.
        /// </summary>
        private const string DEFAULT_TYPE = "System.Object";

        /// <summary>
        ///     Check if the schema is an integer type.
        /// </summary>
        /// <param name="schema">The JSON shema.</param>
        /// <returns>True if it is an integer.</returns>
        public static bool IsNumber(JsonSchema schema)
        {
            return schema.Type != null &&
                   (schema.Type.Value.ToString().Equals("Integer") || schema.Type.Value.ToString().Equals("Float"));
        }

        /// <summary>
        ///     Check if the schema is an string type.
        /// </summary>
        /// <param name="schema">The JSON shema.</param>
        /// <returns>True if it is an string.</returns>
        public static bool IsString(JsonSchema schema)
        {
            return schema.Type != null && schema.Type.Value.ToString().Equals("String");
        }

        /// <summary>
        ///     Check if the schema is an array type.
        /// </summary>
        /// <param name="schema">The JSON shema.</param>
        /// <returns>True if it is an array.</returns>
        public static bool IsArray(JsonSchema schema)
        {
            return schema.Type != null && schema.Type.Value.ToString().Equals("Array");
        }

        /// <summary>
        ///     Get the array type for the given schema.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <exception cref="System.NotSupportedException">Thrown when given schema is not an array type.</exception>
        /// <returns>The array type of the schema.</returns>
        public static ArrayType GetArrayType(JsonSchema schema)
        {
            if (!IsArray(schema))
            {
                throw new NotSupportedException();
            }

            return schema.UniqueItems ? ArrayType.HashSet : ArrayType.List;
        }

        /// <summary>
        ///     Get the type of the schema. If it is an array, get the array type.
        /// </summary>
        /// <param name="schema">The JSON schema.</param>
        /// <param name="ns">The namespace.</param>
        /// <returns>The type of the schema.</returns>
        public static Type GetType(JsonSchema schema, string ns = "")
        {
            string toRet = DEFAULT_TYPE;
            var builder = new TypeBuilderHelper(ns);

            // Set the type to the type if it is not an array
            if (!IsArray(schema))
            {
                if (schema.Title != null)
                {
                    return builder.GetCustomType(schema.Title, true);
                }
                if (schema.Type != null)
                {
                    toRet = TypeUtils.GetPrimitiveTypeAsString(schema.Type);
                }
            }
            else
            {
                // Set the type to the title if it exists
                if (schema.Title != null)
                {
                    return builder.GetCustomType(schema.Title, true);
                }
                if (schema.Items != null && schema.Items.Count > 0)
                {
                    // Set the type to the title of the items
                    if (schema.Items[0].Title != null)
                    {
                        return builder.GetCustomType(schema.Items[0].Title, true);
                    }
                        // Set the type to the type of the items
                    if (schema.Items[0].Type != null)
                    {
                        toRet = TypeUtils.GetPrimitiveTypeAsString(schema.Items[0].Type);
                    }
                }
            }

            return Type.GetType(toRet, true);
        }
    }
}
