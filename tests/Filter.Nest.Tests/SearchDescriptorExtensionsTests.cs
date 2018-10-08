using Nest;
using RimDev.Filter.Nest;
using System.Linq;
using Xunit;

namespace Filter.Nest.Tests
{
    public class SearchDescriptorExtensionsTests
    {
        public class PostFilter : SearchDescriptorExtensionsTests
        {
            [Fact]
            public void Can_query_using_collection()
            {
                using (var elasticsearch = new ElasticsearchInside.Elasticsearch())
                {
                    var elasticClient = new ElasticClient(new ConnectionSettings(elasticsearch.Url));

                    var camaro = new Car { Name = "Camaro" };
                    var corvette = new Car { Name = "Corvette" };
                    var monteCarlo = new Car { Name = "Monte Carlo" };

                    elasticClient.Index(camaro, x => x.Index("vehicles"));
                    elasticClient.Index(corvette, x => x.Index("vehicles"));
                    elasticClient.Index(monteCarlo, x => x.Index("vehicles"));

                    elasticClient.Refresh("vehicles");

                    var results = elasticClient
                        .Search<Car>(x => x.Index("vehicles")
                        .PostFilter(new { Name = new[] { "camaro", "monte carlo" } }));

                    Assert.Collection(
                        results.Hits.OrderBy(x => x.Source.Name),
                        x => Assert.Equal("Camaro", x.Source.Name),
                        x => Assert.Equal("Monte Carlo", x.Source.Name));
                }
            }

            [Fact]
            public void Can_query_using_single_value()
            {
                using (var elasticsearch = new ElasticsearchInside.Elasticsearch())
                {
                    var elasticClient = new ElasticClient(new ConnectionSettings(elasticsearch.Url));

                    var camaro = new Car { Name = "Camaro" };
                    var corvette = new Car { Name = "Corvette" };
                    var monteCarlo = new Car { Name = "Monte Carlo" };

                    elasticClient.Index(camaro, x => x.Index("vehicles"));
                    elasticClient.Index(corvette, x => x.Index("vehicles"));
                    elasticClient.Index(monteCarlo, x => x.Index("vehicles"));

                    elasticClient.Refresh("vehicles");

                    var results = elasticClient
                        .Search<Car>(x => x.Index("vehicles")
                        .PostFilter(new { Name = "camaro" }));

                    Assert.Equal("Camaro", results.Hits.Single().Source.Name);
                }
            }

            [Fact]
            public void Multiple_filter_properties_queried_as_collection_of_and_operators()
            {
                using (var elasticsearch = new ElasticsearchInside.Elasticsearch())
                {
                    var elasticClient = new ElasticClient(new ConnectionSettings(elasticsearch.Url));

                    var camaro = new Car { Name = "Camaro", Year = 2000 };
                    var corvette = new Car { Name = "Corvette", Year = 2016 };
                    var monteCarlo = new Car { Name = "Monte Carlo", Year = 2000 };

                    elasticClient.Index(camaro, x => x.Index("vehicles"));
                    elasticClient.Index(corvette, x => x.Index("vehicles"));
                    elasticClient.Index(monteCarlo, x => x.Index("vehicles"));

                    elasticClient.Refresh("vehicles");

                    var noResults = elasticClient
                        .Search<Car>(x => x.Index("vehicles")
                        .PostFilter(new { Name = new[] { "camaro", "monte carlo" }, Year = 2016 }));

                    Assert.Empty(noResults.Hits);

                    var twoResults = elasticClient
                        .Search<Car>(x => x.Index("vehicles")
                        .PostFilter(new { Name = new[] { "camaro", "monte carlo", "corvette" }, Year = 2000 }));

                    Assert.Collection(
                        twoResults.Hits.OrderBy(x => x.Source.Name),
                        x => Assert.Equal("Camaro", x.Source.Name),
                        x => Assert.Equal("Monte Carlo", x.Source.Name));
                }
            }

            [Fact]
            public void Nullable_boolean_omitted_returns_expected_results()
            {
                using (var elasticsearch = new ElasticsearchInside.Elasticsearch())
                {
                    var elasticClient = new ElasticClient(new ConnectionSettings(elasticsearch.Url));

                    var camaro = new Car { Name = "Camaro", IsElectric = false };
                    var volt = new Car { Name = "Volt", IsElectric = true };

                    elasticClient.Index(camaro, x => x.Index("vehicles"));
                    elasticClient.Index(volt, x => x.Index("vehicles"));

                    elasticClient.Refresh("vehicles");

                    var results = elasticClient
                        .Search<Car>(x => x.Index("vehicles")
                        .PostFilter(new { }));

                    Assert.Collection(
                        results.Hits.OrderBy(x => x.Source.Name),
                        x => Assert.Equal("Camaro", x.Source.Name),
                        x => Assert.Equal("Volt", x.Source.Name));
                }
            }

            [Fact]
            public void Nullable_boolean_null_returns_expected_results()
            {
                using (var elasticsearch = new ElasticsearchInside.Elasticsearch())
                {
                    var elasticClient = new ElasticClient(new ConnectionSettings(elasticsearch.Url));

                    var camaro = new Car { Name = "Camaro", IsElectric = false };
                    var volt = new Car { Name = "Volt", IsElectric = true };

                    elasticClient.Index(camaro, x => x.Index("vehicles"));
                    elasticClient.Index(volt, x => x.Index("vehicles"));

                    elasticClient.Refresh("vehicles");

                    var results = elasticClient
                        .Search<Car>(x => x.Index("vehicles")
                        .PostFilter(new { IsElectric = (bool?)null }));

                    Assert.Collection(
                        results.Hits.OrderBy(x => x.Source.Name),
                        x => Assert.Equal("Camaro", x.Source.Name),
                        x => Assert.Equal("Volt", x.Source.Name));
                }
            }

            [Fact]
            public void Nullable_boolean_true_returns_expected_results()
            {
                using (var elasticsearch = new ElasticsearchInside.Elasticsearch())
                {
                    var elasticClient = new ElasticClient(new ConnectionSettings(elasticsearch.Url));

                    var camaro = new Car { Name = "Camaro", IsElectric = false };
                    var volt = new Car { Name = "Volt", IsElectric = true };

                    elasticClient.Index(camaro, x => x.Index("vehicles"));
                    elasticClient.Index(volt, x => x.Index("vehicles"));

                    elasticClient.Refresh("vehicles");

                    var results = elasticClient
                        .Search<Car>(x => x.Index("vehicles")
                        .PostFilter(new { IsElectric = true }));

                    Assert.Equal("Volt", results.Hits.Single().Source.Name);
                }
            }

            [Fact]
            public void Nullable_boolean_false_returns_expected_results()
            {
                using (var elasticsearch = new ElasticsearchInside.Elasticsearch())
                {
                    var elasticClient = new ElasticClient(new ConnectionSettings(elasticsearch.Url));

                    var camaro = new Car { Name = "Camaro", IsElectric = false };
                    var volt = new Car { Name = "Volt", IsElectric = true };

                    elasticClient.Index(camaro, x => x.Index("vehicles"));
                    elasticClient.Index(volt, x => x.Index("vehicles"));

                    elasticClient.Refresh("vehicles");

                    var results = elasticClient
                        .Search<Car>(x => x.Index("vehicles")
                        .PostFilter(new { IsElectric = false }));

                    Assert.Equal("Camaro", results.Hits.Single().Source.Name);
                }
            }
        }
    }
}
