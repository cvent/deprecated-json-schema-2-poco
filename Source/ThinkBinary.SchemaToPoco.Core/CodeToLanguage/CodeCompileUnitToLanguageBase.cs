using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Text;

namespace ThinkBinary.SchemaToPoco.Core.CodeToLanguage
{
	public class CodeCompileUnitToLanguageBase
	{
		private readonly CodeCompileUnit _codeCompileUnit;

		public CodeCompileUnitToLanguageBase(CodeCompileUnit codeCompileUnit)
		{
			if (codeCompileUnit == null) throw new ArgumentNullException("codeCompileUnit");
			_codeCompileUnit = codeCompileUnit;
		}

		protected string CodeUnitToLanguage(CodeDomProvider codeProvider)
		{
			var stringBuilder = new StringBuilder();
			var stringWriter = new StringWriter(stringBuilder);
			var writer = new IndentedTextWriter(stringWriter, "\t");
			codeProvider.GenerateCodeFromCompileUnit(_codeCompileUnit, writer, new CodeGeneratorOptions
			{
				BlankLinesBetweenMembers = true,
				VerbatimOrder = false,
				BracingStyle = "C"
			});

			var output = stringBuilder.ToString();
			return output;
		}
	}
}