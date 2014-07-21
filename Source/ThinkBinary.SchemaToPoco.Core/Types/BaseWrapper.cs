using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkBinary.SchemaToPoco.Core.Types
{
    /// <summary>
    /// Generic wrapper for property-type objects ie. classes and properties.
    /// </summary>
    /// <typeparam name="T">Type of the property.</typeparam>
    public abstract class BaseWrapper<T> where T : CodeTypeMember
    {
        /// <summary>
        /// The property.
        /// </summary>
        private T _property;

        public T Property
        {
            get
            {
                return _property;
            }
            set
            {
                _property = value;
            }
        }

        public BaseWrapper(T member)
        {
            _property = member;
        }

        /// <summary>
        /// Add comment to property.
        /// </summary>
        /// <param name="s">The comment.</param>
        public void AddComment(string s)
        {
            _property.Comments.Add(new CodeCommentStatement(s));
        }

        /// <summary>
        /// Add attribute to property.
        /// </summary>
        /// <param name="name">The attribute.</param>
        public void AddAttribute(string name)
        {
            _property.CustomAttributes.Add(new CodeAttributeDeclaration(name));
        }

        /// <summary>
        /// Add attribute to property with an argument.
        /// </summary>
        /// <param name="name">The attribute.</param>
        public void AddAttribute(string name, Object arg)
        {
            _property.CustomAttributes.Add(new CodeAttributeDeclaration(name, new CodeAttributeArgument(new CodePrimitiveExpression(arg))));
        }

        /// <summary>
        /// Add attribute to property with an argument.
        /// </summary>
        /// <param name="name">The attribute.</param>
        public void AddAttribute(string name, CodeAttributeArgument args)
        {
            _property.CustomAttributes.Add(new CodeAttributeDeclaration(name, args));
        }

        /// <summary>
        /// Add attribute to property with a list of arguments.
        /// </summary>
        /// <param name="name">The attribute.</param>
        public void AddAttribute(string name, CodeAttributeArgument[] args)
        {
            _property.CustomAttributes.Add(new CodeAttributeDeclaration(name, args));
        }
    }
}
