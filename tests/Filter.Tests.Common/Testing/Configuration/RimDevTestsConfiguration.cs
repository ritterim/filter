namespace Filter.Tests.Common.Testing.Configuration
{
    public class RimDevTestsConfiguration
    {
        public RimDevTestsSqlConfiguration Sql { get; set; } 
            = new RimDevTestsSqlConfiguration();
        
        public RimDevTestsElasticsearchConfiguration Elasticsearch { get; set; } 
            = new RimDevTestsElasticsearchConfiguration();
    }
}