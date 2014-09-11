using System;
using System.CodeDom;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Cvent.SchemaToPoco.Json.Class.Property.Type
{
    class ArrayPropertyType : IJsonPropertyType
    {
        public CodeNamespaceImportCollection GetImports(JsonPropertyInfo info)
        {
            return new CodeNamespaceImportCollection{new CodeNamespaceImport("System.Collections.Generic")};
        }

        public CodeTypeDeclarationCollection GetEmbeddedTypes(JsonPropertyInfo info)
        {
            return new CodeTypeDeclarationCollection();
        }

        public CodeTypeReference GetType(JsonPropertyInfo info)
        {
            if (info.Definition.Items == null || info.Definition.Items.Count != 1)
            {
                throw new ArgumentException("An array type with missing or too many items was found.");
            }

            var baseType = JsonProperty.CreateProperty(null, info.Name, info.Definition.Items[0]).Type;
            return info.Definition.UniqueItems ? 
                new CodeTypeReference(string.Format("HashSet<{0}>", baseType.BaseType)) : 
                new CodeTypeReference(string.Format("List<{0}>", baseType.BaseType));
        }

        public CodeAttributeDeclarationCollection GetCustomAttributes(JsonPropertyInfo info)
        {
            var attributes = new CodeAttributeDeclarationCollection();
            if (info.Definition.MinimumItems.HasValue)
            {
                attributes.Add(new CodeAttributeDeclaration(
                    "MinLength", new CodeAttributeArgument(
                        new CodePrimitiveExpression(info.Definition.MinimumItems.Value))));
            }
            if (info.Definition.MaximumItems.HasValue)
            {
                attributes.Add(new CodeAttributeDeclaration(
                    "MaxLength", new CodeAttributeArgument(
                        new CodePrimitiveExpression(info.Definition.MaximumItems.Value))));
            }
            return attributes;
        }

        public CodeAssignStatement GetDefaultAssignment(JsonPropertyInfo info)
        {
            if (info.Definition.Default == null || info.Definition.Default.Type != JTokenType.Array)
                return null;
            
            return new CodeAssignStatement(
                new CodeFieldReferenceExpression(null, info.FieldName),
                new CodeObjectCreateExpression(GetType(info)));
        }

        public List<JsonClass> GetDependentClasses(JsonPropertyInfo info)
        {
            if (info.Definition.Items == null || info.Definition.Items.Count != 1)
            {
                throw new ArgumentException("An array type with missing or too many items was found.");
            }

            var property = JsonProperty.CreateProperty(info.Namespace, null, info.Definition.Items[0]);
            return property.DependentClasses;
        }
    }
}
