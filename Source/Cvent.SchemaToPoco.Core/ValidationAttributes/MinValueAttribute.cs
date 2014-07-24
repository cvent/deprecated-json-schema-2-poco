using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cvent.SchemaToPoco.Core.ValidationAttributes
{
    /// <summary>
    /// An attribute restricting a minimum value, inclusive, for an integer.
    /// </summary>
    public class MinValueAttribute : ValidationAttribute
    {
        /// <summary>
        /// The minimum value.
        /// </summary>
        private readonly int _minValue;

        public MinValueAttribute(int val)
        {
            _minValue = val;
            ErrorMessage = "Enter a value greater than or equal to " + _minValue;
        }

        /// <summary>
        /// Check if the given integer is valid.
        /// </summary>
        /// <param name="value">The integer.</param>
        /// <returns>True if it is valid.</returns>
        public override bool IsValid(object value)
        {
            return (int)value >= _minValue;
        }
    }
}
