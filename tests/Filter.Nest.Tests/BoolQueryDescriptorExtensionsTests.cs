using Nest;
using RimDev.Filter.Nest;
using RimDev.Filter.Range;
using System;
using System.Diagnostics;
using System.Linq;
using Xunit;

namespace Filter.Nest.Tests
{
    public class BoolQueryDescriptorExtensionsTests
    {
        public class Filter : SearchDescriptorExtensionsTests
        {
            [Fact]
            public void Can_query_using_collection()
            {
                using (var elasticsearch = TestHelpers.GetReadyElasticsearch())
                {
                    var elasticClient = new ElasticClient(new ConnectionSettings(elasticsearch.Url));

                    var camaro = new Car { Name = "Camaro" };
                    var corvette = new Car { Name = "Corvette" };
                    var monteCarlo = new Car { Name = "Monte Carlo" };

                    elasticClient.Index(camaro, x => x.Index("vehicles"));
                    elasticClient.Index(corvette, x => x.Index("vehicles"));
                    elasticClient.Index(monteCarlo, x => x.Index("vehicles"));

                    elasticClient.Refresh("vehicles");

                    var results = elasticClient.Search<Car>(s => s.Index("vehicles").Query(
                        q => q.Bool(x => x.Filter(new { Name = new[] { "camaro", "monte carlo" } }))));

                    Assert.Collection(
                        results.Hits.OrderBy(x => x.Source.Name),
                        x => Assert.Equal("Camaro", x.Source.Name),
                        x => Assert.Equal("Monte Carlo", x.Source.Name));
                }
            }

            [Fact]
            public void Can_query_using_single_value()
            {
                using (var elasticsearch = TestHelpers.GetReadyElasticsearch())
                {
                    var elasticClient = new ElasticClient(new ConnectionSettings(elasticsearch.Url));

                    var camaro = new Car { Name = "Camaro" };
                    var corvette = new Car { Name = "Corvette" };
                    var monteCarlo = new Car { Name = "Monte Carlo" };

                    elasticClient.Index(camaro, x => x.Index("vehicles"));
                    elasticClient.Index(corvette, x => x.Index("vehicles"));
                    elasticClient.Index(monteCarlo, x => x.Index("vehicles"));

                    elasticClient.Refresh("vehicles");
                    
                    var results = elasticClient.Search<Car>(s => s.Index("vehicles").Query(
                        q => q.Bool(x => x.Filter(new { Name = new[] { "camaro" } }))));

                    Assert.Equal("Camaro", results.Hits.Single().Source.Name);
                }
            }

