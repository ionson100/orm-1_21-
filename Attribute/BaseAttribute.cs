﻿using System;

namespace ORM_1_21_.Attribute
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public abstract class BaseAttribute : System.Attribute
    {
        private string _columnName;

        /// <summary>
        /// Название колонки в таблице  базы c учетом sql
        /// </summary>
        public string GetColumnName(ProviderName providerName)
        {
         
                switch (providerName)
                {
                    case ProviderName.MsSql:
                        return $"[{_columnName}]";

                    case ProviderName.MySql:
                        return $"`{_columnName}`";
                    case ProviderName.Postgresql:
                        return $"\"{_columnName}\"";

                    case ProviderName.Sqlite:
                        return $"{_columnName}";
                    default:
                        throw new ArgumentOutOfRangeException();
                }
          
           
        }

        /// <summary>
        /// Название колонки в таблице  без учета sql
        /// </summary>
        /// <returns></returns>
        public string GetColumnNameRaw()
        {
            return _columnName;
        }
      

        /// <summary>
        /// Название свойства, которое соответствует данной
        /// колонке в таблице, в классе сущности
        /// </summary>
        internal string PropertyName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnName"></param>
        protected BaseAttribute(string columnName)
        {
            _columnName = Utils.ClearTrim(columnName);
        }

        internal string DefaultValue { get; set; }

        internal string TypeBase { get; set; }

        internal string ColumnNameAlias { get; set; }
        internal Type DeclareType { get; set; }

        internal bool IsBaseKey { get; set; }
        internal bool IsForeignKey { get; set; }


    }

}
