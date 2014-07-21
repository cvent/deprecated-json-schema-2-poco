using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// Add default imports to the namespace.
        /// </summary>
        private void AddDefaultImports()
        {
            AddImport("System");
        }
    }
}
