using ORM_1_21_;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ORM_1_21_.Linq;
// ReSharper disable All

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
                Console.WriteLine(dataTable.Rows.Count);
            }

            var v1All = ses1.Querion<MyClass>().Where(a => a.Age == 13).Max(s=>s.Age);
            Console.WriteLine($"Max age: {v1All}");

            var v1Aq = ses1.Querion<MyClass>().Where(a => a.Age == 13);

            // var Jq1 = ses1.Querion<MyClass>().Where(a => a.Age == 13).Cast<MyClassBase>();
            // var v1Ae = ses1.Querion<MyClass>().Where(a => a.Age>0 )
            //     .Join(Jq1,s=>s.Id,d=>d.Id,(s,d)=>s);
            // var ss = v1Ae.ToList();

            v1Aq.ForEach(a =>
            {
                Console.WriteLine(a.Id);
            });


            var v1Al = ses1.Querion<MyClass>().Where(a => a.Age == 13).ElementAtOrDefault(0);
            if (v1Al != null) Console.WriteLine("ElementAtOrDefault id = " + v1Al.Id);
            var v1A2 = ses1.Querion<MyClass>().Where(a => a.Age == 13).ElementAtOrDefault(1);
            if (v1A2 != null) Console.WriteLine("ElementAtOrDefault id = " + v1A2.Id);
            var v1A3 = ses1.Querion<MyClass>().Where(a => a.Age == 13).ElementAtOrDefault(2);
            if (v1A3 != null) Console.WriteLine("ElementAtOrDefault id = " + v1A3.Id);
        }

         private static IEnumerable<T> TempSql<T>(T t)
        {
            return Configure.Session.FreeSql<T>("select enum as enum1,age from my_class");
        }

         private static void Running(int i)
         {
             while (true)
             {
                 using (var ses = Configure.Session)
                 {
                     var ts = ses.BeginTransaction(System.Data.IsolationLevel.Serializable);
                     MyClass c = new MyClass();
                     ses.Save(c);
                     var s = Configure.Session.Querion<MyClass>().Count();
                     Console.WriteLine($"{i} -- " + s);
                     ts.Commit();
                 }
             }
        }
        public static void RunThread()
        {
            Task.Factory.StartNew(() => { Running(1);}, TaskCreationOptions.LongRunning);
            Task.Factory.StartNew(() => { Running(2); }, TaskCreationOptions.LongRunning);
           // Task.Factory.StartNew(() => { Running(3); }, TaskCreationOptions.LongRunning);
           // Task.Factory.StartNew(() => { Running(4); }, TaskCreationOptions.LongRunning);

        }

        public static void NewExe(ProviderName name)
        {
            ISession session = Configure.Session;
            var dt = DateTime.Now;
            var myClass = new MyClass
            {
                DateTime = dt,
                Age = 12,
                Description = "simple",
                Name = "name",
                MyEnum = MyEnum.First,
                MyTest = new MyTest { Name = "ass" },
                Test23 = { new MyTest(), new MyTest() }
            };
            session.Save(myClass);
            List<MyClass> res = null;
            res = session.Querion<MyClass>().Where(a => a.Age == 12 && a.DateTime.Year == dt.Year).ToList();
            Console.WriteLine($"{1} {res.Count == 1}");
            res = session.Querion<MyClass>().Where(a => a.Age == 12 && a.DateTime.Month == dt.Month).ToList();
            Console.WriteLine($"{2} {res.Count == 1}");
            res = session.Querion<MyClass>().Where(a => a.Age == 12 && a.DateTime.Hour == dt.Hour).ToList();
            Console.WriteLine($"{3} {res.Count == 1}");
            res = session.Querion<MyClass>().Where(a => a.Age == 12 && a.DateTime.Day == dt.Day).ToList();
            Console.WriteLine($"{4} {res.Count == 1}");
            res = session.Querion<MyClass>().Where(a => a.Age == 12 && a.DateTime.Minute == dt.Minute).ToList();
            Console.WriteLine($"{5} {res.Count == 1} минуты могут не совпадать");
            res = session.Querion<MyClass>().Where(a => a.Age == 12 && a.DateTime.Second == dt.Second).ToList();
            Console.WriteLine($"{6} {res.Count == 1} секунды могут не совпадать");
            res = session.Querion<MyClass>().Where(a => a.Age == 12 && a.DateTime.DayOfYear == dt.DayOfYear).ToList();
            Console.WriteLine($"{7} {res.Count == 1}");
            res = session.Querion<MyClass>().Where(a => a.Age == 12 && a.DateTime.DayOfWeek == dt.DayOfWeek).ToList();
            Console.WriteLine($"{8} {res.Count == 1}");
           



            res = session.Querion<MyClass>().Where(a => a.Age == 12 && a.DateTime.AddYears(1).Year == dt.Year + 1).ToList();
            Console.WriteLine($"{9} {res.Count == 1}");
            res = session.Querion<MyClass>().Where(a => a.Age == 12 && a.DateTime.AddMonths(1).Month == dt.Month + 1).ToList();
            Console.WriteLine($"{10} {res.Count == 1}");
            res = session.Querion<MyClass>().Where(a => a.Age == 12 && a.DateTime.AddHours(1).Hour == dt.Hour + 1).ToList();
            Console.WriteLine($"{11} {res.Count == 1}");
            res = session.Querion<MyClass>().Where(a => a.Age == 12 && a.DateTime.AddDays(1).Day == dt.Day + 1).ToList();
            Console.WriteLine($"{12} {res.Count == 1}");
            res = session.Querion<MyClass>().Where(a => a.Age == 12 && a.DateTime.AddMinutes(1).Minute == dt.Minute + 1).ToList();
            Console.WriteLine($"{13} {res.Count == 1} минуты могут не совпадать" );
            res = session.Querion<MyClass>().Where(a => a.Age == 12 && a.DateTime.AddSeconds(1).Second == dt.Second + 1).ToList();
            Console.WriteLine($"{14} {res.Count == 1}  секунды могут не совпадать");
            res = session.Querion<MyClass>().Where(a => a.Age == 12 && a.Name.Concat("a").Concat("a") == "nameaa").ToList();
            Console.WriteLine($"{15} {res.Count == 1}");
            res = session.Querion<MyClass>().Where(a => a.Age == 12 && a.Name.Substring(0) == "name").ToList();
            Console.WriteLine($"{16} {res.Count == 1}");
            res = session.Querion<MyClass>().Where(a => a.Age == 12 && a.Name.Substring(0, 1) == "n").ToList();
            Console.WriteLine($"{17} {res.Count == 1}");
            res = session.Querion<MyClass>().Where(a => a.Age == 12 && a.Name.Contains("ame")).ToList();
            Console.WriteLine($"{18} {res.Count == 1}");
          
            if (name == ProviderName.MsSql)
            {
                MyClass my1 = session.Querion<MyClass>().FirstOrDefault(A => A.Age == 12);
                my1.Name = " dnamed ";
                session.Save(my1);
                res = session.Querion<MyClass>().Where(a => a.Age == 12 && a.Name.Trim() == "dnamed").ToList();
                Console.WriteLine($"{19} {res.Count == 1}");
                res = session.Querion<MyClass>().Where(a => a.Age == 12 && a.Name.TrimStart() == "dnamed ").ToList();
                Console.WriteLine($"{20} {res.Count == 1}");
                var sss = session.Querion<MyClass>().Select(a => new { sdsd = a.Name.TrimEnd() }).ToList();
                res = session.Querion<MyClass>().Where(a => a.Age == 12 && a.Name.TrimEnd() == " dnamed").ToList();
                Console.WriteLine($"{21} {res.Count == 1}");
            }
            else
            {
                MyClass my2 = session.Querion<MyClass>().FirstOrDefault(A => A.Age == 12);
                my2.Name = "dnamed";
                session.Save(my2);
                res = session.Querion<MyClass>().Where(a => a.Age == 12 && a.Name.Trim('4') == "dnamed").ToList();
                Console.WriteLine($"{19} {res.Count == 1}");
                res = session.Querion<MyClass>().Where(a => a.Age == 12 && a.Name.TrimStart('d') == "named").ToList();
                Console.WriteLine($"{20} {res.Count == 1}");
                res = session.Querion<MyClass>().Where(a => a.Age == 12 && a.Name.TrimEnd('d') == "dname").ToList();
                Console.WriteLine($"{21} {res.Count == 1}");
            }
            MyClass my3 = session.Querion<MyClass>().FirstOrDefault(A => A.Age == 12);
            my3.Name = "name";
            session.Save(my3);
            var err = session.Querion<MyClass>().Select(a => new { sd = a.Name.Length }).ToList();
            res =session.Querion<MyClass>().Where(a => a.Name.Length == "name".Length).ToList();
            Console.WriteLine($"{22} {res.Count == 1}");
            res=session.Querion<MyClass>().Where(a => a.Name.ToUpper() == "NAME".ToUpper().Trim()).ToList();
            Console.WriteLine($"{23} {res.Count == 1}");
            res= session.Querion<MyClass>().Where(a => a.Name.ToLower() == "NAME".ToLower().Trim()).ToList();
            Console.WriteLine($"{24} {res.Count == 1}");
         
            List<MyClass> list = new List<MyClass>()
            {
                Starter.GetMyClass(10, "MyName1"),
                Starter.GetMyClass(30, "MyName3"),
                Starter.GetMyClass(20, "MyName2"),
            };
            var i=session.InsertBulk(list);
            Console.WriteLine($"{24}/1 InsertBulk {i == 3}");

            var count = session.Querion<MyClass>().Count();
           
            Console.WriteLine($"{25} {count == 4}");
            var o = session.Querion<MyClass>().OrderBy(a=>a.Age).FirstOrDefault(); 
           
            Console.WriteLine($"{26} {o!=null&&o.Age==10}");
            Console.WriteLine("Тест транзакции");
            session.TruncateTable<MyClass>();
            count = session.Querion<MyClass>().Count();
            Console.WriteLine($"{27} {count == 0}");

            IsolationLevel? level = null;
            { 
                var ses = Configure.Session;
                try
                {
                    ses.Save(new MyClass());
                    var tr = ses.BeginTransaction(level);
                    ses.Save(new MyClass());
                    ses.Save(new MyClass());
                    ses.Save(new MyClass());
                    tr.Rollback();
                    ses.Save(new MyClass());
                    tr = ses.BeginTransaction(level);
                    ses.Save(new MyClass());
                    ses.Save(new MyClass());
                    ses.Save(new MyClass());
                    tr.Rollback();
                    ses.Save(new MyClass());

                }
                finally
                {
                    ses.Dispose();

                }

                count = session.Querion<MyClass>().Count();
                Console.WriteLine($"{28} {count == 3}");
            }
            session.TruncateTable<MyClass>();
            {
                var ses = Configure.Session;
                try
                {
                    ses.Save(new MyClass());
                    var tr = ses.BeginTransaction(level);
                    ses.Save(new MyClass());
                    ses.Save(new MyClass());
                    ses.Save(new MyClass());
                    tr.Commit();
                    ses.Save(new MyClass());
                    tr = ses.BeginTransaction(level);
                    ses.Save(new MyClass());
                    ses.Save(new MyClass());
                    ses.Save(new MyClass());
                    tr.Rollback();
                    ses.Save(new MyClass());

                }
                finally
                {
                    ses.Dispose();

                }

                count = session.Querion<MyClass>().Count();
                Console.WriteLine($"{29} {count == 6}");
                // var ss = session.Querion<MyClass>().Where(a => a.Name.Substring(1).Reverse() == "ema").ToList();
                //var ss = session.Querion<MyClass>().Where(a => string.IsNullOrEmpty(a.Description.Replace("''", "'")) == true).ToList();
            }





        }
    }
}
