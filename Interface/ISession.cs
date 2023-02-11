
using ORM_1_21_.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;

namespace ORM_1_21_
{
    /// <summary>
    /// Base interface
    /// </summary>
    public interface ISession : IDisposable
    {
        /// <summary>
        /// Dispose?
        /// </summary>
        bool IsDispose { get; }

        /// <summary>
        /// Запрос на выборку с параметрами
        /// </summary>
        /// <param name="sqlWhere">запрос на выборку, начиная с where  с параметрами</param>
        /// <param name="param">список параметров в той последовательности в которой они идут в запросе.</param>
        /// <typeparam name="T">Тип сущности</typeparam>
        /// <returns>Перечисление выбранных объектов</returns>
        IEnumerable<T> GetList<T>(string sqlWhere=null, params object[] param) where T : class;

        /// <summary>
        /// Запрос на выборку 
        /// </summary>
        /// <param name="reader">IDataReader</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>IEnumerable</returns>
        IEnumerable<T> GetListMonster<T>(IDataReader reader) where T : class;

        /// <summary>
        /// Сохранение объекта в базе (insert or update), возвращает количество затронутых строк
        /// </summary>
        /// <typeparam name="T">Тип объекта</typeparam>
        /// <param name="item">Сохраняемый объект</param>
        int Save<T>(T item) where T : class;

        /// <summary>
        /// Удаление объекта из базы, возвращаете количество удаленных строк
        /// </summary>
        /// <typeparam name="T">Тип удаляемого объекта</typeparam>
        /// <param name="item">Удаляемый объект</param>
        int Delete<T>(T item) where T : class;

        /// <summary>
        /// Получение объекта ITransaction с одновременно началом транзакции
        /// </summary>
        ITransaction BeginTransaction();

        /// <summary>
        /// Получение объекта ITransaction с одновременно началом транзакции
        /// </summary>
        /// <param name="value">Параметр изоляции транзакции</param>
        ITransaction BeginTransaction(IsolationLevel value);

        /// <summary>
        /// Получение объекта по первичному ключу
        /// </summary>
        /// <typeparam name="T">Тип объекта</typeparam>
        /// <param name="id">Значение первичного ключа</param>
        /// <returns>Полученный объект, в случае отсутствия  в базe - NULL</returns>
        T Get<T>(object id) where T : class;

        /// <summary>
        /// Создание таблицы
        /// </summary>
        /// <typeparam name="T">Тип создаваемой таблицы</typeparam>
        int TableCreate<T>() where T : class;

        /// <summary>
        /// Получение DbCommand в контексте сессии
        /// </summary>
        IDbCommand GeDbCommand();

        /// <summary>
        /// Удаление таблицы
        /// </summary>
        /// <typeparam name="T">Тип удаляемой таблицы</typeparam>
        int DropTable<T>() where T : class;

        /// <summary>
        /// Проверка на существование таблицы в базе
        /// </summary>
        /// <typeparam name="T">Тип проверяемой таблицы</typeparam>
        bool TableExists<T>() where T : class;

        /// <summary>
        ///  Получение результата запрос в виде ExecuteReader
        /// </summary>
        /// <param name="sql">Строка запроса</param>
        /// <param name="param">Список параметров в той последовательности в которой они расположены в запросе.</param>
        IDataReader ExecuteReader(string sql, params object[] param);

        /// <summary>
        /// Получение ExecuteReader  c изменением времени выполнения  команды-запроса
        /// </summary>
        /// <param name="sql">Строка запроса</param>
        /// <param name="timeOut">Время ожидания выполнения команды (30 сек)</param>
        /// <param name="param">Список параметров в той последовательности в которой они расположены в запросе.</param>
        IDataReader ExecuteReaderT(string sql, int timeOut = 30, params object[] param);

        /// <summary>
        /// Получение объекта DataTable
        /// </summary>
        /// <param name="sql">Строка запроса</param>
        /// <param name="timeout">Время ожидания выполнения команды (30 сек)</param>
        DataTable GetDataTable(string sql, int timeout = 30);

        /// <summary>
        /// Получение объекта DataTable
        /// </summary>
        /// <param name="sql">sql text</param>
        /// <param name="timeout">Время ожидания выполнения команды (30 сек)</param>
        /// <param name="param">Список параметров в той последовательности в которой они расположены в запросе.</param>
        DataTable GetDataTable(string sql, int timeout = 30, params object[] param);

        /// <summary>
        ///Возвращает список названия таблиц из  текущей  базы
        /// </summary>
        List<string> GetTableNames();

        /// <summary>
        /// Создание базы данных
        /// </summary>
        /// <param name="baseName">название базы для Mysql  , путь до базы для Postgesql,Sqlite,MSSql</param>
        int CreateBase(string baseName);

        /// <summary>
        /// Вставка в базу пакетная
        /// </summary>
        /// <param name="list">Перечисление вставляемых объектов</param>
        /// <param name="timeOut">Время ожидания выполнения команды (30 сек)</param>
        /// <typeparam name="T">Тип перечисления</typeparam>
        int InsertBulk<T>(IEnumerable<T> list, int timeOut = 30) where T : class;

