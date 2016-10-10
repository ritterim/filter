using RimDev.Filter.Nest;
using System;
using Xunit;

namespace Filter.Nest.Tests
{
    public class MappingAliasAttributeTests
    {
        public class Constructor : MappingAliasAttributeTests
        {
            [Theory,
                InlineData("valid", false),
                InlineData("", true),
                InlineData(null, true)]
            public void Cannot_pass_null(string alias, bool shouldThrow)
            {
                if (shouldThrow)
                {
                    Assert.Throws<ArgumentNullException>(() => new MappingAliasAttribute(alias));
                }
                else
                {
                    new MappingAliasAttribute(alias);
                }
            }
        }

        public class Alias : MappingAliasAttributeTests
        {
            [Fact]
            public void Can_get_alias_assigned_from_constructor()
            {
                var mappingAlias = new MappingAliasAttribute("valid");

                Assert.Equal("valid", mappingAlias.Alias);
            }
        }
    }
}
