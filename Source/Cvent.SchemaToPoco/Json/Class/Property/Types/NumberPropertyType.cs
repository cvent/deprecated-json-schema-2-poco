using System;
using System.CodeDom;
using Newtonsoft.Json.Linq;

namespace Cvent.SchemaToPoco.Json.Class.Property.Types
{
    /// <summary>
    /// Represents the Number schema property. To handle all domains the type is represented as a double precision float.
    /// </summary>
    class NumberPropertyType : JsonSchemaPropertyType
    {
        /// <see cref="JsonSchemaPropertyType"/>
        public override CodeTypeReference GetType(JsonPropertyInfo info)
        {
            return new CodeTypeReference(typeof(double));
        }

        /// <see cref="JsonSchemaPropertyType"/>
        public override CodeAssignStatement GetDefaultAssignment(JsonPropertyInfo info)
        {
            if (info.Definition.Default == null || info.Definition.Default.Type != JTokenType.Float)
                return null;

            return CreatePrimitiveDefaultAssignment(info.FieldName, info.Definition.Default.Value<double>());
        }
    }
}
