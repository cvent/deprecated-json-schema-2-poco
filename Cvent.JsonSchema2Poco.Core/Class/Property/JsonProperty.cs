using System;
using System.CodeDom;
using System.Collections.Generic;
using Newtonsoft.Json.Schema;

namespace Cvent.SchemaToPoco.Json.Class.Property
{
    public class JsonProperty
    {
        #region Properties
        public List<JsonClass> DependentClasses { get; set; }
        public CodeNamespaceImportCollection RequiredImports { get; set; }
        public CodeTypeDeclarationCollection EmbeddedTypes { get; set; }
        public CodeMemberProperty Property { get; set; }
        public CodeMemberField Field { get; set; }
        public CodeAssignStatement Default { get; set; }
        public CodeTypeReference Type { get; set; }
        #endregion

        #region Public Methods
        public JsonProperty()
        {
            DependentClasses = new List<JsonClass>();
            RequiredImports = new CodeNamespaceImportCollection();
            EmbeddedTypes = new CodeTypeDeclarationCollection();
        }

        public static JsonProperty CreateProperty(
            string propertyNamespace, 
            string propertyName,
            JsonSchema propertyDefinition)
        {
            var builder = new JsonPropertyBuilder();
            return builder.Build(propertyNamespace, propertyName, propertyDefinition);
        }

        #endregion
    }
}
