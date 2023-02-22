using ORM_1_21_.Utils;
using System;

namespace ORM_1_21_
{

    /// <summary>
    ///Table names in the database.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class MapTableNameAttribute : System.Attribute
    {
        private readonly string _tableName;
        private readonly string _sqlWhere;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="tableName">Table name</param>
        public MapTableNameAttribute(string tableName)
        {
            if (string.IsNullOrWhiteSpace(tableName)) throw new ArgumentException("table name zero");
            _tableName = UtilsCore.ClearTrim(tableName);
        }

        ///<summary>
        /// Table names in the database.
        ///</summary>
        ///<param name="tableName">table name</param>
        ///<param name="sqlWhere">Adding a criterion (where) will be added to all requests. example- "id=2"</param>
        public MapTableNameAttribute(string tableName, string sqlWhere)
        {
            if (string.IsNullOrWhiteSpace(tableName)) throw new ArgumentException("table name zero");
            _tableName = UtilsCore.ClearTrim(tableName);
            _sqlWhere = sqlWhere;
        }

        internal string TableName(ProviderName providerName)
        {
            switch (providerName)
            {
                case ProviderName.MsSql:
                    return $"[{_tableName}]";
                case ProviderName.MySql:
                    return $"`{_tableName}`";
                case ProviderName.Postgresql:
                    return $"\"{_tableName}\"";
                case ProviderName.Sqlite:
                    return $"{_tableName}";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        internal string SqlWhere => _sqlWhere ?? "";

    }
}
  