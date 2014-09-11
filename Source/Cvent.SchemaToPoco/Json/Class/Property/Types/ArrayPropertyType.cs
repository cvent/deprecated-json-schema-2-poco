using System;
using System.CodeDom;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Cvent.SchemaToPoco.Json.Class.Property.Types
{
    /// <summary>
    /// Represents the Array schema property type when the items is a single schema. 
    /// Tuple is not supported at the current moment. 
    /// </summary>
    public class ArrayPropertyType : JsonSchemaPropertyType
    {
        /// <see cref="JsonSchemaPropertyType"/>
        public override CodeNamespaceImportCollection GetImports(JsonPropertyInfo info)
        {
            var imports = base.GetImports(info);
            imports.Add(new CodeNamespaceImport("System.Collections.Generic"));
            return imports;
        }

        /// <see cref="JsonSchemaPropertyType"/>
        public override CodeTypeReference GetType(JsonPropertyInfo info)
        {
            if (info.Definition.Items == null || info.Definition.Items.Count != 1)
                throw new ArgumentException("An array type with missing or too many items was found.");

            var baseType = JsonSchemaProperty.CreateProperty(null, info.Name, info.Definition.Items[0]).Type;
            return info.Definition.UniqueItems ? 
                new CodeTypeReference(string.Format("HashSet<{0}>", baseType.BaseType)) : 
                new CodeTypeReference(string.Format("List<{0}>", baseType.BaseType));
        }

        /// <see cref="JsonSchemaPropertyType"/>
        public override CodeAssignStatement GetDefaultAssignment(JsonPropertyInfo info)
        {
            if (info.Definition.Default == null || info.Definition.Default.Type != JTokenType.Array)
                return null;

            return CreateObjectDefaultAssignment(info.FieldName, GetType(info));
        }

        /// <see cref="JsonSchemaPropertyType"/>
        public override List<JsonSchemaClass> GetDependentClasses(JsonPropertyInfo info)
        {
            if (info.Definition.Items == null || info.Definition.Items.Count != 1)
                throw new ArgumentException("An array type with missing or too many items was found.");

            var property = JsonSchemaProperty.CreateProperty(info.Namespace, null, info.Definition.Items[0]);
            return property.DependentClasses;
        }
    }
}
