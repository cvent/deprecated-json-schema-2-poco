using System.CodeDom;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Cvent.SchemaToPoco.Json.Class.Property.Type
{
    class BooleanPropertyType : IJsonPropertyType
    {
        public CodeNamespaceImportCollection GetImports(JsonPropertyInfo info)
        {
            return new CodeNamespaceImportCollection {new CodeNamespaceImport("System")};
        }

        public CodeTypeDeclarationCollection GetEmbeddedTypes(JsonPropertyInfo info)
        {
            return new CodeTypeDeclarationCollection();
        }

        public CodeTypeReference GetType(JsonPropertyInfo info)
        {
            return new CodeTypeReference(typeof(bool));
        }

        public CodeAttributeDeclarationCollection GetCustomAttributes(JsonPropertyInfo info)
        {
            return new CodeAttributeDeclarationCollection();
        }

        public CodeAssignStatement GetDefaultAssignment(JsonPropertyInfo info)
        {
            if (info.Definition.Default == null || info.Definition.Default.Type != JTokenType.Boolean)
                return null;
            
            return new CodeAssignStatement(
                new CodeFieldReferenceExpression(null, info.FieldName),
                new CodePrimitiveExpression(info.Definition.Default.Value<bool>()));
        }

        public List<JsonClass> GetDependentClasses(JsonPropertyInfo info)
        {
            return new List<JsonClass>();
        }
    }
}
