using NUnit.Framework;

namespace Cvent.SchemaToPoco.Core.UnitTests.FunctionalTests
{
    [TestFixture]
    public class ExclusiveMinMaxTest : BaseTest
    {
        [Test]
        public void TestBasic()
        {
            const string schema = @"{
    'type' : 'object',
    'properties' : {
        'foo' : {
            'type' : 'integer',
            'minimum' : 10,
            'maximum' : 15,
            'exclusiveMinimum' : true,
            'exclusiveMaximum' : false
        }
    }
}";
            const string correctResult = @"namespace generated
{
    using System;
    using Cvent.SchemaToPoco.Core.ValidationAttributes;


    public class DefaultClassName
    {

        public int _foo;

        [MinValue(11)]
        [MaxValue(15)]
        public virtual int Foo
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
