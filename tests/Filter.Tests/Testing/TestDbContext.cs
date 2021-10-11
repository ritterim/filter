using System.Data.Entity;

namespace Filter.Tests.Testing
{
    public sealed class TestDbContext : DbContext
    {
        public TestDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString) { }

        public DbSet<Person> People { get; set; }
    }
}