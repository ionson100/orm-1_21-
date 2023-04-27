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
            InnerRun<MyDbPostgres>();
            //InnerRun<MyDbMySql>();
            //InnerRun<MyDbMsSql>();
            //InnerRun<MyDbSqlite>();
        }
        private static void InnerRun<TB>() where TB : IOtherDataBaseFactory, new()
        {
            var s = Activator.CreateInstance<TB>();
            Console.WriteLine($"**************************{s.GetProviderName()}*****************************");
            ISession session = Configure.GetSession<TB>();
            session.DropTableIfExists<TTPostgres>();
            session.TableCreate<TTPostgres>();
            session.InsertBulk(new List<TTPostgres>
            {
                new TTPostgres { Name = "11" },
                new TTPostgres { Name = "12" },
                new TTPostgres { Name = "13" },
            });
            var s1 = new TTPostgres { Name = "14" };
            session.Insert(s1);
            var s2 = new TTPostgres { Name = "15" };
            session.Insert(s2);
            var list = session.Query<TTPostgres>().ToList().Select(a => a.Id).ToList();
            Execute.Log(1, list.Contains(s1.Id));
            Execute.Log(2, list.Contains(s2.Id));
            list.ForEach(a =>
            {
                Console.WriteLine(a);
            });

            session.DropTableIfExists<TestNative>();
            session.TableCreate<TestNative>();
            session.InsertBulk(new List<TestNative>
            {
                new TestNative { Name = "11" },
                new TestNative { Name = "12" },
                new TestNative { Name = "13" },
            });
            var s11 = new TestNative { Name = "14" };
            session.Update(s1);
            var s21 = new TestNative { Name = "15" };
            session.Update(s2);
            var list1 = session.Query<TestNative>().OrderBy(d => d.Id).ToList().Select(a => a.Id).ToList();
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

        [MapTable("assasu")]
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
