using System.CodeDom;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Cvent.SchemaToPoco.Json.Class.Property.Type
{
    class ObjectPropertyType : IJsonPropertyType
    {
        public CodeNamespaceImportCollection GetImports(JsonPropertyInfo info)
        {
            return new CodeNamespaceImportCollection();
        }

        public CodeTypeDeclarationCollection GetEmbeddedTypes(JsonPropertyInfo info)
        {
            return new CodeTypeDeclarationCollection();
        }

        public CodeTypeReference GetType(JsonPropertyInfo info)
        {
            var jsonClass = JsonClass.CreateFromSchema(info.Definition, info.Namespace);
            return new CodeTypeReference(jsonClass.Name);
        }

        public CodeAttributeDeclarationCollection GetCustomAttributes(JsonPropertyInfo info)
        {
            return new CodeAttributeDeclarationCollection();
        }

        public CodeAssignStatement GetDefaultAssignment(JsonPropertyInfo info)
        {
            var defaultValue = info.Definition.Default;
            if (defaultValue == null || defaultValue.Type != JTokenType.Object)
                return null;

            return new CodeAssignStatement(
                new CodeFieldReferenceExpression(null, info.FieldName),
                new CodeObjectCreateExpression(GetType(info)));
        }

        public List<JsonClass> GetDependentClasses(JsonPropertyInfo info)
        {
            return new List<JsonClass> {JsonClass.CreateFromSchema(info.Definition, info.Namespace)};
        }
    }
}
