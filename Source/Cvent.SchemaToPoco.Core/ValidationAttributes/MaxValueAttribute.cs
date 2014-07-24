using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cvent.SchemaToPoco.Core.ValidationAttributes
{
    /// <summary>
    /// An attribute restricting a maximum value, inclusive for an integer.
    /// </summary>
    public class MaxValueAttribute : ValidationAttribute
    {
        /// <summary>
        /// The maximum value.
        /// </summary>
        private readonly int _maxValue;

        public MaxValueAttribute(int val)
        {
            _maxValue = val;
            ErrorMessage = "Enter a value greater than or equal to " + _maxValue;
        }

        /// <summary>
        /// Check if the given integer is valid.
        /// </summary>
        /// <param name="value">The integer.</param>
        /// <returns>True if it is valid.</returns>
        public override bool IsValid(object value)
        {
            return (int)value >= _maxValue;
        }
    }
}
