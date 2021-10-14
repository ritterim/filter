using Xunit;

namespace Filter.Tests.Testing
{
    /// <summary>This xUnit collection (and the fixture <see cref="EntityFrameworkDatabaseFixture"/>
    /// should be used for tests where you want a fresh test database, with all migrations.
    ///</summary>
    [CollectionDefinition(nameof(EntityFrameworkDatabaseCollection))]
    public class EntityFrameworkDatabaseCollection : ICollectionFixture<EntityFrameworkDatabaseFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.        
    }
}