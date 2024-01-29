using System;
using System.Diagnostics;

namespace ORM_1_21_
{
    internal static class MySqlLogger
    {
        private const string Orm = "ORM";
        private static bool _isWrite;
        public static void Info(string message)
        {
            if (_isWrite) { Trace.WriteLine(message, Orm); }

        }

        public static void Error(string sql, Exception ex)
        {
            if (_isWrite) { Trace.WriteLine($"{Environment.NewLine}{sql}{Environment.NewLine}{ex}", Orm); }

        }

        public static void RunLogger(string file)
        {
            _isWrite = true;
            Trace.Listeners.Add(new TextWriterTraceListener(file));
            Trace.AutoFlush = true;
        }
    }
}