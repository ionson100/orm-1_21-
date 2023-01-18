using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORM_1_21_
{
   internal static class MySqlLogger
   {
       private static  ConcurrentQueue<string> _cq;
        private static bool _isActive = false;
        private static string _pathFile;
        
        public static void Info(string message)
        {
            if (_isActive == false) return;
            _cq.Enqueue($"{message}");
        }

         public static void RunLogger(string file)
        {
            _pathFile = file;
            if (file == null)
            {
                _isActive = false;
                return;
            }

            _isActive = true;

            _cq = new ConcurrentQueue<string>();
            Action();
            Info($"---------   Init Log : {DateTime.Now:s} ----------");

        }

        private static void Action()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    while (_cq.TryDequeue(out var sql))
                    {
                        File.AppendAllText(_pathFile, sql + Environment.NewLine);
                    }
                }
            });
        }
    }
}
