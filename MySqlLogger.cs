using System;
using System.Diagnostics;

namespace ORM_1_21_
{
    internal static class MySqlLogger
    {
        private const string Orm = "ORM";
        public static void Info(string message)
        {
            Debug.WriteLine(message, Orm);
        }

        public static void Error(string sql, Exception ex)
        {
            Debug.WriteLine($"{Environment.NewLine}{sql}{Environment.NewLine}{ex}", Orm);
        }

        public static void RunLogger(string file)
        {
            Debug.Listeners.Add(new TextWriterTraceListener(file));
            Debug.AutoFlush = true;
        }
    }
}