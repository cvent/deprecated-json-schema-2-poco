using NUnit.Framework;

namespace Cvent.SchemaToPoco.Core.UnitTests.FunctionalTests
{
    [TestFixture]
    public class MinMaxItems : BaseTest
    {
        [Test]
        public void TestBasic()
        {
            const string schema = @"{
    'type' : 'object',
    'properties' : {
        'foo' : {
            'type' : 'array',
            'items' : {
                'type' : 'string'
            },
            'minItems' : 10,
            'maxItems' : 20
        }
    }
}";
            const string correctResult = @"namespace generated
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;


    public class DefaultClassName
    {

        public List<string> _foo;

        [MinLength(10)]
        [MaxLength(20)]
        public virtual List<string> Foo
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
