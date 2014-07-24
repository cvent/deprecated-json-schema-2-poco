using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cvent.SchemaToPoco.Core.Types
{
    /// <summary>
    /// Wrapper for a CodeDom class.
    /// </summary>
    public class ClassWrapper : BaseWrapper<CodeTypeDeclaration>
    {
        /// <summary>
        /// The constructor for this class.
        /// </summary>
        public CodeConstructor Constructor { get; set; }

        public ClassWrapper(CodeTypeDeclaration cl)
            : base(cl)
        {
            Property.IsClass = true;

            // Create constructor
            Constructor = new CodeConstructor();
            Constructor.Attributes = MemberAttributes.Public;
            cl.Members.Add(Constructor);
        }

        /// <summary>
        /// Add an interface that this class will implement.
        /// </summary>
        /// <param name="name">Interface name.</param>
        public void AddInterface(string name)
        {
            Property.BaseTypes.Add(new CodeTypeReference(name));
        }

        /// <summary>
        /// Add a default value to a property.
        /// </summary>
        /// <param name="property">The property name.</param>
        /// <param name="value">The value to initialize with.</param>
        public void AddDefault(string property, object value)
        {
            CodeFieldReferenceExpression reference = new CodeFieldReferenceExpression(null, property);
            Constructor.Statements.Add(new CodeAssignStatement(reference, new CodePrimitiveExpression(value)));
        }
    }
}
