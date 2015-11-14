using System;
using System.Threading;
using RimDev.Automation.Sql;
using RimDev.Filter.Tests.Extensions;

namespace RimDev.Filter.Tests.Generic.Integration
{
    public sealed class DatabaseFixture : IDisposable
    {
        private readonly Lazy<LocalDb> lazyDatabase;

        public DatabaseFixture()
        {
            lazyDatabase = new Lazy<LocalDb>(
                () => new LocalDb(), LazyThreadSafetyMode.ExecutionAndPublication);
        }

        public string ConnectionString => lazyDatabase.Value.ConnectionString;
        
        public void Dispose()
        {
            lazyDatabase.Dispose();
        }
    }
}
