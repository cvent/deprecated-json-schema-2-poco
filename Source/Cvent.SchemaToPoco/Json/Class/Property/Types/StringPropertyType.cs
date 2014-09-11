using System.CodeDom;
using Newtonsoft.Json.Linq;

namespace Cvent.SchemaToPoco.Json.Class.Property.Types
{
    /// <summary>
    /// Represents the String schema property type.
    /// </summary>
    public class StringPropertyType : JsonSchemaPropertyType
    {
        /// <see cref="JsonSchemaPropertyType"/>
        public override CodeTypeReference GetType(JsonPropertyInfo info)
        {
            return new CodeTypeReference(typeof(string));
        }

        /// <see cref="JsonSchemaPropertyType"/>
        public override CodeAssignStatement GetDefaultAssignment(JsonPropertyInfo info)
        {
            if (info.Definition.Default == null || info.Definition.Default.Type != JTokenType.String)
                return null;
            
            return CreatePrimitiveDefaultAssignment(info.FieldName, info.Definition.Default.Value<string>());
        }
    }
}