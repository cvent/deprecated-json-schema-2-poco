using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkBinary.SchemaToPoco.Core.Types
{
    public class NamespaceWrapper
    {
        public CodeNamespace Namespace { get; set; }

        public NamespaceWrapper(CodeNamespace ns)
        {
            Namespace = ns;
            AddDefaultImports();
        }

        public void AddImport(string import)
        {
            Namespace.Imports.Add(new CodeNamespaceImport(import));
        }

        public void AddClass(CodeTypeDeclaration cl)
        {
            Namespace.Types.Add(cl);
        }

        private void AddDefaultImports()
        {
            AddImport("System");
        }
    }
}
