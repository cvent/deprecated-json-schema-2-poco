using System.CodeDom;
using Microsoft.CSharp;

namespace ThinkBinary.SchemaToPoco.Core.CodeToLanguage
{
	public class CodeCompileUnitToCSharp : CodeCompileUnitToLanguageBase
	{

		public CodeCompileUnitToCSharp(CodeCompileUnit codeCompileUnit)	: base(codeCompileUnit)
		{
		}

		public string Execute()
		{
			using (var codeProvider = new CSharpCodeProvider())
			{
				return CodeUnitToLanguage(codeProvider);
			}
		}
	}
}
