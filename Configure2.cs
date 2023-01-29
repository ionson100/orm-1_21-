using System;


namespace ORM_1_21_
{
    public sealed partial class Configure
    {

        internal static void SendError(string sql, Exception exception)
        {
#if DEBUG
            _configure.OnOnErrorOrm(new ErrorOrmEventArgs { ErrorMessage = exception.ToString(), Sql = sql, InnerException = exception.InnerException });
            return;
#endif
            _configure.OnOnErrorOrm(new ErrorOrmEventArgs { ErrorMessage = exception.Message, Sql = sql, InnerException = exception.InnerException });
        }
        /// <summary>
        /// 
        /// </summary>
        public event ErrorEvent onErrorOrm;


        /// <summary>
        /// Нативная запись в лог файл.
        /// </summary>
        /// <param name="message">текст сообщения</param>
        public static void WriteLogFile(string message)
        {
            MySqlLogger.Info(message);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public delegate void ErrorEvent(object sender, ErrorOrmEventArgs args);

    /// <summary>
    /// 
    /// </summary>
    public class ErrorOrmEventArgs
    {
        /// <summary>
        /// Строка запроса, где возникло исключение
        /// </summary>
        public string Sql { get; set; }
        /// <summary>
        /// Сообщение для исключения
        /// </summary>
        public string ErrorMessage { get; set; }
        /// <summary>
        /// Внутреннее исключение
        /// </summary>
        public Exception InnerException { get; set; }
    }
}