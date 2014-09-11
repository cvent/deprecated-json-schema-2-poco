using System.CodeDom;
using System.Collections.Generic;

namespace Cvent.SchemaToPoco.Json.Class.Property.Type
{
    class AnyPropertyType : IJsonPropertyType
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
            return new CodeTypeReference(typeof(object));
        }

        public CodeAttributeDeclarationCollection GetCustomAttributes(JsonPropertyInfo info)
        {
            return new CodeAttributeDeclarationCollection();
        }

        public CodeAssignStatement GetDefaultAssignment(JsonPropertyInfo info)
        {
            return null;
        }

        public List<JsonClass> GetDependentClasses(JsonPropertyInfo info)
        {
            return new List<JsonClass>();
        }
    }
}
