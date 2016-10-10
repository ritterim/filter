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

                    Assert.NotNull(results);
                    Assert.Equal(2, results.Hits.Count());
                    Assert.Equal("Camaro", results.Hits.First().Source.Name);
                    Assert.Equal("Monte Carlo", results.Hits.Last().Source.Name);
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

                    Assert.NotNull(results);
                    Assert.Equal(1, results.Hits.Count());
                    Assert.Equal("Camaro", results.Hits.First().Source.Name);
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

                    Assert.NotNull(noResults);
                    Assert.Equal(0, noResults.Hits.Count());

                    var twoResults = elasticClient
                        .Search<Car>(x => x.Index("vehicles")
                        .PostFilter(new { Name = new[] { "camaro", "monte carlo", "corvette" }, Year = 2000 }));

                    Assert.NotNull(twoResults);
                    Assert.Equal(2, twoResults.Hits.Count());
                    Assert.Equal("Camaro", twoResults.Hits.First().Source.Name);
                    Assert.Equal("Monte Carlo", twoResults.Hits.Last().Source.Name);
                }
            }
        }

        private class Car
        {
            [MappingAlias("name")]
            public string Name { get; set; }

            [MappingAlias("year")]
            public int Year { get; set; }
        }
    }
}
