using NPoco;

namespace RimDev.Filter.NPoco
{
    public static class SqlBuilderExtensions
    {
        public static SqlBuilder Filter<T>(
            this SqlBuilder value,
            IDatabase database,
            object filter)
        {
            var pocoData = database.PocoDataFactory.ForType(typeof(T));
            var sqlExpression = database.DatabaseType.ExpressionVisitor<T>(database, pocoData, false);

            sqlExpression = sqlExpression.Filter(filter);

            // The `where`-clause is ultimately added by the caller's SQL via SqlBuilder.AddTemplate.
            var cleansedWhereStatement = sqlExpression.Context.ToWhereStatement()
                ?.Replace("WHERE ", string.Empty)
                ?.Trim();

            if (!string.IsNullOrEmpty(cleansedWhereStatement))
            {
                var sql = new Sql(
                    true,
                    cleansedWhereStatement,
                    sqlExpression.Context.Params);

                value = value.Where(sql.SQL, sql.Arguments);
            }

            return value;
        }
    }
}
