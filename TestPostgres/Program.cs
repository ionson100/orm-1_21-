using MySql.Data.MySqlClient;
using ORM_1_21_;
using ORM_1_21_.Attribute;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TestPostgres
{
    internal class Program
    {


        static async Task Main(string[] args)
        {

           
            Starter.Run();



            new Thread(async () =>
            {
                while (true)
                {
                    var ses = Configure.Session;
                    var ts = ses.BeginTransaction();
                    MyClass c = new MyClass();
                    ses.Save(c);

                    var s = Configure.Session.Querion<MyClass>().Where(a => a.Age != -1);
                    var ees = await s.ToListAsync();
                    Console.WriteLine("1 -- " + ees.Count());
                    ts.Commit();

                }

            }).Start();

            new Thread(() =>
            {
                while (true)
                {
                    var ses = Configure.Session;
                    var ts = ses.BeginTransaction();
                    MyClass c = new MyClass();
                    ses.Save(c);

                    var s = Configure.Session.Querion<MyClass>().Where(a => a.Age != -1).ToList();
                    Console.WriteLine("2 -- " + s.Count());
                    ts.Commit();
                }

            }).Start();
            Console.ReadKey();

            //var tcc = Configure.Session.Querion<MyClass>().Where(a=>a.Age!=23)l

            ISession sesw = Configure.GetSession<MyDbMySql>();
            var myClassMysqls = sesw.Querion<MyClassMysql>().Where(a => a.Age != 3);
            //sesw.Dispose();
            myClassMysqls.ToList();

            var tr44 = sesw.BeginTransaction();
            if (sesw.TableExists<MyClassMysql2>())
            {
                var e = sesw.DropTable<MyClassMysql2>();
            }
            if (sesw.TableExists<MyClassMysql2>() == false)
            {
                var e2 = sesw.TableCreate<MyClassMysql2>();
            }
            List<MyClassMysql2> list = new List<MyClassMysql2>
            {
                new MyClassMysql2(),
                new MyClassMysql2(),
                new MyClassMysql2(),
                new MyClassMysql2(),
                new MyClassMysql2()
            };
            var dtr = sesw.GetSqlForInsertBulk(list);
            int ee23 = sesw.InsertBulk(list);
            tr44.Commit();

            var ee = Configure.GetSession<MyDbMySql>().Querion<MyClassMysql2>();
            var eee = ee.ToString();

            ee.ForEach(s =>
            {
                Console.WriteLine("__" + s.Id);
            });

            var sass = Configure.GetSession<MyDbMySql>().GetList<MyClassMysql2>();

            var r2 = Configure.GetSession<MyDbMySql>().FreeSql<MyClassMysql>("select * from my_class where age > @1",
                new Parameter("@1", 20)).ToList();
            r2.ForEach(s =>
            {
                Console.WriteLine($"MySql {s}");
            });

            var tr1 = sesw.BeginTransaction();
            try
            {
                MyClassMysql df = new MyClassMysql() { Age = 120 };
                var r23 = sesw.Save(df);
                if (1 == 1) throw new Exception();
            }
            catch (Exception)
            {
                tr1.Rollback();
            }
            var r233 = Configure.GetSession<MyDbMySql>().FreeSql<dynamic>("select * from my_class where age > @1", new Parameter("@1", 20)).ToList();




            var r1 = Configure.GetSession<MyDbMySql>().Querion<MyClassMysql>().Where(d => d.Age != 123)
                .Select(f => new { namme = f.Name });
            var task = r1.ToListAsync();
            var mysql = await task;


            MyClass myClass = new MyClass()
            {
                Age = 11,
                Description = "simple",
                Name = "ion100"
            };
            List<MyClass> list1 = new List<MyClass>
            {
                new MyClass()
                {
                Age = 12,
                Description = "simple",
                Name = "ion100"
                },
                new MyClass()
                {
                    Age = 13,
                    Description = "simple",
                    Name = "ion100"
                }, new MyClass()
                {
                    Age = 13,
                    Description = "simple",
                    Name = "ion100"
                }
            };
            using (var ses = Configure.Session)
            {
                var tra = ses.BeginTransaction();
                try
                {

                    ses.Save(myClass);

                    myClass.Description = "ss";
                    ses.Save(myClass);




                    ses.InsertBulk(list1);
                    tra.Commit();
                }
                catch (Exception e)
                {
                    tra.Rollback();
                    Console.WriteLine(e);
                    throw;
                }
            }
            ISession session = Configure.Session;
            var tr = session.BeginTransaction();
            try
            {
                var ee12 = await session.Querion<MyClass>().Distinct().ToListAsync();
                session.Save(new MyClass { Age = 6, DateTime = DateTime.Now, Description = "bla" });
                tr.Commit();
            }
            catch (Exception)
            {
                tr.Rollback();
                throw;
            }
            finally
            {
                session.Dispose();
            }

            //var sd5 = Configure.Session.GetListOtherBase<MyClass>(new MyDb(), "select * from my_class where name<> @1","100");

            foreach (var r in Configure.Session.FreeSql<MyEnum>("select enum as enum1 from my_class"))
            {
                Console.WriteLine($" enum={r}");
            }
            foreach (var r in Configure.Session.FreeSql<dynamic>("select enum as enum1,age from my_class"))
                Console.WriteLine($" enum1={r.enum1} age={r.age}");


            foreach (var r in Configure.Session.FreeSql<MyClassTemp>("select enum as enum1, age, id from my_class"))
                Console.WriteLine($"{r}");

            foreach (var r in TempSql(new { enum1 = 1, age = 2 }))
                Console.WriteLine($"{r}");


            var ses1 = Configure.Session;
            string t = ses1.TableName<MyClass>();
            var i0 = Configure.Session.Querion<MyClass>().Where(a => a.Age == 12).ToList();
            var i = Configure.Session.Querion<MyClass>().Where(a => a.Age == 12).
            Update(s => new Dictionary<object, object> { { s.Age, 100 }, { s.Name, "simple" } });
            var @calss = ses1.GetList<MyClass>("age =100 order by age ").FirstOrDefault();

            var eri = Configure.Session.Querion<MyClass>().ToList();
            var list3 = ses1.Querion<MyClass>().
                Where(a => (a.Age > 5 || a.Name.StartsWith("ion100")) && a.Name.Contains("100")).
                OrderBy(d => d.Age).
                Select(f => f.Age).
                Limit(0, 2);
            await list3.ToListAsync().ContinueWith(r =>
            {
                Console.WriteLine(r.Result.Count);
            });
            var ee3 = Configure.Session.Querion<MyClass>().DistinctCore(s => new { ass = s.Age, name = s.Name }).ToList();
            foreach (var s in ee3)
            {

            }



            var rs = await ses1.Querion<MyClass>()
                .Where(a => a.Name != null)
                .Select(d => new { sd = d.Age, name = d.Name })
                .ToListAsync();
            foreach (var r in rs)
            {
                Console.WriteLine(r);
            }
            List<MyClass> val1 = ses1.FreeSql<MyClass>(
                $"select * from {ses1.TableName<MyClass>()} where \"name\" LIKE CONCAT(@p1,'%')", new Parameter("@p1", "ion100")).ToList();
            var isP = ses1.IsPersistent(val1[0]);

            using (var dataTable = ses1.GetDataTable($"select * from {ses1.TableName<MyClass>()} "))
            {
                var coutn = dataTable.Rows.Count;
            }

            Console.ReadKey();
        }

        static IEnumerable<T> TempSql<T>(T t)
        {
            return Configure.Session.FreeSql<T>("select enum as enum1,age from my_class");
        }


    }

    class MyDbMySql : IOtherDataBaseFactory
    {
        private static readonly Lazy<DbProviderFactory> dbProviderFactory = new Lazy<DbProviderFactory>(() =>
        {
            return new MySqlClientFactory();
        });
        public ProviderName  GetProviderName()
        {
            return ProviderName.MySql;
        }
        public string GetConnectionString()
        {
            return "Server=localhost;Database=test;Uid=root;Pwd=12345;";
        }

        public DbProviderFactory GetDbProviderFactories()
        {
            return dbProviderFactory.Value;
        }
    }
    [MapTableName("nomap")]
    class MyClassTemp
    {
        [MapColumnName("id")]
        public Guid Id { get; set; } = Guid.NewGuid();
        [MapColumnName("enum1")]
        public MyEnum MyEnum { get; set; }
        [MapColumnName("age")]
        public int Age { get; set; }
        public override string ToString()
        {
            return $" Id={Id}, MyEnum={MyEnum}, Age={Age}";
        }
    }



    static class Starter
    {
        public static void Run()
        {

            string path = null;
#if DEBUG
            path =  "SqlLog.txt";
#endif
            _ = new Configure("Server=localhost;Port=5432;Database=testorm;User Id=postgres;Password=ion100312873;",
                ProviderName.Postgresql, path);
            using (var ses = Configure.Session)
            {
                if (ses.TableExists<MyClass>())
                {
                    ses.DropTable<MyClass>();
                }
                if (ses.TableExists<MyClass>() == false)
                {
                    ses.TableCreate<MyClass>();
                }
            }
        }
    }

    [MapTableName("my_class")]
    class MyClass : MyClassBase
    {

    }


    [MapTableName("my_class")]
    class MyClassMysql : MyClassBase
    {

    }
    [MapTableName("my_class2")]
    class MyClassMysql2 : MyClassBase
    {

    }

    class MyClassBase
    {
        [MapPrimaryKey("id", Generator.Assigned)]
        public Guid Id { get; set; } = Guid.NewGuid();

        [MapColumnName("name")]
        public string Name { get; set; }


        [MapColumnName("age")]
        [MapIndex]
        public int Age { get; set; }

        [MapColumnName("desc")]
        [MapColumnType("TEXT NULL")]
        public string Description { get; set; }

        [MapColumnName("enum")]
        public MyEnum MyEnum { get; set; } = MyEnum.First;

        [MapColumnName("date")]
        public DateTime DateTime { get; set; } = DateTime.Now;

        [MapColumnName("test")]
        public List<MyTest> Test23 { get; set; } = new List<MyTest>() { new MyTest() { Name = "simple" }
        };


    }

    class MyTest
    {
        public string Name { get; set; }
    }

    enum MyEnum
    {
        Def = 0, First = 1
    }
}
