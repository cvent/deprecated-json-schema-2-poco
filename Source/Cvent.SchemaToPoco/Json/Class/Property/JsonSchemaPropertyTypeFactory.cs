using Cvent.SchemaToPoco.Json.Class.Property.Types;
using Newtonsoft.Json.Schema;

namespace Cvent.SchemaToPoco.Json.Class.Property
{
    public class JsonSchemaPropertyTypeFactory
    {
        public static JsonSchemaPropertyType GetJsonPropertyType(JsonSchema propertyDefinition)
        {
            string type;
            if (propertyDefinition.Enum != null)
            {
                type = "enum";
            }
            else
            {
                type = propertyDefinition.Type != null ? propertyDefinition.Type.ToString().ToLower() : "any";
            }
     
            switch (type)
            {
                case "string":
                    return new StringPropertyType();
                case "boolean":
                    return new BooleanPropertyType();
                case "object":
                    return new ObjectPropertyType();
                case "array":
                    return new ArrayPropertyType();
                case "any":
                    return new AnyPropertyType();
                case "enum":
                    return new EnumPropertyType();
                case "integer":
                    return new IntegerPropertyType();
                case "number":
                    return new NumberPropertyType();
                default:
                    return null;
            }
        }
    }
}
