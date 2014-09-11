using System.CodeDom;

namespace Cvent.SchemaToPoco.Json.Class.Property.Types
{
    /// <summary>
    /// Represents the Any type of a Json Schema property.
    /// </summary>
    public class AnyPropertyType : JsonSchemaPropertyType
    {
        /// <summary>
        /// The Any type should generally not be used if serializing to POCO. However, to support
        /// it the type is represented as a base object.
        /// </summary>
        /// <see cref="JsonSchemaPropertyType"/>
        public override CodeTypeReference GetType(JsonPropertyInfo info)
        {
            return new CodeTypeReference(typeof(object));
        }
    }
}
