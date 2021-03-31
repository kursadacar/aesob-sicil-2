using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;

namespace CTD.Data.GenericRawSql
{
    public interface IGenericRawSql<TEntity> where TEntity : class
    {
        DbRawSqlQuery Execute(string connstr, Type a, string sql, params object[] parameter);
        int ExecuteSqlCommand(string connstr, string sql, params object[] parameter);
        List<Dictionary<string, object>> ExecuteSql(string connstr, string sql, params object[] parameters);
        string ExecuteSqlJsonOutput(string connstr, string sql, params object[] parameter);
        DbRawSqlQuery<T> ExecuteQuery<T>(string connstr, T a, string sql, params object[] parameter);
    }
}