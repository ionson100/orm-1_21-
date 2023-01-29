using ORM_1_21_.Linq;
using ORM_1_21_.Transaction;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;

namespace ORM_1_21_
{
    /// <summary>
    ///     Основной рабочий тип
    /// </summary>
    public interface ISession : IDisposable
    {
        /// <summary>
        ///     is enter Dispose?
        /// </summary>
        bool IsDispose { get; }

        /// <summary>
        ///     запрос на выборку с параметрами
        /// </summary>
        /// <param name="sqlWhere">запрос на выборку, начиная с where  с параметрами</param>
        /// <param name="obj">список параметров в той последовательности в которой они идут в запросе.</param>
        /// <typeparam name="T">Тип сущности</typeparam>
        /// <returns>Перечисление выбранных объектов</returns>
        IEnumerable<T> GetList<T>(string sqlWhere, params object[] obj) where T : class;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader">IDataReader</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>IEnumerable</returns>
        IEnumerable<T> GetListMonster<T>(IDataReader reader) where T : class;




        /// <summary>
        ///     Сохранение объекта в базе равно как вставка или изменение
        /// </summary>
        /// <typeparam name="T">Тип объекта</typeparam>
        /// <param name="item">Сохраняемый объект</param>
        int Save<T>(T item) where T : class;

        /// <summary>
        ///     Удаление объекта из базы, возвращаете количество удаленных объектов
        /// </summary>
        /// <typeparam name="T">Тип удаляемого объекта</typeparam>
        /// <param name="item">Удаляемый объект</param>
        int Delete<T>(T item) where T : class;


        /// <summary>
        ///     Получение объекта ITransaction с одновременно началом транзакции
        /// </summary>
        /// <returns>ITransaction</returns>
        ITransaction BeginTransaction();

        /// <summary>
        ///     Получение объекта ITransaction с одновременно началом транзакции, с параметрами
        /// </summary>
        /// <param name="value">Параметр изоляции транзакции</param>
        /// <returns>ITransaction</returns>
        ITransaction BeginTransaction(IsolationLevel value);

        /// <summary>
        ///     Получение объекта по первичному ключу
        /// </summary>
        /// <typeparam name="T">Тип объекта</typeparam>
        /// <param name="id">Значение первичного ключа</param>
        /// <returns>Полученный объект, в случае отсутствия  в базe - NULL</returns>
        T Get<T>(object id) where T : class;


        /// <summary>
        ///     Создание таблицы
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void TableCreate<T>() where T : class;

        /// <summary>
        /// получение DbCommand
        /// </summary>
        /// <returns>DbCommand</returns>
        IDbCommand GeDbCommand();

        /// <summary>
        ///     Удаление таблицы
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void DropTable<T>() where T : class;

        /// <summary>
        ///     Проверка на существование таблицы в базе
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        bool TableExists<T>() where T : class;


        /// <summary>
        ///     ExecuteReader ( закрываем сами)
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="obj">параметры</param>
        /// <returns></returns>
        IDataReader ExecuteReader(string sql, params object[] obj);


        /// <summary>
        /// ExecuteReader ( закрываем сами)
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="timeOut">тайм аут</param>
        /// <param name="obj"></param>
        /// <returns></returns>
        IDataReader ExecuteReaderT(string sql, int timeOut = -1, params object[] obj);

        /// <summary>
        /// Get DataTable
        /// </summary>
        /// <param name="sql">sql text</param>
        /// <param name="timeout">тайм аут</param>
        /// <returns>DataTable</returns>
        DataTable GetDataTable(string sql, int timeout = -1);
        /// <summary>
        /// Get DataTable
        /// </summary>
        /// <param name="sql">sql text</param>
        /// <param name="timeout">тайм аут</param>
        /// <param name="obj"></param>
        /// <returns>DataTable</returns>
        DataTable GetDataTable(string sql, int timeout = -1, params object[] obj);

        /// <summary>
        ///     возвращает список названия таблиц из базы
        /// </summary>
        /// <returns></returns>
        List<string> GetTableNames();

