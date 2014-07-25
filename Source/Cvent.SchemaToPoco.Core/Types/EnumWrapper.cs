using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Globalization;

namespace Cvent.SchemaToPoco.Core.Types
{
    /// <summary>
    ///     Wrapper for a CodeDom enum.
    /// </summary>
    public class EnumWrapper : BaseWrapper<CodeTypeDeclaration>
    {
        /// <summary>
        ///     Name of the enum.
        /// </summary>
        private readonly string _name;

        public EnumWrapper(CodeTypeDeclaration cl)
            : base(cl)
        {
            Property.IsEnum = true;
            _name = Property.Name.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        ///     Add a field to the enum.
        /// </summary>
        /// <param name="s"></param>
        public void AddMember(String s)
        {
            Property.Members.Add(new CodeMemberField(_name, s));
        }

        /// <summary>
        ///     Add fields to the enum.
        /// </summary>
        /// <param name="s"></param>
        public void AddMembers(List<string> s)
        {
            foreach (string s2 in s)
            {
                AddMember(s2);
            }
        }
    }
}