        /// <summary>
        /// Вставка в базу пакетом, запрос записан в файл
        /// </summary>
        /// <param name="fileCsv">полный путь к файлу, уже записному и готовому для вставки в базу</param>
        /// <param name="FIELDTERMINATOR"></param>
        /// <param name="timeOut"></param>
        /// <typeparam name="T">разделитель полей</typeparam>
        int InsertBulkFromFile<T>(string fileCsv, string FIELDTERMINATOR = ";", int timeOut = 30) where T : class;

        /// <summary>
        /// Возвращает первый элемент запроса
        /// </summary>
        /// <param name="sql">Строка запроса</param>
        /// <param name="obj">параметры запроса</param>
        object ExecuteScalar(string sql, params object[] obj);

        /// <summary>
        /// Возвращает первый элемент запроса с параметром Время ожидания выполнения команды
        /// </summary>
        /// <param name="sql">Строка запроса</param>
        /// <param name="timeOut">Время ожидания выполнения команды (30 сек)</param>
        /// <param name="param">Список параметров в той последовательности в которой они расположены в запросе.</param>
        object ExecuteScalarT(string sql, int timeOut = 30, params object[] param);

        /// <summary>
        /// Пересоздание таблицы
        /// </summary>
        /// <typeparam name="T">Тип пере создаваемой таблицы</typeparam>
        int TruncateTable<T>() where T : class;

        /// <summary>
        /// Выборка через Linq to Sql
        /// </summary>
        /// <typeparam name="T"></typeparam>
        Query<T> Querion<T>() where T : class;

        /// <summary>
        /// Определяет, получен ли объект с базы, или был создан на клиенте
        /// </summary>
        /// <param name="obj">Объект проверяемый</param>
        /// <returns>True -из базы, False - созданный на клиенте</returns>
        bool IsPersistent<T>(T obj) where T : class;

        /// <summary>
        /// Делаем объект персистентным ( как бы объект получен из базы)
        /// </summary>
        /// <param name="obj"></param>
        void ToPersistent<T>(T obj) where T : class;

        /// <summary>
        /// Запись в лог, если запись в лог включена, при инициализации орм, можно записать текст сообщения пользователя
        /// </summary>
        /// <param name="message">сообщение</param>
        void WriteLogFile(string message);

        /// <summary>
        /// Получение автономного IDbCommand, закрывать  вручную
        /// </summary>
        IDbCommand GetCommand();

        /// <summary>
        /// Получение автономного соединения в контексте орм, закрывать и очищать вручную
        /// </summary>
        IDbConnection GetConnection();

        /// <summary>
        /// Получение адаптера, автономного в контексте орм, закрывать и очищать вручную
        /// </summary>
        IDbDataAdapter GetDataAdapter();

        
        /// <summary>
        /// Получение строки соединения в контексте работы орм
        /// </summary>
        /// <returns>Строка соединения</returns>
        string GetConnectionString();

        /// <summary>
        /// выполняет sql-выражение и возвращает количество измененных записей
        /// </summary>
        /// <param name="sql">Строка запроса</param>
        /// <param name="param">Список параметров в той последовательности в которой они расположены в запросе.</param>
        int ExecuteNonQuery(string sql, params object[] param);

        /// <summary>
        /// Выполняет sql-выражение и возвращает количество измененных записей
        /// с параметром время ожидания выполнения команды
        /// </summary>
        /// <param name="sql">Строка запроса</param>
        /// <param name="timeOut">Время ожидания выполнения команды (30 сек)м</param>
        /// <param name="param">Список параметров в той последовательности в которой они расположены в запросе.</param>
        int ExecuteNonQueryT(string sql, int timeOut = 30, params object[] param);

        /// <summary>
        /// Получение названия таблицы, для построения sql запроса.
        /// </summary>
        /// <typeparam name="T">Тип таблицы</typeparam>
        string TableName<T>() where T : class;

        /// <summary>
        /// Возвращает название поля для базы
        /// </summary>
        /// <param name="property"></param>
        /// <typeparam name="T">Тип таблицы</typeparam>
        string ColumnName<T>(Expression<Func<T, object>> property) where T : class;

        /// <summary>
        /// Получает SQL строку Insert
        /// </summary>
        /// <typeparam name="T">Тип имеющий MapAttributes</typeparam>
        string GetSqlInsertCommand<T>(T t) where T : class;

        /// <summary>
        /// Строка запроса на удаление
        /// </summary>
        /// <param name="t"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns>string sql for detete</returns>
        string GetSqlDeleteCommand<T>(T t) where T : class;

        /// <summary>
        /// Клонирование объекта через JSON
        /// </summary>
        /// <typeparam name="T">Тип клонируемого объекта</typeparam>
        /// <param name="ob">Объект для клонирования</param>
        /// <returns>Новый объект</returns>
        T Clone<T>(T ob) where T : class;

        /// <summary>
        /// Получение sql запроса для вставки пакетом
        /// </summary>
        /// <param name="enumerable"></param>
        /// <typeparam name="T"></typeparam>
        string GetSqlForInsertBulk<T>(IEnumerable<T> enumerable) where T : class;
        

        /// <summary>
        /// Писать в лог файл напрямую sql запрос
        /// </summary>
        /// <exception cref="Exception"></exception>
        void WriteLogFile(IDbCommand command);
        /// <summary>
        /// Получает список полей таблицы
        /// </summary>
        /// <param name="tableName"></param>
        IEnumerable<TableColumn> GetTableColumns(string tableName);
    }
}