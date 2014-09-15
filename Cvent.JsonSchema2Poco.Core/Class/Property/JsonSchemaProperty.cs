using System.CodeDom;
using System.Collections.Generic;
using Newtonsoft.Json.Schema;

namespace Cvent.JsonSchema2Poco.Class.Property
{
    public class JsonSchemaProperty
    {
        #region Properties
        public List<JsonSchemaClass> DependentClasses { get; set; }
        public CodeNamespaceImportCollection RequiredImports { get; set; }
        public CodeTypeDeclarationCollection EmbeddedTypes { get; set; }
        public CodeMemberProperty Property { get; set; }
        public CodeMemberField Field { get; set; }
        public CodeAssignStatement Default { get; set; }
        public CodeTypeReference Type { get; set; }
        #endregion

        #region Public Methods
        public JsonSchemaProperty()
        {
            DependentClasses = new List<JsonSchemaClass>();
            RequiredImports = new CodeNamespaceImportCollection();
            EmbeddedTypes = new CodeTypeDeclarationCollection();
        }

        public static JsonSchemaProperty CreateProperty(
            string propertyNamespace, 
            string propertyName,
            JsonSchema propertyDefinition)
        {
            var builder = new JsonSchemaPropertyBuilder();
            return builder.Build(propertyNamespace, propertyName, propertyDefinition);
        }

        #endregion
    }
}
