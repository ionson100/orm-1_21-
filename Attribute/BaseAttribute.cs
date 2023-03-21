using ORM_1_21_.Utils;
using System;

namespace ORM_1_21_
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public abstract class BaseAttribute : Attribute
    {
        private readonly string _columnName;

        /// <summary>
        /// Field name in the table  as sql
        /// </summary>
        public string GetColumnName(ProviderName providerName)
        {
         
                switch (providerName)
                {
                    case ProviderName.MsSql:
                        return $"[{_columnName}]";

                    case ProviderName.MySql:
                        return $"`{_columnName}`";
                    case ProviderName.PostgreSql:
                        return $"\"{_columnName}\"";

                    case ProviderName.SqLite:
                        return $"{_columnName}";
                    default:
                        throw new ArgumentOutOfRangeException();
                }
          
           
        }

        /// <summary>
        /// Field name in the table raw
        /// </summary>
        /// <returns></returns>
        public string GetColumnNameRaw()
        {
            return _columnName;
        }
        internal bool IsNotUpdateInsert { get; set; }
      
        internal string PropertyName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnName"></param>
        protected BaseAttribute(string columnName)
        {
            if (string.IsNullOrWhiteSpace(columnName)) throw new ArgumentException("column name zero");
            _columnName = UtilsCore.ClearTrim(columnName);
        }

        internal string DefaultValue { get; set; }

        internal string TypeBase { get; set; }

        internal string ColumnNameAlias { get; set; }
        internal Type DeclareType { get; set; }

        internal bool IsBaseKey { get; set; }
        internal bool IsForeignKey { get; set; }


    }

}
