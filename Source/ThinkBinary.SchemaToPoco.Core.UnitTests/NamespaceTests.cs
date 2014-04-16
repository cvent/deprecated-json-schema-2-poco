using NUnit.Framework;

namespace ThinkBinary.SchemaToPoco.Core.UnitTests
{
	[TestFixture]
	public class NamespaceTests
	{
		[TestCase("SingleLevel")]
		[TestCase("Two.Levels")]
		public void Requested_name_space_is_used_for_generated_code(string requestedNamespace)
		{
			var sut = new JsonSchemaToCodeUnit(requestedNamespace);
			var codeUnit = sut.Execute();

			Assert.That(codeUnit.Namespaces.Count, Is.EqualTo(1));
			Assert.That( codeUnit.Namespaces[0].Name, Is.EqualTo(requestedNamespace));
		}

		[Test]
		public void Blank_namespace_is_used_for_generated_code_when_no_namespace_specified()
		{
			var sut = new JsonSchemaToCodeUnit();
			var codeUnit = sut.Execute();

			Assert.That(codeUnit.Namespaces.Count, Is.EqualTo(1));
			Assert.That(codeUnit.Namespaces[0].Name, Is.EqualTo(""));
		}
	}
}
