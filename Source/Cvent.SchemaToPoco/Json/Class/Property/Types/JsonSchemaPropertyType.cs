using System.CodeDom;
using System.Collections.Generic;
using Newtonsoft.Json.Schema;

namespace Cvent.SchemaToPoco.Json.Class.Property.Types
{
    /// <summary>
    /// Container holding all necessary information for a Json Schema Property.
    /// </summary>
    public struct JsonPropertyInfo
    {
        /// <summary>The namespace the property resides in.</summary>
        public string Namespace { get; set; }
        /// <summary>The name of the property.</summary>
        public string Name { get; set; }
        /// <summary>The name of the underlying field the property represents.</summary>
        public string FieldName { get; set; }
        /// <summary>The definition of the schema property.</summary>
        public JsonSchema Definition { get; set; }
    }

    /// <summary>
    /// A base for a JSON schema property type. All types should inherit from this and redefine any components which
    /// differ from default. 
    /// </summary>
    public abstract class JsonSchemaPropertyType
    {
        /// <summary>
        /// Gets an in memory representation of the necessary imports the property references.
        /// </summary>
        /// <param name="info">The info for the schema property.</param>
        /// <returns>The in memory representation of the reference imports.</returns>
        public virtual CodeNamespaceImportCollection GetImports(JsonPropertyInfo info)
        {
            return new CodeNamespaceImportCollection { new CodeNamespaceImport("System") };
        }

        /// <summary>
        /// Gets any types which must be embedded in the class for the property to reference.
        /// </summary>
        /// <param name="info">The info for the schema property.</param>
        /// <returns>The collection of types which will be embedded in the class for the property to reference.</returns>
        public virtual CodeTypeDeclarationCollection GetEmbeddedTypes(JsonPropertyInfo info)
        {
            return new CodeTypeDeclarationCollection();
        }

        /// <summary>
        /// Gets the type of the schema property.
        /// </summary>
        /// <param name="info">The info for the schema property.</param>
        /// <returns>The type of the schema property.</returns>
        public abstract CodeTypeReference GetType(JsonPropertyInfo info);

        /// <summary>
        /// Gets any custom attributes to place on the property. At the current moment no validation
        /// attributes are placed on any properties because there is no current attributes to use.
        /// </summary>
        /// <param name="info">The info for the schema property.</param>
        /// <returns>The collection of custom attributes for the schema property.</returns>
        public virtual CodeAttributeDeclarationCollection GetCustomAttributes(JsonPropertyInfo info)
        {
            return new CodeAttributeDeclarationCollection();
        }

        /// <summary>
        /// Gets the default assignment expression used during construction of the class.
        /// </summary>
        /// <param name="info">The info for the schema property.</param>
        /// <returns>The default assignment expression. Null if there is not default assignment.</returns>
        public virtual CodeAssignStatement GetDefaultAssignment(JsonPropertyInfo info)
        {
            return null;
        }

        /// <summary>
        /// Gets the classes that the property is dependent upon to function.
        /// </summary>
        /// <param name="info">The info for the schema property.</param>
        /// <returns>The collection of classes the schema property is dependent upon.</returns>
        public virtual List<JsonSchemaClass> GetDependentClasses(JsonPropertyInfo info)
        {
            return new List<JsonSchemaClass>();
        }

        /// <summary>
        /// Helper method to create the default assignment expression for a primitive type.
        /// </summary>
        /// <typeparam name="T">The primitive type. All other types will throw an exception.</typeparam>
        /// <param name="fieldName">The name of the field the default value will be assigned too.</param>
        /// <param name="value">The value to assign to the field.</param>
        /// <returns>The default assignment expression.</returns>
        protected CodeAssignStatement CreatePrimitiveDefaultAssignment<T>(string fieldName, T value)
        {
            return new CodeAssignStatement(
                new CodeFieldReferenceExpression(null, fieldName),
                new CodePrimitiveExpression(value));
        }

        /// <summary>
        /// Helper method to create the default assignment expression to create the default value of an object type.
        /// If the type does not have a default constructor the generated class will break.
        /// </summary>
        /// <param name="fieldName">The name of the field the default value will be assigned too.</param>
        /// <param name="type">The type to generate the default constructor.</param>
        /// <returns>The default assignment expression.</returns>
        protected CodeAssignStatement CreateObjectDefaultAssignment(string fieldName, CodeTypeReference type)
        {
            return new CodeAssignStatement(
                new CodeFieldReferenceExpression(null, fieldName),
                new CodeObjectCreateExpression(type));
        }

        /// <summary>
        /// Helper method to create the default assignment for a type that does not easily have its values represented in memory
        /// such as an enumeration.
        /// </summary>
        /// <param name="fieldName">The name fo the field the default value will be assigned too.</param>
        /// <param name="valueText">The text representation of the value.</param>
        /// <returns>The default assignment expression.</returns>
        protected CodeAssignStatement CreateReferenceDefaultAssignment(string fieldName, string valueText)
        {
            return new CodeAssignStatement(
                new CodeFieldReferenceExpression(null, fieldName),
                new CodeTypeReferenceExpression(valueText));
        }
    }
}
