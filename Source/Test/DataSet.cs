namespace generated
{
    using System;
    using ThinkBinary.SchemaToPoco.Core.ValidationAttributes;
    using System.ComponentModel.DataAnnotations;


    public class DefaultClassName
    {

        public string _foo;

        [Required()]
        [StringLength(20, MinimumLength = 10)]
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