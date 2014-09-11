using System.CodeDom;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Cvent.SchemaToPoco.Json.Class.Property.Types
{
    /// <summary>
    /// Represents the Object schema property type. Objects are considered other schemas and will be treated as separate classes.
    /// </summary>
    class ObjectPropertyType : JsonSchemaPropertyType
    {
        /// <see cref="JsonSchemaPropertyType"/>
        public override CodeTypeReference GetType(JsonPropertyInfo info)
        {
            var jsonClass = JsonSchemaClass.CreateFromSchema(info.Definition, info.Namespace);
            return new CodeTypeReference(jsonClass.Name);
        }

        /// <see cref="JsonSchemaPropertyType"/>
        public override CodeAssignStatement GetDefaultAssignment(JsonPropertyInfo info)
        {
            var defaultValue = info.Definition.Default;
            if (defaultValue == null || defaultValue.Type != JTokenType.Object)
                return null;

            return CreateObjectDefaultAssignment(info.FieldName, GetType(info));
        }

        /// <summary>
        /// The schema is generated as a class to be referenced by the schema property.
        /// </summary>
        /// <see cref="JsonSchemaPropertyType"/>
        public override List<JsonSchemaClass> GetDependentClasses(JsonPropertyInfo info)
        {
            return new List<JsonSchemaClass> {JsonSchemaClass.CreateFromSchema(info.Definition, info.Namespace)};
        }
    }
}
