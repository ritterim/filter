using MartinCostello.SqlLocalDb;
using System;
using System.Data.Common;

namespace RimDev.Filter.Tests.Generic.Integration
{
    public class DatabaseFixture : IDisposable
    {
        private readonly Lazy<TemporarySqlLocalDbInstance> lazyInstance;

        public DatabaseFixture()
        {
            lazyInstance = new Lazy<TemporarySqlLocalDbInstance>(() =>
            {
                using (var localDb = new SqlLocalDbApi())
                {
                    return localDb.CreateTemporaryInstance(deleteFiles: true);
                }
            });
        }

        public string ConnectionString => lazyInstance.Value.ConnectionString;

        public void Dispose()
        {
            if (lazyInstance.IsValueCreated)
                lazyInstance.Value.Dispose();
        }
    }
}
