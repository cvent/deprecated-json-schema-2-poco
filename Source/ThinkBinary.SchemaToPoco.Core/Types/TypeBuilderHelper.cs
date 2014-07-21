using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThinkBinary.SchemaToPoco.Core.Types
{
    public class TypeBuilderHelper
    {
        private AssemblyBuilder _ab;
        private ModuleBuilder _mb;
        private string _ns;

        public TypeBuilderHelper(string ns)
        {
            _ns = ns;
            Init();
        }

        private void Init()
        {
            AssemblyName name = new AssemblyName();
            name.Name = "TmpAssembly";
            _ab = Thread.GetDomain().DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
            _mb = _ab.DefineDynamicModule("ModuleOne", false);
        }

        public Type GetCustomType(string type)
        {
            return _mb.DefineType(_ns + "." + type).CreateType();
        }
    }
}
