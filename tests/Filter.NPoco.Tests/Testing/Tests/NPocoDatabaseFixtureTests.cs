using System.Data.SqlClient;
using Xunit;

namespace Filter.NPoco.Tests.Testing.Tests
{
    [Collection(nameof(NPocoDatabaseCollection))]
    public class NPocoDatabaseFixtureTests
    {
        private readonly NPocoDatabaseFixture fixture;

        public NPocoDatabaseFixtureTests(NPocoDatabaseFixture fixture)
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
        public void Person_table_has_data()
        {
            using var connection = fixture.CreateSqlConnection();
            using var command = new SqlCommand("select count(*) from Person;", connection);
            connection.Open();
            var result = (int)command.ExecuteScalar();
            Assert.True(result > 0);
        }
    }
}