using System.Data.SqlClient;
using Xunit;

namespace Filter.Tests.Testing.Tests
{
    [Collection(nameof(EntityFrameworkDatabaseCollection))]
    public class EntityFrameworkDatabaseFixtureTests
    {
        private readonly EntityFrameworkDatabaseFixture fixture;

        public EntityFrameworkDatabaseFixtureTests(EntityFrameworkDatabaseFixture fixture)
        {
            this.fixture = fixture;  
        }
        
        [Fact]
        public void MasterConnectionStringDebug_not_empty()
        {
            Assert.NotEmpty(fixture.MasterConnectionStringDebug);
        }
        
        [Fact]
        public void ConnectionStringDebug_not_empty()
        {
            Assert.NotEmpty(fixture.ConnectionStringDebug);
        }

        [Fact]
        public void CreateSqlConnection_can_be_used()
        {
            using var connection = fixture.CreateSqlConnection();
            using var command = new SqlCommand("select 46894;", connection);
            connection.Open();
            var result = (int)command.ExecuteScalar();
            Assert.Equal(46894, result);
        }
        
        [Fact]
        public void People_table_has_data()
        {
            using var connection = fixture.CreateSqlConnection();
            using var command = new SqlCommand("select count(*) from People;", connection);
            connection.Open();
            var result = (int)command.ExecuteScalar();
            Assert.True(result > 0);
        }
    }
}