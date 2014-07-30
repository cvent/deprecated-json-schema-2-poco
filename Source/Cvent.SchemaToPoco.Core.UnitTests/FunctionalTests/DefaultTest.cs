using NUnit.Framework;

namespace Cvent.SchemaToPoco.Core.UnitTests.FunctionalTests
{
    [TestFixture]
    public class DefaultTest : BaseTest
    {
        [Test]
        public void TestBasic()
        {
            const string schema = @"{
    'type' : 'object',
    'properties' : {
        'foo' : {
            'type': 'string',
            'default': 'hello'
        },
        'number' : {
            'type': 'integer',
            'default': 10
        }
    }
}";
            const string correctResult = @"namespace generated
{
    using System;


    public class DefaultClassName
    {

        private string _foo;

        private int _number;

        public DefaultClassName()
        {
            _foo = ""hello"";
            _number = 10;
        }

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

        public virtual int Number
        {
            get
            {
                return _number;
            }
            set
            {
                _number = value;
            }
        }
    }
}";

            TestBasicEquals(correctResult, JsonSchemaToPoco.Generate(schema));
        }
    }
}
