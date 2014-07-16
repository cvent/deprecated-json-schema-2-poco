using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkBinary.SchemaToPoco.Core.ValidationAttributes
{
    public class DistinctArrayAttribute : ValidationAttribute
    {
        public DistinctArrayAttribute()
        {
            ErrorMessage = "All array items must be unique";
        }

        public override bool IsValid(object value)
        {
            if(!value.GetType().IsArray)
                return true;

            //foreach (var i in value)
                return true;
        }
    }
}
