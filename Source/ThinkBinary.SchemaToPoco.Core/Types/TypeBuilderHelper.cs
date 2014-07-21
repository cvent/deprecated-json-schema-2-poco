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
    /// <summary>
    /// Type builder to build custom Type objects.
    /// </summary>
    public class TypeBuilderHelper
    {
        private AssemblyBuilder _ab;
        private ModuleBuilder _mb;

        /// <summary>
        /// The common namespace for this builder.
        /// </summary>
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

        /// <summary>
        /// Get a custom Type object.
        /// </summary>
        /// <param name="type">The name of the type.</param>
        /// <returns>A custom Type object.</returns>
        public Type GetCustomType(string type)
        {
            return _mb.DefineType(_ns + "." + type).CreateType();
        }
    }
}
