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
    class PropertyWrapper : BaseWrapper
    {
        private CodeMemberProperty _property;

        public PropertyWrapper(CodeMemberProperty p)
        {
            _property = p;
        }

        // Add comment to property
        public void AddComment(string s)
        {
            _property.Comments.Add(new CodeCommentStatement(s));
        }

        // Add attribute to property
        public void AddAttribute(string name)
        {
            _property.CustomAttributes.Add(new CodeAttributeDeclaration(name));
        }

        // Add attribute to property
        public void AddAttribute(string name, Object arg)
        {
            _property.CustomAttributes.Add(new CodeAttributeDeclaration(name, new CodeAttributeArgument(new CodePrimitiveExpression(arg))));
        }

        // Add attribute to property
        public void AddAttribute(string name, CodeAttributeArgument args)
        {
            _property.CustomAttributes.Add(new CodeAttributeDeclaration(name, args));
        }

        // Add attribute to property
        public void AddAttribute(string name, CodeAttributeArgument[] args)
        {
            _property.CustomAttributes.Add(new CodeAttributeDeclaration(name, args));
        }

        // Add all comments and attributes
        public void Populate(JsonSchema schema)
        {
//System.Console.WriteLine("schema type: " + schema.Type.Value.ToString());
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

                if (schema.MaximumLength != null)
                    args.Add(new CodeAttributeArgument(new CodePrimitiveExpression(schema.MaximumLength.Value)));

                if (schema.MinimumLength != null)
                    args.Add(new CodeAttributeArgument("MinimumLength", new CodePrimitiveExpression(schema.MinimumLength.Value)));
                
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
