using System;
using ORM_1_21_.Utils;


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
        public string GetSymbolParam(ProviderName provider)
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
                    throw new ArgumentOutOfRangeException();
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

            /// <summary>
            /// Check if the object is received from the database (true) - the object is received from the database
            /// </summary>
            public static bool IsPersistent(object obj)
            {
                return UtilsCore.IsPersistent(obj);
            }

            /// <summary>
            /// As the object persistent - obtained from the database.
            /// </summary>
            /// <param name="obj"></param>
            public static void SetPersistent(object obj)
            {
                UtilsCore.SetPersistent(obj);
            }
        }
       
    }


    
}