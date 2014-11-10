using System;
using System.CodeDom;
using System.Linq;
using Cvent.JsonSchema2Poco.Class.Property;
using Newtonsoft.Json.Schema;

namespace Cvent.JsonSchema2Poco.Class
{
    class JsonSchemaClassBuilder
    {
        public JsonSchemaClass Build(JsonSchema schema, string className, string schemaNamespace)
        {
            if (schema.Properties == null && (schema.Enum == null || schema.Enum.Count <= 0))
                return null;

            var jsonClass = new JsonSchemaClass();
            jsonClass.Id = string.IsNullOrEmpty(schema.Id) ? "MissingId" : schema.Id;
            jsonClass.Name = ConvertToPascalCase(schema.Title) ?? ConvertToPascalCase(className);
            jsonClass.Namespace = schemaNamespace;
            jsonClass.Comment = new CodeComment(schema.Description ?? string.Empty);

            if (schema.Enum != null && schema.Enum.Count > 0)
            {
                jsonClass.Name = jsonClass.Name + "Enum";
                var jsonProperty = JsonSchemaProperty.CreateProperty(jsonClass.Namespace, jsonClass.Name, schema);
                if (jsonProperty == null)
                {
                    return null;
                }
                schema.Enum.ToList().ForEach(x => jsonClass.Fields.Add(new CodeMemberField(jsonClass.Name, x.ToString())));
                jsonClass.IsEnum = true;
                return jsonClass;
            }

            if (schema.Properties == null)
                return null;

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
