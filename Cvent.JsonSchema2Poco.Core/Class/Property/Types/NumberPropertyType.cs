using System.CodeDom;
using Newtonsoft.Json.Linq;

namespace Cvent.JsonSchema2Poco.Class.Property.Types
{
    /// <summary>
    /// Represents the Number schema property. To handle all domains the type is represented as a double precision float.
    /// </summary>
    class NumberPropertyType : JsonSchemaPropertyType
    {
        /// <see cref="JsonSchemaPropertyType"/>
        public override CodeTypeReference GetType(JsonPropertyInfo info)
        {
            var required = info.Definition.Required.HasValue && info.Definition.Required.Value;
            var defaultPresent = GetDefaultAssignment(info) != null;
            return required || defaultPresent
                ? new CodeTypeReference(typeof(double))
                : new CodeTypeReference(typeof(double?));
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
