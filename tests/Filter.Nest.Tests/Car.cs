using System;

namespace Filter.Nest.Tests
{
    public class Car
    {
        public bool IsElectric { get; set; }
        public string Name { get; set; }
        public DateTimeOffset StartProductionRun { get; set; }
        public int Year { get; set; }
    }
}