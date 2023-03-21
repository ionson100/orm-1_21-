using ORM_1_21_;
using ORM_1_21_.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

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
                    MyClass c = new MyClass(1);
                    ses.Save(c);
                    var s = Configure.Session.Query<MyClass>().Count();
                    Console.WriteLine($"{i} -- " + s);
                    ts.Commit();
                }
            }
        }

        public static void RunThread()
        {
            Task.Factory.StartNew(() => { Running(1); }, TaskCreationOptions.LongRunning);
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

        public static void TotalTestNull()
        {

            NewExeNull<ClassNullPostgres, MyDbPostgres>();
            NewExeNull<ClassNullMysql, MyDbMySql>();
            NewExeNull<ClassNullMsSql, MyDbMsSql>();
            NewExeNull<ClassNullSqlite, MyDbSqlite>();
        }

        private static void NewExe<T, Tb>() where T : MyClassBase, new() where Tb : IOtherDataBaseFactory, new()
        {
            var s = Activator.CreateInstance<Tb>();
            Configure.WriteLogFile($"**************************{s.GetProviderName()}*****************************");
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
            Configure.WriteLogFile($"{1} {res.Count == 1}");
            res = session.Query<T>().Where(a => a.Age == 12 && a.DateTime.Month == dt.Month).ToList();
            Configure.WriteLogFile($"{2} {res.Count == 1}");
            res = session.Query<T>().Where(a => a.Age == 12 && a.DateTime.Hour == dt.Hour).ToList();
            Configure.WriteLogFile($"{3} {res.Count == 1}");
            res = session.Query<T>().Where(a => a.Age == 12 && a.DateTime.Day == dt.Day).ToList();
            Configure.WriteLogFile($"{4} {res.Count == 1}");
            res = session.Query<T>().Where(a => a.Age == 12 && a.DateTime.Minute == dt.Minute).ToList();
            Configure.WriteLogFile($"{5} {res.Count == 1} minutes may not match");
            res = session.Query<T>().Where(a => a.Age == 12 && a.DateTime.Second == dt.Second).ToList();
            Configure.WriteLogFile($"{6} {res.Count == 1} seconds may not match");
            res = session.Query<T>().Where(a => a.Age == 12 && a.DateTime.DayOfYear == dt.DayOfYear).ToList();
            Configure.WriteLogFile($"{7} {res.Count == 1}");
            res = session.Query<T>().Where(a => a.Age == 12 && a.DateTime.DayOfWeek == dt.DayOfWeek).ToList();
            Configure.WriteLogFile($"{8} {res.Count == 1}");




            res = session.Query<T>().Where(a => a.Age == 12 && a.DateTime.AddYears(1).Year == dt.Year + 1)
                .ToList();
            Configure.WriteLogFile($"{9} {res.Count == 1}");
            res = session.Query<T>().Where(a => a.Age == 12 && a.DateTime.AddMonths(1).Month == dt.Month + 1)
                .ToList();
            Configure.WriteLogFile($"{10} {res.Count == 1}");
            res = session.Query<T>().Where(a => a.Age == 12 && a.DateTime.AddHours(1).Hour == dt.Hour + 1)
                .ToList();
            Configure.WriteLogFile($"{11} {res.Count == 1}");
            res = session.Query<T>().Where(a => a.Age == 12 && a.DateTime.AddDays(1).Day == dt.Day + 1)
                .ToList();
            Configure.WriteLogFile($"{12} {res.Count == 1}");
            res = session.Query<T>().Where(a => a.Age == 12 && a.DateTime.AddMinutes(1).Minute == dt.Minute + 1)
                .ToList();
            Configure.WriteLogFile($"{13} {res.Count == 1} minutes may not match");
            res = session.Query<T>().Where(a => a.Age == 12 && a.DateTime.AddSeconds(1).Second == dt.Second + 1)
                .ToList();
            Configure.WriteLogFile($"{14} {res.Count == 1}  seconds may not match");
            res = session.Query<T>().Where(a => a.Age == 12 && a.Name.Concat("a").Concat("a") == "nameaa")
                .ToList();
            Configure.WriteLogFile($"{15} {res.Count == 1}");
            res = session.Query<T>().Where(a => a.Age == 12 && a.Name.Substring(0) == "name").ToList();
            Configure.WriteLogFile($"{16} {res.Count == 1}");
            res = session.Query<T>().Where(a => a.Age == 12 && a.Name.Substring(0, 1) == "n").ToList();
            Configure.WriteLogFile($"{17} {res.Count == 1}");
            res = session.Query<T>().Where(a => a.Age == 12 && a.Name.Contains("ame")).ToList();
            Configure.WriteLogFile($"{18} {res.Count == 1}");

            if (s.GetProviderName() == ProviderName.MsSql)
            {
                T my1 = session.Query<T>().FirstOrDefault(A => A.Age == 12);
                my1.Name = " dnamed ";
                session.Save(my1);
                res = session.Query<T>().Where(a => a.Age == 12 && a.Name.Trim() == "dnamed").ToList();
                Configure.WriteLogFile($"{19} {res.Count == 1}");
                res = session.Query<T>().Where(a => a.Age == 12 && a.Name.TrimStart() == "dnamed ").ToList();
                Configure.WriteLogFile($"{20} {res.Count == 1}");
                var sss = session.Query<T>().Select(a => new { sdsd = a.Name.TrimEnd() }).ToList();
                res = session.Query<T>().Where(a => a.Age == 12 && a.Name.TrimEnd() == " dnamed").ToList();
                Configure.WriteLogFile($"{21} {res.Count == 1}");
            }
            else
            {
                T my2 = session.Query<T>().FirstOrDefault(A => A.Age == 12);
                my2.Name = "dnamed";
                session.Save(my2);
                res = session.Query<T>().Where(a => a.Age == 12 && a.Name.Trim('4') == "dnamed").ToList();
                Configure.WriteLogFile($"{19} {res.Count == 1}");
                res = session.Query<T>().Where(a => a.Age == 12 && a.Name.TrimStart('d') == "named").ToList();
                Configure.WriteLogFile($"{20} {res.Count == 1}");
                res = session.Query<T>().Where(a => a.Age == 12 && a.Name.TrimEnd('d') == "dname").ToList();
                Configure.WriteLogFile($"{21} {res.Count == 1}");
            }

            T my3 = session.Query<T>().FirstOrDefault(A => A.Age == 12);
            my3.Name = "name";
            session.Save(my3);
            var err = session.Query<T>().Select(a => new { sd = a.Name.Length }).ToList();
            res = session.Query<T>().Where(a => a.Name.Length == "name".Length).ToList();
            Configure.WriteLogFile($"{22} {res.Count == 1}");
            res = session.Query<T>().Where(a => a.Name.ToUpper() == "NAME".ToUpper().Trim()).ToList();
            Configure.WriteLogFile($"{23} {res.Count == 1}");
            res = session.Query<T>().Where(a => a.Name.ToLower() == "NAME".ToLower().Trim()).ToList();
            Configure.WriteLogFile($"{24} {res.Count == 1}");

            List<T> list = new List<T>()
            {
                new T() { Name = "MyName1", Age = 10 },
                new T() { Name = "MyName2", Age = 20 },
                new T() { Name = "MyName3", Age = 30 },
            };
            var i = session.InsertBulk(list);
            Configure.WriteLogFile($"{24}/1 InsertBulk {i == 3}");

            var count = session.Query<T>().Count();

            Configure.WriteLogFile($"{25} {count == 4}");
            var o = session.Query<T>().OrderBy(a => a.Age).FirstOrDefault();

            Configure.WriteLogFile($"{26} {o != null && o.Age == 10}");
            Configure.WriteLogFile("Test transaction");
            session.TruncateTable<T>();
            count = session.Query<T>().Count();
            Configure.WriteLogFile($"{27} {count == 0}");

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
                Configure.WriteLogFile($"{28} {count == 3}");
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
                Configure.WriteLogFile($"{29} {count == 6}");
                session.TruncateTable<T>();
                session.Save(new T
                {
                    Name = "name",
                    Age = 12
                });
                res = session.Query<T>().Where(a => a.Name.Substring(1).Reverse() == "ema").ToList();
                Configure.WriteLogFile($"{30} {res.Count == 1}");
                res = session.Query<T>().Where(a => string.IsNullOrEmpty(a.Description)).ToList();
                Configure.WriteLogFile($"{31} {res.Count == 1}");
                o = session.Query<T>().Where(a => a.Age == 12).Single();
                Configure.WriteLogFile($"{311} {o != null}");
                try
                {
                    o = session.Query<T>().Where(a => a.Age == 14).Single();
                    Configure.WriteLogFile($"{32} {false}");
                }
                catch (Exception e)
                {
                    Configure.WriteLogFile($"{32} {true} {e.Message}");
                }

                o = session.Query<T>().Where(a => a.Age == 14).SingleOrDefault();
                Configure.WriteLogFile($"{33} {o == null}");
                try
                {
                    o = session.Query<T>().Where(a => a.Age == 14).First();
                    Configure.WriteLogFile($"{34} {false}");
                }
                catch (Exception e)
                {
                    Configure.WriteLogFile($"{34} {true} {e.Message}");
                }

                o = session.Query<T>().Where(a => a.Age == 14).FirstOrDefault();
                Configure.WriteLogFile($"{34 / 1} {o == null}");
                session.TruncateTable<T>();
                count=session.InsertBulk(new List<T>()
                {
                    new T() { Age = 40, Name = "name", MyTest = new MyTest { Name = "simple" } },
                    new T() { Age = 20, Name = "name1", MyTest = new MyTest { Name = "simple" } },
                    new T() { Age = 30, Name = "name1", MyTest = new MyTest { Name = "simple" } },
                    new T() { Age = 50, Name = "name1", MyTest = new MyTest { Name = "simple" } },
                    new T() { Age = 60, Name = "name", MyTest = new MyTest { Name = "simple" } },
                    new T() { Age = 10, Name = "name", MyTest = new MyTest { Name = "simple" } },
                });
                var ob = session.Query<T>().Select(a => new { ass = a.Age, asss = string.Concat(a.Name, a.Age) })
                    .ToList();
                Configure.WriteLogFile($"{35} {ob.Count() == 6}");
                count = session.Query<T>().Where(a => a.Name == "name").OrderBy(r => r.Age).ToList().Sum(a => a.Age);
                Configure.WriteLogFile($"{36} {count == 110}");
                var groupList = session.Query<T>().GroupBy(r => r.Name).ToListAsync().Result;
                Configure.WriteLogFile(
                    $"{37} {groupList.Count() == 2 && groupList[0].Count() == 3 && groupList[1].Count() == 3}");

                o = session.Query<T>().OrderBy(a => a.Age).First();
                Configure.WriteLogFile($"{38} {o.Age == 10}");
                o = session.Query<T>().OrderByDescending(a => a.Age).First();
                Configure.WriteLogFile($"{39} {o.Age == 60}");
                count = session.Query<T>().Where(a => a.Age < 100).OrderBy(ds => ds.Age).ToListAsync().Result
                    .Sum(a => a.Age);
                Configure.WriteLogFile($"{40} {count == 210}");
                var sCore = session.Query<T>().Where(a => a.Name.Contains("1")).Distinct(a => a.Name);
                Configure.WriteLogFile($"{41} {sCore.Count() == 1}");
                count = session.Query<T>().Where(sw => sw.Age == 10).Update(d => new Dictionary<object, object>
                {
                    { d.Name, string.Concat(d.Name, d.Age) },
                    { d.DateTime, DateTime.Now }
                });

                res = session.Query<T>().Where(a => a.Name == "name10").ToList();
                Configure.WriteLogFile($"{42} {res.Count() == 1}");
                session.Query<T>().Delete(a => a.Name == "name10");
                res = session.Query<T>().Where(a => a.Name == "name10").ToList();
                Configure.WriteLogFile($"{43} {res.Count() == 0}");
                session.Query<T>().Where(a => a.Age == 10).Delete();
                count = session.Query<T>().Where(a => a.Age == 10).Count();
                Configure.WriteLogFile($"{44} {count == 0}");
                if (s.GetProviderName() == ProviderName.MySql)
                {
                    res = session.FreeSql<T>(
                        $"select * from {session.TableName<T>()} where {session.ColumnName<T>(a => a.Age)} = ?1", 40).ToList();
                    Configure.WriteLogFile($"{45} {res.Count() == 1}");
                }
                else
                {
                    res = session.FreeSql<T>(
                        $"select * from {session.TableName<T>()} where {session.ColumnName<T>(a => a.Age)} = @1", 40).ToList();
                    Configure.WriteLogFile($"{45} {res.Count() == 1}");
                }



                var anon1 = session.Query<T>().Where(a => a.Age == 40).Select(d => new { age = d.Age, name = d.Name })
                    .ToList();
                Configure.WriteLogFile($"{46} {anon1.Count() == 1}");


                dynamic di = session.FreeSql<dynamic>($"select age, name from {session.TableName<T>()}");
                Configure.WriteLogFile($"{47} {di.Count == 5}");
                if (s.GetProviderName() == ProviderName.SqLite)
                {
                    var anon = TempSql(new { age = 3L, name = "asss" }, session,
                        $"select age,name from {session.TableName<T>()}");
                    Configure.WriteLogFile($"{48} {anon.Count() == 5}");
                }
                else
                {
                    var anon = TempSql(new { age = 3, name = "asss" }, session,
                        $"select age,name from {session.TableName<T>()}");
                    Configure.WriteLogFile($"{48} {anon.Count() == 5}");
                }

                var tempFree = session.FreeSql<MyFreeSql>($"select id,name,age,enum from {session.TableName<T>()}");
                Configure.WriteLogFile($"{49} {tempFree.Count() == 5}");
                res = session.Query<T>().Where(a => a.Age < 200).CacheUsage().ToList();
                Configure.WriteLogFile($"{50} {res.Count() == 5}");
                res = session.Query<T>().Where(a => a.Age < 200).CacheUsage().ToList();
                Configure.WriteLogFile($"{51} {res.Count() == 5}");
                var ii = session.Query<T>().Where(a => a.Age < 200).CacheGetKey();
                res = (List<T>)session.CacheGetValue<T>(ii);
                Configure.WriteLogFile($"{52} {res.Count() == 5}");
                session.Query<T>().Where(a => a.Age == 20).Update(f => new Dictionary<object, object>()
                {
                    { f.Age, 400 }
                });
                res = session.Query<T>().Where(a => a.Age < 200).CacheOver().ToList();
                res = session.Query<T>().Where(a => a.Age < 200).CacheUsage().ToList();
                Configure.WriteLogFile($"{53} {res.Count() == 4}");
                var ano = session.Query<T>().Where(a => a.Age < 500).Select(f =>
                    new { test = f.MyTest, e = f.MyEnum, r = f.Test23, c = f.DateTime }).ToList();
                Configure.WriteLogFile($"{54} {ano.Count() == 5}");
                var ano1 = session.Query<T>().Distinct(a => a.Age);
                Configure.WriteLogFile($"{55} {ano1.Count() == 5}");
                var ano2 = session.Query<T>().Distinct(a => new { ago = a.Age, myTest = a.MyTest, date = a.DateTime });
                Configure.WriteLogFile($"{56} {ano2.Count() == 5}");



                List<T> list22 = new List<T>();
                for (int iz = 0; iz < 2; iz++)
                {
                    list22.Add(new T() { Age = 30, Name = "name1", MyTest = new MyTest { Name = "simple" } });
                    list22.Add(new T() { Age = 10, Name = "name2", MyTest = new MyTest { Name = "simple" } });
                }

                var myTest = new MyTest() { Name = "simple" };
                var guid = new Guid("87ae6aba-086e-49e3-b569-1145b0a2744e");
                var data = new DateTime(2023, 3, 4);

                session.InsertBulk(list22);
                session.Query<T>().Update(a => new Dictionary<object, object>()
                {
                    { a.Age, new int() }
                });
                session.Query<T>().Delete(a => a.Age == new int());
                res = session.Query<T>().ToListAsync().Result;
                Configure.WriteLogFile($"{57} {res.Count() == 0}");
                
                Configure.WriteLogFile($"{Environment.NewLine}/*--------------mytest-------------*/{Environment.NewLine}");
                session.InsertBulk(list22);
                session.Query<T>().Update(a => new Dictionary<object, object>()
                {
                    { a.MyTest, myTest }
                });
                session.Query<T>().Delete(a => a.MyTest == myTest);
                res = session.Query<T>().ToListAsync().Result;
                Configure.WriteLogFile($"{58} {res.Count() == 0}");

                session.InsertBulk(list22);
                session.Query<T>().Update(a => new Dictionary<object, object>()
                {
                    { a.MyTest, new MyTest() }
                });
                session.Query<T>().Delete(a => a.MyTest == new MyTest());
                res = session.Query<T>().ToListAsync().Result;
                Configure.WriteLogFile($"{59} {res.Count() == 0}");
               
                Configure.WriteLogFile($"{Environment.NewLine} /*--------------guid-------------*/{Environment.NewLine}");
                session.InsertBulk(list22);
                session.Query<T>().Update(a => new Dictionary<object, object>()
                {
                    { a.ValGuid, guid }
                });
                session.Query<T>().Delete(a => a.ValGuid == guid);
                res = session.Query<T>().ToListAsync().Result;
                Configure.WriteLogFile($"{60} {res.Count() == 0}");

                session.InsertBulk(list22);
                session.Query<T>().Update(a => new Dictionary<object, object>()
                {
                    { a.ValGuid, new Guid("87ae6aba-086e-49e3-b569-1145b0a2744e") }
                });
                session.Query<T>().Delete(a => a.ValGuid == new Guid("87ae6aba-086e-49e3-b569-1145b0a2744e"));
                res = session.Query<T>().ToListAsync().Result;
                Configure.WriteLogFile($"{61} {res.Count() == 0}");
                
                Configure.WriteLogFile($"{Environment.NewLine}/*--------------date-------------*/{Environment.NewLine}");
                session.InsertBulk(list22);
                session.Query<T>().Update(a => new Dictionary<object, object>()
                {
                    { a.DateTime, data }
                });
                session.Query<T>().Delete(a => a.DateTime == data);
                res = session.Query<T>().ToListAsync().Result;
                Configure.WriteLogFile($"{62} {res.Count() == 0}");

                session.InsertBulk(list22);
                session.Query<T>().Update(a => new Dictionary<object, object>()
                {
                    { a.DateTime, new DateTime(2023, 3, 4) }
                });
                session.Query<T>().Delete(a => a.DateTime == new DateTime(2023, 3, 4));
                res = session.Query<T>().ToListAsync().Result;
                Configure.WriteLogFile($"{63} {res.Count() == 0}");
                session.TruncateTable<T>();
                list22.Clear();
                for (int j = 0; j < 10; j++)
                {
                    list22.Add(new T() { Age = j * 10, Name = "name1", MyTest = new MyTest { Name = "simple" } });
                }

                session.InsertBulk(list22);
                var rBases = session.Query<T>().OrderBy(ss => ss.Age).Select(a => a.Age).ToList();
                res = session.Query<T>().OrderBy(a => a.Age).Limit(2, 2).ToList();
                count = res.Sum(a => a.Age);
                Configure.WriteLogFile($"{64} {count == 50}");

                rBases = session.Query<T>().OrderByDescending(ss => ss.Age).Select(a => a.Age).ToList();
                res = session.Query<T>().OrderByDescending(a => a.Age).Limit(2, 2).ToList();
                count = res.Sum(a => a.Age);
                Configure.WriteLogFile($"{65} {count == 130}");
                
                Configure.WriteLogFile($"{Environment.NewLine}/*-------------------------test serialize-----------------------------*/{Environment.NewLine}");
                session.TruncateTable<T>();
                string str = "7''\"#$@@''";
                list22.Clear();
                list22.Add(new T() { MyTest = new MyTest() { Name = str } });
                session.InsertBulk(list22);
                o = session.Query<T>().First();
                var b = o.MyTest.Name == str;
                Configure.WriteLogFile($"{66} {b} test serialize");
                o.TestUser.Id = 100;
                session.Save(o);
                o = session.Query<T>().First();
                Configure.WriteLogFile($"{67} {o.TestUser.Id == 100}");
                session.TruncateTable<T>();
                list22.Add(new T() { ValInt4 = 20 });
                list22.Add(new T() { ValInt4 = 20 });
                list22.Add(new T() { ValInt4 = 20 });
                list22.Add(new T() { ValInt4 = 20 });
                session.InsertBulk(list22);
                var intVals = session.Query<T>().Where(a => a.ValInt4 == 20).Select(f => f.ValInt4).ToListAsync()
                    .Result;
                Configure.WriteLogFile($"{68} {intVals.Count == 4} test select ");
                session.TruncateTable<T>();
                for (int j = 0; j < 10; j++)
                {
                    session.Save(new T() { Age = 10 * j, Name = "name" + j, DateTime = DateTime.Now, Valdecimal = new decimal(123) });
                    session.Save(new T() { Age = 10 * j, Name = "name" + j, DateTime = DateTime.Now });
                }
                count = session.Query<T>().Distinct(a => a.Age).ToList().Sum();
                Configure.WriteLogFile($"{69} {count == 450} test distinct ");
                var r1 = session.Query<T>().Distinct(a => new { age = a.Age, mane = a.Name });
                Configure.WriteLogFile($"{70} {r1.Count() == 10} test distinct ");
                
                Configure.WriteLogFile($"{Environment.NewLine}/*----------------------------params------------------------------*/{Environment.NewLine}");
                string sql = "";
                if (s.GetProviderName() == ProviderName.MySql)
                {
                    sql = $"select {session.ColumnName<T>(ss => ss.Age)} from {session.TableName<T>()} where  " +
                          $"{session.ColumnName<T>(ss => ss.Name)}=?assa22 and " +
                          $"{session.ColumnName<T>(ss => ss.Valdecimal)} =?assa and" +
                          $" {session.ColumnName<T>(d => d.Age)} = ?age";
                    count = (int)session.ExecuteScalar(sql, "name1", 123, 10);
                    Configure.WriteLogFile($"{72} {count == 10} test params ");
                }
                else if (s.GetProviderName() == ProviderName.MsSql)
                {
                    var ssp = session.GetCommand().CreateParameter();
                    ssp.Precision = 18;
                    ssp.Scale = 2;
                    ssp.ParameterName = "@2";
                    ssp.DbType = DbType.Decimal;
                    ssp.Value = 123.3;
                    sql = $"select {session.ColumnName<T>(ss => ss.Age)} from {session.TableName<T>()} where  " +
                          $"{session.ColumnName<T>(ss => ss.Name)}=@1 and " +
                          $"{session.ColumnName<T>(ss => ss.Valdecimal)} = @2 and" +
                          $" {session.ColumnName<T>(d => d.Age)} = @3";
                    //count = (int)session.ExecuteScalar("select [age] from [my_class5] where  [name]=@1 and [test5] = '123.3' and [age] = '10'", "name1");
                    count = (int)session.ExecuteScalar(sql, "name1", new decimal(123), 10);
                    Configure.WriteLogFile($"{72} {count == 10} test params ");
                }
                else
                {
                    sql = $"select {session.ColumnName<T>(ss => ss.Age)} from {session.TableName<T>()} where  " +
                          $"{session.ColumnName<T>(ss => ss.Name)}=@assa22 and " +
                          $"{session.ColumnName<T>(ss => ss.Valdecimal)} =@assa and" +
                          $" {session.ColumnName<T>(d => d.Age)} = @age";
                    var ssT = session.ExecuteScalar(sql, "name1", 123, 10);
                    Configure.WriteLogFile($"{72} {ssT.ToString() == "10"} test params ");

                }

                session.TruncateTable<T>();
                for (int j = 0; j < 10; j++)
                {
                    session.Save(new T() { Age = j, Name = "name" + j, DateTime = DateTime.Now, Valdecimal = new decimal(123) });
                }

                res = session.Query<T>().OrderBy(a => a.Age).Limit(0, 1).ToList();
                Configure.WriteLogFile($"{73} {res.Count == 1 && res.First().Age == 0} test limit ");
                res = session.Query<T>().OrderByDescending(a => a.Age).Limit(0, 1).ToList();
                Configure.WriteLogFile($"{74} {res.Count == 1 && res.First().Age == 9} ");

                
                Configure.WriteLogFile($"{Environment.NewLine}/*----------test ElementAtOrDefault-------------*/{Environment.NewLine}");
                o = session.Query<T>().OrderBy(a => a.Age).ElementAtOrDefault(0);
                Configure.WriteLogFile($"{75} {o.Age == 0} test ElementAtOrDefault ");

                o = session.Query<T>().OrderBy(a => a.Age).ElementAtOrDefault(9);
                Configure.WriteLogFile($"{76} {o.Age == 9} ");

                o = session.Query<T>().OrderBy(a => a.Age).ElementAtOrDefault(10);
                Configure.WriteLogFile($"{77} {o == null} ");
                
                Configure.WriteLogFile($"{Environment.NewLine}/*----------test ElementAt-------------*/{Environment.NewLine}");
                o = session.Query<T>().OrderBy(a => a.Age).ElementAt(0);
                Configure.WriteLogFile($"{78} {o.Age == 0} test ElementAt ");

                o = session.Query<T>().OrderBy(a => a.Age).ElementAt(9);
                Configure.WriteLogFile($"{79} {o.Age == 9} ");

                try
                {
                    o = session.Query<T>().OrderBy(a => a.Age).ElementAt(10);
                    Configure.WriteLogFile($"{80} {1 != 1} ");
                }
                catch (Exception e)
                {
                    Configure.WriteLogFile($"{80} {1 == 1} {e.Message}");
                }
                
                Configure.WriteLogFile($"{Environment.NewLine}/*-----------test FirstOrDefault------------*/{Environment.NewLine}");
                o = session.Query<T>().OrderBy(a => a.Age).FirstOrDefault(d => d.Age == 0);
                Configure.WriteLogFile($"{81} {o.Age == 0} test FirstOrDefault ");

                o = session.Query<T>().OrderBy(a => a.Age).FirstOrDefault(d => d.Age == -10);
                Configure.WriteLogFile($"{82} {o == null} test ");
               
                Configure.WriteLogFile($"{Environment.NewLine} /*------------test first-----------*/{Environment.NewLine}");
                o = session.Query<T>().OrderBy(a => a.Age).First(d => d.Age == 0);
                Configure.WriteLogFile($"{83} {o.Age == 0} test First");

                try
                {
                    o = session.Query<T>().OrderBy(a => a.Age).First(d => d.Age == -10);
                    Configure.WriteLogFile($"{84} {false}  ");
                }
                catch (Exception e)
                {
                    Configure.WriteLogFile($"{84} {true} {e.Message} ");
                }
               
                Configure.WriteLogFile($"{Environment.NewLine} /*-----------test last------------*/{Environment.NewLine}");
                o = session.Query<T>().OrderBy(a => a.Age).Last(d => d.Age == 0);
                Configure.WriteLogFile($"{85} {o.Age == 0} test last");

                try
                {
                    o = session.Query<T>().OrderBy(a => a.Age).Last(d => d.Age == -10);
                    Configure.WriteLogFile($"{86} {false}  ");
                }
                catch (Exception e)
                {
                    Configure.WriteLogFile($"{86} {true} {e.Message} ");
                }
                Configure.WriteLogFile($"{Environment.NewLine}*------------test LastOrDefault-----------*{Environment.NewLine}");
                
                o = session.Query<T>().OrderBy(a => a.Age).LastOrDefault(d => d.Age == 0);
                Configure.WriteLogFile($"{87} {o.Age == 0} test LastOrDefault");

                o = session.Query<T>().OrderBy(a => a.Age).LastOrDefault(d => d.Age == -10);
                Configure.WriteLogFile($"{88} {o==null} ");
                Configure.WriteLogFile($"{Environment.NewLine} /*------------test test SingleOrDefault-----------*/{Environment.NewLine}");
               
                o = session.Query<T>().OrderBy(a => a.Age).SingleOrDefault(d => d.Age == 0);
                Configure.WriteLogFile($"{87} {o.Age == 0} test SingleOrDefault");

                o = session.Query<T>().OrderBy(a => a.Age).SingleOrDefault(d => d.Age == -10);
                Configure.WriteLogFile($"{88} {o == null} ");
                try
                {
                    o = session.Query<T>().OrderBy(a => a.Age).SingleOrDefault();
                    Configure.WriteLogFile($"{89} {false} ");
                }
                catch (Exception e)
                {
                    Configure.WriteLogFile($"{89} {true} {e.Message}");
                }
                Configure.WriteLogFile($"{Environment.NewLine}/*------------test test Single-----------*/{Environment.NewLine}");
                
                o = session.Query<T>().OrderBy(a => a.Age).Single(d => d.Age == 0);
                Configure.WriteLogFile($"{90} {o.Age == 0} test Single");

              
                try
                {
                    o = session.Query<T>().OrderBy(a => a.Age).Single(a=>a.Age==-10);
                    Configure.WriteLogFile($"{91} {false} ");
                }
                catch (Exception e)
                {
                    Configure.WriteLogFile($"{91} {true} {e.Message}");
                }

                Configure.WriteLogFile($"{Environment.NewLine}/*------------test Any-----------*/{Environment.NewLine}");
                bool bll = session.Query<T>().Where(a => a.Age == 1).Any();
                Configure.WriteLogFile($"{92} {bll} test Any");
                 bll = session.Query<T>().Any(a => a.Age == 1);
                Configure.WriteLogFile($"{93} {bll} ");
                bll = session.Query<T>().Any(a => a.Age == -1);
                Configure.WriteLogFile($"{94} {!bll} ");

                Configure.WriteLogFile($"{Environment.NewLine}/*------------test All-----------*/{Environment.NewLine}");
                session.TruncateTable<T>();
                for (int j = 0; j < 10; j++)
                {
                    session.Save(new T() { Age = 10, Name = "name1" });
                }
                for (int j = 0; j < 10; j++)
                {
                    session.Save(new T() { Age = 20, Name = "name1" });
                }

                bll = session.Query<T>().Where(d=>d.Name.StartsWith("name")).All(a => a.Age == 10);
                Configure.WriteLogFile($"{94} {bll==false} ");
                session.TruncateTable<T>();
                for (int j = 0; j < 10; j++)
                {
                    session.Save(new T() { Age = 10, Name = "name1" });
                }
                bll = session.Query<T>().Where(d => d.Name.StartsWith("name")).All(a => a.Age == 10);
                Configure.WriteLogFile($"{94} {bll == true} ");
                bll = session.Query<T>().Where(d => d.Name.Contains("name")).All(a => a.Age == 10);
                Configure.WriteLogFile($"{95} {bll == true} ");
                bll = session.Query<T>().Where(d => d.Name.EndsWith("e1")).All(a => a.Age == 11);
                Configure.WriteLogFile($"{96} {bll != true} ");

                Configure.WriteLogFile($"{Environment.NewLine}/*------------test skip-----------*/{Environment.NewLine}");
                session.TruncateTable<T>();
                for (int j = 0; j < 10; j++)
                {
                    session.Save(new T() { Age = j, Name = "name1" });
                }

                count = session.Query<T>().Count();

                res = session.Query<T>().Where(a => a.Age >= 0).OrderBy(d => d.Age).Skip(2).ToListAsync().Result;
                Configure.WriteLogFile($"{97} {res.Count==8&&res.First().Age==2} ");
                Configure.WriteLogFile($"{Environment.NewLine}/*------------test select-----------*/{Environment.NewLine}");
                var listInt = session.Query<T>().Where(a => a.Age >= 0).OrderBy(d => d.Age).Select((d, ir) => ir).ToList();
                Configure.WriteLogFile($"{98} {listInt.Count == 10} ");
                i = session.Query<T>().Where(a => a.Age >= 0).OrderBy(d => d.Age).Select((d,index)=>index).ToList().Sum();
                Configure.WriteLogFile($"{98} {i==55} ");
                var listOb = session.Query<T>().Where(a => a.Age >= 0).OrderBy(d => d.Age)
                    .Select((d, index) => new { Age = d.Age, Name = string.Concat(index, "-", d.Name) }).ToListAsync()
                    .Result;
                Configure.WriteLogFile($"{98} {listOb.Count == 10} ");




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

        public static IEnumerable<Ts> TempSql<Ts>(Ts t, ISession session, string sql)
        {
            return session.FreeSql<Ts>(sql);
        }

        private static void NewExeNull<T, Tb>() where T : MyClassNullBase, new() where Tb : IOtherDataBaseFactory, new()
        {
            var s = Activator.CreateInstance<Tb>();
            Configure.WriteLogFile($"**************************{s.GetProviderName()}*****************************");

            ISession session = Configure.GetSession<Tb>();
            if (session.TableExists<T>())
            {
                session.DropTable<T>();
            }
            session.TableCreate<T>();

            session.Save(new T());
            var tttest = session.Query<T>().First();

            if (s.GetProviderName() == ProviderName.PostgreSql || s.GetProviderName() == ProviderName.MsSql)
            {
                var res = (tttest.V5 == null
                           || tttest.V6 == null
                           || tttest.V8 == null
                           || tttest.V9 == null
                           || tttest.V10 == null
                           || tttest.V11 == null
                           || tttest.V12 == null
                           || tttest.V13 == null
                           || tttest.V15 == null
                           || tttest.V16 == null);
                Configure.WriteLogFile($"{1} {res} is null");
                tttest.V5 = true;
                tttest.V6 = 1;
                tttest.V8 = DateTime.Now;
                tttest.V9 = new decimal(1.22);
                tttest.V10 = 1D;
                tttest.V11 = 1;
                tttest.V12 = 1;
                tttest.V13 = 1L;
                tttest.V15 = 1.5F;
                tttest.V16 = Guid.Empty;
                session.Save(tttest);

            }
            if (s.GetProviderName() == ProviderName.MySql)
            {
                ClassNullMysql tttest1 = tttest as ClassNullMysql;
                var res = (
                              tttest1.V5 == null
                           || tttest1.V6 == null
                           || tttest1.V8 == null
                           || tttest1.V9 == null
                           || tttest1.V10 == null
                           || tttest1.V11 == null
                           || tttest1.V12 == null
                           || tttest1.V13 == null
                           || tttest1.V15 == null
                           || tttest1.V16 == null
                           || tttest1.V2 == null
                           || tttest1.V3 == null
                           || tttest1.V4 == null
                           || tttest1.V14 == null);
                Configure.WriteLogFile($"{1} {res} is null");
                tttest1.V5 = true;
                tttest1.V6 = 1;
                tttest1.V8 = DateTime.Now;
                tttest1.V9 = new decimal(1.22); ;
                tttest1.V10 = 1.3;
                tttest1.V11 = 1;
                tttest1.V12 = 1;
                tttest1.V13 = 1L;
                tttest1.V15 = 1.5F;
                tttest1.V16 = Guid.Empty;
                tttest1.V2 = 1;
                tttest1.V3 = 1;
                tttest1.V4 = 1;
                tttest1.V14 = 1;
                session.Save(tttest1);
            }
            if (s.GetProviderName() == ProviderName.SqLite)
            {
                ClassNullSqlite tttest1 = tttest as ClassNullSqlite;
                var res = (tttest1.V5 == null
                           || tttest1.V6 == null
                           || tttest1.V8 == null
                           || tttest1.V9 == null
                           || tttest1.V10 == null
                           || tttest1.V11 == null
                           || tttest1.V12 == null
                           || tttest1.V13 == null
                           || tttest1.V15 == null
                           || tttest1.V16 == null
                           || tttest1.V2 == null
                           || tttest1.V3 == null
                           || tttest1.V4 == null
                           || tttest1.V14 == null);
                Configure.WriteLogFile($"{1} {res} is null");
                tttest1.V5 = true;
                tttest1.V6 = 1;
                tttest1.V8 = DateTime.Now;
                tttest1.V9 = new decimal(1.22); ;
                tttest1.V10 = 1.4;
                tttest1.V11 = 1;
                tttest1.V12 = 1;
                tttest1.V13 = 1L;
                tttest1.V15 = 1.5F;
                tttest1.V16 = Guid.Empty;
                tttest1.V2 = 1;
                tttest1.V3 = 1;
                tttest1.V4 = 1;
                tttest1.V14 = 1;
                session.Save(tttest1);
            }
            tttest = session.Query<T>().First();

            if (s.GetProviderName() == ProviderName.SqLite)
            {
                ClassNullSqlite tttest1 = tttest as ClassNullSqlite;
                var res = (tttest1.V5 != null
                           || tttest1.V6 != null
                           || tttest1.V8 != null
                           || tttest1.V9 != null
                           || tttest1.V10 != null
                           || tttest1.V11 != null
                           || tttest1.V12 != null
                           || tttest1.V13 != null
                           || tttest1.V15 != null
                           || tttest1.V16 != null
                           || tttest1.V2 != null
                           || tttest1.V3 != null
                           || tttest1.V4 != null
                           || tttest1.V14 != null);
                Configure.WriteLogFile($"{2} {res} is not null");

            }
            if (s.GetProviderName() == ProviderName.MySql)
            {
                ClassNullMysql tttest1 = tttest as ClassNullMysql;
                var res = (tttest1.V5 != null
                           || tttest1.V6 != null
                           || tttest1.V8 != null
                           || tttest1.V9 != null
                           || tttest1.V10 != null
                           || tttest1.V11 != null
                           || tttest1.V12 != null
                           || tttest1.V13 != null
                           || tttest1.V15 != null
                           || tttest1.V16 != null
                           || tttest1.V2 != null
                           || tttest1.V3 != null
                           || tttest1.V4 != null
                           || tttest1.V14 != null);
                Configure.WriteLogFile($"{2} {res} is not null");
            }
            if (s.GetProviderName() == ProviderName.PostgreSql || s.GetProviderName() == ProviderName.MsSql)
            {
                var res = (tttest.V5 != null
                           || tttest.V6 != null
                           || tttest.V8 != null
                           || tttest.V9 != null
                           || tttest.V10 != null
                           || tttest.V11 != null
                           || tttest.V12 != null
                           || tttest.V13 != null
                           || tttest.V15 != null
                           || tttest.V16 != null);
                Configure.WriteLogFile($"{2} {res} is not null");

            }




        }

        public static void TestNativeInsert()
        {

            TestNativeInser<TiPostgresNative, MyDbPostgres>();
            TestNativeInser< TiMysqlNative, MyDbMySql>();
            TestNativeInser< TiMsSqlNative, MyDbMsSql>();
            TestNativeInser< TiSqliteNative, MyDbSqlite>();
        }

        static void TestNativeInser<T,Tb>() where Tb: IOtherDataBaseFactory, new() where T : TestInsertBaseNative, new()
        {
            IOtherDataBaseFactory s = Activator.CreateInstance<Tb>();
            Console.WriteLine($@"*********************NativeInser {s.GetProviderName()} **********************");

            ISession session = Configure.GetSession<Tb>();
            if (session.TableExists<T>())
            {
                session.DropTable<T>();
            }
            session.TableCreate<T>();
            T t = new T();
            var i=session.Save(t);
            Console.WriteLine($@"{1} {i == 1}");
            Console.WriteLine($@"{2} {t.Id==1}");
            Console.WriteLine($@"{3} {session.IsPersistent(t)}");
            List<T> list = new List<T>
            {
                new T(), new T(), new T()
            };
            i = session.InsertBulk(list);
            Console.WriteLine($@"{4} {i==3}");
            i=list.Where(a => session.IsPersistent(a)).Count();
            Console.WriteLine($@"{5} {i == 3}");
            i = session.Query<T>().Count();
            Console.WriteLine($@"{6} {i == 4}");
        }
        public static void TestAssignetInsert()
        {

            TestAssignetInser<TiPostgresAssignet, MyDbPostgres>();
            TestAssignetInser<TiMysqlAssignet, MyDbMySql>();
            TestAssignetInser<TiMsSqlAssignet, MyDbMsSql>();
            TestAssignetInser<TiSqliteAssignet, MyDbSqlite>();
        }
        static void TestAssignetInser<T, Tb>() where Tb : IOtherDataBaseFactory, new() where T : TestInsertBaseAssignet, new()
        {
            IOtherDataBaseFactory s = Activator.CreateInstance<Tb>();
            Console.WriteLine($@"********************* AssignetInser {s.GetProviderName()} **********************");

            ISession session = Configure.GetSession<Tb>();
            if (session.TableExists<T>())
            {
                session.DropTable<T>();
            }
            session.TableCreate<T>();
            T t = new T();
            var i = session.Save(t);
            Console.WriteLine($@"{1} {i == 1}");
          
            Console.WriteLine($@"{3} {session.IsPersistent(t)}");
            List<T> list = new List<T>
            {
                new T(), new T(), new T()
            };
            i = session.InsertBulk(list);
            Console.WriteLine($@"{4} {i == 3}");
            i = list.Where(a => session.IsPersistent(a)).Count();
            Console.WriteLine($@"{5} {i == 3}");
            i = session.Query<T>().Count();
            Console.WriteLine($@"{6} {i == 4}");
        }



    }
}
