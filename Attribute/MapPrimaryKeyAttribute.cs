using System;
using System.Data;
using ORM_1_21_.Utils;

namespace ORM_1_21_
{
    /// <summary>
    ///     Table primary key attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class MapPrimaryKeyAttribute : BaseAttribute
    {
        /// <summary>
        ///     Purpose of the primary key field.
        /// </summary>
        /// <param name="columnName">Column table name</param>
        /// <param name="generator">Primary key type, native-autoincrement ,assigned-user assigned</param>
        public MapPrimaryKeyAttribute(string columnName, Generator generator) : base(columnName)
        {
            if (string.IsNullOrWhiteSpace(columnName)) throw new ArgumentException("column name zero");
            Generator = generator;
        }

        /// <summary>
        ///     Purpose of the primary key field.
        ///     The name of the primary key field, corresponds to the name of the property.
        /// </summary>
        /// <param name="generator"></param>
        public MapPrimaryKeyAttribute(Generator generator)
        {
            Generator = generator;
        }

        internal Type TypeColumn { get; set; }

        internal Generator Generator { get; }

        internal string ColumnNameForRider(ProviderName providerName)
        {
            return UtilsCore.ClearTrim(GetColumnName(providerName));
        }

        internal DbType DbType()
        {
            return DbTypeConverter.ConvertFrom(TypeColumn);
        }
    }

  //  /// <summary>
  //  ///     Additional primary key constraint
  //  /// </summary>
  //  public sealed class MapConstraintKeyAttribute : BaseAttribute
  //  {
  //      /// <summary>
  //      ///     Purpose of the primary key field.
  //      /// </summary>
  //      /// <param name="columnName">Column table name</param>
  //      /// <exception cref="ArgumentException"></exception>
  //      public MapConstraintKeyAttribute(string columnName) : base(columnName)
  //      {
  //          if (string.IsNullOrWhiteSpace(columnName)) throw new ArgumentException("column name zero");
  //      }
  //
  //      /// <summary>
  //      ///     Purpose of the primary key field.
  //      ///     The name of the primary key field, corresponds to the name of the property.
  //      /// </summary>
  //      public MapConstraintKeyAttribute() 
  //      {
  //      }
  //  }
}