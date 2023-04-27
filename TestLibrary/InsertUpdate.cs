using System;
using System.Collections.Generic;
using System.Linq;
using KellermanSoftware.CompareNetObjects;
using ORM_1_21_;

namespace TestLibrary
{
    public static class InsertUpdate
    {
        public static void Run()
        {
            InnerRun<OrderPostgres, MyDbPostgres>();
            InnerRun<OrderMysql, MyDbMySql>();
            InnerRun<OrderMsSql, MyDbMsSql>();
            InnerRun<OrderSqlite, MyDbSqlite>();


        }

        private static void InnerRun<T, TB>() where T : Order, new() where TB : IOtherDataBaseFactory, new()
        {

            var s = Activator.CreateInstance<TB>();
            Console.WriteLine($"**************************{s.GetProviderName()}*****************************");
            ISession session = Configure.GetSession<TB>();
            CompareLogic compareLogic = new CompareLogic();
            session.DropTableIfExists<T>();
            session.TableCreate<T>();
            var o = new T
            {
                Text = "name", DecimalNull = new decimal(3.8), FloatNull = 3.8f, DoubleNull = 3.8,Date = Configure.Utils.DefaultSqlDateTime(),
                    LongNull = 4L,Char = '5'
            };
            session.Insert(o);
            var o1 = session.Query<T>().First();
            Console.WriteLine(o.Date.ToString());
            Console.WriteLine(o1.Date.ToString());
            var res = compareLogic.Compare(o, o1);
            Execute.Log(1, res.Differences.Count == 0, "insert");

            o1.Text = "AA";
            o1.Guid=Guid.NewGuid();
            o1.GuidNull = Guid.NewGuid();
            var ss = session.Update(o1);
            if (ss == 0)
            {
                Execute.Log(2,false,"update");
            }
            var o2 = session.Query<T>().First();
            compareLogic.Config.ClassTypesToIgnore.Add(typeof(DateTime));
             res = compareLogic.Compare(o1, o2);
            Execute.Log(2, res.Differences.Count == 0, "update");

            session.TruncateTable<T>();
            o.GuidNull=Guid.NewGuid();
            session.InsertBulk(new List<T>() { o });
            var o3 = session.Query<T>().First();
            res = compareLogic.Compare(o, o3);
            Console.WriteLine(o.Date.ToString());
            Console.WriteLine(o3.Date.ToString());
            Execute.Log(3, res.Differences.Count == 0, "bulk");


        }
    }
}