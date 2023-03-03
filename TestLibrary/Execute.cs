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
using ORM_1_21_.Extensions;
using ORM_1_21_.Linq;
// ReSharper disable All

namespace TestLibrary
{
    public class Execute
    {
      

       

         private static void Running(int i)
         {
             while (true)
             {
                 using (var ses = Configure.Session)
                 {
                     var ts = ses.BeginTransaction();
                     MyClass c = new MyClass();
                     ses.Save(c);
                     var s = Configure.Session.Query<MyClass>().ToList().Count();
                     Console.WriteLine($"{i} -- " + s);
                     ts.Commit();
                 }
             }
        }

        public static void RunThread()
        {
            Task.Factory.StartNew(() => { Running(1);}, TaskCreationOptions.LongRunning);
            Task.Factory.StartNew(() => { Running(2); }, TaskCreationOptions.LongRunning);
            Task.Factory.StartNew(() => { Running(3); }, TaskCreationOptions.LongRunning);
            Task.Factory.StartNew(() => { Running(4); }, TaskCreationOptions.LongRunning);

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
                session.DropTable<T>();
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
            res = session.Query<T>().Where(a => a.Age == 12 && a.DateTime.Year == dt.Year).ToList();
            Console.WriteLine($"{1} {res.Count == 1}");
            res = session.Query<T>().Where(a => a.Age == 12 && a.DateTime.Month == dt.Month).ToList();
            Console.WriteLine($"{2} {res.Count == 1}");
            res = session.Query<T>().Where(a => a.Age == 12 && a.DateTime.Hour == dt.Hour).ToList();
            Console.WriteLine($"{3} {res.Count == 1}");
            res = session.Query<T>().Where(a => a.Age == 12 && a.DateTime.Day == dt.Day).ToList();
            Console.WriteLine($"{4} {res.Count == 1}");
            res = session.Query<T>().Where(a => a.Age == 12 && a.DateTime.Minute == dt.Minute).ToList();
            Console.WriteLine($"{5} {res.Count == 1} минуты могут не совпадать");
            res = session.Query<T>().Where(a => a.Age == 12 && a.DateTime.Second == dt.Second).ToList();
            Console.WriteLine($"{6} {res.Count == 1} секунды могут не совпадать");
            res = session.Query<T>().Where(a => a.Age == 12 && a.DateTime.DayOfYear == dt.DayOfYear).ToList();
            Console.WriteLine($"{7} {res.Count == 1}");
            res = session.Query<T>().Where(a => a.Age == 12 && a.DateTime.DayOfWeek == dt.DayOfWeek).ToList();
            Console.WriteLine($"{8} {res.Count == 1}");




            res = session.Query<T>().Where(a => a.Age == 12 && a.DateTime.AddYears(1).Year == dt.Year + 1)
                .ToList();
            Console.WriteLine($"{9} {res.Count == 1}");
            res = session.Query<T>().Where(a => a.Age == 12 && a.DateTime.AddMonths(1).Month == dt.Month + 1)
                .ToList();
            Console.WriteLine($"{10} {res.Count == 1}");
            res = session.Query<T>().Where(a => a.Age == 12 && a.DateTime.AddHours(1).Hour == dt.Hour + 1)
                .ToList();
            Console.WriteLine($"{11} {res.Count == 1}");
            res = session.Query<T>().Where(a => a.Age == 12 && a.DateTime.AddDays(1).Day == dt.Day + 1)
                .ToList();
            Console.WriteLine($"{12} {res.Count == 1}");
            res = session.Query<T>().Where(a => a.Age == 12 && a.DateTime.AddMinutes(1).Minute == dt.Minute + 1)
                .ToList();
            Console.WriteLine($"{13} {res.Count == 1} минуты могут не совпадать");
            res = session.Query<T>().Where(a => a.Age == 12 && a.DateTime.AddSeconds(1).Second == dt.Second + 1)
                .ToList();
            Console.WriteLine($"{14} {res.Count == 1}  секунды могут не совпадать");
            res = session.Query<T>().Where(a => a.Age == 12 && a.Name.Concat("a").Concat("a") == "nameaa")
                .ToList();
            Console.WriteLine($"{15} {res.Count == 1}");
            res = session.Query<T>().Where(a => a.Age == 12 && a.Name.Substring(0) == "name").ToList();
            Console.WriteLine($"{16} {res.Count == 1}");
            res = session.Query<T>().Where(a => a.Age == 12 && a.Name.Substring(0, 1) == "n").ToList();
            Console.WriteLine($"{17} {res.Count == 1}");
            res = session.Query<T>().Where(a => a.Age == 12 && a.Name.Contains("ame")).ToList();
            Console.WriteLine($"{18} {res.Count == 1}");

            if (s.GetProviderName() == ProviderName.MsSql)
            {
                T my1 = session.Query<T>().FirstOrDefault(A => A.Age == 12);
                my1.Name = " dnamed ";
                session.Save(my1);
                res = session.Query<T>().Where(a => a.Age == 12 && a.Name.Trim() == "dnamed").ToList();
                Console.WriteLine($"{19} {res.Count == 1}");
                res = session.Query<T>().Where(a => a.Age == 12 && a.Name.TrimStart() == "dnamed ").ToList();
                Console.WriteLine($"{20} {res.Count == 1}");
                var sss = session.Query<T>().Select(a => new { sdsd = a.Name.TrimEnd() }).ToList();
                res = session.Query<T>().Where(a => a.Age == 12 && a.Name.TrimEnd() == " dnamed").ToList();
                Console.WriteLine($"{21} {res.Count == 1}");
            }
            else
            {
                T my2 = session.Query<T>().FirstOrDefault(A => A.Age == 12);
                my2.Name = "dnamed";
                session.Save(my2);
                res = session.Query<T>().Where(a => a.Age == 12 && a.Name.Trim('4') == "dnamed").ToList();
                Console.WriteLine($"{19} {res.Count == 1}");
                res = session.Query<T>().Where(a => a.Age == 12 && a.Name.TrimStart('d') == "named").ToList();
                Console.WriteLine($"{20} {res.Count == 1}");
                res = session.Query<T>().Where(a => a.Age == 12 && a.Name.TrimEnd('d') == "dname").ToList();
                Console.WriteLine($"{21} {res.Count == 1}");
            }

            T my3 = session.Query<T>().FirstOrDefault(A => A.Age == 12);
            my3.Name = "name";
            session.Save(my3);
            var err = session.Query<T>().Select(a => new { sd = a.Name.Length }).ToList();
            res = session.Query<T>().Where(a => a.Name.Length == "name".Length).ToList();
            Console.WriteLine($"{22} {res.Count == 1}");
            res = session.Query<T>().Where(a => a.Name.ToUpper() == "NAME".ToUpper().Trim()).ToList();
            Console.WriteLine($"{23} {res.Count == 1}");
            res = session.Query<T>().Where(a => a.Name.ToLower() == "NAME".ToLower().Trim()).ToList();
            Console.WriteLine($"{24} {res.Count == 1}");

            List<T> list = new List<T>()
            {
                new T(){Name = "MyName1",Age = 10},
                new T(){Name = "MyName2",Age = 20},
                new T(){Name = "MyName3",Age = 30},
            };
            var i = session.InsertBulk(list);
            Console.WriteLine($"{24}/1 InsertBulk {i == 3}");

            var count = session.Query<T>().Count();

            Console.WriteLine($"{25} {count == 4}");
            var o = session.Query<T>().OrderBy(a => a.Age).FirstOrDefault();

            Console.WriteLine($"{26} {o != null && o.Age == 10}");
            Console.WriteLine("Тест транзакции");
            session.TruncateTable<T>();
            count = session.Query<T>().Count();
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

                count = session.Query<T>().Count();
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

                count = session.Query<T>().Count();
                Console.WriteLine($"{29} {count == 6}");
                session.TruncateTable<T>();
                session.Save(new T
                {
                    Name = "name",
                    Age = 12
                });
                res = session.Query<T>().Where(a => a.Name.Substring(1).Reverse() == "ema").ToList();
                Console.WriteLine($"{30} {res.Count == 1}");
                res = session.Query<T>().Where(a => string.IsNullOrEmpty(a.Description)).ToList();
                Console.WriteLine($"{31} {res.Count == 1}");
                o = session.Query<T>().Where(a => a.Age == 12).Single();
                Console.WriteLine($"{31} {o != null}");
                try
                {
                    o = session.Query<T>().Where(a => a.Age == 14).Single();
                    Console.WriteLine($"{32} {false}");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{32} {true} {e.Message}");
                }

                o = session.Query<T>().Where(a => a.Age == 14).SingleOrDefault();
                Console.WriteLine($"{33} {o == null}");
                try
                {
                    o = session.Query<T>().Where(a => a.Age == 14).First();
                    Console.WriteLine($"{34} {false}");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{34} {true} {e.Message}");
                }

                o = session.Query<T>().Where(a => a.Age == 14).FirstOrDefault();
                Console.WriteLine($"{34} {o == null}");
                session.TruncateTable<T>();
                session.InsertBulk(new List<T>()
                {
                    new T() { Age = 40, Name = "name" ,MyTest = new MyTest{Name = "simple"}},
                    new T() { Age = 20, Name = "name1",MyTest = new MyTest{Name = "simple"} },
                    new T() { Age = 30, Name = "name1",MyTest = new MyTest{Name = "simple"} },
                    new T() { Age = 50, Name = "name1",MyTest = new MyTest{Name = "simple"} },
                    new T() { Age = 60, Name = "name" ,MyTest = new MyTest{Name = "simple"}},
                    new T() { Age = 10, Name = "name",MyTest = new MyTest{Name = "simple"}},
                });
                var ob=session.Query<T>().Select(a => new { ass = a.Age, asss = string.Concat(a.Name, a.Age) }).ToList();
                Console.WriteLine($"{35} {ob.Count()==6}");
                count = session.Query<T>().Where(a => a.Name == "name").OrderBy(r => r.Age).ToList().Sum(a => a.Age);
                Console.WriteLine($"{36} {count == 110}");
                //List<IGrouping<string, T>> groupList = session.Query<T>().GroupBy(r => r.Name).ToListAsyncEx().Result;
                //Console.WriteLine($"{37} {groupList.Count()==2&&groupList[0].Count()==3&&groupList[1].Count()==3}");

                o = session.Query<T>().OrderBy(a => a.Age).First();
                Console.WriteLine($"{38} {o.Age==10}");
                o = session.Query<T>().OrderByDescending(a => a.Age).First();
                Console.WriteLine($"{39} {o.Age == 60}");
                count =  session.Query<T>().Where(a => a.Age < 100).OrderBy(ds => ds.Age).ToListAsync().Result.Sum(a=>a.Age);
                Console.WriteLine($"{40} {count == 210}");
                var sCore = session.Query<T>().Where(a=>a.Name.Contains("1")).Distinct(a => a.Name);
                Console.WriteLine($"{41} {sCore.Count() == 1}");
                count = session.Query<T>().Where(sw=>sw.Age==10).Update(d => new Dictionary<object, object>
                {
                    { d.Name,string.Concat(d.Name,d.Age)},
                    { d.DateTime,DateTime.Now}
                });
               
                res = session.Query<T>().Where(a => a.Name == "name10").ToList();
                Console.WriteLine($"{42} {res.Count()==1}");
                session.Query<T>().Delete(a => a.Name == "name10");
                res = session.Query<T>().Where(a => a.Name == "name10").ToList();
                Console.WriteLine($"{43} {res.Count() == 0}");
                session.Query<T>().Where(a => a.Age == 10).Delete();
                count = session.Query<T>().Where(a => a.Age == 10).Count();
                Console.WriteLine($"{44} {count == 0}");
                res =session.FreeSql<T>($"select * from {session.TableName<T>()} where {session.ColumnName<T>(a=>a.Age)} = @1",
                    new Parameter("@1",40)).ToList();
                Console.WriteLine($"{45} {res.Count() == 1}");


                 var anon1 = session.Query<T>().Where(a => a.Age == 40).Select(d => new { age = d.Age, name = d.Name })
                    .ToList();
                Console.WriteLine($"{46} {anon1.Count() == 1}");


                dynamic di = session.FreeSql<dynamic>($"select age, name from {session.TableName<T>()}");
                Console.WriteLine($"{47} {di.Count == 5}");
                if (s.GetProviderName() == ProviderName.Sqlite)
                {
                    var anon = TempSql(new { age = 3L, name = "asss" }, session, $"select age,name from {session.TableName<T>()}");
                    Console.WriteLine($"{48} {anon.Count() == 5}");
                }
                else
                {
                    var anon = TempSql(new { age = 3, name = "asss" }, session, $"select age,name from {session.TableName<T>()}");
                    Console.WriteLine($"{48} {anon.Count() == 5}");
                }

                var tempFree = session.FreeSql<MyFreeSql>($"select id,name,age,enum from {session.TableName<T>()}");
                Console.WriteLine($"{49} {tempFree.Count() == 5}");
                res = session.Query<T>().Where(a => a.Age < 200).CacheUsage().ToList();
                Console.WriteLine($"{50} {res.Count() == 5}");
                res = session.Query<T>().Where(a => a.Age < 200).CacheUsage().ToList();
                Console.WriteLine($"{51} {res.Count() == 5}");
                var ii = session.Query<T>().Where(a => a.Age < 200).CacheGetKey();
                res = (List<T>)session.CacheGetValue<T>(ii);
                Console.WriteLine($"{52} {res.Count() == 5}");
                session.Query<T>().Where(a => a.Age == 20).Update(f=>new Dictionary<object, object>()
                {
                    {f.Age,400}
                });
                res = session.Query<T>().Where(a => a.Age < 200).CacheOver().ToList();
                res = session.Query<T>().Where(a => a.Age < 200).CacheUsage().ToList();
                Console.WriteLine($"{53} {res.Count() == 4}");
                var ano=session.Query<T>().Where(a => a.Age < 500).Select(f =>
                    new { test = f.MyTest, e = f.MyEnum, r = f.Test23, c = f.DateTime }).ToList();
                Console.WriteLine($"{54} {ano.Count() == 5}");
                var ano1 = session.Query<T>().Distinct(a => a.Age);
                Console.WriteLine($"{55} {ano1.Count() == 5}");
                var ano2 = session.Query<T>().Distinct(a => new {ago=a.Age,myTest=a.MyTest,date=a.DateTime});
                Console.WriteLine($"{56} {ano2.Count() == 5}");








            }
           





        }
        [MapReceiverFreeSql]
        class MyFreeSql
        {
           
            public Guid IdGuid { get; }
            public string Name { get; }
            public int Age { get; }
            public MyEnum MyEnum { get; }

            public MyFreeSql(Guid idGuid, string name, int age, MyEnum @enum)
            {
                IdGuid = idGuid;
                Name = name;
                Age = age;
                MyEnum = (MyEnum)@enum;
            }
         

        }
        public static IEnumerable<Ts> TempSql<Ts>(Ts t, ISession session,string sql)
        {
            return session.FreeSql<Ts>(sql);
        }
    }
}