            [Fact]
            public void Multiple_filter_properties_queried_as_collection_of_and_operators()
            {
                using (var elasticsearch = TestHelpers.GetReadyElasticsearch())
                {
                    var elasticClient = new ElasticClient(new ConnectionSettings(elasticsearch.Url));

                    var camaro = new Car { Name = "Camaro", Year = 2000 };
                    var corvette = new Car { Name = "Corvette", Year = 2016 };
                    var monteCarlo = new Car { Name = "Monte Carlo", Year = 2000 };

                    elasticClient.Index(camaro, x => x.Index("vehicles"));
                    elasticClient.Index(corvette, x => x.Index("vehicles"));
                    elasticClient.Index(monteCarlo, x => x.Index("vehicles"));

                    elasticClient.Refresh("vehicles");

                    var noResults = elasticClient.Search<Car>(s => s.Index("vehicles").Query(
                        q => q.Bool(x => x.Filter(new { Name = new[] { "camaro", "monte carlo" }, Year = 2016 }))));

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
                using (var elasticsearch = TestHelpers.GetReadyElasticsearch())
                {
                    var elasticClient = new ElasticClient(new ConnectionSettings(elasticsearch.Url));

                    var camaro = new Car { Name = "Camaro", IsElectric = false };
                    var volt = new Car { Name = "Volt", IsElectric = true };

                    elasticClient.Index(camaro, x => x.Index("vehicles"));
                    elasticClient.Index(volt, x => x.Index("vehicles"));

                    elasticClient.Refresh("vehicles");

                    var results = elasticClient.Search<Car>(s => s.Index("vehicles").Query(
                        q => q.MatchAll() && q.Bool(x => x.Filter(new { }))));

                    Assert.Collection(
                        results.Hits.OrderBy(x => x.Source.Name),
                        x => Assert.Equal("Camaro", x.Source.Name),
                        x => Assert.Equal("Volt", x.Source.Name));
                }
            }

            [Fact]
            public void Nullable_boolean_null_returns_expected_results()
            {
                using (var elasticsearch = TestHelpers.GetReadyElasticsearch())
                {
                    var elasticClient = new ElasticClient(new ConnectionSettings(elasticsearch.Url));

                    var camaro = new Car { Name = "Camaro", IsElectric = false };
                    var volt = new Car { Name = "Volt", IsElectric = true };

                    elasticClient.Index(camaro, x => x.Index("vehicles"));
                    elasticClient.Index(volt, x => x.Index("vehicles"));

                    elasticClient.Refresh("vehicles");

                    var results = elasticClient.Search<Car>(s => s.Index("vehicles").Query(
                        q => q.MatchAll() && q.Bool(x => x.Filter(new { IsElectric = (bool?)null }))));

                    Assert.Collection(
                        results.Hits.OrderBy(x => x.Source.Name),
                        x => Assert.Equal("Camaro", x.Source.Name),
                        x => Assert.Equal("Volt", x.Source.Name));
                }
            }

            [Fact]
            public void Nullable_boolean_true_returns_expected_results()
            {
                using (var elasticsearch = TestHelpers.GetReadyElasticsearch())
                {
                    var elasticClient = new ElasticClient(new ConnectionSettings(elasticsearch.Url));

                    var camaro = new Car { Name = "Camaro", IsElectric = false };
                    var volt = new Car { Name = "Volt", IsElectric = true };

                    elasticClient.Index(camaro, x => x.Index("vehicles"));
                    elasticClient.Index(volt, x => x.Index("vehicles"));

                    elasticClient.Refresh("vehicles");

                    var results = elasticClient.Search<Car>(s => s.Index("vehicles").Query(
                        q => q.Bool(x => x.Filter(new { IsElectric = true }))));

                    Assert.Equal("Volt", results.Hits.Single().Source.Name);
                }
            }

            [Fact]
            public void Nullable_boolean_false_returns_expected_results()
            {
                using (var elasticsearch = TestHelpers.GetReadyElasticsearch())
                {
                    var elasticClient = new ElasticClient(new ConnectionSettings(elasticsearch.Url));

                    var camaro = new Car { Name = "Camaro", IsElectric = false };
                    var volt = new Car { Name = "Volt", IsElectric = true };

                    elasticClient.Index(camaro, x => x.Index("vehicles"));
                    elasticClient.Index(volt, x => x.Index("vehicles"));

                    elasticClient.Refresh("vehicles");

                    var results = elasticClient.Search<Car>(s => s.Index("vehicles").Query(
                        q => q.Bool(x => x.Filter(new { IsElectric = false }))));

                    Assert.Equal("Camaro", results.Hits.Single().Source.Name);
                }
            }

            [Theory]
            [InlineData("[2000-01-01,]", 3)]
            [InlineData("[2010-01-01,]", 2)]
            [InlineData("[2020-01-01,]", 1)]
            [InlineData("[2030-01-01,]", 0)]
            [InlineData("[,1999-01-01]", 0)]
            [InlineData("[,2000-01-01]", 1)]
            [InlineData("[,2010-01-01]", 2)]
            [InlineData("[,2020-01-01]", 3)]
            [InlineData("(2000-01-01,]", 2)]
            [InlineData("(2010-01-01,]", 1)]
            [InlineData("(2020-01-01,]", 0)]
            [InlineData("[,2000-01-01)", 0)]
            [InlineData("[,2010-01-01)", 1)]
            [InlineData("[,2020-01-01)", 2)]
            [InlineData("[,2030-01-01)", 3)]
            public void Properly_filters_date_ranges(string startProductionRunRange, int expectedResults)
            {
                using (var elasticsearch = TestHelpers.GetReadyElasticsearch())
                {
                    var elasticClient = new ElasticClient(new ConnectionSettings(elasticsearch.Url).EnableDebugMode(x =>
                    {
                        Debug.WriteLine(x.DebugInformation);
                    }));

                    var camaro = new Car { Name = "Camaro", StartProductionRun = DateTimeOffset.Parse("2020-01-01T00:00:00Z") };
                    var corvette = new Car { Name = "Corvette", StartProductionRun = DateTimeOffset.Parse("2010-01-01T00:00:00Z") };
                    var monteCarlo = new Car { Name = "Monte Carlo", StartProductionRun = DateTimeOffset.Parse("2000-01-01T00:00:00Z") };

                    elasticClient.Index(camaro, x => x.Index("vehicles"));
                    elasticClient.Index(corvette, x => x.Index("vehicles"));
                    elasticClient.Index(monteCarlo, x => x.Index("vehicles"));

                    elasticClient.Refresh("vehicles");

                    var noResults = elasticClient.Search<Car>(s => s.Index("vehicles").Query(
                        q => q.Bool(x => x.Filter(new { StartProductionRun = Range.FromString<DateTime>(startProductionRunRange) }))));

                    Assert.Equal(expectedResults, noResults.Hits.Count());
                }
            }

            [Theory]
            [InlineData("[2000,]", 3)]
            [InlineData("[2010,]", 2)]
            [InlineData("[2020,]", 1)]
            [InlineData("[2030,]", 0)]
            [InlineData("[,1999]", 0)]
            [InlineData("[,2000]", 1)]
            [InlineData("[,2010]", 2)]
            [InlineData("[,2020]", 3)]
            [InlineData("(2000,]", 2)]
            [InlineData("(2010,]", 1)]
            [InlineData("(2020,]", 0)]
            [InlineData("[,2000)", 0)]
            [InlineData("[,2010)", 1)]
            [InlineData("[,2020)", 2)]
            [InlineData("[,2030)", 3)]
            public void Properly_filters_numeric_ranges(string yearRange, int expectedResults)
            {
                using (var elasticsearch = TestHelpers.GetReadyElasticsearch())
                {
                    var elasticClient = new ElasticClient(new ConnectionSettings(elasticsearch.Url).EnableDebugMode(x =>
                    {
                        Debug.WriteLine(x.DebugInformation);
                    }));

                    var camaro = new Car { Name = "Camaro", Year = 2020 };
                    var corvette = new Car { Name = "Corvette", Year = 2010 };
                    var monteCarlo = new Car { Name = "Monte Carlo", Year = 2000 };

                    elasticClient.Index(camaro, x => x.Index("vehicles"));
                    elasticClient.Index(corvette, x => x.Index("vehicles"));
                    elasticClient.Index(monteCarlo, x => x.Index("vehicles"));

                    elasticClient.Refresh("vehicles");

                    var noResults = elasticClient.Search<Car>(s => s.Index("vehicles").Query(
                        q => q.Bool(x => x.Filter(new { Year = Range.FromString<int>(yearRange) }))));

                    Assert.Equal(expectedResults, noResults.Hits.Count());
                }
            }
        }
    }
}