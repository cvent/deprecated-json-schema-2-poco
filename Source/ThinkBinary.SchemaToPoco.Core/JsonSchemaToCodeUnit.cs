using Microsoft.CSharp;
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
            codeClass.Attributes = MemberAttributes.Public;
            ClassWrapper clWrap = new ClassWrapper(codeClass);

            // Add comments and attributes for class
            if(!String.IsNullOrEmpty(_schemaDocument.Description))
                clWrap.AddComment(_schemaDocument.Description);

            // Add interfaces & import
            foreach (Type t in _schemaWrapper.Interfaces)
            {
                clWrap.AddInterface(t.Name);
                nsWrap.AddImport(t.Namespace);
            }

            // Add properties with getters/setters
            if (_schemaDocument.Properties != null)
            {
                foreach (var i in _schemaDocument.Properties)
                {
                    // If it is an enum
                    if (i.Value.Enum != null)
                    {
                        string name = StringUtils.Capitalize(i.Key.ToString());
                        CodeTypeDeclaration enumField = new CodeTypeDeclaration(name);
                        EnumWrapper enumWrap = new EnumWrapper(enumField);

                        // Add comment if not null
                        if (!String.IsNullOrEmpty(i.Value.Description))
                            enumField.Comments.Add(new CodeCommentStatement(i.Value.Description));

                        foreach(var j in i.Value.Enum)
                            enumWrap.AddMember(StringUtils.Sanitize(j.ToString()));

                        // Add to namespace
                        nsWrap.AddClass(enumWrap.Property);
                    }
                    else
                    {
                        JsonSchemaWrapper dependency = _schemaWrapper.GetDependencyFromSchema(i.Value);
                        Type type;
//if(dependency != null)
//System.Console.WriteLine(dependency.FullPath ?? "nope");
                        if(dependency != null)
                            type = JsonSchemaUtils.GetType(dependency);
                        else
                            type = JsonSchemaUtils.GetType(i.Value, _codeNamespace);

                        bool isCustomType = type.Namespace.Equals(_codeNamespace);
                        string strType = String.Empty;

                        // Add imports
                        nsWrap.AddImport(type.Namespace);
                        nsWrap.AddImportsFromSchema(i.Value);

                        // Get the property type
                        if (isCustomType) {
                            if (JsonSchemaUtils.IsArray(i.Value))
                                strType = string.Format("{0}<{1}>", JsonSchemaUtils.GetArrayType(i.Value), type.Name);
                            else
                                strType = type.Name;
                        }
                        else if (JsonSchemaUtils.IsArray(i.Value))
                            strType = string.Format("{0}<{1}>", JsonSchemaUtils.GetArrayType(i.Value), new CSharpCodeProvider().GetTypeOutput(new CodeTypeReference(type)));

                        CodeMemberField field = new CodeMemberField
                        {
                            Attributes = MemberAttributes.Public,
                            Name = "_" + i.Key.ToString(),
                            Type = IsPrimitive(type) && !JsonSchemaUtils.IsArray(i.Value) ? new CodeTypeReference(type) : new CodeTypeReference(strType)
                        };

                        // Add comment if not null
                        if (!String.IsNullOrEmpty(i.Value.Description))
                            field.Comments.Add(new CodeCommentStatement(i.Value.Description));

                        clWrap.Property.Members.Add(field);

                        // Add setters/getters
                        CodeMemberProperty property = CreateProperty("_" + i.Key.ToString(), StringUtils.Capitalize(i.Key.ToString()), field.Type.BaseType);
                        PropertyWrapper prWrap = new PropertyWrapper(property);

                        // Add comments and attributes
                        prWrap.Populate(i.Value);

                        clWrap.Property.Members.Add(property);
                    }
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

        /// <summary>
        /// Check if a type is a primitive, or can be treated like one (ie. lowercased type).
        /// </summary>
        /// <param name="t">The type.</param>
        /// <returns>Whether or not it is a primitive type.</returns>
        private static bool IsPrimitive(Type t)
        {
            return t.IsPrimitive || t == typeof(Decimal) || t == typeof(String) || t == typeof(Object);
        }
    }
}