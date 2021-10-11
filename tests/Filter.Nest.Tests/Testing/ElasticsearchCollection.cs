using Xunit;

namespace Filter.Nest.Tests.Testing
{
    /// <summary>This xUnit collection (and the fixture <see cref="ElasticsearchFixture"/>
    /// should be used for tests where you want access to an Elasticsearch instance.
    ///</summary>
    [CollectionDefinition(nameof(ElasticsearchCollection))]
    public class ElasticsearchCollection : ICollectionFixture<ElasticsearchFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.        
    }
}