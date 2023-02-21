using ORM_1_21_;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ORM_1_21_.Linq;
// ReSharper disable All

namespace TestLibrary
{
    public class Execute
    {
      

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

        public static void TotalTest()
        {

            NewExe<MyClassPostgres, MyDbPostgres>();
            NewExe<MyClassMysql, MyDbMySql>();
            NewExe<MyClassMsSql, MyDbMsSql>();
            NewExe<MyClassSqlite, MyDbSqlite>();
        }

        private static void NewExe<T,Tb>() where T : MyClassBase, new() where Tb: IOtherDataBaseFactory, new ()
        {
            var s = Activator.CreateInstance<Tb>();
            Console.WriteLine($"**************************{s.GetProviderName()}*****************************");
            
            ISession session = Configure.GetSession<Tb>();

            if (session.TableExists<T>())
            {
                session.TruncateTable<T>();
            }

            session.TableCreate<T>();

            var dt = DateTime.Now;
            var myClass = new T()
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
            List<T> res = null;
            res = session.Querion<T>().Where(a => a.Age == 12 && a.DateTime.Year == dt.Year).ToList();
            Console.WriteLine($"{1} {res.Count == 1}");
            res = session.Querion<T>().Where(a => a.Age == 12 && a.DateTime.Month == dt.Month).ToList();
            Console.WriteLine($"{2} {res.Count == 1}");
            res = session.Querion<T>().Where(a => a.Age == 12 && a.DateTime.Hour == dt.Hour).ToList();
            Console.WriteLine($"{3} {res.Count == 1}");
            res = session.Querion<T>().Where(a => a.Age == 12 && a.DateTime.Day == dt.Day).ToList();
            Console.WriteLine($"{4} {res.Count == 1}");
            res = session.Querion<T>().Where(a => a.Age == 12 && a.DateTime.Minute == dt.Minute).ToList();
            Console.WriteLine($"{5} {res.Count == 1} минуты могут не совпадать");
            res = session.Querion<T>().Where(a => a.Age == 12 && a.DateTime.Second == dt.Second).ToList();
            Console.WriteLine($"{6} {res.Count == 1} секунды могут не совпадать");
            res = session.Querion<T>().Where(a => a.Age == 12 && a.DateTime.DayOfYear == dt.DayOfYear).ToList();
            Console.WriteLine($"{7} {res.Count == 1}");
            res = session.Querion<T>().Where(a => a.Age == 12 && a.DateTime.DayOfWeek == dt.DayOfWeek).ToList();
            Console.WriteLine($"{8} {res.Count == 1}");




            res = session.Querion<T>().Where(a => a.Age == 12 && a.DateTime.AddYears(1).Year == dt.Year + 1)
                .ToList();
            Console.WriteLine($"{9} {res.Count == 1}");
            res = session.Querion<T>().Where(a => a.Age == 12 && a.DateTime.AddMonths(1).Month == dt.Month + 1)
                .ToList();
            Console.WriteLine($"{10} {res.Count == 1}");
            res = session.Querion<T>().Where(a => a.Age == 12 && a.DateTime.AddHours(1).Hour == dt.Hour + 1)
                .ToList();
            Console.WriteLine($"{11} {res.Count == 1}");
            res = session.Querion<T>().Where(a => a.Age == 12 && a.DateTime.AddDays(1).Day == dt.Day + 1)
                .ToList();
            Console.WriteLine($"{12} {res.Count == 1}");
            res = session.Querion<T>().Where(a => a.Age == 12 && a.DateTime.AddMinutes(1).Minute == dt.Minute + 1)
                .ToList();
            Console.WriteLine($"{13} {res.Count == 1} минуты могут не совпадать");
            res = session.Querion<T>().Where(a => a.Age == 12 && a.DateTime.AddSeconds(1).Second == dt.Second + 1)
                .ToList();
            Console.WriteLine($"{14} {res.Count == 1}  секунды могут не совпадать");
            res = session.Querion<T>().Where(a => a.Age == 12 && a.Name.Concat("a").Concat("a") == "nameaa")
                .ToList();
            Console.WriteLine($"{15} {res.Count == 1}");
            res = session.Querion<T>().Where(a => a.Age == 12 && a.Name.Substring(0) == "name").ToList();
            Console.WriteLine($"{16} {res.Count == 1}");
            res = session.Querion<T>().Where(a => a.Age == 12 && a.Name.Substring(0, 1) == "n").ToList();
            Console.WriteLine($"{17} {res.Count == 1}");
            res = session.Querion<T>().Where(a => a.Age == 12 && a.Name.Contains("ame")).ToList();
            Console.WriteLine($"{18} {res.Count == 1}");

            if (s.GetProviderName() == ProviderName.MsSql)
            {
                T my1 = session.Querion<T>().FirstOrDefault(A => A.Age == 12);
                my1.Name = " dnamed ";
                session.Save(my1);
                res = session.Querion<T>().Where(a => a.Age == 12 && a.Name.Trim() == "dnamed").ToList();
                Console.WriteLine($"{19} {res.Count == 1}");
                res = session.Querion<T>().Where(a => a.Age == 12 && a.Name.TrimStart() == "dnamed ").ToList();
                Console.WriteLine($"{20} {res.Count == 1}");
                var sss = session.Querion<T>().Select(a => new { sdsd = a.Name.TrimEnd() }).ToList();
                res = session.Querion<T>().Where(a => a.Age == 12 && a.Name.TrimEnd() == " dnamed").ToList();
                Console.WriteLine($"{21} {res.Count == 1}");
            }
            else
            {
                T my2 = session.Querion<T>().FirstOrDefault(A => A.Age == 12);
                my2.Name = "dnamed";
                session.Save(my2);
                res = session.Querion<T>().Where(a => a.Age == 12 && a.Name.Trim('4') == "dnamed").ToList();
                Console.WriteLine($"{19} {res.Count == 1}");
                res = session.Querion<T>().Where(a => a.Age == 12 && a.Name.TrimStart('d') == "named").ToList();
                Console.WriteLine($"{20} {res.Count == 1}");
                res = session.Querion<T>().Where(a => a.Age == 12 && a.Name.TrimEnd('d') == "dname").ToList();
                Console.WriteLine($"{21} {res.Count == 1}");
            }

            T my3 = session.Querion<T>().FirstOrDefault(A => A.Age == 12);
            my3.Name = "name";
            session.Save(my3);
            var err = session.Querion<T>().Select(a => new { sd = a.Name.Length }).ToList();
            res = session.Querion<T>().Where(a => a.Name.Length == "name".Length).ToList();
            Console.WriteLine($"{22} {res.Count == 1}");
            res = session.Querion<T>().Where(a => a.Name.ToUpper() == "NAME".ToUpper().Trim()).ToList();
            Console.WriteLine($"{23} {res.Count == 1}");
            res = session.Querion<T>().Where(a => a.Name.ToLower() == "NAME".ToLower().Trim()).ToList();
            Console.WriteLine($"{24} {res.Count == 1}");

            List<T> list = new List<T>()
            {
                new T(){Name = "MyName1",Age = 10},
                new T(){Name = "MyName2",Age = 20},
                new T(){Name = "MyName3",Age = 30},
            };
            var i = session.InsertBulk(list);
            Console.WriteLine($"{24}/1 InsertBulk {i == 3}");

            var count = session.Querion<T>().Count();

            Console.WriteLine($"{25} {count == 4}");
            var o = session.Querion<T>().OrderBy(a => a.Age).FirstOrDefault();

            Console.WriteLine($"{26} {o != null && o.Age == 10}");
            Console.WriteLine("Тест транзакции");
            session.TruncateTable<T>();
            count = session.Querion<T>().Count();
            Console.WriteLine($"{27} {count == 0}");

            IsolationLevel? level = null;
            {
                var ses = Configure.GetSession<Tb>();
                try
                {
                    ses.Save(new T());
                    var tr = ses.BeginTransaction(level);
                    ses.Save(new T());
                    ses.Save(new T());
                    ses.Save(new T());
                    tr.Rollback();
                    ses.Save(new T());
                    tr = ses.BeginTransaction(level);
                    ses.Save(new T());
                    ses.Save(new T());
                    ses.Save(new T());
                    tr.Rollback();
                    ses.Save(new T());

                }
                finally
                {
                    ses.Dispose();

                }

                count = session.Querion<T>().Count();
                Console.WriteLine($"{28} {count == 3}");
            }
            session.TruncateTable<T>();
            {
                var ses = Configure.GetSession<Tb>();
                try
                {
                    ses.Save(new T());
                    var tr = ses.BeginTransaction(level);
                    ses.Save(new T());
                    ses.Save(new T());
                    ses.Save(new T());
                    tr.Commit();
                    ses.Save(new T());
                    tr = ses.BeginTransaction(level);
                    ses.Save(new T());
                    ses.Save(new T());
                    ses.Save(new T());
                    tr.Rollback();
                    ses.Save(new T());

                }
                finally
                {
                    ses.Dispose();

                }

                count = session.Querion<T>().Count();
                Console.WriteLine($"{29} {count == 6}");
                session.TruncateTable<T>();
                session.Save(new T
                {
                    Name = "name",
                    Age = 12
                });
                res = session.Querion<T>().Where(a => a.Name.Substring(1).Reverse() == "ema").ToList();
                Console.WriteLine($"{30} {res.Count == 1}");
                res = session.Querion<T>().Where(a => string.IsNullOrEmpty(a.Description)).ToList();
                Console.WriteLine($"{31} {res.Count == 1}");
                o = session.Querion<T>().Where(a => a.Age == 12).Single();
                Console.WriteLine($"{31} {o != null}");
                try
                {
                    o = session.Querion<T>().Where(a => a.Age == 14).Single();
                    Console.WriteLine($"{32} {false}");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{32} {true} {e.Message}");
                }

                o = session.Querion<T>().Where(a => a.Age == 14).SingleOrDefault();
                Console.WriteLine($"{33} {o == null}");
                try
                {
                    o = session.Querion<T>().Where(a => a.Age == 14).First();
                    Console.WriteLine($"{34} {false}");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{34} {true} {e.Message}");
                }

                o = session.Querion<T>().Where(a => a.Age == 14).FirstOrDefault();
                Console.WriteLine($"{34} {o == null}");
                session.TruncateTable<T>();
                session.InsertBulk(new List<T>()
                {
                    new T() { Age = 40, Name = "name" },
                    new T() { Age = 20, Name = "name1" },
                    new T() { Age = 30, Name = "name1" },
                    new T() { Age = 50, Name = "name1" },
                    new T() { Age = 60, Name = "name" },
                    new T() { Age = 10, Name = "name" },
                });
                var ob=session.Querion<T>().Select(a => new { ass = a.Age, asss = string.Concat(a.Name, a.Age) }).ToList();
                Console.WriteLine($"{35} {ob.Count()==6}");
                count = session.Querion<T>().Where(a => a.Name == "name").OrderBy(r => r.Age).ToList().Sum(a => a.Age);
                Console.WriteLine($"{36} {count == 110}");
                var groupList = session.Querion<T>().GroupBy(r => r.Name).ToList();
                Console.WriteLine($"{37} {groupList.Count()==2&&groupList[0].Count()==3&&groupList[1].Count()==3}");

                o = session.Querion<T>().OrderBy(a => a.Age).First();
                Console.WriteLine($"{38} {o.Age==10}");
                o = session.Querion<T>().OrderByDescending(a => a.Age).First();
                Console.WriteLine($"{39} {o.Age == 60}");
                count =  session.Querion<T>().Where(a => a.Age < 100).OrderBy(ds => ds.Age).Limit(0,2).ToListAsync().Result.Sum(a=>a.Age);
                Console.WriteLine($"{39} {count == 30}");
                var sCore = session.Querion<T>().Where(a=>a.Name.Contains("1")).DistinctCore(a => a.Name);
                Console.WriteLine($"{40} {sCore.Count() == 1}");
                count = session.Querion<T>().Where(sw=>sw.Age==10).Update(d => new Dictionary<object, object>
                {
                    { d.Name,"ass"},
                    { d.DateTime,DateTime.Now}
                });
               
                res = session.Querion<T>().Where(a => a.Name == "ass").ToList();
                Console.WriteLine($"{41} {res.Count()==1}");
                session.Querion<T>().Delete(a => a.Name == "ass");
                res = session.Querion<T>().Where(a => a.Name == "ass").ToList();
                Console.WriteLine($"{42} {res.Count() == 0}");
                session.Querion<T>().Where(a => a.Age == 10).Delete();
                count = session.Querion<T>().Where(a => a.Age == 10).Count();
                Console.WriteLine($"{42} {count == 0}");






            }





        }
    }
}
