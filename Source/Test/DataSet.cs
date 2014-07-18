using System;

namespace Test
{
    public class DataSet
    {

        public Array _results;

        public Array _dimensions;

        public Object _measure;

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
