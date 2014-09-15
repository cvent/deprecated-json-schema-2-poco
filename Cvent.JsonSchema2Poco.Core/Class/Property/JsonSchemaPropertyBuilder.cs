using System;
using System.CodeDom;
using Newtonsoft.Json.Schema;
using Cvent.JsonSchema2Poco.Class.Property.Types;

namespace Cvent.JsonSchema2Poco.Class.Property
{
    class JsonSchemaPropertyBuilder
    {
        public JsonSchemaProperty Build(
            string propertyNamespace,
            string propertyName, 
            JsonSchema propertyDefinition)
        {
            // GET THE UNDERLYING SCHEMA PROPERTY TYPE.
            var propertyType = JsonSchemaPropertyTypeFactory.GetJsonPropertyType(propertyDefinition);
            if (propertyType == null)
                return null;

            var propertyInfo = new JsonPropertyInfo
            {
                Namespace = propertyNamespace,
                Name = ConvertToPascalCase(propertyName),
                FieldName = "_" + ConvertToCamelCase(propertyName),
                Definition = propertyDefinition
            };

            // GET IN MEMORY PROPERTY DEFINITION.
            var property = new JsonSchemaProperty();
            property.RequiredImports = propertyType.GetImports(propertyInfo);
            property.EmbeddedTypes = propertyType.GetEmbeddedTypes(propertyInfo);
            property.Type = propertyType.GetType(propertyInfo);
            property.Field = CreateField(propertyInfo.FieldName, property.Type);
            property.Property = CreateProperty(propertyInfo.Name, propertyDefinition.Description, property.Field, propertyType.GetCustomAttributes(propertyInfo));
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
