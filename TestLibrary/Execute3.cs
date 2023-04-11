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
        public static async Task TotalTestAsync()
        {
            await TestAsync<MyClassPostgres, MyDbPostgres>();
            await TestAsync<MyClassMysql, MyDbMySql>();
            await TestAsync<MyClassMsSql, MyDbMsSql>();
            await TestAsync<MyClassSqlite, MyDbSqlite>();
        }

        private static async Task TestAsync<T, TB>() where T : MyClassBase, new()
            where TB : IOtherDataBaseFactory, new()
        {
            var sb = Activator.CreateInstance<TB>();
            Configure.WriteLogFile(
                $"**************************{sb.GetProviderName()} Async*****************************");
            var session = await Configure.GetSessionAsync<TB>();

            if (await session.TableExistsAsync<T>()) await session.DropTableAsync<T>();

            await session.TableCreateAsync<T>();

            await session.InsertBulkAsync(new List<T>
            {
                new T { Age = 20 },
                new T { Age = 20 },
                new T { Age = 30 }
            }, 30);
            var count = await session.Query<T>().CountAsync();
            Execute.Log(1, count == 3);
            count = await session.Query<T>().CountAsync(a => a.Age == 20);
            Execute.Log(2, count == 2);
            var list = await session.Query<T>().Where(s => s.Age > 0).ToListAsync();
            Execute.Log(3, list.Count == 3);
            //var gb = await session.Query<T>().Where(s => s.Age > 0).GroupBy(d => d.Age).ToListAsync();
            //Execute.Log(4, gb.Count == 2);
            await session.Query<T>().Where(a => a.Age > 0).OrderByDescending(s => s.Age).ForEachAsync(d =>
            {
                Console.WriteLine($@"{d.Age}");
            });
            var f = await session.Query<T>().OrderBy(a => a.Age).FirstOrDefaultAsync();
            Execute.Log(5, f.Age == 20);
            f = await session.Query<T>().OrderBy(a => a.Age).FirstOrDefaultAsync(d => d.Age == 50);
            Execute.Log(6, f == null);

            f = await session.Query<T>().OrderBy(a => a.Age).FirstAsync();
            Execute.Log(7, f.Age == 20);
            try
            {
                _ = await session.Query<T>().OrderBy(a => a.Age).FirstAsync(d => d.Age == 50);
                Execute.Log(8, false);
            }
            catch (Exception)
            {
                Execute.Log(8, true);
            }

            f = await session.Query<T>().OrderBy(a => a.Age).LastOrDefaultAsync();
            Execute.Log(9, f.Age == 30);
            f = await session.Query<T>().OrderBy(a => a.Age).LastOrDefaultAsync(d => d.Age == 50);
            Execute.Log(10, f == null);
            f = await session.Query<T>().OrderBy(a => a.Age).LastAsync();
            Execute.Log(11, f.Age == 30);
            try
            {
                _ = await session.Query<T>().OrderBy(a => a.Age).LastAsync(d => d.Age == 50);
                Execute.Log(12, false);
            }
            catch (Exception)
            {
                Execute.Log(12, true);
            }

            try
            {
                _ = await session.Query<T>().Where(a => a.Age > 0).OrderBy(d => d.Age).SingleOrDefaultAsync();
                Execute.Log(13, false);
            }
            catch (Exception)
            {
                Execute.Log(13, true);
            }

            f = await session.Query<T>().Where(a => a.Age > 0).OrderBy(d => d.Age)
                .SingleOrDefaultAsync(t => t.Age == 100);
            Execute.Log(14, f == null);
            f = await session.Query<T>().SingleAsync(a => a.Age == 30);
            Execute.Log(15, f != null);
            try
            {
                _ = await session.Query<T>().SingleAsync(a => a.Age == 20);
                Execute.Log(15, false);
            }
            catch (Exception)
            {
                Execute.Log(15, true);
            }

            count = await session.Query<T>().SumAsync(a => a.Age);
            Execute.Log(16, count == 70);

            float sd = await session.Query<T>().AverageAsync(a => a.Age);
            Execute.Log(17, Math.Abs(sd - 23f) < 2);

            var v1 = await session.ExecuteNonQueryAsync("select 1", null);
            Execute.Log(18, v1 == -1);

            var o1 = await session.ExecuteScalarAsync("select 1", null);
            Execute.Log(19, o1.ToString() == "1");

            var dt = await session.GetDataTableAsync($"select * from {session.TableName<T>()} where age=20", null);
            Execute.Log(20, dt.Rows.Count == 2);

            var dr = await session.ExecuteReaderAsync($"select age from {session.TableName<T>()} where age=20", null);
            while (dr.Read()) Console.WriteLine($@"{dr[0]}");
            dr.Dispose();

            var b = await session.TableExistsAsync<T>();
            Execute.Log(21, b);
            await session.TruncateTableAsync<T>();
            count = await session.Query<T>().CountAsync();
            Execute.Log(22, count == 0);
            await session.DropTableAsync<T>();
            b = await session.TableExistsAsync<T>();
            Execute.Log(23, b == false);
            await session.TableCreateAsync<T>();

            await session.InsertBulkAsync(new List<T>
            {
                new T { Age = 20 },
                new T { Age = 20 },
                new T { Age = 30 }
            }, 30);
            count = await session.Query<T>().CountAsync();
            Execute.Log(24, count == 3);
            count = await session.SaveAsync(new T { Age = 5 });
            Execute.Log(25, count == 1);
            count = await session.Query<T>().CountAsync();
            Execute.Log(26, count == 4);
            f = await session.Query<T>().SingleAsync(a => a.Age == 5);
            f.Name = "123";
            count = await session.UpdateAsync(f, new[] { new AppenderWhere(session.ColumnName<T>(a => a.Age), 5) });
            Execute.Log(27, count == 1);
            _ = await session.Query<T>().SingleAsync(a => a.Age == 5 && a.Name == "123");
            await session.TruncateTableAsync<T>();
            try
            {
                using (await session.BeginTransactionAsync())
                {
                    list = Enumerable.Range(0, 200000).Select(i => new T()).ToList();
                    var source = new CancellationTokenSource(50);
                    await session.InsertBulkAsync(list, 30, source.Token);
                }

                Execute.Log(28, false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            count = await session.Query<T>().CountAsync();
            Execute.Log(29, count == 0);
            using (await session.BeginTransactionAsync())
            {
                await session.SaveAsync(new T());
                await session.SaveAsync(new T());
            }


            count = await session.Query<T>().UpdateAsync(a => new Dictionary<object, object>
            {
                { a.Age, 23 }
            });
            Execute.Log(30, count == 2);
            count = await session.Query<T>().CountAsync(a => a.Age == 23);
            Execute.Log(31, count == 2);
            count = await session.Query<T>().DeleteAsync(a => a.Age == 23);
            Execute.Log(32, count == 2);
            count = await session.Query<T>().CountAsync();
            Execute.Log(33, count == 0);

            await session.TruncateTableAsync<T>();
            try
            {
                using (await session.BeginTransactionAsync())
                {
                    var source = new CancellationTokenSource();
                    source.Cancel();
                    await session.SaveAsync(new T(), source.Token);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            count = await session.Query<T>().CountAsync();
            Execute.Log(34, count == 0);
            try
            {
                using (await session.BeginTransactionAsync())
                {
                    var source = new CancellationTokenSource();
                    source.Cancel();
                    await session.Query<T>().UpdateAsync(a => new Dictionary<object, object>
                    {
                        { a.Age, 10 }
                    });
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            count = await session.Query<T>().CountAsync();
            Execute.Log(35, count == 0);
            await session.TruncateTableAsync<T>();
            using (var tr = await session.BeginTransactionAsync())
            {
                await session.SaveAsync(new T());
                await session.SaveAsync(new T());
                await tr.RollbackAsync();
            }

            count = await session.Query<T>().CountAsync();
            Execute.Log(36, count == 0);
            try
            {
                using (var tr = await session.BeginTransactionAsync())
                {
                    await session.SaveAsync(new T());
                    await session.SaveAsync(new T());
                    await tr.RollbackAsync();
                    await tr.RollbackAsync();
                }
            }
            catch (Exception e)
            {
                Execute.Log(37, true, e.Message);
            }
            try
            {
                using (var tr = await session.BeginTransactionAsync())
                {
                    await session.SaveAsync(new T());
                    await session.SaveAsync(new T());
                    await tr.CommitAsync();
                    await tr.CommitAsync();
                }
            }
            catch (Exception e)
            {
                Execute.Log(38, true, e.Message);
            }

            count = await session.Query<T>().CountAsync();
            Execute.Log(39, count == 2);


            if (await session.TableExistsAsync<T>())
            {
                await session.DropTableAsync<T>();
            }

            await session.TableCreateAsync<T>();
            await session.InsertBulkAsync(new List<T>()
                {
                    new T()
                }, 30);
            var u = await session.Query<T>().SingleAsync();
            u.TestUser = new TestUser() { Name = "asas", Id = 2 };
            await session.SaveAsync(u);
            u = await session.Query<T>().SingleAsync();
            Execute.Log(40,  u.TestUser!=null&&u.TestUser.Name=="asas");



            await session.DisposeAsync();
        }
    }
}