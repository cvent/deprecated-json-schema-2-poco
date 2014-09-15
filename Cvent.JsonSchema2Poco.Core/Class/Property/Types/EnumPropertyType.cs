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
            return required || defaultPresent
                ? new CodeTypeReference(info.Name + "Enum")
                : new CodeTypeReference(info.Name + "Enum?");
        }

        /// <see cref="JsonSchemaPropertyType"/>
        public override CodeAssignStatement GetDefaultAssignment(JsonPropertyInfo info)
        {
            if (info.Definition.Default == null || info.Definition.Default.Type != JTokenType.String)
                return null;

            var value = string.Format("{0}.{1}", info.Name + "Enum", info.Definition.Default.Value<string>());
            return CreateReferenceDefaultAssignment(info.FieldName, value);
        }
    }
}
