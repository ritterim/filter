using System.Threading.Tasks;

namespace Filter.Nest.Tests
{
    public static class TestHelpers
    {
        public static ElasticsearchInside.Elasticsearch GetReadyElasticsearch()
        {
            var elasticsearch = new ElasticsearchInside.Elasticsearch();

            elasticsearch.ReadySync();

            return elasticsearch;
        }

        public static async Task<ElasticsearchInside.Elasticsearch> GetReadyElasticsearchAsync()
        {
            var elasticsearch = new ElasticsearchInside.Elasticsearch();

            await elasticsearch.Ready();

            return elasticsearch;
        }
    }
}
