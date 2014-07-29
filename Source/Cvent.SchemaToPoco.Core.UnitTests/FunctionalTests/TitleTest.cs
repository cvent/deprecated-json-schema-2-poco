using NUnit.Framework;

namespace Cvent.SchemaToPoco.Core.UnitTests.FunctionalTests
{
    [TestFixture]
    public class TitleTest : BaseTest
    {
        [Test]
        public void TestBasic()
        {
            const string schema = @"{
    'title' : 'NewClassName',
    'type' : 'object'
}";
            const string correctResult = @"namespace generated
{
    using System;


    public class NewClassName
    {
    }
}";

            TestBasicEquals(correctResult, JsonSchemaToPoco.Generate(schema));
        }
    }
}
