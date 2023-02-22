
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
        /// Запрос на выборку 
        /// </summary>
        /// <param name="reader">IDataReader</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>IEnumerable</returns>
        IEnumerable<T> GetListMonster<T>(IDataReader reader) where T : class;

        /// <summary>
        /// Saving an object in the database (insert or update), returns the number of rows affected
        /// </summary>
      
        int Save<T>(T item) where T : class;

        /// <summary>
        /// Removing an object from the database, return the number of affected rows
        /// </summary>
        int Delete<T>(T item) where T : class;

        /// <summary>
        /// Getting ITransaction with the start of the transaction
        /// </summary>
        ITransaction BeginTransaction();

        /// <summary>
        /// Getting ITransaction with the start of the transaction
        /// </summary>
        ITransaction BeginTransaction(IsolationLevel? value);

        /// <summary>
        /// Create a table
        /// </summary>
        int TableCreate<T>() where T : class;

        /// <summary>
        /// Getting DbCommand
        /// </summary>
        IDbCommand GeDbCommand();

        /// <summary>
        /// Drop table
        /// </summary>
        /// <typeparam name="T">Тип удаляемой таблицы</typeparam>
        int DropTable<T>() where T : class;

        /// <summary>
        /// Checking if a table exists in database
        /// </summary>
        bool TableExists<T>() where T : class;

        /// <summary>
        ///  Getting ExecuteReader
        /// </summary>
        /// <param name="sql">request string</param>
        /// <param name="param">parameter list</param>
        IDataReader ExecuteReader(string sql, params object[] param);

        /// <summary>
        /// Getting ExecuteReader 
        /// </summary>
        /// <param name="sql">request string</param>
        /// <param name="timeOut">timeout connection</param>
        /// <param name="param">parameter list</param>
        IDataReader ExecuteReaderT(string sql, int timeOut = 30, params object[] param);

        /// <summary>
        /// Getting DataTable
        /// </summary>
        /// <param name="sql">request string</param>
        /// <param name="timeout">timeout connection</param>
        DataTable GetDataTable(string sql, int timeout = 30);

        /// <summary>
        /// Getting DataTable
        /// </summary>
        /// <param name="sql">sql text</param>
        /// <param name="timeout">timeout connection</param>
        /// <param name="param">parameter list</param>
        DataTable GetDataTable(string sql, int timeout = 30, params object[] param);

        /// <summary>
        /// Returns a list of table names from the current session database
        /// </summary>
        List<string> GetTableNames();

        /// <summary>
        /// Database creation
        /// </summary>
        int CreateBase(string baseName);

        /// <summary>
        /// Insert bulk from list
        /// </summary>
        int InsertBulk<T>(IEnumerable<T> list, int timeOut = 30) where T : class;

        /// <summary>
        /// Insert bulk to database from file
        /// </summary>
        /// <param name="fileCsv">path to file</param>
        /// <param name="FIELDTERMINATOR">terminator, default - ;</param>
        /// <param name="timeOut">timeout connection</param>
        /// <typeparam name="T">разделитель полей</typeparam>
        int InsertBulkFromFile<T>(string fileCsv, string FIELDTERMINATOR = ";", int timeOut = 30) where T : class;

        /// <summary>
        /// Returns the first element of the request
        /// </summary>
        object ExecuteScalar(string sql, params object[] obj);

        /// <summary>
        /// Возвращает первый элемент запроса с параметром Время ожидания выполнения команды
        /// </summary>
        /// <param name="sql">Строка запроса</param>
        /// <param name="timeOut">Время ожидания выполнения команды (30 сек)</param>
        /// <param name="param">Список параметров в той последовательности в которой они расположены в запросе.</param>
        object ExecuteScalarT(string sql, int timeOut = 30, params object[] param);

        /// <summary>
        /// Recreating a table
        /// </summary>
        int TruncateTable<T>() where T : class;

        /// <summary>
        /// Main point  Linq to Sql
        /// </summary>
        /// <typeparam name="T"></typeparam>
        Query<T> Querion<T>() where T : class;

        /// <summary>
        /// Determines if the object is received from the database, or was created on the client
        /// </summary>
        bool IsPersistent<T>(T obj) where T : class;

        /// <summary>
        /// Making an object persistent ( as object received from database)
        /// </summary>
        void ToPersistent<T>(T obj) where T : class;

        /// <summary>
        /// Запись в лог, если запись в лог включена, при инициализации орм, можно записать текст сообщения пользователя
        /// </summary>
        /// <param name="message">сообщение</param>
        void WriteLogFile(string message);

        /// <summary>
        /// Getting IDbCommand. Does not belong to the current session!
        /// </summary>
        IDbCommand GetCommand();

        /// <summary>
        /// Getting IDbConnection. Does not belong to the current session!
        /// </summary>
        IDbConnection GetConnection();

        /// <summary>
        /// Getting IDbDataAdapter? Dispose manual
        /// </summary>
        IDbDataAdapter GetDataAdapter();


        /// <summary>
        /// Getting the connection string for the current session
        /// </summary>
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
        /// Getting the name of the table to build an sql query.
        /// </summary>
        /// <typeparam name="T">Тип таблицы</typeparam>
        string TableName<T>() where T : class;

        /// <summary>
        /// Getting the field name for a table
        /// </summary>
        string ColumnName<T>(Expression<Func<T, object>> property) where T : class;

        /// <summary>
        /// Getting string SQL for insert command
        /// </summary>
        /// <typeparam name="T">Тип имеющий MapAttributes</typeparam>
        string GetSqlInsertCommand<T>(T t) where T : class;

        /// <summary>
        /// Getting string SQL for delete command
        /// </summary>
        string GetSqlDeleteCommand<T>(T t) where T : class;

        /// <summary>
        /// Cloning an object using JSON
        /// </summary>
        T Clone<T>(T ob) where T : class;

        /// <summary>
        /// Getting string SQL for bulk insert command
        /// </summary>
        /// <param name="enumerable"></param>
        /// <typeparam name="T"></typeparam>
        string GetSqlForInsertBulk<T>(IEnumerable<T> enumerable) where T : class;


        /// <summary>
        /// Write sql query directly to log file
        /// </summary>
        /// <exception cref="Exception"></exception>
        void WriteLogFile(IDbCommand command);
        /// <summary>
        /// Gets a list of table fields
        /// </summary>
        /// <param name="tableName"></param>
        IEnumerable<TableColumn> GetTableColumns(string tableName);
    }
}