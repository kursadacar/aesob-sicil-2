using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Dynamic;
using CTD.Data.Context;
using Newtonsoft.Json;

namespace CTD.Data.GenericRawSql
{
    public class GenericRawSql<TEntity> : IGenericRawSql<TEntity> where TEntity : class
    {
        private readonly CommonContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public GenericRawSql(CommonContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public virtual DbRawSqlQuery Execute(string connstr, Type a, string sql, params object[] Parameter)
        {
            _context.Database.Connection.ConnectionString = connstr;
            return _context.Database.SqlQuery(a, sql, Parameter);
        }

        public virtual int ExecuteSqlCommand(string connstr, string sql, params object[] Parameter)
        {
            _context.Database.Connection.ConnectionString = connstr;
            return _context.Database.ExecuteSqlCommand(sql, Parameter);
        }

        public virtual List<Dictionary<string, object>> ExecuteSql(string connstr, string sql,
            params object[] Parameters)
        {
            SqlConnection connection = new SqlConnection(connstr);
            connection.Open();
            var cmd = new SqlCommand(sql, connection);
            if (Parameters.Length > 0 && Parameters[0] is int == false && (object[]) Parameters[0] != null &&
                ((object[]) Parameters[0]).Length > 0)
                for (var i = 0; i < ((object[]) Parameters[0]).Length; i++)
                    cmd.Parameters.AddWithValue("@" + i,
                        ((object[]) Parameters[0])[i] == null || ((object[]) Parameters[0])[i] == ""
                            ? DBNull.Value
                            : ((object[]) Parameters[0])[i]);
            else
                for (var i = 0; i < Parameters.Length; i++)
                    cmd.Parameters.AddWithValue("@" + i, Parameters[i] ?? DBNull.Value);
            SqlDataReader reader = cmd.ExecuteReader();
            var expandolist = new List<Dictionary<string, object>>();
            foreach (var item in reader)
            {
                IDictionary<string, object> expando = new ExpandoObject();
                foreach (PropertyDescriptor propertyDescriptor in TypeDescriptor.GetProperties(item))
                {
                    var obj = propertyDescriptor.GetValue(item);
                    expando.Add(propertyDescriptor.Name, obj);
                }

                expandolist.Add(new Dictionary<string, object>(expando));
            }

            reader.Close();
            connection.Close();
            return expandolist;
        }

        public virtual DbRawSqlQuery<T> ExecuteQuery<T>(string connstr, T a, string sql, params object[] Parameter)
        {
            _context.Database.Connection.ConnectionString = connstr;
            return _context.Database.SqlQuery<T>(sql, Parameter);
        }

        public virtual string ExecuteSqlJsonOutput(string connstr, string sql, params object[] Parameter)
        {
            SqlConnection connection = new SqlConnection(connstr);
            connection.Open();
            var cmd = new SqlCommand(sql, connection) {Connection = connection, CommandType = CommandType.Text};
            SqlDataReader reader = cmd.ExecuteReader();
            var dt = new DataTable();
            dt.Load(reader);
            reader.Close();
            connection.Close();
            var rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;
            foreach (DataRow dr in dt.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns) row.Add(col.ColumnName, dr[col]);
                rows.Add(row);
            }

            return JsonConvert.SerializeObject(rows);
        }
    }
}