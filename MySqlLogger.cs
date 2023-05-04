using System;
using System.Diagnostics;

namespace ORM_1_21_
{
    internal static class MySqlLogger
    {
        private const string Orm = "ORM";
        public static void Info(string message)
        {
            Trace.WriteLine(message, Orm);
        }

        public static void Error(string sql, Exception ex)
        {
            Trace.WriteLine($"{Environment.NewLine}{sql}{Environment.NewLine}{ex}", Orm);
        }

        public static void RunLogger(string file)
        {
            Trace.Listeners.Add(new TextWriterTraceListener(file));
            Trace.AutoFlush = true;
        }
    }
}