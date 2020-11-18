using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace mws_sim
{
    public class Repository<T> : IRepository<T> where T : IDatabaseTable
    {
        private ILogger<Repository<T>> logger;
        private readonly string connectionString;
        public Repository(IConfiguration configuration, ILogger<Repository<T>> logger)
        {
            connectionString = configuration.GetConnectionString("conn");
            this.logger = logger;
        }
        IEnumerable<string> GetFields()
        {
            var result = new List<string>();
            var obj = (T)Activator.CreateInstance(typeof(T));
            Type objType = obj.GetType();
            PropertyInfo[] arrayPropertyInfos = objType.GetProperties();
            foreach (PropertyInfo property in arrayPropertyInfos)
                if (property.DeclaringType.Equals(objType))
                    result.Add(property.Name);
            return result;
        }

        string GetFieldNames()
        {
            var delimiter = ',';
            return "[" + ((List<string>)GetFields()).Aggregate((a, b) => a + "]" + delimiter + "[" + b) + "]";
        }
        string GetSchema()
        {
            var obj = (T)Activator.CreateInstance(typeof(T));
            return ((IDatabaseTable)obj).Schema;
        }
        string GetTableName()
        {
            var obj = (T)Activator.CreateInstance(typeof(T));
            return ((IDatabaseTable)obj).TableName;
        }
        string GetInsertSql()
        {
            var delimiter = ',';
            var _fields = GetFields();
            var columns = ((List<string>)_fields).Select(r => "[" + r + "]").Aggregate((a, b) => a + delimiter + b);
            var fields = ((List<string>)_fields).Select(r => "@" + r).Aggregate((a, b) => a + delimiter + b);
            var sql =
                "insert into " +
                GetSchema() +
                "." +
                GetTableName() +
                " (" +
                columns +
                ") values (" +
                fields +
                ")";
            return sql;
        }
        string GetUpdateSql()
        {
            var delimiter = ',';
            var delim2 = " and ";
            var _fields = GetFields();
            var columnFields = ((List<string>)_fields).Select(r => "[" + r + "]=@" + r).Aggregate((a, b) => a + delimiter + b);
            var condition = ((IDatabaseTable)Activator.CreateInstance(typeof(T))).PivotFields.Select(r => "[" + r + "]=@" + r).Aggregate((a, b) => a + delim2 + b);
            var sql =
                "update " +
                GetSchema() +
                "." +
                GetTableName() +
                " set " +
                columnFields +
                " where " +
                condition;
            return sql;
        }
        string GetDeleteSql()
        {
            var delimiter = " and ";
            var _fields = GetFields();
            var columnFields = ((List<string>)_fields).Select(r => "[" + r + "]=@" + r).Aggregate((a, b) => a + delimiter + b);
            var sql =
                "delete " +
                GetSchema() +
                "." +
                GetTableName() +
                " where " +
                columnFields;
            return sql;
        }
        public virtual T GetById(Guid? id)
        {
            string sql =
                "select 1 IsInflated," +
                GetFieldNames() +
                " from " +
                GetSchema() +
                "." +
                GetTableName() +
                " where Id=@ID";
            using (var conn = new SqlConnection(connectionString))
                return conn.QueryAsync<T>(sql, new { ID = id }).Result.FirstOrDefault();
        }

        public virtual IEnumerable<T> List()
        {
            string sql =
                "select 1 IsInflated," +
                GetFieldNames() +
                " from " +
                GetSchema() +
                "." +
                GetTableName();
            using (var conn = new SqlConnection(connectionString))
                return conn.QueryAsync<T>(sql).Result;
        }

        public virtual IEnumerable<T> List(Func<T, bool> predicate)
        {
            string sql =
                "select 1 IsInflated," +
                GetFieldNames() +
                " from " +
                GetSchema() +
                "." +
                GetTableName();
            using (var conn = new SqlConnection(connectionString))
                return conn.QueryAsync<T>(sql).Result
                   .Where(predicate)
                   .AsEnumerable();
        }
        public void Add(T entity)
        {
            var sql = GetInsertSql();
            logger.LogDebug("Insert SQL/ {0}", sql);
            using (var conn = new SqlConnection(connectionString))
                _ = conn.ExecuteAsync(sql, entity).Result;
        }
        public void Add(IEnumerable<T> entities)
        {
            var sql = GetInsertSql();
            using (var conn = new SqlConnection(connectionString))
                _ = conn.ExecuteAsync(sql, entities).Result;
        }
        public void Edit(T entity)
        {
            var sql = GetUpdateSql();
            using (var conn = new SqlConnection(connectionString))
                _ = conn.ExecuteAsync(sql, entity).Result;
        }
        public void Edit(IEnumerable<T> entities)
        {
            var sql = GetUpdateSql();
            using (var conn = new SqlConnection(connectionString))
                _ = conn.ExecuteAsync(sql, entities).Result;
        }
        public void Delete(T entity)
        {
            var sql = GetDeleteSql();
            using (var conn = new SqlConnection(connectionString))
                _ = conn.ExecuteAsync(sql, entity).Result;
        }
        public void Delete(IEnumerable<T> entities)
        {
            var sql = GetDeleteSql();
            using (var conn = new SqlConnection(connectionString))
                _ = conn.ExecuteAsync(sql, entities).Result;
        }
    }
}
