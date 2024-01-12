using ORM_1_21_.Utils;
using System;


namespace ORM_1_21_
{
    public sealed partial class Configure
    {

        /// <summary>
        /// Gets symbol of the parameter for sql request
        /// </summary>
        /// <param name="provider">Provider database</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static string SymbolParam(ProviderName provider)
        {
            switch (provider)
            {
                case ProviderName.MsSql:
                    return "@";
                case ProviderName.MySql:
                    return "?";
                case ProviderName.PostgreSql:
                    return "@";
                case ProviderName.SqLite:
                    return "@";
                default:
                    throw new ArgumentOutOfRangeException($"Database type is not defined:{provider}");
            }
        }

        /// <summary>
        /// Native entry to the log file. Only Debug state
        /// </summary>
        public static void WriteLogFile(string message)
        {
            MySqlLogger.Info(message);
        }
        /// <summary>
        /// Utilities
        /// </summary>
        public static class Utils
        {
            /// <summary>
            /// Only MSSql
            /// </summary>
            /// <returns></returns>
            public static DateTime DefaultSqlDateTime()
            {
                return new DateTime(1753, 1, 1, 12, 0, 0);
            }

            /// <summary>
            /// Convert date to SQL format
            /// </summary>
            /// <param name="dateTime"></param>
            /// <returns></returns>
            public static string DateToString(DateTime dateTime)
            {
                return $"{dateTime:yyyy-MM-dd HH:mm:ss.fff}";
            }
            /// <summary>
            /// Serialization to bytes
            /// </summary>
            /// <param name="obj">Object for serialization </param>
            /// <returns>byte[]</returns>
            public static byte[] ObjectToByteArray(object obj)
            {
                return UtilsCore.ObjectToByteArray(obj);
            }
            /// <summary>
            /// De serialization from bytes
            /// </summary>
            public static object ByteArrayToObject(byte[] arrBytes)
            {
                return UtilsCore.ByteArrayToObject(arrBytes);
            }

        }

    }



}