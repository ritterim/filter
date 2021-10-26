namespace Filter.Tests.Common.Testing.Configuration
{
    public class RimDevTestsSqlConfiguration
    {
        public string Hostname { get; set; } = "localhost";
        
        /// <summary>Port that SQL Server will be listening on.  This is kept separate for ease of
        /// use in the build scripts if Docker SQL is used.</summary>
        public int? Port { get; set; } = 1433;
        
        public string InitialCatalog { get; set; } = "master";
        public string UserId { get; set; } = "sa";
        public string Password { get; set; }
        
        public string DataSource => $"{Hostname},{Port}";
    }
}