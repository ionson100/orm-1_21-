using ORM_1_21_;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TestLibrary
{
    public static class ExecutePrimaryKey
    {
        public static void Run()
        {
            InnerRun<TTPostgres,MyDbPostgres,TNatPos>();
            InnerRun<TTMysl, MyDbMySql, TNatMy>();
            InnerRun<TTMsql, MyDbMsSql, TNatMs>();
            InnerRun<TTsql, MyDbSqlite, TNatSql>();
        }
        private static void InnerRun<T,TB, TN>() where TB : IOtherDataBaseFactory, new() where T : TestAssigned,new() where TN : TestNative,new()
        {
            var s = Activator.CreateInstance<TB>();
            Console.WriteLine($"**************************{s.GetProviderName()}*****************************");
            ISession session = Configure.GetSession<TB>();
            session.DropTableIfExists<T>();
            session.TableCreate<T>();
            session.InsertBulk(new List<T>
            {
                new T { Name = "11" },
                new T { Name = "12" },
                new T { Name = "13" },
            });
            var s1 = new T { Name = "14",Id = Guid.NewGuid()};
            session.Insert(s1);
            var s2 = new T { Name = "15" };
            session.Insert(s2);
            var list = session.Query<T>().ToList().Select(a => a.Id).ToList();
            Execute.Log(1, list.Contains(s1.Id));
            Execute.Log(2, list.Contains(s2.Id));
            list.ForEach(a =>
            {
                Console.WriteLine(a);
            });

            session.DropTableIfExists<TN>();
            session.TableCreate<TN>();
            session.InsertBulk(new List<TN>
            {
                new TN { Name = "11" },
                new TN { Name = "12" },
                new TN { Name = "13" },
            });
            var s11 = new TN { Name = "14" };
            session.Update(s1);
            var s21 = new TN { Name = "15" };
            session.Update(s2);
            var list1 = session.Query<TN>().OrderBy(d => d.Id).ToList().Select(a => a.Id).ToList();
            list1.ForEach(Console.WriteLine);
        }
        [MapTable]
        class TestAssigned
        {
            [MapPrimaryKey("id", Generator.Assigned)]
            public Guid Id { get; set; } = Guid.NewGuid();
            [MapColumn]
            public string Name { get; set; }

        }

        [MapTable] class TTPostgres : TestAssigned { }
        [MapTable] class TTMysl : TestAssigned { }
        [MapTable] class TTMsql : TestAssigned { }
        [MapTable] class TTsql : TestAssigned { }



        [MapTable("assasu")] class TNatPos:TestNative { }
        [MapTable("assasu")] class TNatMy : TestNative { }
        [MapTable("assasu")] class TNatMs : TestNative { }
        [MapTable("assasu")] class TNatSql : TestNative { }

        
        class TestNative
        {
            [MapPrimaryKey("id", Generator.Native)]
            public Int32 Id { get; set; }
            [MapColumn]
            public string Name { get; set; }
        }
        class TT1Postgres : TestNative { }
        class TT1Mysl : TestNative { }
        class TT1Msql : TestNative { }
        class TT1sql : TestNative { }


    }


    [MapTable]
    class TestBaseGenerate
    {
        public Guid Id { get; set; }
        [MapColumn]
        public string Name { get; set; }
    }
}
