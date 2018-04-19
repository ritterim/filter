using System;
using System.Data.Common;
using System.Data.SqlLocalDb;

namespace RimDev.Filter.Tests.Generic.Integration
{
    public class DatabaseFixture : IDisposable
    {
        private readonly Lazy<TemporarySqlLocalDbInstance> lazyInstance;

        public DatabaseFixture()
        {
            lazyInstance = new Lazy<TemporarySqlLocalDbInstance>(
                () => TemporarySqlLocalDbInstance.Create(true));
        }

        public DbConnectionStringBuilder CreateConnectionStringBuilder()
        {
            var instance = lazyInstance.Value;

            var builder = lazyInstance.Value.CreateConnectionStringBuilder();
            builder.SetInitialCatalogName(Guid.NewGuid().ToString("N"));

            return builder;
        }

        public string ConnectionString => CreateConnectionStringBuilder().ConnectionString;

        public void Dispose()
        {
            if (lazyInstance.IsValueCreated)
                lazyInstance.Value.Dispose();
        }
    }
}
