using System.CodeDom;
using System.Collections.Generic;
using Newtonsoft.Json.Schema;

namespace Cvent.SchemaToPoco.Json.Class.Property
{
    public class JsonPropertyInfo
    {
        public string Namespace { get; set; }
        public string Name { get; set; }
        public string FieldName { get; set; }
        public JsonSchema Definition { get; set; }
    }

    public interface IJsonPropertyType
    {
        CodeNamespaceImportCollection GetImports(JsonPropertyInfo info);
        CodeTypeDeclarationCollection GetEmbeddedTypes(JsonPropertyInfo info);
        CodeTypeReference GetType(JsonPropertyInfo info);
        CodeAttributeDeclarationCollection GetCustomAttributes(JsonPropertyInfo info);
        CodeAssignStatement GetDefaultAssignment(JsonPropertyInfo info);
        List<JsonClass> GetDependentClasses(JsonPropertyInfo info);
    }
}
