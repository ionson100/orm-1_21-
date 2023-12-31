using System;
using System.Linq;

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

        public static IQueryable<T> FromString<T>(string sql, IQueryable<T> queryable)
        {
            return queryable;
        }
        public static IQueryable<T> FromStringP<T>(string sql, SqlParam[] sqlParams, IQueryable<T> queryable)
        {
            return queryable;
        }

        public static bool WhereStringP(string sql,params SqlParam[] sqlParams)
        {
            return true;
        }



        public static bool GeoST_Contains(object s, object d,  bool f)
        {
            return true;
        }
        
        public static bool GeoST_Intersects(object s, object d, bool f)
        {
            return true;
        }

        public static bool GeoST_Disjoint(object s, object d, bool f)
        {
            return true;
        }

        public static bool GeoST_DWithin( object d, object e,int f,bool h)
        {
            return true;
        }

        public static bool GeoST_GeometryType(object o,int t)
        {
            return true;
        }
        public static bool GeoST_Within(object s,object d, bool f)
        {
            return true;
        }

        public static bool GeoST_IsValid(object e, bool f)
        {
            return true;
        }

        public static bool GeoST_Area(object o, int i, bool b)
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