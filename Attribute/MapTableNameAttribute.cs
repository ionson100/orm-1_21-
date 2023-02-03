using System;
using System.CodeDom;
using System.Linq;

namespace ORM_1_21_.Attribute
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class MapProviderNameAttribute : System.Attribute
    {
        public ProviderName ProviderName { get; }

        public MapProviderNameAttribute(ProviderName providerName)
        {
            ProviderName = providerName;
        }
    }
    /// <summary>
    /// Атрибут для навешивания названия таблицы на класс сущности.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class MapTableNameAttribute : System.Attribute
    {
        private readonly string _tableName;
        private readonly string _sqlWhere;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="tableName">Название таблицы в базе данных</param>
        public MapTableNameAttribute(string tableName)
        {
            _tableName = tableName.Trim(new[] { ' ', '[', ']', '`', '"' });
        }

        ///<summary>
        /// Конструктор Определяет название таблицы, условие для всех выборок
        ///</summary>
        ///<param name="tableName">Название таблицы в базе данных</param>
        ///<param name="sqlWhere">добавление критерия запроса по where пример: "id='1'"</param>
        public MapTableNameAttribute(string tableName, string sqlWhere)
        {
            _tableName = tableName.Trim(new[] { ' ', '[', ']', '`', '"' });
            _sqlWhere = sqlWhere;
        }

        /// <summary>
        /// Название таблицы в базе данных
        /// </summary>
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

    /// <summary>
    /// Подключение к таргированной базе данных
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class MapTableDataBaseAttribute : System.Attribute
    {
        private readonly Type _factory;

        /// <summary>
        /// Тип реализующий IOtherDataBaseFactory, имеет конструктор по умолчанию
        /// </summary>
        /// <param name="factory"></param>
        public MapTableDataBaseAttribute(Type factory)
        {
            var any = factory.GetInterfaces().Any(si => si == typeof(IOtherDataBaseFactory));
            if (any == false)
            {
                throw new Exception($"Тип {factory.Name} не реализует интерфейс IOtherDataBaseFactory");
            }
            var isExistCtor = factory.GetConstructor(Type.EmptyTypes);
            if (isExistCtor == null)
            {
                throw new Exception($"Тип {factory.Name} не имеет конструктора по умолчанию");
            }
            _factory = factory;
        }

        internal Type GetTypeDataBaseFactory()
        {
            return _factory;
        }

    }
}