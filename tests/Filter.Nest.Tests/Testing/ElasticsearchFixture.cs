using System;
using System.Security.Cryptography;
using Filter.Tests.Common.Testing.Configuration;
using Nest;

namespace Filter.Nest.Tests.Testing
{
    public class ElasticsearchFixture
    {
        public Uri ElasticsearchUri = ConfigurationHelpers.GetRimDevTestsElasticsearchUri();
        private static readonly RandomNumberGenerator Rng = RandomNumberGenerator.Create();
        
        public ElasticClient GetElasticClient()
        {
            return new ElasticClient(
                new ConnectionSettings(ElasticsearchUri)
                );
        }

        public string GetRandomIndexName(string prefix)
        {
            if (string.IsNullOrWhiteSpace(prefix)) prefix = "idx";
            return $"{prefix}-{GetRandomName()}";
        }
        
        private static string GetRandomName()
        {
            var bytes = new byte[8];
            Rng.GetBytes(bytes);
            return string.Concat(Array.ConvertAll(bytes, x => x.ToString("X2"))).ToLower();
        }
    }
}