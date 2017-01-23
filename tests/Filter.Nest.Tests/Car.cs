using RimDev.Filter.Nest;

namespace Filter.Nest.Tests
{
    public class Car
    {
        [MappingAlias("name")]
        public string Name { get; set; }

        [MappingAlias("year")]
        public int Year { get; set; }

        [MappingAlias("isElectric")]
        public bool IsElectric { get; set; }
    }
}