using MySqlX.XDevAPI;
using ORM_1_21_;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TestLibrary
{
    public static class TestCapacity
    {
        public static void Run()
        {
            NewExe<CapacitySqlite, MyDbSqlite>();
            NewExe<CapacityNative, MyDbPostgres>(true);
            NewExe<CapacityPostgres, MyDbPostgres>();
            NewExe<CapacityMysql, MyDbMySql>();
            NewExe<CapacityMsSql, MyDbMsSql>();

            Console.ReadKey();
        }

        private static void NewExe<T, Tb>(bool isNative = false) where T : CapacityBase, new() where Tb : IOtherDataBaseFactory, new()
        {
            const int count = 1000;
            var s = Activator.CreateInstance<Tb>();
            ISession GetSession()
            {
                if (isNative == false)
                    return Configure.GetSession<Tb>();
                else
                {
                    return Configure.Session;
                }
            }


            if (isNative == false)
            {
                Console.WriteLine($"**************************{s.GetProviderName()}*****************************");
            }
            else
            {
                Console.WriteLine($"**************************NATIVE {Configure.Provider}*****************************");
            }

            {

            }
            

            using (var session = GetSession())
            {
                session.DropTableIfExists<T>();
                session.TableCreate<T>();
                session.Insert(new T());
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                using (session.BeginTransaction())
                {
                    for (int i = 0; i < count; i++)
                    {
                        var tt = session.Insert(new T());
                    }

                }
                stopwatch.Stop();
                Console.WriteLine($"{stopwatch.ElapsedMilliseconds} insert" );
                GC.Collect();
            }

            using (var session = GetSession())
            {
               
                Stopwatch stopwatch = new Stopwatch();
                var sql = session.GetSqlInsertCommand(new T());
                stopwatch.Start();
                for (int i = 0; i < count; i++)
                {
                   var ee= session.ExecuteNonQuery(sql);
                }
                stopwatch.Stop();
                Console.WriteLine($"{stopwatch.ElapsedMilliseconds}  insert native ");
                GC.Collect();
            }


            using (var session = GetSession())
            {
                //var myClassBase = session.Query<T>().First();
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                var list = session.Query<T>().ToList();
                stopwatch.Stop();
                Console.WriteLine($"{stopwatch.ElapsedMilliseconds}  select1 {list.Count}");
                GC.Collect();
            }
            using (var session = GetSession())
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                var list = session.Query<T>().ToList();
                stopwatch.Stop();
                Console.WriteLine($"{stopwatch.ElapsedMilliseconds}  select2 {list.Count()}");
                GC.Collect();
            }

            using (var session = GetSession())
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                var list = session.Query<T>().ToList();
                stopwatch.Stop();
                Console.WriteLine($"{stopwatch.ElapsedMilliseconds}  select3 {list.Count()}");
                GC.Collect();
            }

            using (var session = GetSession())
            {
                //var myClassBase = session.Query<T>().First();
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                var list = session.FreeSql<T>($" select * from {session.TableName<T>()}");
                stopwatch.Stop();
                Console.WriteLine($"{stopwatch.ElapsedMilliseconds}  select free {list.Count()}");
                GC.Collect();
            }
            using (var session = GetSession())
            {
                //var myClassBase = session.Query<T>().First();
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                var list = session.FreeSql<string>($" select name from {session.TableName<T>()}");
                stopwatch.Stop();
                Console.WriteLine($"{stopwatch.ElapsedMilliseconds}  select1 free Guid {list.Count()}");
                GC.Collect();
            }
            using (var session = GetSession())
            {
                //var myClassBase = session.Query<T>().First();
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                var list = session.FreeSql<dynamic>($" select * from {session.TableName<T>()}");
                stopwatch.Stop();
                Console.WriteLine($"{stopwatch.ElapsedMilliseconds}  select1 free dynamic {list.Count()}");
                GC.Collect();
            }

            using (var session = GetSession())
            {
                //var myClassBase = session.Query<T>().First();
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                var list = session.FreeSql<CapacityReceiverFreeSql>($" select id,name,description from {session.TableName<T>()}");
                stopwatch.Stop();
                Console.WriteLine($"{stopwatch.ElapsedMilliseconds}  select free receiver {list.Count()}");
                GC.Collect();
            }

          
            

            using (var session = GetSession())
            {
                var list = session.Query<T>().ToList();
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                using (session.BeginTransaction())
                {
                    list.ForEach(a => session.Update(a));
                }

                stopwatch.Stop();
                Console.WriteLine(stopwatch.ElapsedMilliseconds + " update1");
                GC.Collect();
            }

            using (var session = GetSession())
            {
                var list = session.Query<T>().ToList();
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                using (session.BeginTransaction())
                {
                    list.ForEach(a => session.Update(a));
                }

                stopwatch.Stop();
                Console.WriteLine(stopwatch.ElapsedMilliseconds + " update2");
                GC.Collect();
            }
            using (var session = GetSession())
            {
                var list = session.Query<T>().ToList();
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                using (session.BeginTransaction())
                {
                    list.ForEach(a => session.Update(a));
                }

                stopwatch.Stop();
                Console.WriteLine(stopwatch.ElapsedMilliseconds + " update3");
                GC.Collect();
            }

            using (var session = GetSession())
            {
                var list = session.Query<T>().ToList();
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                using (session.BeginTransaction())
                {
                    list.ForEach(a => session.Delete(a));
                }

                stopwatch.Stop();
                Console.WriteLine(stopwatch.ElapsedMilliseconds + " delete");
                GC.Collect();
            }
            using (var session = GetSession())
            {
                List<T> list = new List<T>(10000);
                for (int i = 0; i < 10000; i++)
                {
                    list.Add(new T());
                }
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                using (session.BeginTransaction())
                {
                    if (session.ProviderName == ProviderName.PostgreSql ||
                        session.ProviderName == ProviderName.SqLite ||
                        session.ProviderName == ProviderName.MySql)
                    {
                        session.InsertBulk(list);
                    }
                    else
                    {
                        foreach (IEnumerable<T> bases in list.Split(1000))
                        {
                            session.InsertBulk(bases);
                        }
                    }

                }

                stopwatch.Stop();
                Console.WriteLine(stopwatch.ElapsedMilliseconds + " inser bulk partion mssql");
                GC.Collect();
            }
            using (var session = GetSession())
            {
                var list = session.Query<T>().ToList();
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                session.TruncateTable<T>();

                stopwatch.Stop();
                Console.WriteLine(stopwatch.ElapsedMilliseconds + " Truncate Table");
                GC.Collect();
            }

            ;
        }

        public class CapacityBase
        {
            [MapPrimaryKey("id",Generator.Native)]
            public int Id { get; set; }

            [MapColumn("name")] public string Name { get; set; } = "asas";
            [MapColumn("description")] public string Description { get; set; }
            [MapColumn] public string Name2 { get; set; }
            [MapColumn] public string Description2 { get; set; }
        }

        [MapTable]
        class CapacityPostgres : CapacityBase
        {

        }
        [MapTable]
        class CapacityMysql : CapacityBase
        {

        }
        [MapTable]
        class CapacityMsSql : CapacityBase
        {

        }
        [MapTable]
        class CapacitySqlite : CapacityBase
        {

        }
        [MapTable]
      public  class CapacityNative : CapacityBase
        {

        }

        [MapReceiverFreeSql]
        class CapacityReceiverFreeSql
        {
            private readonly int _guid;
            private readonly string _name;
            private readonly string _description;

            public CapacityReceiverFreeSql(int guid, string name,string description)
            {
                _guid = guid;
                _name = name;
                _description = description;
            }
        }

        
    }
}
