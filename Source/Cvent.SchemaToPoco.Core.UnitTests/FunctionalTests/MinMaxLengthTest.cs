using NUnit.Framework;

namespace Cvent.SchemaToPoco.Core.UnitTests.FunctionalTests
{
    [TestFixture]
    public class MinMaxLengthTest : BaseTest
    {
        [Test]
        public void TestBasic()
        {
            const string schema = @"{
    'type' : 'object',
    'properties' : {
        'foo' : {
            'type' : 'string',
            'maxLength' : 10,
            'minLength' : 2
        }
    }
}";
            const string correctResult = @"namespace generated
{
    using System;
    using System.ComponentModel.DataAnnotations;


    public class DefaultClassName
    {

        public string _foo;

        [StringLength(10, MinimumLength=2)]
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
