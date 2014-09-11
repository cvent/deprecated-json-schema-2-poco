using System;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace Cvent.SchemaToPoco.Json.Schema
{
    public static class JsonSchemaParser
    {
        public static JsonSchema Parse(Uri schemaUri)
        {
            var referenceSchemaResolver = new JsonSchemaResolver();
            return Parse(schemaUri, referenceSchemaResolver);
        }

        private static JsonSchema Parse(Uri schemaUri, JsonSchemaResolver referenceSchemaResolver)
        {
            var schemaContent = GetJsonFromResource(schemaUri);
            ResolveReferenceSchemas(schemaContent, schemaUri, referenceSchemaResolver);
            return JsonSchema.Parse(schemaContent.ToString(), referenceSchemaResolver);
        }

        private static void ResolveReferenceSchemas(JObject schemaContent, Uri baseSchemaUri, JsonSchemaResolver referenceSchemaResolver)
        {
            var reference = schemaContent.SelectToken("$ref");
            if (reference != null && reference.Type == JTokenType.String)
            {
                var referenceUri = new Uri(reference.Value<string>(), UriKind.RelativeOrAbsolute);
                referenceUri = new Uri(baseSchemaUri, referenceUri);
                schemaContent["$ref"].Replace(JToken.Parse(string.Format(@"""{0}""", referenceUri)));
                if (referenceSchemaResolver.GetSchema(referenceUri.ToString()) == null)
                {
                    Parse(referenceUri, referenceSchemaResolver);    
                }
                return;
            }

            var items = schemaContent.SelectToken("items");
            if (items != null && items.Type == JTokenType.Object)
            {
                ResolveReferenceSchemas((JObject)items, baseSchemaUri, referenceSchemaResolver);
                return;
            }

            if (items != null && items.Type == JTokenType.Array)
            {
                // Todo: Not sure if tupling is needed but could be.
                return;
            }

            var type = schemaContent.SelectToken("type");
            if (type == null || type.Type != JTokenType.String || type.Value<string>() != "object")
            {
                return;
            }

            var properties = schemaContent.SelectToken("properties");
            if (properties == null || properties.Type != JTokenType.Object)
            {
                return;
            }

            var id = schemaContent.SelectToken("id");
            if (id != null && id.Type == JTokenType.String)
            {
                var resolvedUri = new Uri(baseSchemaUri, new Uri(id.Value<string>(), UriKind.RelativeOrAbsolute));
                schemaContent["id"].Replace(JToken.Parse(string.Format(@"""{0}""", resolvedUri)));
            }
            else
            {
                schemaContent.Add("id", JToken.Parse(string.Format(@"""{0}""", baseSchemaUri)));
                id = schemaContent.SelectToken("id");
                
            }

            foreach (var token in properties)
            {
                if (token.Type != JTokenType.Property)
                    continue;

                var property = (JProperty) token;
                if (property.Value.Type != JTokenType.Object)
                    continue;

                //Todo: Need to understand fragments for embedded schemas better.
                var idUri = new Uri(id.Value<string>(), UriKind.Absolute);
                var possibleSchemaUri = AppendPropertyOnFragment(idUri, property.Name);
                ResolveReferenceSchemas((JObject)property.Value, possibleSchemaUri, referenceSchemaResolver);
            }
        }

        private static Uri AppendPropertyOnFragment(Uri uri, string propertyName)
        {
            var fragmentComponent = string.IsNullOrEmpty(uri.Fragment)
                    ? string.Format("#{0}", propertyName)
                    : string.Format("{0}/{1}", uri.Fragment, propertyName);
            return new Uri(uri, new Uri(fragmentComponent, UriKind.Relative));
        }

        private static JObject GetJsonFromResource(Uri jsonResource)
        {
            string schemaText;
            switch (jsonResource.Scheme.ToLower())
            {
                case "file":
                    schemaText = File.ReadAllText(jsonResource.AbsolutePath);
                    break;
                case "http":
                case "https":
                    using (var client = new WebClient())
                    {
                        schemaText = client.DownloadString(jsonResource.ToString());
                    }
                    break;
                case "ftp":
                case "gopher":
                case "idap":
                case "mailto":
                case "net.pipe":
                case "net.tcp":
                case "news":
                case "nntp":
                case "telnet":
                case "uuid":
                    throw new ArgumentException(
                        string.Format(
                            "The uri scheme {0} for the json schema {1} is currently not supported by the application.",
                            jsonResource.Scheme, jsonResource.ToString()));
                default:
                    throw new ArgumentException(
                        string.Format(
                            "An unknown uri scheme {0} for the json schema {1} is currently not supported by the application.",
                            jsonResource.Scheme, jsonResource.ToString()));
            }

            return JObject.Parse(schemaText);
        }
    }
}
