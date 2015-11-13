using System;

namespace RimDev.Filter.Tests.Extensions
{
    internal static class LazyExtensions
    {
        public static void Dispose<T>(this Lazy<T> lazyDisposable) where T : IDisposable
        {
            if (lazyDisposable != null && lazyDisposable.IsValueCreated)
            {
                lazyDisposable.Value.Dispose();
            }
        }
    }
}
