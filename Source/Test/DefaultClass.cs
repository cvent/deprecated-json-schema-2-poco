using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Test
{
    class DefaultClass
    {
        private List<string> _foo;

        private string _str;

        public virtual List<string> Foo
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

        [Required()]
        [StringLength(10)]
        [JsonProperty(PropertyName = "str")]
        public virtual string Str
        {
            get { return _str; }
            set { _str = value; }
        }
    }
}
