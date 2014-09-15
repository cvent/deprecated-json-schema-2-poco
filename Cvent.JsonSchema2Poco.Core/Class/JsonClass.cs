using System;
using System.CodeDom;
using System.Collections.Generic;
using Newtonsoft.Json.Schema;

namespace Cvent.SchemaToPoco.Json.Class
{
    public class JsonClass
    {
        public string Id { get; set; }

        public CodeNamespaceImportCollection Imports { get; set; }

        public String Namespace { get; set; }

        public CodeComment Comment { get; set; }

        public String Name { get; set; }

        public List<CodeMemberField> Fields { get; set; }

        public List<CodeMemberProperty> Properties { get; set; }

        public List<CodeAssignStatement> PropertyDefaults { get; set; } 

        public List<JsonClass> DependentClasses { get; set; } 

        public CodeTypeDeclarationCollection EmbeddedTypes { get; set; }

        public static JsonClass CreateFromSchema(JsonSchema schema, string schemaNamespace)
        {
            var classBuilder = new JsonClassBuilder();
            return classBuilder.Build(schema, null, schemaNamespace);
        }

        public JsonClass()
        {
            Imports = new CodeNamespaceImportCollection();
            Fields = new List<CodeMemberField>();
            Properties = new List<CodeMemberProperty>();
            PropertyDefaults = new List<CodeAssignStatement>();
            DependentClasses = new List<JsonClass>();
            EmbeddedTypes = new CodeTypeDeclarationCollection();
        }

        public CodeCompileUnit GetClassRepresentation()
        {
            var constructor = new CodeConstructor();
            constructor.Attributes = MemberAttributes.Public;
            PropertyDefaults.ForEach(x => constructor.Statements.Add(x));

            var jsonClassRepresentation = new CodeTypeDeclaration();
            jsonClassRepresentation.IsClass = true;
            jsonClassRepresentation.Name = Name;
            jsonClassRepresentation.Comments.Add(new CodeCommentStatement(Comment));
            Fields.ForEach(x => jsonClassRepresentation.Members.Add(x));
            Properties.ForEach(x => jsonClassRepresentation.Members.Add(x));
            foreach (CodeTypeDeclaration embeddedType in EmbeddedTypes)
            {
                jsonClassRepresentation.Members.Add(embeddedType);
            }
            if (constructor.Statements.Count > 0)
            {
                jsonClassRepresentation.Members.Add(constructor);
            }

            var jsonNamespaceRepresentation = new CodeNamespace(Namespace);
            jsonNamespaceRepresentation.Types.Add(jsonClassRepresentation);
            foreach (CodeNamespaceImport import in Imports)
            {
                jsonNamespaceRepresentation.Imports.Add(import);
            }

            var jsonFullClassRepresentation = new CodeCompileUnit();
            jsonFullClassRepresentation.Namespaces.Add(jsonNamespaceRepresentation);
            return jsonFullClassRepresentation;
        }
    }
}
