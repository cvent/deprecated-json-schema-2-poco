using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using ThinkBinary.SchemaToPoco.Core.ValidationAttributes;

namespace Test
{
    public class DataSet
    {

        public Array _results;

        public Array _dimensions;

        public Object _measure;

        [MinValue(2)]
        public virtual Array Results
        {
            get
            {
                return _results;
            }
            set
            {
                _results = value;
            }
        }

        public virtual Array Dimensions
        {
            get
            {
                return _dimensions;
            }
            set
            {
                _dimensions = value;
            }
        }

        public virtual Object Measure
        {
            get
            {
                return _measure;
            }
            set
            {
                _measure = value;
            }
        }
    }
}
