namespace Filter.Tests.Common.Testing.Configuration
{
    public class RimDevTestsSqlConfiguration
    {
        public string DataSource { get; set; } = "localhost,1433";
        public string InitialCatalog { get; set; } = "master";
        public string UserId { get; set; } = "sa";
        public string Password { get; set; }
    }
}