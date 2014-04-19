using System;
using System.CodeDom;

namespace ThinkBinary.SchemaToPoco.Core
{
	public class JsonSchemaToCodeUnit
	{
		private readonly string _codeNamespace;
		private string _schemaDocument;

		public JsonSchemaToCodeUnit(string schemaDocument, string requestedNamespace)
		{
			if (schemaDocument == null) throw new ArgumentNullException("schemaDocument");

			_schemaDocument = schemaDocument;
			_codeNamespace = requestedNamespace;
		}

		public JsonSchemaToCodeUnit(string schemaDocument) : this(schemaDocument, "")
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