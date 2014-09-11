using System.CodeDom;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Cvent.SchemaToPoco.Json.Class.Property.Type
{
    public class StringPropertyType : IJsonPropertyType
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
            return new CodeTypeReference(typeof(string));
        }

        public CodeAttributeDeclarationCollection GetCustomAttributes(JsonPropertyInfo info)
        {
            var attributes = new CodeAttributeDeclarationCollection();
            if (!string.IsNullOrEmpty(info.Definition.Pattern))
            {
                var pattern = string.Format(@"@""{0}""", info.Definition.Pattern);
                attributes.Add(new CodeAttributeDeclaration(
                    "RegularExpression",
                    new CodeAttributeArgument(new CodeSnippetExpression(pattern))));
            }

            var lengthAttributeArguments = new List<CodeAttributeArgument>();
            if (info.Definition.MaximumLength.HasValue)
            {
                lengthAttributeArguments.Add(
                    new CodeAttributeArgument(new CodePrimitiveExpression(info.Definition.MaximumLength.Value)));
            }

            if (info.Definition.MinimumLength.HasValue)
            {
                lengthAttributeArguments.Add(new CodeAttributeArgument("MinimumLength",
                    new CodePrimitiveExpression(info.Definition.MinimumLength.Value)));
            }

            if (lengthAttributeArguments.Count > 0)
            {
                attributes.Add(new CodeAttributeDeclaration("StringLength", lengthAttributeArguments.ToArray()));
            }

            return attributes;
        }

        public CodeAssignStatement GetDefaultAssignment(JsonPropertyInfo info)
        {
            if (info.Definition.Default == null || info.Definition.Default.Type != JTokenType.String)
                return null;
            
            return new CodeAssignStatement(
                new CodeFieldReferenceExpression(null, info.FieldName),
                new CodePrimitiveExpression(info.Definition.Default.Value<string>()));
        }

        public List<JsonClass> GetDependentClasses(JsonPropertyInfo info)
        {
            return new List<JsonClass>();
        }
    }
}



