using System;

namespace RimDev.Filter.Nest
{
    public class MappingAliasAttribute : Attribute
    {
        public MappingAliasAttribute(string alias)
        {
            if (string.IsNullOrEmpty(alias))
            {
                throw new ArgumentNullException(nameof(alias));
            }

            Alias = alias;
        }

        public string Alias { get; private set; }
    }
}
