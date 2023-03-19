
using ORM_1_21_.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

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
        /// Request for selection from IDataReader
        /// </summary>
        /// <param name="reader">IDataReader</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>IEnumerable</returns>
        IEnumerable<T> GetListMonster<T>(IDataReader reader) where T : class;

        /// <summary>
        /// Saving an object in the database (insert or update), returns the number of rows affected
        /// </summary>

        int Save<TSource>(TSource source) where TSource : class;

        /// <summary>
        /// Removing an object from the database, return the number of affected rows
        /// </summary>
        int Delete<TSource>(TSource source) where TSource : class;

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
        int TableCreate<TSource>() where TSource : class;

        /// <summary>
        /// Getting DbCommand
        /// </summary>
        IDbCommand GeDbCommand();

        /// <summary>
        /// Drop table
        /// </summary>
        int DropTable<TSource>() where TSource : class;

        /// <summary>
        /// Drop table
        /// </summary>
        int DropTableIfExists<TSource>() where TSource : class;

        /// <summary>
        /// Checking if a table exists in database
        /// </summary>
        bool TableExists<TSource>() where TSource : class;

        /// <summary>
        ///  Getting ExecuteReader
        /// </summary>
        /// <param name="sql">request string</param>
        /// <param name="params">parameters array( param name: mysql-?,postgresql-@,mssql-@,sqlite-@)</param>
        IDataReader ExecuteReader(string sql, params object[] @params);

        /// <summary>
        /// Getting ExecuteReader 
        /// </summary>
        /// <param name="sql">request string</param>
        /// <param name="timeOut">timeout connection</param>
        /// <param name="param">parameters array (param name: mysql-?,postgresql-@,mssql-@,sqlite-@)</param>
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
        /// <param name="param">parameters array (param name: mysql-?,postgresql-@,mssql-@,sqlite-@)</param>
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
        int InsertBulk<TSource>(IEnumerable<TSource> list, int timeOut = 30) where TSource : class;

        /// <summary>
        /// Insert bulk from list
        /// </summary>
        Task<int> InsertBulkAsync<TSource>(IEnumerable<TSource> list, int timeOut,CancellationToken cancellationToken=default) where TSource : class;



        /// <summary>
        /// Insert bulk to database from file
        /// </summary>
        /// <param name="fileCsv">path to file</param>
        /// <param name="FIELDTERMINATOR">terminator, default - ;</param>
        /// <param name="timeOut">timeout connection</param>
        int InsertBulkFromFile<T>(string fileCsv, string FIELDTERMINATOR = ";", int timeOut = 30) where T : class;


        /// <summary>
        /// Returns the first element of the request
        /// </summary>
        /// <param name="sql">sql text</param>
        /// <param name="param">parameters array (param name: mysql-?,postgresql-@,mssql-@,sqlite-@)</param>
        /// <returns></returns>
        object ExecuteScalar(string sql, params object[] param);



        /// <summary>
        /// Returns the first element of the request 
        /// </summary>
        /// <param name="sql">sql text</param>
        /// <param name="timeOut">timeout connection</param>
        /// <param name="param">parameter array (param name: mysql-?,postgresql-@,mssql-@,sqlite-@)</param>
        object ExecuteScalarT(string sql, int timeOut = 30, params object[] param);



        /// <summary>
        /// Recreating a table
        /// </summary>
        int TruncateTable<TSource>() where TSource : class;

        /// <summary>
        /// Recreating a table
        /// </summary>
        Task<int> TruncateTableAsync<TSource>(CancellationToken cancellationToken=default) where TSource : class;



        /// <summary>
        /// Main point  Linq to Sql
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        Query<TSource> Query<TSource>() where TSource : class;

        /// <summary>
        /// Determines if the object is received from the database, or was created on the client
        /// </summary>
        bool IsPersistent<TSource>(TSource obj) where TSource : class;

        /// <summary>
        /// Making an object persistent ( as object received from database)
        /// </summary>
        void ToPersistent<TSource>(TSource source) where TSource : class;

        /// <summary>
        /// Write to log file
        /// </summary>
        /// <param name="message">message</param>
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
        /// executes the query and returns the number of records affected
        /// </summary>
        /// <param name="sql">sql text</param>
        /// <param name="param">parameters array (param name: mysql-?,postgresql-@,mssql-@,sqlite-@)</param>
        int ExecuteNonQuery(string sql, params object[] param);




        /// <summary>
        /// executes the query and returns the number of records affected
        /// </summary>
        /// <param name="sql">sql text</param>
        /// <param name="timeOut">timeout connection</param>
        /// <param name="param">parameters array (param name: mysql-?,postgresql-@,mssql-@,sqlite-@)</param>
        int ExecuteNonQueryT(string sql, int timeOut = 30, params object[] param);



        /// <summary>
        /// Getting the name of the table to build an sql query.
        /// </summary>
        string TableName<TSource>() where TSource : class;

        /// <summary>
        /// Getting the field name for a table
        /// </summary>
        string ColumnName<TSource>(Expression<Func<TSource, object>> property) where TSource : class;

        /// <summary>
        /// Getting string SQL for insert command
        /// </summary>
        string GetSqlInsertCommand<TSource>(TSource source) where TSource : class;

        /// <summary>
        /// Getting string SQL for delete command
        /// </summary>
        string GetSqlDeleteCommand<TSource>(TSource source) where TSource : class;

        /// <summary>
        /// Cloning an object using JSON
        /// </summary>
        TSource Clone<TSource>(TSource source) where TSource : class;

        /// <summary>
        /// Getting string SQL for bulk insert command
        /// </summary>
        /// <param name="enumerable"></param>
        /// <typeparam name="TSource"></typeparam>
        string GetSqlForInsertBulk<TSource>(IEnumerable<TSource> enumerable) where TSource : class;


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

        /// <summary>
        /// Update table with additional condition where
        /// </summary>
        /// <param name="source">object for update</param>
        /// <param name="whereObjects">list condition</param>
        /// <returns>Query result: 1 - ok  0-Record not updated</returns>
        int Update<TSource>(TSource source, params AppenderWhere[] whereObjects) where TSource : class;
    }
}