using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ORM_1_21_
{
    internal static class MySqlLogger
    {
        private static ConcurrentQueue<string> _cq;
        private static bool _isActive;
        private static StreamWriter _sw;
        private static long _runner;

        public static void StopLogger(bool isFinish=false)
        {
            if (Interlocked.Read(ref _runner) == 1) Interlocked.Decrement(ref _runner);
            if(isFinish)
                _sw.Dispose();
        }

        public static void Info(string message)
        {
            if (_isActive == false) return;
            _cq.Enqueue($"{message}");
        }
        public static void Error(string sql,Exception ex)
        {
            if (_isActive == false) return;
            _cq.Enqueue($"{Environment.NewLine}{sql}{Environment.NewLine}{ex}");
        }

        public static async Task RunLogger(string file)
        {
            if (file == null)
            {
                _isActive = false;
                return;
            }

            _isActive = true;
            Interlocked.Increment(ref _runner);
            _cq = new ConcurrentQueue<string>();

            using (_sw = File.AppendText(file))
            {
                await Action();
            }

            Info($"---------   Init Log : {DateTime.Now:s} ----------");
        }
      

        private static async Task Action()
        {
            await Task.Run(() =>
            {
                while (Interlocked.Read(ref _runner) == 1)
                {
                    while (_cq.TryDequeue(out var sql))
                    {
                        _sw.WriteLine(sql);
                        _sw.Flush();
                    }
                }
            });
        }
    }
}
