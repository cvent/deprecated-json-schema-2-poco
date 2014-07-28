using System.ComponentModel.DataAnnotations;

namespace generated
{
    public class DefaultClassName
    {
        public string _foo;

        [Required]
        [MinLength(2)]
        [RegularExpression(@"^\\\""dev\""\'[a-c]\t$")]
        public virtual string Foo
        {
            get { return _foo; }
            set { _foo = value; }
        }
    }
}
