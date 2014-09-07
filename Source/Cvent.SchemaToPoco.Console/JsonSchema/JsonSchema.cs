using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace Cvent.SchemaToPoco.JsonSchema
{
    class JsonSchema
    {

        private Newtonsoft.Json.Schema.JsonSchema _schema;
        private Dictionary<Uri, JsonSchema> _dependantSchemas = new Dictionary<Uri, JsonSchema>(); 
        
        public Newtonsoft.Json.Schema.JsonSchema Schema
        {
            get { return _schema; }
        }

        public Dictionary<Uri, JsonSchema> DependantSchemas
        {
            get { return _dependantSchemas; }
        }

        public void ParseFromFile(string filepath)
        {
            var fullpath = Path.GetFullPath(filepath);
            var parentUri = new Uri(Path.GetDirectoryName(fullpath));
            var schemaContent = File.ReadAllText(fullpath);
            var rawSchema = JObject.Parse(schemaContent);
            ParseFromJsonObject(rawSchema, new Uri(filepath),  parentUri);
        }

        public void ParseFromJsonObject(JToken schema, Uri schemaUri, Uri parentUri)
        {
            _dependantSchemas = ResolveDependentSchemas(schema, parentUri);
            var schemaResolver = new JsonSchemaResolver();
            CreateSchemaResolver(schemaResolver, _dependantSchemas);
            Console.WriteLine("{0}", schemaUri);
            _schema = Newtonsoft.Json.Schema.JsonSchema.Parse(schema.ToString(), schemaResolver);
            _schema.Id = _schema.Id ?? schemaUri.ToString();
        }

        private Dictionary<Uri, JsonSchema> ResolveDependentSchemas(JToken schema, Uri parentUri)
        {
            // Check if the JSON is a schema.
            if (schema.Type != JTokenType.Object)
            {
                return new Dictionary<Uri, JsonSchema>();
            }

            var properties = schema.SelectToken("properties");
            if (properties == null || properties.Type != JTokenType.Object)
            {
                return new Dictionary<Uri, JsonSchema>();
            }

            // Determine the URI of the schema. 
            var schemaUri = parentUri;
            var idToken = schema.SelectToken("id");
            if (idToken != null && idToken.Type == JTokenType.String)
            {
                var idUri = new Uri(idToken.Value<string>());
                schemaUri = GetAbsoluteUri(parentUri, idUri);
            }

            var embededReferenceSchemas = new Dictionary<Uri, JsonSchema>();
            foreach (JProperty propertyToken in properties)
            {
                var property =  propertyToken.Value;
                //Console.WriteLine("{0} {1}", property.Path, property.Type);
                if (property.Type != JTokenType.Object)
                {
                    continue;
                }

                bool referenceSchemaAdded = AddReferenceSchema(property, schemaUri, embededReferenceSchemas);
                if (referenceSchemaAdded)
                {
                    continue;
                }

                embededReferenceSchemas = AddEmbeddedReferenceSchemas(property, schemaUri, embededReferenceSchemas);

                var itemsToken = property.SelectToken("items");
                if (itemsToken != null)
                {
                    if (itemsToken.Type == JTokenType.Object)
                    {
                        embededReferenceSchemas = AddEmbeddedReferenceSchemas(itemsToken, schemaUri, embededReferenceSchemas);
                    }
                    else if (itemsToken.Type == JTokenType.Array)
                    {
                        foreach (var arraySchema in itemsToken)
                        {
                            embededReferenceSchemas = AddEmbeddedReferenceSchemas(arraySchema, schemaUri, embededReferenceSchemas);
                        }
                    }
                }
            }

            return embededReferenceSchemas;
        }

        private Dictionary<Uri, JsonSchema> AddEmbeddedReferenceSchemas(JToken property, Uri schemaUri, Dictionary<Uri, JsonSchema> embededReferenceSchemas)
        {
            var propertiesToken = property.SelectToken("properties");
            if (propertiesToken != null && propertiesToken.Type == JTokenType.Object)
            {
                var embeddedSchema = new JsonSchema();
                embeddedSchema.ParseFromJsonObject(property, schemaUri, schemaUri);
                embededReferenceSchemas = embededReferenceSchemas.Concat(
                    embeddedSchema.DependantSchemas.Where(x => !embededReferenceSchemas.ContainsKey(x.Key)))
                    .ToDictionary(x => x.Key, x => x.Value);
            }
            return embededReferenceSchemas;
        }

        private bool AddReferenceSchema(JToken property, Uri schemaUri, Dictionary<Uri, JsonSchema> embededReferenceSchemas)
        {
            // Check for a referenced schema.
            var refToken = property.SelectToken("$ref");
            if (refToken != null && refToken.Type == JTokenType.String)
            {
                var refUri = new Uri(refToken.Value<string>(), UriKind.RelativeOrAbsolute);
                refUri = GetAbsoluteUri(schemaUri, refUri);
                refToken.Replace(JToken.FromObject(refUri.ToString()));
                var referenceSchema = new JsonSchema();
                if (refUri.IsFile)
                {
                    referenceSchema.ParseFromFile(refUri.AbsolutePath);
                    embededReferenceSchemas.Add(refUri, referenceSchema);
                    return true;
                }
            }
            return false;
        }

        private void CreateSchemaResolver(JsonSchemaResolver resolver, Dictionary<Uri, JsonSchema> schemas)
        {
            schemas.ToList().ForEach(x => resolver.LoadedSchemas.Add(x.Value.Schema));
            schemas.ToList().ForEach(x => CreateSchemaResolver(resolver, x.Value.DependantSchemas));
        }

        private Uri GetAbsoluteUri(Uri baseUri, Uri relativeUri)
        {
            if (relativeUri.IsAbsoluteUri)
            {
                return relativeUri;
            }

            if (!baseUri.ToString().EndsWith("/") && !baseUri.ToString().EndsWith(@"\"))
            {
                baseUri = new Uri(baseUri + "/");
            }
            return new Uri(baseUri, relativeUri);
        }
    }
}
