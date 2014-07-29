using NUnit.Framework;

namespace Cvent.SchemaToPoco.Core.UnitTests.FunctionalTests
{
    /// <summary>
    ///     Base test class with generic test cases.
    /// </summary>
    public abstract class BaseTest
    {
        /// <summary>
        ///     Generic test to test equivalence between expected and actual C# code.
        /// </summary>
        /// <param name="expectedJson">Expected C# code.</param>
        /// <param name="actualJson">Generated C# code.</param>
        public void TestBasicEquals(string expectedJson, string actualJson)
        {
            Assert.AreEqual(TestUtils.SanitizeCSharpCode(expectedJson), TestUtils.SanitizeCSharpCode(actualJson));
        }
    }
}
