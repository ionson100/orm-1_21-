using ORM_1_21_;
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
                    ses.Insert(c);
                    var s = Configure.Session.Query<MyClass>().Count();
                    Console.WriteLine($"{i} -- " + s);
                    ts.Commit();
                }
            }
        }

        private static void InnerRunOtherSession(int i)
        {

            while (true)
            {
                using (var ses = Configure.GetSession<MyDbPostgres>())
                {
                    using (var t = ses.BeginTransaction())
                    {
                        var ee = ses.Query<MyClass>().JoinCore(ses.Query<MyClass>(), a => a.Age, b => b.Age,
                      (aa, bb) => new { name1 = aa.Name, name2 = bb.Name }).ToList();
                        var l = ses.Query<MyClass>().ToList();
                        var e1e = ses.Query<MyClass>().JoinCore(l, a => a.Age, b => b.Age,
                            (aa, bb) => new { name1 = aa.Name, name2 = bb.Name });

                    }
                    Console.WriteLine($"{i}--");
                }
            }
        }

        public static void RunOtherSession()
        {


            Task.Run(() => { InnerRunOtherSession(1); });
            Task.Run(() => { InnerRunOtherSession(2); });
            Task.Run(() => { InnerRunOtherSession(4); });
            Task.Run(() => { InnerRunOtherSession(5); });
            Task.Run(() => { InnerRunOtherSession(6); });
            Task.Run(() => { InnerRunOtherSession(7); });
            Task.Factory.StartNew(() => { InnerRunOtherSession(3); }, TaskCreationOptions.LongRunning);
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
            Console.WriteLine($"**************************{s.GetProviderName()}*****************************");
            ISession session = Configure.GetSession<Tb>();


            if (session.TableExists<T>())
            {
                session.DropTable<T>();
            }

            session.TableCreate<T>();

            var dt = DateTime.Now;
            var myClass = new T
            {
                DateTime = dt,
                Age = 12,
                Description = "simple",
                Name = "name",
                MyEnum = MyEnum.First
            };
            session.Insert(myClass);
            List<T> res = null;
            res = session.Query<T>().Where(a => a.Age == 12 && a.DateTime.Year == dt.Year).ToList();
            Log(1, res.Count == 1);

            res = session.Query<T>().Where(a => a.Age == 12 && a.DateTime.Month == dt.Month).ToList();
            Log(2, res.Count == 1);

            res = session.Query<T>().Where(a => a.Age == 12 && a.DateTime.Hour == dt.Hour).ToList();
            Log(3, res.Count == 1);


            res = session.Query<T>().Where(a => a.Age == 12 && a.DateTime.Day == dt.Day).ToList();
            Log(4, res.Count == 1);

            res = session.Query<T>().Where(a => a.Age == 12 && a.DateTime.Minute == dt.Minute).ToList();
            Log(5, res.Count == 1, "minutes may not match");

            res = session.Query<T>().Where(a => a.Age == 12 && a.DateTime.Second == dt.Second).ToList();
            Log(6, res.Count == 1, "seconds may not match");

            res = session.Query<T>().Where(a => a.Age == 12 && a.DateTime.DayOfYear == dt.DayOfYear).ToList();
            Log(7, res.Count == 1);

            res = session.Query<T>().Where(a => a.Age == 12 && a.DateTime.DayOfWeek == dt.DayOfWeek).ToList();
            Log(8, res.Count == 1);

            res = session.Query<T>().Where(a => a.Age == 12 && a.DateTime.AddYears(1).Year == dt.AddYears(1).Year)
                .ToList();
            Log(9, res.Count == 1);
            res = session.Query<T>().Where(a => a.Age == 12 && a.DateTime.AddMonths(1).Month == dt.AddMonths(1).Month)
                .ToList();
            Log(10, res.Count == 1);

            res = session.Query<T>().Where(a => a.Age == 12 && a.DateTime.AddHours(1).Hour == dt.AddHours(1).Hour)
                .ToList();
            Log(11, res.Count == 1);

            res = session.Query<T>().Where(a => a.Age == 12 && a.DateTime.AddDays(1).Day == dt.AddDays(1).Day)
                .ToList();
            Log(12, res.Count == 1);
            res = session.Query<T>()
                .Where(a => a.Age == 12 && a.DateTime.AddMinutes(1).Minute == dt.AddMinutes(1).Minute)
                .ToList();
            Log(13, res.Count == 1, "minutes may not match");

            res = session.Query<T>()
                .Where(a => a.Age == 12 && a.DateTime.AddSeconds(1).Second == dt.AddSeconds(1).Second)
                .ToList();
            Log(14, res.Count == 1, "seconds may not match");


            res = session.Query<T>().Where(a => a.Age == 12 && a.Name.Concat("a").Concat("a") == "nameaa")
                .ToList();
            Log(15, res.Count == 1);

            res = session.Query<T>().Where(a => a.Age == 12 && a.Name.Substring(0) == "name").ToList();
            Log(16, res.Count == 1);

            res = session.Query<T>().Where(a => a.Age == 12 && a.Name.Substring(0, 1) == "n").ToList();
            Log(17, res.Count == 1);

            res = session.Query<T>().Where(a => a.Age == 12 && a.Name.Contains("ame")).ToList();
            Log(18, res.Count == 1);


            if (s.GetProviderName() == ProviderName.MsSql)
            {
                T my1 = session.Query<T>().FirstOrDefault(A => A.Age == 12);
                my1.Name = " dnamed ";
                session.Update(my1);
                res = session.Query<T>().Where(a => a.Age == 12 && a.Name.Trim() == "dnamed").ToList();
                Log(19, res.Count == 1);

                res = session.Query<T>().Where(a => a.Age == 12 && a.Name.TrimStart() == "dnamed ").ToList();
                Log(20, res.Count == 1);

                var sss = session.Query<T>().Select(a => new { sdsd = a.Name.TrimEnd() }).ToList();
                res = session.Query<T>().Where(a => a.Age == 12 && a.Name.TrimEnd() == " dnamed").ToList();
                Log(21, res.Count == 1);
            }
            else
            {
                T my2 = session.Query<T>().FirstOrDefault(A => A.Age == 12);
                my2.Name = "dnamed";
                var ss=session.Update(my2);
                res = session.Query<T>().Where(a => a.Age == 12 && a.Name.Trim('4') == "dnamed").ToList();
                Log(22, res.Count == 1);

                res = session.Query<T>().Where(a => a.Age == 12 && a.Name.TrimStart('d') == "named").ToList();
                Log(20, res.Count == 1);


                res = session.Query<T>().Where(a => a.Age == 12 && a.Name.TrimEnd('d') == "dname").ToList();
                Log(21, res.Count == 1);
            }

            T my3 = session.Query<T>().FirstOrDefault(A => A.Age == 12);
            my3.Name = "name";
            session.Update(my3);
            var err = session.Query<T>().Select(a => new { sd = a.Name.Length }).ToList();
            res = session.Query<T>().Where(a => a.Name.Length == "name".Length).ToList();
            Log(22, res.Count == 1);

            res = session.Query<T>().Where(a => a.Name.ToUpper() == "NAME".ToUpper().Trim()).ToList();
            Log(23, res.Count == 1);

            res = session.Query<T>().Where(a => a.Name.ToLower() == "NAME".ToLower().Trim()).ToList();
            Log(24, res.Count == 1);


            List<T> list = new List<T>
            {
                new T { Name = "MyName1", Age = 10 },
                new T { Name = "MyName2", Age = 20 },
                new T { Name = "MyName3", Age = 30 }
            };
            var i = session.InsertBulk(list);
            Log(24, i == 3, "/1 InsertBulk");


            var count = session.Query<T>().Count();
            Log(25, count == 4);


            var o = session.Query<T>().OrderBy(a => a.Age).FirstOrDefault();
            Log(26, o != null && o.Age == 10);

            Console.WriteLine("Test transaction");
            session.TruncateTable<T>();
            count = session.Query<T>().Count();
            Log(27, count == 0);


            IsolationLevel? level = null;
            {
                var ses = Configure.GetSession<Tb>();
                try
                {
                    ses.Insert(new T());
                    var tr = ses.BeginTransaction(level);
                    ses.Insert(new T());
                    ses.Insert(new T());
                    ses.Insert(new T());
                    tr.Rollback();
                    ses.Insert(new T());
                    tr = ses.BeginTransaction(level);
                    ses.Insert(new T());
                    ses.Insert(new T());
                    ses.Insert(new T());
                    tr.Rollback();
                    ses.Insert(new T());
                }
                finally
                {
                    ses.Dispose();
                }

                count = session.Query<T>().Count();
                Log(28, count == 3);
            }
            session.TruncateTable<T>();
            {
                var ses = Configure.GetSession<Tb>();
                try
                {
                    ses.Insert(new T());
                    var tr = ses.BeginTransaction(level);
                    ses.Insert(new T());
                    ses.Insert(new T());
                    ses.Insert(new T());
                    tr.Commit();
                    ses.Insert(new T());
                    tr = ses.BeginTransaction(level);
                    ses.Insert(new T());
                    ses.Insert(new T());
                    ses.Insert(new T());
                    tr.Rollback();
                    ses.Insert(new T());
                }
                finally
                {
                    ses.Dispose();
                }

                count = session.Query<T>().Count();
                Log(28, count == 6);

                session.TruncateTable<T>();
                session.Insert(new T
                {
                    Name = "name",
                    Age = 12
                });
                res = session.Query<T>().Where(a => a.Name.Substring(1).Reverse() == "ema").ToList();
                Log(30, res.Count == 1);

                res = session.Query<T>().Where(a => string.IsNullOrEmpty(a.Description)).ToList();
                Log(31, res.Count == 1);

                o = session.Query<T>().Where(a => a.Age == 12).Single();
                Log(32, o != null);

                try
                {
                    o = session.Query<T>().Where(a => a.Age == 14).Single();
                    Log(32, false);
                }
                catch (Exception e)
                {
                    Log(32, true);
                }

                o = session.Query<T>().Where(a => a.Age == 14).SingleOrDefault();
                Log(33, o == null);

                try
                {
                    o = session.Query<T>().Where(a => a.Age == 14).First();
                    Log(34, false);
                }
                catch (Exception e)
                {
                    Log(34, true);
                }

                o = session.Query<T>().Where(a => a.Age == 14).FirstOrDefault();
                Log(34, o == null);

                session.TruncateTable<T>();
                count = session.InsertBulk(new List<T>
                {
                    new T { Age = 40, Name = "name" },
                    new T { Age = 20, Name = "name1" },
                    new T { Age = 30, Name = "name1" },
                    new T { Age = 50, Name = "name1" },
                    new T { Age = 60, Name = "name" },
                    new T { Age = 10, Name = "name" }
                });
                var ob = session.Query<T>().Select(a => new { ass = a.Age, asss = string.Concat(a.Name, a.Age) })
                    .ToList();
                Log(35, ob.Count() == 6);

                count = session.Query<T>().Where(a => a.Name == "name").OrderBy(r => r.Age).ToList().Sum(a => a.Age);
                Log(36, count == 110);

                // var groupList = session.Query<T>().GroupBy(r => r.Name).ToListAsync().Result;
                // Log(37, groupList.Count() == 2 && groupList[0].Count() == 3 && groupList[1].Count() == 3);


                o = session.Query<T>().OrderBy(a => a.Age).First();
                Log(38, o.Age == 10);

                o = session.Query<T>().OrderByDescending(a => a.Age).First();
                Log(39, o.Age == 60);

                count = session.Query<T>().Where(a => a.Age < 100).OrderBy(ds => ds.Age).ToListAsync().Result
                    .Sum(a => a.Age);
                Log(40, count == 210);

                var sCore = session.Query<T>().Where(a => a.Name.Contains("1")).Distinct(a => a.Name);
                Log(41, sCore.Count() == 1);

                count = session.Query<T>().Where(sw => sw.Age == 10).Update(d => new Dictionary<object, object>
                {
                    { d.Name, string.Concat(d.Name, d.Age) },
                    { d.DateTime, DateTime.Now }
                });

                res = session.Query<T>().Where(a => a.Name == "name10").ToList();
                Log(42, res.Count() == 1);

                session.Query<T>().Delete(a => a.Name == "name10");
                res = session.Query<T>().Where(a => a.Name == "name10").ToList();
                Log(43, res.Count() == 0);

                session.Query<T>().Where(a => a.Age == 10).Delete();
                count = session.Query<T>().Where(a => a.Age == 10).Count();
                Log(44, count == 0);

                if (s.GetProviderName() == ProviderName.MySql)
                {
                    res = session.FreeSql<T>(
                            $"select * from {session.TableName<T>()} where {session.ColumnName<T>(a => a.Age)} = ?1",
                            40)
                        .ToList();
                    Log(45, res.Count() == 1);
                }
                else
                {
                    res = session.FreeSql<T>(
                            $"select * from {session.TableName<T>()} where {session.ColumnName<T>(a => a.Age)} = @1",
                            40)
                        .ToList();
                    Log(45, res.Count() == 1);
                }


                var anon1 = session.Query<T>().Where(a => a.Age == 40).Select(d => new { age = d.Age, name = d.Name })
                    .ToList();
                Log(46, anon1.Count() == 1);


                dynamic di = session.FreeSql<dynamic>($"select age, name from {session.TableName<T>()}");
                Log(47, di.Count == 5);

                if (s.GetProviderName() == ProviderName.SqLite)
                {
                    var anon = TempSql(new { age = 3L, name = "asss" }, session,
                        $"select age,name from {session.TableName<T>()}");
                    Log(48, anon.Count() == 5);
                }
                else
                {
                    var anon = TempSql(new { age = 3, name = "asss" }, session,
                        $"select age,name from {session.TableName<T>()}");
                    Log(48, anon.Count() == 5);
                }

                var tempFree = session.FreeSql<MyFreeSql>($"select {session.ColumnName<T>(a => a.Id)}," +
                                                          $"name,age,enum from {session.TableName<T>()}");

                Log(49, tempFree.Count() == 5);
                res = session.Query<T>().Where(a => a.Age < 200).CacheUsage().ToList();
                Log(50, res.Count() == 5);
                res = session.Query<T>().Where(a => a.Age < 200).CacheUsage().ToList();
                Log(51, res.Count() == 5);
                var ii = session.Query<T>().Where(a => a.Age < 200).CacheGetKey();
                res = (List<T>)session.CacheGetValue<T>(ii);
                Log(52, res.Count() == 5);

                session.Query<T>().Where(a => a.Age == 20).Update(f => new Dictionary<object, object>
                {
                    { f.Age, 400 }
                });
                res = session.Query<T>().Where(a => a.Age < 200).CacheOver().ToList();
                res = session.Query<T>().Where(a => a.Age < 200).CacheUsage().ToList();
                Log(53, res.Count() == 4);
                var ano = session.Query<T>().Where(a => a.Age < 500).Select(f =>
                    new { e = f.MyEnum, c = f.DateTime }).ToList();
                Log(54, ano.Count() == 5);
                var ano1 = session.Query<T>().Distinct(a => a.Age);
                Log(55, ano1.Count() == 5);
                var ano2 = session.Query<T>().Distinct(a => new { ago = a.Age, date = a.DateTime });
                Log(56, ano2.Count() == 5);


                List<T> list22 = new List<T>();
                for (int iz = 0; iz < 2; iz++)
                {
                    list22.Add(new T { Age = 30, Name = "name1" });
                    list22.Add(new T { Age = 10, Name = "name2" });
                }


                var guid = new Guid("87ae6aba-086e-49e3-b569-1145b0a2744e");
                var data = new DateTime(2023, 3, 4);

                session.InsertBulk(list22);
                session.Query<T>().Update(a => new Dictionary<object, object>
                {
                    { a.Age, new int() }
                });
                session.Query<T>().Delete(a => a.Age == new int());
                res = session.Query<T>().ToListAsync().Result;
                Log(57, res.Count() == 0);

                Console.WriteLine($"{Environment.NewLine}/*--------------mytest-------------*/{Environment.NewLine}");
                session.InsertBulk(list22);


                Console.WriteLine($"{Environment.NewLine} /*--------------guid-------------*/{Environment.NewLine}");

                session.Query<T>().Update(a => new Dictionary<object, object>
                {
                    { a.ValGuid, guid }
                });
                session.Query<T>().Delete(a => a.ValGuid == guid);
                res = session.Query<T>().ToListAsync().Result;
                Log(60, res.Count() == 0);

                session.InsertBulk(list22);
                session.Query<T>().Update(a => new Dictionary<object, object>
                {
                    { a.ValGuid, new Guid("87ae6aba-086e-49e3-b569-1145b0a2744e") }
                });
                session.Query<T>().Delete(a => a.ValGuid == new Guid("87ae6aba-086e-49e3-b569-1145b0a2744e"));
                res = session.Query<T>().ToListAsync().Result;
                Log(61, res.Count() == 0);

                Console.WriteLine($"{Environment.NewLine}/*--------------date-------------*/{Environment.NewLine}");
                session.InsertBulk(list22);
                session.Query<T>().Update(a => new Dictionary<object, object>
                {
                    { a.DateTime, data }
                });
                session.Query<T>().Delete(a => a.DateTime == data);
                res = session.Query<T>().ToListAsync().Result;
                Log(62, res.Count() == 0);

                session.InsertBulk(list22);
                session.Query<T>().Update(a => new Dictionary<object, object>
                {
                    { a.DateTime, new DateTime(2023, 3, 4) }
                });
                session.Query<T>().Delete(a => a.DateTime == new DateTime(2023, 3, 4));
                res = session.Query<T>().ToListAsync().Result;
                Log(63, res.Count() == 0);
                session.TruncateTable<T>();
                list22.Clear();
                for (int j = 0; j < 10; j++)
                {
                    list22.Add(new T { Age = j * 10, Name = "name1" });
                }

                session.InsertBulk(list22);
                var rBases = session.Query<T>().OrderBy(ss => ss.Age).Select(a => a.Age).ToList();
                res = session.Query<T>().OrderBy(a => a.Age).Limit(2, 2).ToList();
                count = res.Sum(a => a.Age);
                Log(64, count == 50);

                rBases = session.Query<T>().OrderByDescending(ss => ss.Age).Select(a => a.Age).ToList();
                res = session.Query<T>().OrderByDescending(a => a.Age).Limit(2, 2).ToList();
                count = res.Sum(a => a.Age);
                Log(65, count == 130);

                Console.WriteLine(
                    $"{Environment.NewLine}/*-------------------------test serialize-----------------------------*/{Environment.NewLine}");
                session.TruncateTable<T>();
                string str = "7''\"#$@@''";
                list22.Clear();
                list22.Add(new T { });
                session.InsertBulk(list22);
                o = session.Query<T>().First();
                var b = true;
                Log(66, b, "test serialize");
                o.TestUser="A";
                session.Update(o);
                o = session.Query<T>().First();
                Log(67, o.TestUser=="A");
                session.TruncateTable<T>();
                list22.Add(new T { ValInt4 = 20 });
                list22.Add(new T { ValInt4 = 20 });
                list22.Add(new T { ValInt4 = 20 });
                list22.Add(new T { ValInt4 = 20 });
                session.InsertBulk(list22);
                var intVals = session.Query<T>().Where(a => a.ValInt4 == 20).Select(f => f.ValInt4).ToListAsync()
                    .Result;
                Log(68, intVals.Count == 4, "test select");
                session.TruncateTable<T>();
                for (int j = 0; j < 10; j++)
                {
                    session.Insert(new T { Age = 10 * j, Name = "name" + j, DateTime = DateTime.Now, Valdecimal = new decimal(123) });
                    session.Insert(new T { Age = 10 * j, Name = "name" + j, DateTime = DateTime.Now });
                }

                count = session.Query<T>().Distinct(a => a.Age).ToList().Sum();
                Log(69, count == 450, "test distinct");
                var r1 = session.Query<T>().Distinct(a => new { age = a.Age, mane = a.Name });
                Log(70, r1.Count() == 10, "test distinct");

                Console.WriteLine(
                    $"{Environment.NewLine}/*----------------------------params------------------------------*/{Environment.NewLine}");
                string sql = "";
                if (s.GetProviderName() == ProviderName.MySql)
                {
                    sql = $"select {session.ColumnName<T>(ss => ss.Age)} from {session.TableName<T>()} where  " +
                          $"{session.ColumnName<T>(ss => ss.Name)}=?assa22 and " +
                          $"{session.ColumnName<T>(ss => ss.Valdecimal)} =?assa and" +
                          $" {session.ColumnName<T>(d => d.Age)} = ?age";
                    count = (int)session.ExecuteScalar(sql, "name1", 123, 10);
                    Log(72, count == 10, "test params");
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
                    Log(72, count == 10, "test params");
                }
                else
                {
                    sql = $"select {session.ColumnName<T>(ss => ss.Age)} from {session.TableName<T>()} where  " +
                          $"{session.ColumnName<T>(ss => ss.Name)}=@assa22 and " +
                          $"{session.ColumnName<T>(ss => ss.Valdecimal)} =@assa and" +
                          $" {session.ColumnName<T>(d => d.Age)} = @age";
                    var ssT = session.ExecuteScalar(sql, "name1", 123, 10);
                    Log(72, ssT.ToString() == "10", "test params");
                }

                session.TruncateTable<T>();
                for (int j = 0; j < 10; j++)
                {
                    session.Insert(new T { Age = j, Name = "name" + j, DateTime = DateTime.Now, Valdecimal = new decimal(123) });
                }

                res = session.Query<T>().OrderBy(a => a.Age).Limit(0, 1).ToList();
                Log(73, res.Count == 1 && res.First().Age == 0, "test limit");
                res = session.Query<T>().OrderByDescending(a => a.Age).Limit(0, 1).ToList();
                Log(74, res.Count == 1 && res.First().Age == 9);


                Console.WriteLine(
                    $"{Environment.NewLine}/*----------test ElementAtOrDefault-------------*/{Environment.NewLine}");
                o = session.Query<T>().OrderBy(a => a.Age).ElementAtOrDefault(0);
                Log(75, o.Age == 0, "test ElementAtOrDefault");

                o = session.Query<T>().OrderBy(a => a.Age).ElementAtOrDefault(9);
                Log(76, o.Age == 9);

                o = session.Query<T>().OrderBy(a => a.Age).ElementAtOrDefault(10);
                Log(77, o == null);

                Console.WriteLine(
                    $"{Environment.NewLine}/*----------test ElementAt-------------*/{Environment.NewLine}");
                o = session.Query<T>().OrderBy(a => a.Age).ElementAt(0);
                Log(78, o.Age == 0, "test ElementAt");

                o = session.Query<T>().OrderBy(a => a.Age).ElementAt(9);
                Log(79, o.Age == 9);

                try
                {
                    o = session.Query<T>().OrderBy(a => a.Age).ElementAt(10);
                    Log(80, 1 != 1);
                }
                catch (Exception)
                {
                    Log(80, 1 == 1);
                }

                Console.WriteLine(
                    $"{Environment.NewLine}/*-----------test FirstOrDefault------------*/{Environment.NewLine}");
                o = session.Query<T>().OrderBy(a => a.Age).FirstOrDefault(d => d.Age == 0);
                Log(81, o.Age == 0, "test FirstOrDefault");

                o = session.Query<T>().OrderBy(a => a.Age).FirstOrDefault(d => d.Age == -10);
                Log(82, o == null);

                Console.WriteLine($"{Environment.NewLine} /*------------test first-----------*/{Environment.NewLine}");
                o = session.Query<T>().OrderBy(a => a.Age).First(d => d.Age == 0);
                Log(83, o.Age == 0);

                try
                {
                    o = session.Query<T>().OrderBy(a => a.Age).First(d => d.Age == -10);
                    Log(84, false);
                }
                catch (Exception)
                {
                    Log(84, true);
                }

                Console.WriteLine($"{Environment.NewLine} /*-----------test last------------*/{Environment.NewLine}");
                o = session.Query<T>().OrderBy(a => a.Age).Last(d => d.Age == 0);
                Log(85, o.Age == 0);

                try
                {
                    o = session.Query<T>().OrderBy(a => a.Age).Last(d => d.Age == -10);
                    Log(86, false);
                }
                catch (Exception)
                {
                    Log(86, true);
                }

                Console.WriteLine(
                    $"{Environment.NewLine}*------------test LastOrDefault-----------*{Environment.NewLine}");

                o = session.Query<T>().OrderBy(a => a.Age).LastOrDefault(d => d.Age == 0);
                Log(87, o.Age == 0);

                o = session.Query<T>().OrderBy(a => a.Age).LastOrDefault(d => d.Age == -10);
                Log(88, o == null);
                Console.WriteLine(
                    $"{Environment.NewLine} /*------------test test SingleOrDefault-----------*/{Environment.NewLine}");

                o = session.Query<T>().OrderBy(a => a.Age).SingleOrDefault(d => d.Age == 0);
                Log(87, o.Age == 0);

                o = session.Query<T>().OrderBy(a => a.Age).SingleOrDefault(d => d.Age == -10);
                Log(88, o == null);
                try
                {
                    o = session.Query<T>().OrderBy(a => a.Age).SingleOrDefault();
                    Log(89, false);
                }
                catch (Exception)
                {
                    Log(89, true);
                }

                Console.WriteLine(
                    $"{Environment.NewLine}/*------------test test Single-----------*/{Environment.NewLine}");

                o = session.Query<T>().OrderBy(a => a.Age).Single(d => d.Age == 0);
                Log(90, o.Age == 0);


                try
                {
                    o = session.Query<T>().OrderBy(a => a.Age).Single(a => a.Age == -10);
                    Log(91, false);
                }
                catch (Exception)
                {
                    Log(91, true);
                }

                Console.WriteLine($"{Environment.NewLine}/*------------test Any-----------*/{Environment.NewLine}");
                bool bll = session.Query<T>().Where(a => a.Age == 1).Any();
                Log(92, bll);
                bll = session.Query<T>().Any(a => a.Age == 1);
                Log(93, bll);
                bll = session.Query<T>().Any(a => a.Age == -1);
                Log(94, !bll);

                Console.WriteLine($"{Environment.NewLine}/*------------test All-----------*/{Environment.NewLine}");
                session.TruncateTable<T>();
                for (int j = 0; j < 10; j++)
                {
                    session.Insert(new T { Age = 10, Name = "name1" });
                }

                for (int j = 0; j < 10; j++)
                {
                    session.Insert(new T { Age = 20, Name = "name1" });
                }

                bll = session.Query<T>().Where(d => d.Name.StartsWith("name")).All(a => a.Age == 10);
                Log(94, bll == false);
                session.TruncateTable<T>();
                for (int j = 0; j < 10; j++)
                {
                    session.Insert(new T { Age = 10, Name = "name1" });
                }

                bll = session.Query<T>().Where(d => d.Name.StartsWith("name")).All(a => a.Age == 10);
                Log(94, bll == true);
                bll = session.Query<T>().Where(d => d.Name.Contains("name")).All(a => a.Age == 10);
                Log(95, bll == true);
                bll = session.Query<T>().Where(d => d.Name.EndsWith("e1")).All(a => a.Age == 11);
                Log(96, bll != true);

                Console.WriteLine($"{Environment.NewLine}/*------------test skip-----------*/{Environment.NewLine}");
                session.TruncateTable<T>();
                for (int j = 0; j < 10; j++)
                {
                    session.Insert(new T { Age = j, Name = "name1" });
                }

                count = session.Query<T>().Count();

                res = session.Query<T>().Where(a => a.Age >= 0).OrderBy(d => d.Age).Skip(2).ToListAsync().Result;
                Log(97, res.Count == 8 && res.First().Age == 2);
                Console.WriteLine($"{Environment.NewLine}/*------------test select-----------*/{Environment.NewLine}");
                var listInt = session.Query<T>().Where(a => a.Age >= 0).OrderBy(d => d.Age).Select((d, ir) => ir)
                    .ToList();
                Log(98, listInt.Count == 10);
                i = session.Query<T>().Where(a => a.Age >= 0).OrderBy(d => d.Age).Select((d, index) => index).ToList()
                    .Sum();
                Log(98, i == 55);
                var listOb = session.Query<T>().Where(a => a.Age >= 0).OrderBy(d => d.Age)
                    .Select((d, index) => new { Age = d.Age, Name = string.Concat(index, "-", d.Name) }).ToListAsync()
                    .Result;
                Log(98, listOb.Count == 10);
            }
        }

        public static IEnumerable<Ts> TempSql<Ts>(Ts t, ISession session, string sql)
        {
            return session.FreeSql<Ts>(sql);
        }

        private static void NewExeNull<T, Tb>() where T : MyClassNullBase, new() where Tb : IOtherDataBaseFactory, new()
        {
            var s = Activator.CreateInstance<Tb>();
            Console.WriteLine($"**************************{s.GetProviderName()}*****************************");

            ISession session = Configure.GetSession<Tb>();
            if (session.TableExists<T>())
            {
                session.DropTable<T>();
            }

            session.TableCreate<T>();

            session.Insert(new T());
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
                Log(1, res);
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
                session.Insert(tttest);
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
                Log(1, res);
                tttest1.V5 = true;
                tttest1.V6 = 1;
                tttest1.V8 = DateTime.Now;
                tttest1.V9 = new decimal(1.22);
                ;
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
                session.Insert(tttest1);
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
                Log(1, res);
                tttest1.V5 = true;
                tttest1.V6 = 1;
                tttest1.V8 = DateTime.Now;
                tttest1.V9 = new decimal(1.22);
                ;
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
                session.Insert(tttest1);
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
                Log(2, res);
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
                Log(2, res);
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
                Log(2, res);
            }
        }

        public static void TestNativeInsert()
        {
            TestNativeInser<TiPostgresNative, MyDbPostgres>();
            // TestNativeInser<TiMysqlNative, MyDbMySql>();
            // TestNativeInser<TiMsSqlNative, MyDbMsSql>();
            // TestNativeInser<TiSqliteNative, MyDbSqlite>();
        }

        static void TestNativeInser<T, Tb>() where Tb : IOtherDataBaseFactory, new()
            where T : TestInsertBaseNative, new()
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
            var i = session.Insert(t);
            Log(1, i == 1);
            Log(2, t.Id == 1);
           // Log(3, session.IsPersistent(t));
            List<T> list = new List<T>
            {
                new T(), new T(), new T()
            };
            i = session.InsertBulk(list);
            Log(4, i == 3);
           // i = list.Where(a => session.IsPersistent(a)).Count();
           // Log(5, i == 3);
            i = session.Query<T>().Count();
            Log(6, i == 4);
        }

        public static void TestAssignetInsert()
        {
            TestAssignetInser<TiPostgresAssignet, MyDbPostgres>();
            TestAssignetInser<TiMysqlAssignet, MyDbMySql>();
            TestAssignetInser<TiMsSqlAssignet, MyDbMsSql>();
            TestAssignetInser<TiSqliteAssignet, MyDbSqlite>();
        }

        static void TestAssignetInser<T, Tb>() where Tb : IOtherDataBaseFactory, new()
            where T : TestInsertBaseAssignet, new()
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
            var i = session.Insert(t);
            Log(1, i == 1);

           // Log(3, session.IsPersistent(t));
            List<T> list = new List<T>
            {
                new T(), new T(), new T()
            };
            i = session.InsertBulk(list);
            Log(4, i == 3);
           // i = list.Where(a => session.IsPersistent(a)).Count();
           // Log(5, i == 3);
            i = session.Query<T>().Count();
            Log(6, i == 4);
        }

        public static void Log(int i, bool b, string appen = null)
        {
            if (b)
            {
                Console.ForegroundColor
                    = ConsoleColor.Green;
            }
            else
            {
                Console.ForegroundColor
                    = ConsoleColor.Red;
            }

            Console.WriteLine($"{i} {b} {appen}", Console.ForegroundColor);
            Console.ForegroundColor
                = ConsoleColor.White;
        }

        [MapReceiverFreeSql]
        class MyFreeSql
        {
            public MyFreeSql(Guid idGuid, string name, int age, MyEnum @enum)
            {
                IdGuid = idGuid;
                Name = name;
                Age = age;
                MyEnum = (MyEnum)@enum;
            }

            public Guid IdGuid { get; }
            public string Name { get; }
            public int Age { get; }
            public MyEnum MyEnum { get; }
        }
    }
}