using System;
using System.CodeDom;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Cvent.JsonSchema2Poco.Class.Property.Types
{
    /// <summary>
    /// Represents the Enum schema property type. The enumeration type name will have Enum appended to resolve issues with the property.
    /// </summary>
    public class EnumPropertyType : JsonSchemaPropertyType
    {
        /// <summary>
        /// The enumeration definition is considered an embedded type to help with scoping of enumerations between classes
        /// that share the same name.
        /// </summary>
        /// <see cref="JsonSchemaPropertyType"/>
        public override CodeTypeDeclarationCollection GetEmbeddedTypes(JsonPropertyInfo info)
        {
            if (info.Definition.Id != null)
                return base.GetEmbeddedTypes(info);

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

        /// <see cref="JsonSchemaPropertyType"/>
        public override CodeTypeReference GetType(JsonPropertyInfo info)
        {
            var required = info.Definition.Required.HasValue && info.Definition.Required.Value;
            var defaultPresent = GetDefaultAssignment(info) != null;
            var name = info.Definition.Title != null ? ConvertToPascalCase(info.Definition.Title) : info.Name;
            return required || defaultPresent
                ? new CodeTypeReference(name + "Enum")
                : new CodeTypeReference(name + "Enum?");
        }

        /// <see cref="JsonSchemaPropertyType"/>
        public override CodeAssignStatement GetDefaultAssignment(JsonPropertyInfo info)
        {
            if (info.Definition.Default == null || info.Definition.Default.Type != JTokenType.String)
                return null;

            var name = info.Definition.Title != null ? ConvertToPascalCase(info.Definition.Title) : info.Name;
            var value = string.Format("{0}.{1}", name + "Enum", info.Definition.Default.Value<string>());
            return CreateReferenceDefaultAssignment(info.FieldName, value);
        }

        private string ConvertToPascalCase(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return s;
            }

            // Capitalize all words
            string[] arr = s.Split(null);

            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = Capitalize(arr[i]);
            }

            // Remove whitespace
            string ret = string.Join(null, arr);

            // Make sure it begins with a letter or underscore
            if (!Char.IsLetter(ret[0]) && ret[0] != '_')
            {
                ret = "_" + ret;
            }

            return ret;
        }

        public string Capitalize(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return s;
            }

            char[] arr = s.ToCharArray();
            arr[0] = Char.ToUpper(arr[0]);
            return new string(arr);
        }
    }
}
