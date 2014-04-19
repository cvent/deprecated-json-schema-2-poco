using System.CodeDom;

namespace ThinkBinary.SchemaToPoco.Core
{
	public class JsonSchemaToCodeUnit
	{
		private string _codeNamespace;

		public JsonSchemaToCodeUnit(string requestedNamespace)
		{
			_codeNamespace = requestedNamespace;
		}

		public JsonSchemaToCodeUnit() : this("")
		{
		}

		public CodeCompileUnit Execute()
		{
			var codeCompileUnit =  new CodeCompileUnit();
			codeCompileUnit.Namespaces.Add(new CodeNamespace(_codeNamespace));
			
			return codeCompileUnit;
		}
	}
}