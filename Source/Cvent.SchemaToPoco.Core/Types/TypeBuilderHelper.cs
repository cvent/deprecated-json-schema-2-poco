using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace Cvent.SchemaToPoco.Core.Types
{
    /// <summary>
    ///     Type builder to build custom Type objects.
    /// </summary>
    public class TypeBuilderHelper
    {
        /// <summary>
        ///     The common namespace for this builder.
        /// </summary>
        private readonly string _ns;

        private AssemblyBuilder _ab;
        private ModuleBuilder _mb;

        public TypeBuilderHelper(string ns)
        {
            _ns = ns;
            Init();
        }

        private void Init()
        {
            var name = new AssemblyName {Name = "TmpAssembly"};
            _ab = Thread.GetDomain().DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
            _mb = _ab.DefineDynamicModule("ModuleOne", false);
        }

        /// <summary>
        ///     Get a custom Type object.
        /// </summary>
        /// <param name="type">The name of the type.</param>
        /// <param name="includeNs">Whether or not to include the namespace.</param>
        /// <returns>A custom Type object.</returns>
        public Type GetCustomType(string type, bool includeNs)
        {
            return _mb.DefineType(includeNs ? _ns + "." + type : type).CreateType();
        }
    }
}
