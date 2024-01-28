using System;
using System.Linq;
using System.Linq.Expressions;

namespace ORM_1_21_
{
    class V
    {
        public V(string sql)
        {
            _sql = sql;

        }
        private readonly string _sql;

        public string FreeSql()
        {
            return _sql;
        }

        public static bool WhereIn<T>(T d, T[] z)
        {
            return true;
        }
        public static bool WhereNotIn<T>(T d, T[] z)
        {
            return true;
        }

        public static bool WhereString( string sql)
        {
            return true;
        }

        public static IQueryable<T> SelectSqlE<T>(object e,object sql,  SqlParam[] sqlParams)
        {
            var o1 = Expression.Constant(e);
            var o = Expression.Constant(sql);
            var sqlParamsE = Expression.Constant(sqlParams);
            return new EnumerableQuery<T>(Expression.ArrayIndex(o1,o, sqlParamsE));
        }


        public static IQueryable<T> SelectSqlP<T>(string sql, params SqlParam[] sqlParams)
        {
            var sqlE = Expression.Constant(sql);
            var sqlParamsE = Expression.Constant(sqlParams);
            return new EnumerableQuery<T>(Expression.ArrayIndex(sqlE,sqlParamsE));
        }
        public static IQueryable<T> SelectSql<T>(string sql)
        {
            var sqlE = Expression.Constant(sql);
            return new EnumerableQuery<T>(Expression.ArrayIndex(sqlE));
        }

        public static IQueryable<T> FromString<T>(string sql, IQueryable<T> queryable)
        {
            return queryable;
        }

        public static int UpdateSql(Object query, string sql)
        {
            return 0;
        }

        public static int UpdateSqlE(Object query, object sql, SqlParam[] sqlParams)
        {
            return 0;
        }

       
        public static IQueryable<T> FromStringP<T>(string sql, SqlParam[] sqlParams, IQueryable<T> queryable)
        {
            return queryable;
        }

        public static bool WhereSqlE(Object query, object sql, SqlParam[] sqlParams)
        {
            return true;
        }
        
      



        



        public string TableCreate()
        {
            return _sql;
        }

        public string DropTable()
        {
           return _sql;
        }

        public string TableExists()
        {
            return _sql;
        }

        public string ExecuteScalar()
        {
            return _sql;
        }

        public string TruncateTable()
        {
            return _sql;
        }

        public string ExecuteNonQuery()
        {
            return _sql;
        }

        public string DataTable()
        {
            return _sql;
        }
    }

     class VT<T> where T : new()
     {
        /// <summary>
        /// 
        /// </summary>
        public VT()
        {

        }
        public T GeoST_Within()
        {
            return new T();
        }



    }
}