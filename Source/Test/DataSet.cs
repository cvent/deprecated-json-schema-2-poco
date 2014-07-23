namespace generated
{
    using System;
    using ThinkBinary.SchemaToPoco.Core.ValidationAttributes;
    using System.ComponentModel.DataAnnotations;


    public class DefaultClassName
    {

        public string _foo;

        [Required()]
        [MinLength(2)]
        public virtual string Foo
        {
            get
            {
                return _foo;
            }
            set
            {
                _foo = value;
            }
        }
    }
}