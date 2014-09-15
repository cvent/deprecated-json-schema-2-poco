using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Schema;
using Cvent.SchemaToPoco.Json.Class.Property.Type;

namespace Cvent.SchemaToPoco.Json.Class.Property
{
    public class JsonPropertyTypeFactory
    {
        public static IJsonPropertyType GetJsonPropertyType(JsonSchema propertyDefinition)
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
                default:
                    return null;
            }
        }
    }
}
