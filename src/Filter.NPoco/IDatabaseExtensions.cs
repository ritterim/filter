using NPoco;

namespace RimDev.Filter.NPoco
{
    public static class IDatabaseExtensions
    {
        public static FilterBuilder<T> With<T>(
            this IDatabase database,
            string template,
            params object[] parameters)
        {
            return new FilterBuilder<T>(database, template, parameters);
        }
    }
}
