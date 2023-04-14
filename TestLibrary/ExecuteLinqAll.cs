using ORM_1_21_;
using ORM_1_21_.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestLibrary
{
    public class ExecuteLinqAll
    {
        public static async Task Run()
        {

            await InnerRun<MyClassPostgres, MyClassPostgres1, MyDbPostgres>();
            await InnerRun<MyClassMysql, MyClassMysql1, MyDbMySql>();
            await InnerRun<MyClassMsSql, MyClassMsSql1, MyDbMsSql>();
            await InnerRun<MyClassSqlite, MyClassSqlite1, MyDbSqlite>();
        }

        static async Task InnerRun<T, T1, TDB>() where T : MyClassBase, new() where T1 : MyClassBase, new() where TDB : IOtherDataBaseFactory, new()
        {
            var s = Activator.CreateInstance<TDB>();
            Console.WriteLine($"**************************{s.GetProviderName()}*****************************");
            ISession session = Configure.GetSession<TDB>();
            if (await session.TableExistsAsync<T>())
            {
                await session.DropTableAsync<T>();
            }

            await session.TableCreateAsync<T>();


            await session.DropTableIfExistsAsync<T1>();
            await session.TableCreateAsync<T1>();

            session.InsertBulk(new List<T>
            {
                new T { Age = 20, Name = "name20" },
                new T { Age = 20, Name = "name20" },
                new T { Age = 30, Name = "name30" },
                new T { Age = 30, Name = "name30" }
            });
            session.InsertBulk(new List<T1>
            {
                new T1 { Age = 20, Name = "name20" },
                new T1 { Age = 20, Name = "name20" },
                new T1 { Age = 30, Name = "name30" },
                new T1 { Age = 30, Name = "name30" }
            });
            var listT = session.Query<T>().Where(a => a.Age > 20).ToList();
            Execute.Log(1, listT.Count == 2, " list");

            listT = session.Query<T>().Where(a => a.Age > 20).ToList();
            Execute.Log(2, listT.Count == 2, " listAsync");

            Console.WriteLine("ForEach");
            session.Query<T>().ForEach(a =>
            {
                Console.WriteLine(a.Age + " " + a.Name);
            });
            Console.WriteLine("ForEachAsync");
            await session.Query<T>().ForEachAsync(a =>
            {
                Console.WriteLine(a.Age + " " + a.Name);
            });
            listT = session.Query<T>().Append(new T { Age = 100 }).ToList();
            var oT = listT.Last();
            Execute.Log(3, listT.Count == 5 && oT.Age == 100, " listAsync");

            var array = session.Query<T>().ToArray();
            Execute.Log(4, array.Length == 4, " array");

            array = (T[])await session.Query<T>().ToArrayAsync();
            Execute.Log(5, array.Length == 4, " arrayAsync");

            var count = session.Query<T>().Count();
            Execute.Log(6, array.Length == 4, " count");

            count = await session.Query<T>().CountAsync();
            Execute.Log(7, array.Length == 4, " count");

            oT = session.Query<T>().OrderBy(a => a.Age).AggregateCore((t1, t2) =>
                {
                    t2.Name += $"{t1.Name} {t1.Age}";
                    return t2;
                });//
            Execute.Log(8, oT.Name == "name30name30name20name20 20 20 30", " AggregateCore 1");

            oT = await session.Query<T>().OrderBy(a => a.Age).AggregateCoreAsync((t1, t2) =>
               {
                   t2.Name += $"{t1.Name} {t1.Age}";
                   return t2;
               });//
            Execute.Log(9, oT.Name == "name30name30name20name20 20 20 30", " AggregateCoreAsync 1");

            oT = session.Query<T>().AggregateCore(new T(), (a, b) =>
             {
                 a.Age += b.Age;
                 return a;
             });
            Execute.Log(10, oT.Age == 100, " AggregateCore 2");

            oT = await session.Query<T>().AggregateCoreAsync(new T(), (a, b) =>
            {
                a.Age += b.Age;
                return a;
            });
            Execute.Log(11, oT.Age == 100, " AggregateCoreAsync 2");

            count = session.Query<T>().AggregateCore(new T(), new Func<T, T, T>((a, b) =>
              {
                  a.Age += b.Age;
                  return a;
              }), d => d.Age);
            Execute.Log(12, oT.Age == 100, " AggregateCore 3");

            count = await session.Query<T>().AggregateCoreAsync(new T(), new Func<T, T, T>((a, b) =>
            {
                a.Age += b.Age;
                return a;
            }), d => d.Age);
            Execute.Log(12, oT.Age == 100, " AggregateCoreAsync 3");

            var booL = session.Query<T>().All(a => a.Age == 30);
            Execute.Log(13, booL == false, " all");
            booL = session.Query<T>().Where(a => a.Age == 30).All(a => a.Age == 30);
            Execute.Log(14, booL == true, " all");

            booL = await session.Query<T>().AllAsync(a => a.Age == 30);
            Execute.Log(15, booL == false, " allAsync");
            booL = await session.Query<T>().Where(a => a.Age == 30).AllAsync(a => a.Age == 30);
            Execute.Log(16, booL == true, " allAsync");

            booL = session.Query<T>().Any(a => a.Age == 40);
            Execute.Log(17, booL == false, " any");
            booL = session.Query<T>().Where(a => a.Age == 40).Any();
            Execute.Log(18, booL == false, " any");
            booL = session.Query<T>().Any(a => a.Age == 30);
            Execute.Log(19, booL == true, " any");
            booL = session.Query<T>().Where(a => a.Age == 30).Any();
            Execute.Log(20, booL == true, " any");

            booL = await session.Query<T>().AnyAsync(a => a.Age == 40);
            Execute.Log(21, booL == false, " anyAsync");
            booL = await session.Query<T>().Where(a => a.Age == 40).AnyAsync();
            Execute.Log(22, booL == false, " anyAsync");
            booL = await session.Query<T>().AnyAsync(a => a.Age == 30);
            Execute.Log(23, booL == true, " anyAsync");
            booL = await session.Query<T>().Where(a => a.Age == 30).AnyAsync();
            Execute.Log(24, booL == true, " anyAsync");
            Console.WriteLine("AsEnumerable");
            foreach (var myClassBase in session.Query<T>().AsEnumerable())
            {
                Console.WriteLine(myClassBase.Age);
            }
            Console.WriteLine("AsEnumerableAsync");
            var r = await session.Query<T>().AsEnumerableAsync();
            foreach (var myClassBase in r)
            {
                Console.WriteLine(myClassBase.Age);
            }
            Console.WriteLine("AsQueryable");
            var myClassBases = session.Query<T>().AsQueryable();
            foreach (T classBase in myClassBases)
            {
                Console.WriteLine(classBase.Age);
            }

            count = (int)session.Query<T>().Average(a => a.Age);
            Execute.Log(25, count == 25, " Averagev");
            count = await session.Query<T>().AverageAsync(a => a.Age);
            Execute.Log(26, count == 25, " AverageAsync");

            listT = session.Query<T>().Between(a => a.Age, 25, 30).ToList();
            Execute.Log(27, listT.Count == 2, " Between");
            Console.WriteLine("CastCore");
            foreach (var classBase in session.Query<T>().CastCore<MyClassBase>())
            {
                Console.WriteLine(classBase.Age);
            }
            Console.WriteLine("CastCoreAsync");
            var re = await session.Query<T>().CastCoreAsync<MyClassBase>();
            foreach (var classBase in re)
            {
                Console.WriteLine(classBase.Age);
            }

            listT = session.Query<T>().Where(a => a.Age == 30).ExceptCore(session.Query<T>().Where(a => a.Age == 20), new MyComparer<T>()).ToList();
            Execute.Log(28, listT.Count == 1, " ExceptCore");

            var res1 = await session.Query<T>().Where(a => a.Age == 30).ExceptCoreAsync(session.Query<T>().Where(a => a.Age == 20), new MyComparer<T>());
            Execute.Log(29, res1.Count() == 1, " ExceptCoreAsync");

            listT = session.Query<T>().ConcatCore(session.Query<T>()).ToList();
            Execute.Log(30, listT.Count() == 8, " ConcatCore");

            var res = await session.Query<T>().ConcatCoreAsync(session.Query<T>());
            Execute.Log(301, res.Count() == 8, " ConcatCore");

            listT = session.Query<T>().Where(a => a.Age == 30).UnionCore(session.Query<T>()).ToList();
            Execute.Log(31, listT.Count == 4, " UnionCore");

            var en = session.Query<T>().AsEnumerable();
            listT = session.Query<T>().Where(a => a.Age == 30).UnionCore(en).ToList();
            Execute.Log(32, listT.Count == 4, " UnionCore");

            en = await session.Query<T>().Where(a => a.Age == 30).UnionCoreAsync(session.Query<T>());
            Execute.Log(33, en.Count() == 4, " UnionCoreAsync");

            Console.WriteLine("ZipCore");
            foreach (string s1 in session.Query<T>().Where(a => a.Age == 20).ZipCore(session.Query<T>().ToList(), (a, b) => (a.Age + @" " + a.Name)))
            {
                Console.WriteLine(s1);
            }
            Console.WriteLine("ZipCoreAsync");
            var reSs = await session.Query<T>().Where(a => a.Age == 20)
                .ZipCoreAsync(session.Query<T>().ToList(), (a, b) => (a.Age + @" " + a.Name));
            foreach (string reS in reSs)
            {
                Console.WriteLine(reS);
            }

            Console.WriteLine("GroupByCore");
            List<IGrouping<int, T>> list = session.Query<T>().GroupByCore(a => a.Age).ToList();
            foreach (IGrouping<int, T> classBases in list)
            {
                Console.WriteLine(classBases.Key);
                foreach (T st in classBases)
                {
                    Console.WriteLine($"{st.Age} {st.Name}");
                }
            }

            var unused = session.Query<T>().GroupByCore(a => a.Age, (i, b) =>
             {
                 return new { id = i, sum = b.Sum(ss => ss.Age) };
             });

            Console.WriteLine("GroupByCore");
            foreach (var x1 in unused)
            {
                Console.WriteLine(x1);
            }
            unused = await session.Query<T>().GroupByCoreAsync(a => a.Age, (i, b) =>
            {
                return new { id = i, sum = b.Sum(ss => ss.Age) };
            });

            Console.WriteLine("GroupByCoreAsync");
            foreach (var x1 in unused)
            {
                Console.WriteLine(x1);
            }
            Console.WriteLine(Environment.NewLine+"Split IQueryable :2 ");
            var res2 = session.Query<T>().Split(2);
            foreach (IEnumerable<T> classBases in res2)
            {
                Console.WriteLine($"portion: {classBases.Count()}");
                foreach (T t in classBases)
                {
                    Console.WriteLine($"   {t.Age} {t.Name}");
                }
            }

            Console.WriteLine(Environment.NewLine + "SplitAsync IQueryable :2 ");
            res2 = await session.Query<T>().SplitAsync(2);
            foreach (IEnumerable<T> classBases in res2)
            {
                Console.WriteLine($"portion: {classBases.Count()}");
                foreach (T t in classBases)
                {
                    Console.WriteLine($"   {t.Age} {t.Name}");
                }
            }

            listT = session.Query<T>().ToList();
            Console.WriteLine(Environment.NewLine + "Split IEnumerable :2 ");
            res2 = listT.Split(2);
            foreach (IEnumerable<T> classBases in res2)
            {
                Console.WriteLine($"portion: {classBases.Count()}");
                foreach (T t in classBases)
                {
                    Console.WriteLine($"   {t.Age} {t.Name}");
                }
            }

            count = session.Query<T>().ToArray().Length;
            Execute.Log(34, count == 4, " ToArray");

            var rs = await session.Query<T>().ToArrayAsync();
            Execute.Log(35, rs.Length==4, " ToArrayAsync");


           
            session.Query<T>().Update(st => new Dictionary<object, object>
            {
                { st.DateTime, DateTime.Now }
            });
            listT = session.Query<T>().Where(a => a.DateTime.Day == DateTime.Now.Day).ToList();
            Execute.Log(36, listT.Count == 4, " update");

            await session.Query<T>().UpdateAsync(st => new Dictionary<object, object>
            {
                { st.DateTime, DateTime.Now }
            });
            listT = session.Query<T>().Where(a => a.DateTime.Day == DateTime.Now.Day).ToList();
            Execute.Log(36, listT.Count == 4, " updateAsync");

            Console.WriteLine(Environment.NewLine + "GroupJoinCore ");
            var sas = session.Query<T>().GroupJoinCore(session.Query<T1>(),
                a => a.Age,
                b => b.Age,
                (m, ss) => new { id=m.Id, sum = ss.Sum(d => d.Age) });
            foreach (var sa in sas)
            {
                Console.WriteLine($"  {sa}");
            }

            Console.WriteLine(Environment.NewLine + "GroupJoinCoreAsync ");
             sas = await session.Query<T>().GroupJoinCoreAsync(session.Query<T1>(),
                a => a.Age,
                b => b.Age,
                (m, ss) => new { id = m.Id, sum = ss.Sum(d => d.Age) });
            foreach (var sa in sas)
            {
                Console.WriteLine($"  {sa}");
            }

            var dictionary = session.Query<T>().ToDictionary(a => a.Id,d=>d.Name);
            Execute.Log(36, dictionary.Count == 4, " ToDictionary");

             dictionary = await session.Query<T>().ToDictionaryAsync(a => a.Id, d => d.Name);
            Execute.Log(37, dictionary.Count == 4, " ToDictionaryAsync");

            var l = session.Query<T1>().ToList();
            count= session.Query<T>().SelectManyCore(a=>a.Name,(t, c) => new {t.Age,c}).Select(c=>c).Count();
            Execute.Log(38, count==24, " SelectManyCore");

            var res3 = await session.Query<T>().SelectManyCoreAsync(a => a.Name, (t, c) => new { t.Age, c });
            Execute.Log(38, res3.Count() == 24, " SelectManyCoreAsync");

            Console.WriteLine(Environment.NewLine + "JoinCore IQueryable");
            var join= session.Query<T>().JoinCore(session.Query<T1>(), a => a.Age, 
                b => b.Age, (aa, bb) => new { aa.Age, bb.Name });
            foreach (var x1 in @join)
            {
                Console.WriteLine($"  {x1}");
            }

            Console.WriteLine(Environment.NewLine + "JoinCoreAsync IQueryable");
            join = await session.Query<T>().JoinCoreAsync(session.Query<T1>(), a => a.Age,
                b => b.Age, (aa, bb) => new { aa.Age, bb.Name });
            foreach (var x1 in @join)
            {
                Console.WriteLine($"  {x1}");
            }

            var listT1 = session.Query<T1>().AsEnumerable();
            Console.WriteLine(Environment.NewLine + "JoinCore IEnumerable");
            join = session.Query<T>().JoinCore(listT1, a => a.Age,
                b => b.Age, (aa, bb) => new { aa.Age, bb.Name });
            foreach (var x1 in @join)
            {
                Console.WriteLine($"  {x1}");
            }
            Console.WriteLine(Environment.NewLine + "JoinCoreAsync IEnumerable");
            join = await session.Query<T>().JoinCoreAsync(listT1, a => a.Age,
                b => b.Age, (aa, bb) => new { aa.Age, bb.Name });
            foreach (var x1 in @join)
            {
                Console.WriteLine($"  {x1}");
            }


        }



    }
    internal class MyComparer<T> : IEqualityComparer<T> where T : MyClassBase, new()
    {
        public bool Equals(T x, T y)
        {
            return x.Age == y.Age;
        }

        public int GetHashCode(T obj)
        {
            return obj.Age;
        }
    }


}
