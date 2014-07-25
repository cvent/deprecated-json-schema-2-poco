using System.CodeDom;

namespace Cvent.SchemaToPoco.Core.Types
{
    /// <summary>
    ///     Wrapper for a CodeDom class.
    /// </summary>
    public class ClassWrapper : BaseWrapper<CodeTypeDeclaration>
    {
        public ClassWrapper(CodeTypeDeclaration cl)
            : base(cl)
        {
            Property.IsClass = true;

            // Create constructor
            Constructor = new CodeConstructor {Attributes = MemberAttributes.Public};
            cl.Members.Add(Constructor);
        }

        /// <summary>
        ///     The constructor for this class.
        /// </summary>
        public CodeConstructor Constructor { get; set; }

        /// <summary>
        ///     Add an interface that this class will implement.
        /// </summary>
        /// <param name="name">Interface name.</param>
        public void AddInterface(string name)
        {
            Property.BaseTypes.Add(new CodeTypeReference(name));
        }

        /// <summary>
        ///     Add a default value to a property.
        /// </summary>
        /// <param name="property">The property name.</param>
        /// <param name="type">The type of the property.</param>
        /// <param name="value">The value to initialize with.</param>
        public void AddDefault(string property, CodeTypeReference type, object value)
        {
            var reference = new CodeFieldReferenceExpression(null, property);
            Constructor.Statements.Add(new CodeAssignStatement(reference, new CodePrimitiveExpression(value)));
        }
    }
}
