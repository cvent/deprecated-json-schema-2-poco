using NUnit.Framework;

namespace Cvent.SchemaToPoco.Core.UnitTests.FunctionalTests
{
    [TestFixture]
    public class DescriptionTest : BaseTest
    {
        [Test]
        public void TestBasic()
        {
            const string schema = @"{
    'title' : 'NewClassName',
    'type' : 'object',
    'description' : 'Description for class!',
    'properties' : {
        'foo' : {
            'type' : 'string',
            'description' : 'Description for foo!'
        }
    }
}";
            const string correctResult = @"namespace generated
{
    using System;


    // Description for class!
    public class NewClassName
    {

        // Description for foo!
        public string _foo;

        // Description for foo!
        public virtual string Foo
        {
            get
            {
                return _foo;
            }
            set
            {
                _foo = value;
            }
        }
    }
}";

            TestBasicEquals(correctResult, JsonSchemaToPoco.Generate(schema));
        }
    }
}
