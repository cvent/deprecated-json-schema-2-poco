using System;
using System.CodeDom;
using Newtonsoft.Json.Schema;

namespace Cvent.SchemaToPoco.Json.Class.Property
{
    class JsonPropertyBuilder
    {
        public JsonProperty Build(
            string propertyNamespace,
            string propertyName, 
            JsonSchema propertyDefinition)
        {
            var propertyType = JsonPropertyTypeFactory.GetJsonPropertyType(propertyDefinition);
            if (propertyType == null)
                return null;

            var propertyInfo = new JsonPropertyInfo
            {
                Namespace = propertyNamespace,
                Name = ConvertToPascalCase(propertyName),
                FieldName = "_" + ConvertToCamelCase(propertyName),
                Definition = propertyDefinition
            };

            var property = new JsonProperty();
            property.RequiredImports = propertyType.GetImports(propertyInfo);
            property.EmbeddedTypes = propertyType.GetEmbeddedTypes(propertyInfo);
            property.Type = propertyType.GetType(propertyInfo);
            property.Field = CreateField(propertyInfo.FieldName, property.Type);
            property.Property = CreateProperty(propertyInfo.Name, propertyDefinition.Description, property.Field, propertyType.GetCustomAttributes(propertyInfo));
            if (propertyDefinition.Required.HasValue && propertyDefinition.Required.Value)
            {
                property.Property.CustomAttributes.Add(new CodeAttributeDeclaration("Required"));
            }
            if (property.Property.CustomAttributes.Count > 0)
            {
                property.RequiredImports.Add(new CodeNamespaceImport("System.ComponentModel.DataAnnotations"));
            }
            property.Default = propertyType.GetDefaultAssignment(propertyInfo);
            property.DependentClasses = propertyType.GetDependentClasses(propertyInfo);
            return property;
        }

        private CodeMemberField CreateField(string fieldName, CodeTypeReference type)
        {
            var field = new CodeMemberField
            {
                Name = fieldName,
                Type = type,
                Attributes = MemberAttributes.Private
            };
            return field;
        }

        private CodeMemberProperty CreateProperty(string propertyName, string description, CodeMemberField field, CodeAttributeDeclarationCollection attributes)
        {
            var property = new CodeMemberProperty
            {
                Name = propertyName,
                Type = field.Type,
                CustomAttributes = attributes,
                Attributes = MemberAttributes.Public
            };

            if (!string.IsNullOrEmpty(description))
            {
                property.Comments.Add(new CodeCommentStatement(new CodeComment(description, docComment: true)));    
            }
            
            property.SetStatements.Add(
                new CodeAssignStatement(
                    new CodeFieldReferenceExpression(null, field.Name),
                    new CodePropertySetValueReferenceExpression()));
            property.GetStatements.Add(
                new CodeMethodReturnStatement(
                    new CodeFieldReferenceExpression(null, field.Name)));
            return property;
        }

        private string ConvertToPascalCase(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return s;
            }

            char[] arr = s.ToCharArray();
            arr[0] = Char.ToUpper(arr[0]);
            return new string(arr);
        }

        private string ConvertToCamelCase(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return s;
            }

            char[] arr = s.ToCharArray();
            arr[0] = Char.ToLower(arr[0]);
            return new string(arr);
        }
    }
}
