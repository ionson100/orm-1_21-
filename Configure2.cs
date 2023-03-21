using System;
using System.Drawing;
using ORM_1_21_.Utils;


namespace ORM_1_21_
{
    public sealed partial class Configure
    {

        /// <summary>
        /// Native entry to the log file.
        /// </summary>
        public static void WriteLogFile(string message)
        {
            MySqlLogger.InfoAppendConsole(message);
            

        }
        /// <summary>
        /// Utilities
        /// </summary>
        public static class Utils
        {
            /// <summary>
            /// Getting System.Drawing.Image from file
            /// </summary>
            /// <param name="path">The path to the file</param>
            /// <returns></returns>
            public static Image GetImageFromFile(string path)
            {
                return Image.FromFile(path);
            }
            /// <summary>
            /// Convert date to SQL format
            /// </summary>
            /// <param name="dateTime"></param>
            /// <returns></returns>
            public static string DateToString(DateTime dateTime)
            {
                return $"'{dateTime:yyyy-MM-dd HH:mm:ss.fff}'";
            }
            /// <summary>
            /// Serialization
            /// </summary>
            /// <param name="obj">Объект</param>
            /// <returns>byte[]</returns>
            public static byte[] ObjectToByteArray(object obj)
            {
                return UtilsCore.ObjectToByteArray(obj);
            }
            /// <summary>
            /// De serialization
            /// </summary>
            public static object ByteArrayToObject(byte[] arrBytes)
            {
               return UtilsCore.ByteArrayToObject(arrBytes);
            }

            /// <summary>
            /// Getting an array of bytes from an Image
            /// </summary>
            public static byte[] ImageToByte(Image img)
            {
                return UtilsCore.ImageToByte(img);
            }

            /// <summary>
            /// Getting an Image from an array of bytes
            /// </summary>
            public static Image ImageFromByte(byte[]  bytes)
            {
                return UtilsCore.ImageFromByte(bytes);
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