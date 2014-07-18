using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkBinary.SchemaToPoco.Core.Types
{
    public abstract class BaseWrapper<T> where T : CodeTypeMember
    {
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

        // Add comment to property
        public void AddComment(string s)
        {
            _property.Comments.Add(new CodeCommentStatement(s));
        }

        // Add attribute to property
        public void AddAttribute(string name)
        {
            _property.CustomAttributes.Add(new CodeAttributeDeclaration(name));
        }

        // Add attribute to property
        public void AddAttribute(string name, Object arg)
        {
            _property.CustomAttributes.Add(new CodeAttributeDeclaration(name, new CodeAttributeArgument(new CodePrimitiveExpression(arg))));
        }

        // Add attribute to property
        public void AddAttribute(string name, CodeAttributeArgument args)
        {
            _property.CustomAttributes.Add(new CodeAttributeDeclaration(name, args));
        }

        // Add attribute to property
        public void AddAttribute(string name, CodeAttributeArgument[] args)
        {
            _property.CustomAttributes.Add(new CodeAttributeDeclaration(name, args));
        }
    }
}
