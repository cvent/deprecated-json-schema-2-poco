using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using ThinkBinary.SchemaToPoco.Util;

namespace ThinkBinary.SchemaToPoco.Core
{
    public class JsonSchemaToCodeUnit
    {
        private readonly string _codeNamespace;
        private JsonSchema _schemaDocument;
        private JsonSchemaResolver _resolver = new JsonSchemaResolver();
        private List<string> _files = new List<string>();

        public JsonSchemaToCodeUnit(string schemaDocument, string requestedNamespace)
        {
            if (schemaDocument == null) throw new ArgumentNullException("schemaDocument");

            _schemaDocument = LoadSchema(schemaDocument);
            _codeNamespace = requestedNamespace;
        }

        public JsonSchemaToCodeUnit(string schemaDocument)
            : this(schemaDocument, "")
        {
        }

        /**
         * Convert JsonSchema to CodeCompileUnit
         * */
        public CodeCompileUnit Execute()
        {
            var codeCompileUnit = new CodeCompileUnit();

            // Set namespace
            CodeNamespace ns = new CodeNamespace(_codeNamespace);
            codeCompileUnit.Namespaces.Add(ns);

            // Set class
            CodeTypeDeclaration codeClass = new CodeTypeDeclaration(_schemaDocument.Title);
            codeClass.IsClass = true;
            codeClass.Attributes = MemberAttributes.Public;
            ns.Types.Add(codeClass);

            // Add properties with getters/setters
            foreach (var i in _schemaDocument.Properties)
            {
                var type = i.Value.Type.ToString();

                CodeMemberField field = new CodeMemberField
                {
                    Attributes = MemberAttributes.Public,
                    Name = "_" + i.Key.ToString(),
                    Type = new CodeTypeReference(type)
                };

                codeClass.Members.Add(field);

                // Add setters/getters
                codeClass.Members.Add(CreateProperty("_" + i.Key.ToString(), StringUtils.Capitalize(i.Key.ToString()), type));
            }

            return codeCompileUnit;
        }

        private JsonSchema LoadSchema(string file)
        {
            _files.Add(file);

            using (TextReader reader = File.OpenText(file))
            {
                return ResolveSchemas(file, reader);
            }
        }

        /// <summary>
        /// Creates a public property with getters and setters that wrap the 
        /// specified field.
        /// </summary>
        /// <param name="field">The field to get and set.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="type">The type of the property.</param>
        /// <returns></returns>
        static CodeMemberProperty CreateProperty(string field, string name, string type)
        {
            CodeMemberProperty property = new CodeMemberProperty()
            {
                Name = name,
                Type = new CodeTypeReference(type),
                Attributes = MemberAttributes.Public
            };

            property.SetStatements.Add(
              new CodeAssignStatement(
                new CodeFieldReferenceExpression(null, field),
                    new CodePropertySetValueReferenceExpression()));

            property.GetStatements.Add(
              new CodeMethodReturnStatement(
                new CodeFieldReferenceExpression(null, field)));

            return property;
        }

        //
        private JsonSchema ResolveSchemas(string prevPath, TextReader reader)
        {
            string data = reader.ReadToEnd();

            MatchCollection matches = Regex.Matches(data, @"\""\$ref\""\s*:\s*\""(.*.json)\""");
            foreach(Match match in matches) {
                string currPath = Path.GetDirectoryName(prevPath) + @"\" + match.Groups[1].Value;

                if(!_files.Contains(currPath)) {
                    _files.Add(currPath);

                    using (TextReader reader2 = File.OpenText(currPath))
                    {
                        ResolveSchemas(currPath, reader2);
                    }
                }
            }

            JsonSchema ret = JsonSchema.Parse(data, _resolver);
            ret.Id = Path.GetFileName(prevPath);
            return ret;
        }
    }
}