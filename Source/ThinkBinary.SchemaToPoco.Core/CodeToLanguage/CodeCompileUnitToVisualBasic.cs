using System.CodeDom;
using Microsoft.VisualBasic;

namespace ThinkBinary.SchemaToPoco.Core.CodeToLanguage
{
	public class CodeCompileUnitToVisualBasic : CodeCompileUnitToLanguageBase
	{
		public CodeCompileUnitToVisualBasic(CodeCompileUnit codeCompileUnit) : base( codeCompileUnit)
		{
		}

		public string Execute()
		{
			using (var codeProvider = new VBCodeProvider())
			{
				return CodeUnitToLanguage(codeProvider);
			}
		}
	}
}