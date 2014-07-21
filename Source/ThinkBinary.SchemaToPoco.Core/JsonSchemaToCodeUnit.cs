using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using ThinkBinary.SchemaToPoco.Core.Types;
using ThinkBinary.SchemaToPoco.Core.Util;
using ThinkBinary.SchemaToPoco.Util;

namespace ThinkBinary.SchemaToPoco.Core
{
    /// <summary>
    /// Model for converting a JsonSchema to a CodeCompileUnit
    /// </summary>
    public class JsonSchemaToCodeUnit
    {
        /// <summary>
        /// The namespace for the document.
        /// </summary>
        private readonly string _codeNamespace;

        /// <summary>
        /// The extended JsonSchema wrapper.
        /// </summary>
        private JsonSchemaWrapper _schemaWrapper;

        /// <summary>
        /// The JsonSchema, for easy access.
        /// </summary>
        private JsonSchema _schemaDocument;

        public JsonSchemaToCodeUnit(JsonSchemaWrapper schema, string requestedNamespace)
        {
            if (schema == null || schema.Schema == null) throw new ArgumentNullException("schemaDocument");

            _schemaWrapper = schema;
            _schemaDocument = schema.Schema;
            _codeNamespace = requestedNamespace;
        }

        public JsonSchemaToCodeUnit(JsonSchemaWrapper schema)
            : this(schema, "")
        {
        }

        /// <summary>
        /// Main executor function.
        /// </summary>
        /// <returns>A CodeCompileUnit.</returns>
        public CodeCompileUnit Execute()
        {
            var codeCompileUnit = new CodeCompileUnit();

            // Set namespace
            NamespaceWrapper nsWrap = new NamespaceWrapper(new CodeNamespace(_codeNamespace));

            // Set class
            CodeTypeDeclaration codeClass = new CodeTypeDeclaration(_schemaDocument.Title);
            codeClass.IsClass = true;
            codeClass.Attributes = MemberAttributes.Public;
            ClassWrapper clWrap = new ClassWrapper(codeClass);

            // Add comments and attributes for class
            clWrap.AddComment(_schemaDocument.Description);

            // Add interfaces
            foreach (string s in _schemaWrapper.Interfaces)
                clWrap.AddInterface(s);

            // Add properties with getters/setters
            if (_schemaDocument.Properties != null)
            {
                foreach (var i in _schemaDocument.Properties)
                {
                    string type = JsonSchemaUtils.getTypeString(i.Value);

                    CodeMemberField field = new CodeMemberField
                    {
                        Attributes = MemberAttributes.Public,
                        Name = "_" + i.Key.ToString(),
                        Type = new CodeTypeReference(type)
                    };

                    // Add comment if not null
                    if(i.Value.Description != null)
                        field.Comments.Add(new CodeCommentStatement(i.Value.Description));
                    
                    clWrap.Property.Members.Add(field);

                    // Add setters/getters
                    CodeMemberProperty property = CreateProperty("_" + i.Key.ToString(), StringUtils.Capitalize(i.Key.ToString()), type);
                    PropertyWrapper prWrap = new PropertyWrapper(property);

                    // Add comments and attributes
                    prWrap.Populate(i.Value);

                    clWrap.Property.Members.Add(property);
                }
            }

            // Add class to namespace
            nsWrap.AddClass(clWrap.Property);
            codeCompileUnit.Namespaces.Add(nsWrap.Namespace);

            return codeCompileUnit;
        }

        /// <summary>
        /// Creates a public property with getters and setters that wrap the 
        /// specified field.
        /// </summary>
        /// <param name="field">The field to get and set.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="type">The type of the property.</param>
        /// <returns>The property.</returns>
        public static CodeMemberProperty CreateProperty(string field, string name, string type)
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
    }
}