        //// <summary>
        /// Создает базу данных
        /// <param name="baseName">название базы для Mysql  , путь до базы для Postgesql,Sqlite,MSSql</param>
        /// <returns>-1 успешно</returns>
        int CreateBase(string baseName);

        /// <summary>
        ///     Вставка в базу из файла
        /// </summary>
        /// <param name="list">список вставляемых объектов</param>
        /// <param name="timeOut"></param>
        /// <typeparam name="T"></typeparam>
        void InsertBulk<T>(IEnumerable<T> list, int timeOut = -1);


        /// <summary>
        /// </summary>
        /// <param name="fileCsv">полный путь к файлу, уже записному и готовому для вставки в базу</param>
        /// <param name="FIELDTERMINATOR"></param>
        /// <param name="timeOut"></param>
        /// <typeparam name="T">разделитель полей</typeparam>
        void InsertBulkFomFile<T>(string fileCsv, string FIELDTERMINATOR = ";", int timeOut = -1);

        /// <summary>
        ///     Возвращает первый элемент запроса
        /// </summary>
        /// <param name="sql">строка запроса</param>
        /// <param name="obj">параметры запроса</param>
        /// <returns></returns>
        object ExecuteScalar(string sql, params object[] obj);


        /// <summary>
        ///     Возвращает первый элемент запроса
        /// </summary>
        /// <param name="sql">строка запроса</param>
        /// <param name="timeOut">timeout default 30 </param>
        /// <param name="obj">параметры запроса</param>
        /// <returns></returns>
        object ExecuteScalarT(string sql, int timeOut = -1, params object[] obj);

        /// <summary>
        ///     Пересоздание таблицы
        /// </summary>
        /// <typeparam name="T"></typeparam>
        int TruncateTable<T>();


        /// <summary>
        ///     Выборка через Linq to Sql
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Query<T> Querion<T>();





        /// <summary>
        ///     Определяет, получен ли объект с базы, или был создан на клиенте
        /// </summary>
        /// <param name="obj">Объект проверяемый</param>
        /// <returns>True -из базы, False - созданный на клиенте</returns>
        bool IsPersistent(object obj);

        /// <summary>
        ///     Делаем объект персистентным ( как бы объект получен из базы)
        /// </summary>
        /// <param name="obj"></param>
        void ToPersistent(object obj);


        /// <summary>
        ///     Запись в лог, если запись в лог включена, при инициализации орм, можно записать текст сообщения напрямую
        /// </summary>
        /// <param name="message">сообщение</param>
        void WriteLogFile(string message);

        /// <summary>
        ///     Получение автономного IDbCommand, закрывать и диспозить вручную
        /// </summary>
        /// <returns>IDbCommand</returns>
        IDbCommand GetCommand();

        /// <summary>
        ///     Получение автономного соединения в контексте орм, закрывать и очищать вручную
        /// </summary>
        /// <returns>IDbConnection</returns>
        IDbConnection GetConnection();

        /// <summary>
        ///     Получение адаптера, автономного в контексте орм, закрывать и очищать вручную
        /// </summary>
        /// <returns>IDbDataAdapter</returns>
        IDbDataAdapter GetDataAdapter();

        /// <summary>
        ///     Получение параметра в контексте орм
        /// </summary>
        /// <returns></returns>
        IDataParameter GetDataParameter();

        /// <summary>
        ///     Получение строки соединения в контексте работы орм
        /// </summary>
        /// <returns>Строка соединения</returns>
        string GetConnectionString();

        /// <summary>
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="obj"></param>
        int ExecuteNonQuery(string sql, params object[] obj);


        /// <summary>
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="timeOut"></param>
        /// <param name="obj"></param>
        int ExecuteNonQueryT(string sql, int timeOut = -1, params object[] obj);

        /// <summary>
        ///     Получение названия таблицы, для построения sql запроса.
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
        /// Получает SQL строку Insert (бойся инъекций)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        string InsertCommand<T>(T t);

        /// <summary>
        /// Строка запроса на удаление
        /// </summary>
        /// <param name="t"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns>string sql for detete</returns>
        string DeleteCommand<T>(T t);

        /// <summary>
        /// Клонирование объекта
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ob"></param>
        /// <returns>T</returns>
        T Clone<T>(T ob);


    }
}