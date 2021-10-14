using System;

namespace Filter.Tests.Common.Testing.Configuration
{
    public class RimDevTestsElasticsearchConfiguration
    {
        public Uri Uri { get; set; } = new Uri("http://localhost:9201/");
    }
}