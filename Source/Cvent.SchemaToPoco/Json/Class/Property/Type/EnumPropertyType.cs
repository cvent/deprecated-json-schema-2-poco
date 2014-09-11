using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Cvent.SchemaToPoco.Json.Class.Property.Type
{
    class EnumPropertyType : IJsonPropertyType
    {
        public CodeNamespaceImportCollection GetImports(JsonPropertyInfo info)
        {
            return new CodeNamespaceImportCollection { new CodeNamespaceImport("System") };
        }

        public CodeTypeDeclarationCollection GetEmbeddedTypes(JsonPropertyInfo info)
        {
            var enumeration = new CodeTypeDeclaration(info.Name + "Enum");
            enumeration.IsEnum = true;
            if (!string.IsNullOrEmpty(info.Definition.Description))
            {
                enumeration.Comments.Add(new CodeCommentStatement(info.Definition.Description, docComment: true));
            }

            info.Definition.Enum.ToList()
                .ForEach(x => enumeration.Members.Add(new CodeMemberField(info.Name, x.ToString())));

            var embeddedTypes = new CodeTypeDeclarationCollection();
            embeddedTypes.Add(enumeration);
            return embeddedTypes;
        }

        public CodeTypeReference GetType(JsonPropertyInfo info)
        {
            return new CodeTypeReference(info.Name + "Enum");
        }

        public CodeAttributeDeclarationCollection GetCustomAttributes(JsonPropertyInfo info)
        {
            return new CodeAttributeDeclarationCollection();
        }

        public CodeAssignStatement GetDefaultAssignment(JsonPropertyInfo info)
        {
            if (info.Definition.Default == null || info.Definition.Default.Type != JTokenType.String)
                return null;

            return new CodeAssignStatement(
                new CodeFieldReferenceExpression(null, info.FieldName),
                new CodeTypeReferenceExpression(string.Format("{0}.{1}", info.Name + "Enum", info.Definition.Default.Value<string>())));

            return new CodeAssignStatement(
                new CodeFieldReferenceExpression(null, info.FieldName),
                new CodePrimitiveExpression(
                    string.Format("{0}.{1}", info.Name, info.Definition.Default.Value<string>())));
        }

        public List<JsonClass> GetDependentClasses(JsonPropertyInfo info)
        {
            return new List<JsonClass>();
        }
    }
}
