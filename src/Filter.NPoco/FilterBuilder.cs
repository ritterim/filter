using NPoco;
using System.Collections.Generic;

namespace RimDev.Filter.NPoco
{
    public class FilterBuilder<T>
    {
        public FilterBuilder(
            IDatabase database,
            string sql,
            params object[] parameters)
        {
            this.database = database;
            this.sql = sql;
            this.parameters = parameters;
        }

        private readonly IDatabase database;
        private readonly string sql;
        private readonly object[] parameters;

        public List<T> Fetch(object filter)
        {
            var template = new SqlBuilder()
                .Filter<T>(database, filter)
                .AddTemplate(sql, parameters);

            return database.Fetch<T>(template);
        }
    }
}
