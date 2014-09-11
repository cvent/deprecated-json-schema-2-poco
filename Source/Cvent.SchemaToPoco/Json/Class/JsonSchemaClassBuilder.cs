using System;
using System.CodeDom;
using Cvent.SchemaToPoco.Json.Class.Property;
using Newtonsoft.Json.Schema;

namespace Cvent.SchemaToPoco.Json.Class
{
    class JsonSchemaClassBuilder
    {
        public JsonSchemaClass Build(JsonSchema schema, string className, string schemaNamespace)
        {
            var jsonClass = new JsonSchemaClass();
            jsonClass.Id = string.IsNullOrEmpty(schema.Id) ? "MissingId" : schema.Id;
            jsonClass.Name = ConvertToPascalCase(schema.Title) ?? ConvertToPascalCase(className);
            jsonClass.Namespace = schemaNamespace;
            jsonClass.Comment = new CodeComment(schema.Description ?? string.Empty);

            foreach (var schemaProperty in schema.Properties)
            {
                var jsonProperty = JsonSchemaProperty.CreateProperty(schemaNamespace, schemaProperty.Key, schemaProperty.Value);
                if (jsonProperty == null)
                    continue;
                
                foreach (CodeNamespaceImport requiredImport in jsonProperty.RequiredImports)
                {
                    jsonClass.Imports.Add(requiredImport);
                }

                jsonClass.Fields.Add(jsonProperty.Field);
                jsonClass.Properties.Add(jsonProperty.Property);

                if (jsonProperty.Default != null)
                {
                    jsonClass.PropertyDefaults.Add(jsonProperty.Default);
                }

                jsonClass.DependentClasses.AddRange(jsonProperty.DependentClasses);
                jsonClass.EmbeddedTypes.AddRange(jsonProperty.EmbeddedTypes);
            }

            return jsonClass;
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
