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
        public void AddDefault(string property, CodeTypeReference type, string value)
        {
            // Create constructor if doesn't already exist
            if (Constructor == null)
            {
                Constructor = new CodeConstructor {Attributes = MemberAttributes.Public};
                Property.Members.Add(Constructor);
            }

            var reference = new CodeFieldReferenceExpression(null, property);
            CodeExpression exp;
            int n;
            double m;
            bool b;

            // Check for int
            if (int.TryParse(value, out n))
            {
                exp = new CodePrimitiveExpression(n);
            }
            // Check for double
            else if (double.TryParse(value, out m))
            {
                exp = new CodePrimitiveExpression(m);
            }
            // Check for bool
            else if (bool.TryParse(value, out b))
            {
                exp = new CodePrimitiveExpression(b);
            }
            // Check for {}
            else if (value.Equals("{}"))
            {
                exp = new CodeObjectCreateExpression(type);
            }
            else
            {
                exp = new CodePrimitiveExpression(value);
            }

            Constructor.Statements.Add(new CodeAssignStatement(reference, exp));
        }
    }
}
