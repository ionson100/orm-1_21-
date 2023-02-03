using ORM_1_21_.Linq;
using ORM_1_21_.Transaction;
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
        IEnumerable<T> GetList<T>(string sqlWhere, params object[] param) where T : class;

        /// <summary>
        /// Для чужой базы из родственной DataBase type
        /// </summary>
        /// <param name="reader">IDataReader</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>IEnumerable</returns>
        IEnumerable<T> GetListMonster<T>(IDataReader reader) where T : class;

        /// <summary>
        /// Сохранение объекта в базе равно как вставка или изменение
        /// </summary>
        /// <typeparam name="T">Тип объекта</typeparam>
        /// <param name="item">Сохраняемый объект</param>
        int Save<T>(T item) where T : class;

        /// <summary>
        /// Удаление объекта из базы, возвращаете количество удаленных объектов
        /// </summary>
        /// <typeparam name="T">Тип удаляемого объекта</typeparam>
        /// <param name="item">Удаляемый объект</param>
        int Delete<T>(T item) where T : class;

        /// <summary>
        /// Получение объекта ITransaction с одновременно началом транзакции
        /// </summary>
        /// <returns>ITransaction</returns>
        ITransaction BeginTransaction();

        /// <summary>
        /// Получение объекта ITransaction с одновременно началом транзакции
        /// </summary>
        /// <param name="value">Параметр изоляции транзакции</param>
        /// <returns>ITransaction</returns>
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
        /// <typeparam name="T"></typeparam>
        void TableCreate<T>() where T : class;

        /// <summary>
        /// Получение DbCommand
        /// </summary>
        /// <returns>DbCommand</returns>
        IDbCommand GeDbCommand();

        /// <summary>
        /// Удаление таблицы
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void DropTable<T>() where T : class;

        /// <summary>
        /// Проверка на существование таблицы в базе
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        bool TableExists<T>() where T : class;

        /// <summary>
        ///  Получение ExecuteReader ( закрываем сами)
        /// </summary>
        /// <param name="sql">Строка запроса</param>
        /// <param name="param">Список параметров в той последовательности в которой они расположены в запросе.</param>
        /// <returns></returns>
        IDataReader ExecuteReader(string sql, params object[] param);

        /// <summary>
        /// Получение ExecuteReader ( закрываем сами)
        /// </summary>
        /// <param name="sql">Строка запроса</param>
        /// <param name="timeOut">Время ожидания выполнения команды (30 сек)</param>
        /// <param name="param">Список параметров в той последовательности в которой они расположены в запросе.</param>
        /// <returns></returns>
        IDataReader ExecuteReaderT(string sql, int timeOut = 30, params object[] param);

        /// <summary>
        /// Получение DataTable
        /// </summary>
        /// <param name="sql">Строка запроса</param>
        /// <param name="timeout">Время ожидания выполнения команды (30 сек)</param>
        /// <returns>DataTable</returns>
        DataTable GetDataTable(string sql, int timeout = 30);

        /// <summary>
        /// Получение DataTable
        /// </summary>
        /// <param name="sql">sql text</param>
        /// <param name="timeout">Время ожидания выполнения команды (30 сек)</param>
        /// <param name="param">Список параметров в той последовательности в которой они расположены в запросе.</param>
        /// <returns>DataTable</returns>
        DataTable GetDataTable(string sql, int timeout = 30, params object[] param);

        /// <summary>
        ///Возвращает список названия таблиц из базы
        /// </summary>
        /// <returns></returns>
        List<string> GetTableNames();

        /// Создает базу данных
        /// <param name="baseName">название базы для Mysql  , путь до базы для Postgesql,Sqlite,MSSql</param>
        /// <returns>-1 успешно</returns>
        int CreateBase(string baseName);

        /// <summary>
        /// Вставка в базу пакетная
        /// </summary>
        /// <param name="list">Перечисление вставляемых объектов</param>
        /// <param name="timeOut">Время ожидания выполнения команды (30 сек)</param>
        /// <typeparam name="T"></typeparam>
        void InsertBulk<T>(IEnumerable<T> list, int timeOut = 30);

        /// <summary>
        /// Вставка в базу пакетом, запрос записан в файл
        /// </summary>
        /// <param name="fileCsv">полный путь к файлу, уже записному и готовому для вставки в базу</param>
        /// <param name="FIELDTERMINATOR"></param>
        /// <param name="timeOut"></param>
        /// <typeparam name="T">разделитель полей</typeparam>
        void InsertBulkFromFile<T>(string fileCsv, string FIELDTERMINATOR = ";", int timeOut = 30);

        /// <summary>
        /// Возвращает первый элемент запроса
        /// </summary>
        /// <param name="sql">Строка запроса</param>
        /// <param name="obj">параметры запроса</param>
        /// <returns></returns>
        object ExecuteScalar(string sql, params object[] obj);

        /// <summary>
        /// Возвращает первый элемент запроса
        /// </summary>
        /// <param name="sql">Строка запроса</param>
        /// <param name="timeOut">Время ожидания выполнения команды (30 сек)</param>
        /// <param name="param">Список параметров в той последовательности в которой они расположены в запросе.</param>
        /// <returns></returns>
        object ExecuteScalarT(string sql, int timeOut = 30, params object[] param);

        /// <summary>
        /// Пересоздание таблицы
        /// </summary>
        /// <typeparam name="T"></typeparam>
        int TruncateTable<T>();

        /// <summary>
        /// Выборка через Linq to Sql
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Query<T> Querion<T>();

        /// <summary>
        /// Определяет, получен ли объект с базы, или был создан на клиенте
        /// </summary>
        /// <param name="obj">Объект проверяемый</param>
        /// <returns>True -из базы, False - созданный на клиенте</returns>
        bool IsPersistent(object obj);

        /// <summary>
        /// Делаем объект персистентным ( как бы объект получен из базы)
        /// </summary>
        /// <param name="obj"></param>
        void ToPersistent(object obj);

        /// <summary>
        /// Запись в лог, если запись в лог включена, при инициализации орм, можно записать текст сообщения пользователя
        /// </summary>
        /// <param name="message">сообщение</param>
        void WriteLogFile(string message);

        /// <summary>
        /// Получение автономного IDbCommand, закрывать  вручную
        /// </summary>
        /// <returns>IDbCommand</returns>
        IDbCommand GetCommand();

        /// <summary>
        /// Получение автономного соединения в контексте орм, закрывать и очищать вручную
        /// </summary>
        /// <returns>IDbConnection</returns>
        IDbConnection GetConnection();

        /// <summary>
        /// Получение адаптера, автономного в контексте орм, закрывать и очищать вручную
        /// </summary>
        /// <returns>IDbDataAdapter</returns>
        IDbDataAdapter GetDataAdapter();

        /// <summary>
        /// Получение параметра в контексте орм
        /// </summary>
        /// <returns></returns>
        IDataParameter GetDataParameter();

        /// <summary>
        /// Получение строки соединения в контексте работы орм
        /// </summary>
        /// <returns>Строка соединения</returns>
        string GetConnectionString();

        /// <summary>
        /// </summary>
        /// <param name="sql">Строка запроса</param>
        /// <param name="param">Список параметров в той последовательности в которой они расположены в запросе.</param>
        int ExecuteNonQuery(string sql, params object[] param);

        /// <summary>
        /// Выполнение ExecuteNonQuery
        /// </summary>
        /// <param name="sql">Строка запроса</param>
        /// <param name="timeOut">Время ожидания выполнения команды (30 сек)м</param>
        /// <param name="param">Список параметров в той последовательности в которой они расположены в запросе.</param>
        int ExecuteNonQueryT(string sql, int timeOut = 30, params object[] param);

        /// <summary>
        /// Получение названия таблицы, для построения sql запроса.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        string TableName<T>();

        /// <summary>
        /// возвращает название поля для базы
        /// </summary>
        /// <param name="property"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        string ColumnName<T>(Expression<Func<T, object>> property);

        /// <summary>
        /// Получает SQL строку Insert
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        string GetSqlInsertCommand<T>(T t);

        /// <summary>
        /// Строка запроса на удаление
        /// </summary>
        /// <param name="t"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns>string sql for detete</returns>
        string GetSqlDeleteCommand<T>(T t);

        /// <summary>
        /// Клонирование объекта
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ob"></param>
        /// <returns>T</returns>
        T Clone<T>(T ob);

        /// <summary>
        /// Получение sql запроса для вставки пакетом
        /// </summary>
        /// <param name="enumerable"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        string GetSqlForInsertBulk<T>(IEnumerable<T> enumerable);

        /// <summary>
        /// Получение перечисления из чужой базы
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandFactory"></param>
        /// <param name="sql">Строка запроса</param>
        /// <param name="param">Список параметров в той последовательности в которой они расположены в запросе.</param>
        /// <returns></returns>
        IEnumerable<T> GetListOtherBase<T>(IOtherBaseCommandFactory commandFactory, string sql, params object[] param);

        /// <summary>
        /// Выполнение ExecuteScalar  к чужой базе
        /// </summary>
        /// <param name="commandFactory"></param>
        /// <param name="sql">Строка запроса</param>
        /// <param name="param">Список параметров в той последовательности в которой они расположены в запросе.</param>
        /// <returns></returns>
        object GetObjectOtherBase(IOtherBaseCommandFactory commandFactory, string sql, params object[] param);

        /// <summary>
        /// Выполнение запроса ExecuteNonQuery к чужой базе
        /// </summary>
        /// <param name="commandFactory"></param>
        /// <param name="sql">Строка запроса</param>
        /// <param name="param">Список параметров в той последовательности в которой они расположены в запросе.</param>
        /// <returns></returns>
        int ExecuteNonQueryOtherBase(IOtherBaseCommandFactory commandFactory, string sql, params object[] param);


    }
}