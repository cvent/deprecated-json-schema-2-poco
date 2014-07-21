using Newtonsoft.Json.Schema;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkBinary.SchemaToPoco.Core.Util;

namespace ThinkBinary.SchemaToPoco.Core.Types
{
    /// <summary>
    /// Wrapper for an automatic property.
    /// </summary>
    class PropertyWrapper : BaseWrapper<CodeMemberProperty>
    {
        public PropertyWrapper(CodeMemberProperty p) : base(p)
        {
        }

        /// <summary>
        /// Add all comments and attributes.
        /// </summary>
        /// <param name="schema">The JsonSchema.</param>
        public void Populate(JsonSchema schema)
        {
            if (schema.Description != null)
                AddComment(schema.Description);

            if (schema.Required != null && schema.Required.Value)
                AddAttribute("Required");

            // Integer only flags
            if (JsonSchemaUtils.isInteger(schema))
            {
                if (schema.Minimum != null)
                {
                    if (schema.ExclusiveMinimum != null)
                    {
                        if (schema.ExclusiveMinimum.Value)
                            AddAttribute("MinValue", schema.Minimum.Value + 1);
                        else
                            AddAttribute("MinValue", schema.Minimum.Value);
                    }
                    else
                        AddAttribute("MinValue", schema.Minimum.Value);
                }
                if (schema.Maximum != null)
                {
                    if (schema.ExclusiveMaximum != null)
                    {
                        if (schema.ExclusiveMaximum.Value)
                            AddAttribute("MaxValue", schema.Maximum.Value - 1);
                        else
                            AddAttribute("MaxValue", schema.Maximum.Value);
                    }
                    else
                        AddAttribute("MinValue", schema.Maximum.Value);
                }
            }

            // String only flags
            if (JsonSchemaUtils.isString(schema))
            {
                List<CodeAttributeArgument> args = new List<CodeAttributeArgument>();
                bool flag = false;

                if (schema.MaximumLength != null)
                {
                    args.Add(new CodeAttributeArgument(new CodePrimitiveExpression(schema.MaximumLength.Value)));
                    flag = true;
                }

                if (schema.MinimumLength != null)
                {
                    args.Add(new CodeAttributeArgument("MinimumLength", new CodePrimitiveExpression(schema.MinimumLength.Value)));
                    flag = true;
                }
                
                if(flag)
                    AddAttribute("StringLength", args.ToArray());
            }

            // Array only flags
            if (JsonSchemaUtils.isArray(schema))
            {
                //if(schema.MinimumItems != null)

            }
        }
    }
}
