using ORM_1_21_;
using ORM_1_21_.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TestLibrary
{
    public static class Execute3
    {

        public static async Task TotalTestNull()
        {
            await TestAsync<MyClassPostgres, MyDbPostgres>();
            await TestAsync<MyClassMysql, MyDbMySql>();
            await TestAsync<MyClassMsSql, MyDbMsSql>();
            await TestAsync<MyClassSqlite, MyDbSqlite>();
        }

        private static async Task TestAsync<T, Tb>() where T : MyClassBase, new() where Tb : IOtherDataBaseFactory, new()
        {
            var sb = Activator.CreateInstance<Tb>();
            Configure.WriteLogFile($"**************************{sb.GetProviderName()} Async*****************************");
            ISession session = Configure.GetSession<Tb>();

            if (await session.TableExistsAsync<T>())
            {
                await session.DropTableAsync<T>();
            }

            await session.TableCreateAsync<T>();

            await session.InsertBulkAsync(new List<T>()
            {
                new T(){Age = 20},
                new T(){Age = 20},
                new T(){Age = 30},
            }, 30);
            var count = await session.Query<T>().CountAsync();
            Console.WriteLine($@"{1} {count == 3}");
            count = await session.Query<T>().CountAsync(a => a.Age == 20);
            Console.WriteLine($@"{2} {count == 2}");
            var list = await session.Query<T>().Where(s => s.Age > 0).ToListAsync();
            Console.WriteLine($@"{3} {list.Count == 3}");
            var gb = await session.Query<T>().Where(s => s.Age > 0).GroupBy(d => d.Age).ToListAsync();
            Console.WriteLine($@"{4} {gb.Count == 2}");
            await session.Query<T>().Where(a => a.Age > 0).OrderByDescending(s => s.Age).ForEachAsync(d =>
            {
                Console.WriteLine($@"{d.Age}");
            });
            var f = await session.Query<T>().OrderBy(a => a.Age).FirstOrDefaultAsync();
            Console.WriteLine($@"{5} {f.Age == 20}");
            f = await session.Query<T>().OrderBy(a => a.Age).FirstOrDefaultAsync(d => d.Age == 50);
            Console.WriteLine($@"{6} {f == null}");

            f = await session.Query<T>().OrderBy(a => a.Age).FirstAsync();
            Console.WriteLine($@"{7} {f.Age == 20}");
            try
            {
                f = await session.Query<T>().OrderBy(a => a.Age).FirstAsync(d => d.Age == 50);
                Console.WriteLine($@"{8} {false}");
            }
            catch (Exception e)
            {
                Console.WriteLine($@"{8} {true}");
            }
            f = await session.Query<T>().OrderBy(a => a.Age).LastOrDefaultAsync();
            Console.WriteLine($@"{9} {f.Age == 30}");
            f = await session.Query<T>().OrderBy(a => a.Age).LastOrDefaultAsync(d => d.Age == 50);
            Console.WriteLine($@"{10} {f == null}");
            f = await session.Query<T>().OrderBy(a => a.Age).LastAsync();
            Console.WriteLine($@"{11} {f.Age == 30}");
            try
            {
                f = await session.Query<T>().OrderBy(a => a.Age).LastAsync(d => d.Age == 50);
                Console.WriteLine($@"{12} {false}");
            }
            catch (Exception e)
            {
                Console.WriteLine($@"{12} {true}");
            }

            try
            {
                f = await session.Query<T>().Where(a => a.Age > 0).OrderBy(d => d.Age).SingleOrDefaultAsync();
                Console.WriteLine($@"{13} {false}");
            }
            catch (Exception e)
            {
                Console.WriteLine($@"{13} {true}");
            }

            f = await session.Query<T>().Where(a => a.Age > 0).OrderBy(d => d.Age).SingleOrDefaultAsync(t => t.Age == 100);
            Console.WriteLine($@"{14} {f == null}");
            f = await session.Query<T>().SingleAsync(a => a.Age == 30);
            Console.WriteLine($@"{15} {f != null}");
            try
            {
                f = await session.Query<T>().SingleAsync(a => a.Age == 20);
                Console.WriteLine($@"{15} {false}");
            }
            catch (Exception e)
            {
                Console.WriteLine($@"{15} {true}");
            }
            count = await session.Query<T>().SumAsync(a => a.Age);
            Console.WriteLine($@"{16} {count == 70}");

            float sd = await session.Query<T>().AverageAsync(a => a.Age);
            Console.WriteLine($@"{17} {Math.Abs(sd - 23f) < 2}");

            var v1 = await session.ExecuteNonQueryAsync("select 1", null);
            Console.WriteLine($@"{18} {v1 == -1}");

            var o1 = await session.ExecuteScalarAsync("select 1", null);
            Console.WriteLine($@"{19} {o1.ToString() == "1"}");

            var dt = await session.GetDataTableAsync($"select * from {session.TableName<T>()} where age=20", null);
            Console.WriteLine($@"{20} {dt.Rows.Count == 2}");

            var dr = await session.ExecuteReaderAsync($"select age from {session.TableName<T>()} where age=20", null);
            while (dr.Read())
            {
                Console.WriteLine($@"{dr[0]}");
            }
            dr.Dispose();

            var b = await session.TableExistsAsync<T>();
            Console.WriteLine($@"{21} {b}");
            await session.TruncateTableAsync<T>();
            count = await session.Query<T>().CountAsync();
            Console.WriteLine($@"{22} {count == 0}");
            await session.DropTableAsync<T>();
            b = await session.TableExistsAsync<T>();
            Console.WriteLine($@"{23} {b == false}");
            await session.TableCreateAsync<T>();

            await session.InsertBulkAsync(new List<T>()
            {
                new T(){Age = 20},
                new T(){Age = 20},
                new T(){Age = 30},
            }, 30);
            count = await session.Query<T>().CountAsync();
            Console.WriteLine($@"{24} {count == 3}");
            count = await session.SaveAsync(new T() { Age = 5 });
            Console.WriteLine($@"{25} {count == 1}");
            count = await session.Query<T>().CountAsync();
            Console.WriteLine($@"{26} {count == 4}");
            f = await session.Query<T>().SingleAsync(a => a.Age == 5);
            f.Name = "123";
            count = await session.UpdateAsync(f, new[] { new AppenderWhere(session.ColumnName<T>(a => a.Age), 5) });
            Console.WriteLine($@"{27} {count == 1}");
            f = await session.Query<T>().SingleAsync(a => a.Age == 5 && a.Name == "123");
            await session.TruncateTableAsync<T>();
            try
            {
                
                using (var tr = session.BeginTransaction())
                {
                    list = Enumerable.Range(0, 200000).Select(i => new T()).ToList();
                    CancellationTokenSource source = new CancellationTokenSource(50);
                    await session.InsertBulkAsync(list, 30, source.Token);
                   
                }
                Console.WriteLine($@"{28} {false}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            count = await session.Query<T>().CountAsync();
            Console.WriteLine($@"{29} {count == 0}");
            using (var tr = session.BeginTransaction())
            {
                await session.SaveAsync(new T());
                await session.SaveAsync(new T());
            }

            var rr=session.Query<T>().Count();

           
            count = await session.Query<T>().UpdateAsync(a => new Dictionary<object, object>()
            {
                { a.Age, 23 }
            });
            Console.WriteLine($@"{30} {count == 2}");
            count = await session.Query<T>().CountAsync(a => a.Age == 23);
            Console.WriteLine($@"{31} {count == 2}");
            count = await session.Query<T>().DeleteAsync(a => a.Age == 23);
            Console.WriteLine($@"{32} {count == 2}");
            count = await session.Query<T>().CountAsync();
            Console.WriteLine($@"{33} {count == 0}");

            await session.TruncateTableAsync<T>();
            try
            {
                using (var tr = session.BeginTransaction())
                {
                    CancellationTokenSource source = new CancellationTokenSource();
                    source.Cancel();
                    await session.SaveAsync(new T(), source.Token);
                }
            }
            catch (Exception e)
            {
               Console.WriteLine(e.Message);
            }
            count = await session.Query<T>().CountAsync();
            Console.WriteLine($@"{34} {count == 0}");
            try
            {
                using (var tr = session.BeginTransaction())
                {
                    CancellationTokenSource source = new CancellationTokenSource();
                    source.Cancel();
                    await session.Query<T>().UpdateAsync(a=>new Dictionary<object, object>()
                    {
                        {a.Age,10}
                    });
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            count = await session.Query<T>().CountAsync();
            Console.WriteLine($@"{35} {count == 0}");








        }
    }
}
