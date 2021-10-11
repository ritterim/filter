using Xunit;

namespace Filter.NPoco.Tests.Testing
{
    /// <summary>This xUnit collection (and the fixture <see cref="NPocoDatabaseFixture"/>
    /// should be used for tests where you want a fresh test database, with all migrations.
    ///</summary>
    [CollectionDefinition(nameof(NPocoDatabaseCollection))]
    public class NPocoDatabaseCollection : ICollectionFixture<NPocoDatabaseFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.        
    }
}