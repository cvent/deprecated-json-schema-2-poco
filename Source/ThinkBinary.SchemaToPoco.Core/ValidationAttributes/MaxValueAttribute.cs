using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkBinary.SchemaToPoco.Core.ValidationAttributes
{
    public class MaxValueAttribute : ValidationAttribute
    {
        private readonly int _maxValue;

        public MaxValueAttribute(int val)
        {
            _maxValue = val;
            ErrorMessage = "Enter a value greater than or equal to " + _maxValue;
        }

        public override bool IsValid(object value)
        {
            return (int)value >= _maxValue;
        }
    }
}
