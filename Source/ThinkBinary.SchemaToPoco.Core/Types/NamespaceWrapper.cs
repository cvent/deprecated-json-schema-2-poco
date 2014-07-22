using Newtonsoft.Json.Schema;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkBinary.SchemaToPoco.Core.Util;

namespace ThinkBinary.SchemaToPoco.Core.Types
{
    /// <summary>
    /// Wrapper for a CodeDom namespace.
    /// </summary>
    public class NamespaceWrapper
    {
        /// <summary>
        /// The namespace.
        /// </summary>
        public CodeNamespace Namespace { get; set; }

        public NamespaceWrapper(CodeNamespace ns)
        {
            Namespace = ns;
            AddDefaultImports();
        }

        /// <summary>
        /// Add an import to the namespace.
        /// </summary>
        /// <param name="import">The namespace to import.</param>
        public void AddImport(string import)
        {
            Namespace.Imports.Add(new CodeNamespaceImport(import));
        }

        /// <summary>
        /// Add a class to the namespace.
        /// </summary>
        /// <param name="cl">The CodeDom class to add.</param>
        public void AddClass(CodeTypeDeclaration cl)
        {
            Namespace.Types.Add(cl);
        }

        /// <summary>
        /// Adds imports for attributes and lists.
        /// </summary>
        /// <param name="schema">The schema to import from.</param>
        public void AddImportsFromSchema(JsonSchema schema)
        {
            // Arrays
            if(JsonSchemaUtils.IsArray(schema))
                AddImport("System.Collections.Generic");

            // MinValue | MaxValue
            if (schema.Minimum != null || schema.Maximum != null)
                AddImport("ThinkBinary.SchemaToPoco.Core.ValidationAttributes");

            // Required | StringLength
            if ((schema.Required != null && schema.Required.Value) || schema.MaximumLength != null || schema.MinimumLength != null)
                AddImport("System.ComponentModel.DataAnnotations");
        }

        /// <summary>
        /// Add imports for dependencies and interfaces.
        /// </summary>
        /// <param name="wrapper"></param>
        public void AddImportsFromWrapper(JsonSchemaWrapper wrapper)
        {
            foreach (var i in wrapper.Interfaces)
                AddImport(i.Namespace);

            foreach (var i in wrapper.Dependencies)
                AddImport(i.Namespace);
        }

        /// <summary>
        /// Add default imports to the namespace.
        /// </summary>
        private void AddDefaultImports()
        {
            AddImport("System");
        }
    }
}
