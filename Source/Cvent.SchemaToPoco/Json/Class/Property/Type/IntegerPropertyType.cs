using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Cvent.SchemaToPoco.Json.Class.Property.Type
{
    class IntegerPropertyType : IJsonPropertyType
    {
        public CodeNamespaceImportCollection GetImports(JsonPropertyInfo info)
        {
            return new CodeNamespaceImportCollection { new CodeNamespaceImport("System") };
        }

        public CodeTypeDeclarationCollection GetEmbeddedTypes(JsonPropertyInfo info)
        {
            return new CodeTypeDeclarationCollection();
        }

        public CodeTypeReference GetType(JsonPropertyInfo info)
        {
            return new CodeTypeReference(typeof(long));
        }

        public CodeAttributeDeclarationCollection GetCustomAttributes(JsonPropertyInfo info)
        {
            return new CodeAttributeDeclarationCollection();
        }

        public CodeAssignStatement GetDefaultAssignment(JsonPropertyInfo info)
        {
            if (info.Definition.Default == null || info.Definition.Default.Type != JTokenType.Integer)
                return null;

            return new CodeAssignStatement(
                new CodeFieldReferenceExpression(null, info.FieldName),
                new CodePrimitiveExpression(info.Definition.Default.Value<long>()));
        }

        public List<JsonClass> GetDependentClasses(JsonPropertyInfo info)
        {
            return new List<JsonClass>();
        }
    }
}
