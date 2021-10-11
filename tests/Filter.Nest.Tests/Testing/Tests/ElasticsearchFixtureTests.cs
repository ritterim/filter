using Xunit;

namespace Filter.Nest.Tests.Testing.Tests
{
    [Collection(nameof(ElasticsearchCollection))]
    public class ElasticsearchFixtureTests
    {
        private readonly ElasticsearchFixture fixture;

        public ElasticsearchFixtureTests(ElasticsearchFixture fixture)
        {
            this.fixture = fixture;  
        }
        
        [Fact]
        public void ElasticsearchUri_not_null()
        {
            Assert.NotNull(fixture.ElasticsearchUri);
        }

        [Fact]
        public void GetElasticClient_returns_something()
        {
            var connection = fixture.GetElasticClient();
            Assert.NotNull(connection);
        }
    }
}