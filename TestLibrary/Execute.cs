using ORM_1_21_;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestLibrary
{
    public class Execute
    {
        public static async Task RunExecute()
        {
            ISession sesw = Configure.GetSession<MyDbMySql>();
            if (sesw.TableExists<MyClassMysql>())
            {
                sesw.DropTable<MyClassMysql>();
            }
            sesw.TableCreate<MyClassMysql>();
            List<MyClassMysql> l = new List<MyClassMysql>
            {
                new MyClassMysql(){Age=100},
                new MyClassMysql(){Age=101},
                new MyClassMysql(){Age=102},
            };
            sesw.InsertBulk(l);
            var myClassMysqls = sesw.Querion<MyClassMysql>().Where(a => a.Age != 3);
            foreach(var my in myClassMysqls)
            {
                Console.WriteLine(my.Age);
            }
            
            

            var tr44 = sesw.BeginTransaction(System.Data.IsolationLevel.Serializable);
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

            var tr1 = sesw.BeginTransaction(System.Data.IsolationLevel.Serializable);
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
                var tra = ses.BeginTransaction(System.Data.IsolationLevel.Serializable);
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
            var tr = session.BeginTransaction(System.Data.IsolationLevel.Serializable);
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


            //foreach (var r in Configure.Session.FreeSql<MyClassTemp>("select enum as enum1, age, id from my_class"))
            //    Console.WriteLine($"{r}");

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
            //List<MyClass> val1 = ses1.FreeSql<MyClass>(
              //  $"select * from {ses1.TableName<MyClass>()} where \"name\" LIKE CONCAT(@p1,'%')", new Parameter("@p1", "ion100")).ToList();
            //var isP = ses1.IsPersistent(val1[0]);

            using (var dataTable = ses1.GetDataTable($"select * from {ses1.TableName<MyClass>()} "))
            {
                var coutn = dataTable.Rows.Count;
            }
        }

        static IEnumerable<T> TempSql<T>(T t)
        {
            return Configure.Session.FreeSql<T>("select enum as enum1,age from my_class");
        }
        public static void RunThread()
        {
            new Thread(async () =>
            {
                while (true)
                {
                    var ses = Configure.Session;
                    var ts = ses.BeginTransaction(System.Data.IsolationLevel.Serializable);
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
                    var ts = ses.BeginTransaction(System.Data.IsolationLevel.Serializable);
                    MyClass c = new MyClass();
                    ses.Save(c);

                    var s = Configure.Session.Querion<MyClass>().Where(a => a.Age != -1).ToList();
                    Console.WriteLine("2 -- " + s.Count());
                    ts.Commit();
                }

            }).Start();
        }
    }
}
