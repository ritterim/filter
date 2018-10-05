using System.Threading.Tasks;

namespace Filter.Nest.Tests
{
    public static class TestHelpers
    {
        public static async Task<ElasticsearchInside.Elasticsearch> GetReadyElasticsearchAsync()
        {
            var elasticsearch = new ElasticsearchInside.Elasticsearch();

            await elasticsearch.Ready().ConfigureAwait(false);

            return elasticsearch;
        }
    }
}
