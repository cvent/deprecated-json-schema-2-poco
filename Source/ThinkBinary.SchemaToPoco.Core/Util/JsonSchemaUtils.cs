using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkBinary.SchemaToPoco.Core.Util
{
    class JsonSchemaUtils
    {
        public static bool isInteger(JsonSchema schema)
        {
            return schema.Type.Value.ToString().Equals("Integer");
        }

        public static bool isString(JsonSchema schema)
        {
            return schema.Type.Value.ToString().Equals("String");
        }

        public static bool isArray(JsonSchema schema)
        {
            return schema.Type.Value.ToString().Equals("Array");
        }

        public static string getTypeString(JsonSchema schema)
        {
            // Set the type to the type if it is not an array
            if (!JsonSchemaUtils.isArray(schema)) {
                if (schema.Title != null)
                    return schema.Title;
                if (schema.Type != null)
                    return schema.Type.ToString();
            }

            if (JsonSchemaUtils.isArray(schema)) {
                var listWrap = schema.UniqueItems ? "HashSet<" : "List<";

                // Set the type to the title if it exists
                if(schema.Title != null)
                    return listWrap + schema.Title + ">";

                if(schema.Items.Count > 0 && schema.Items[0].Title != null)
                    return listWrap + schema.Items[0].Title + ">";
            }

            // Default type
            return "object";
        }
    }
}
