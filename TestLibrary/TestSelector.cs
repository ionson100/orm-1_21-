using ORM_1_21_;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestLibrary
{
    public static class TestSelector
    {
        public static async Task Run()
        {

            await InnerRun<OrderPostgres, MyDbPostgres>();
            await InnerRun<OrderMysql, MyDbMySql>();
            //await InnerRun<OrderMsSql, MyDbMsSql>();
            await InnerRun<OrderSqlite, MyDbSqlite>();
        }

        private static async Task InnerRun<T, TB>() where T : Order, new() where TB : IOtherDataBaseFactory, new()
        {
            var s = Activator.CreateInstance<TB>();
            Console.WriteLine($"**************************{s.GetProviderName()}*****************************");
            ISession session = Configure.GetSession<TB>();
            await session.DropTableIfExistsAsync<T>();
            await session.TableCreateAsync<T>();

            session.InsertBulk(new List<T>
            {
                new T { Date = DateTime.Now,Text = "name",DecimalNull = new decimal(3.4),FloatNull = 3.4f ,DoubleNull = 3.4,LongNull = 3L},
                new T { Date = DateTime.Now,Text = "name",DecimalNull = new decimal(3.8),FloatNull = 3.8f,DoubleNull = 3.8,LongNull = 4L},
            });
            var list = session.Query<T>().ToList();
            {
                var v1 = session.Query<T>().Where(a => a.Text == "name").Select(r => r.DecimalNull).ToList();

                Execute.Log(1, Decimal.Compare(v1.Sum(a => a.Value), new decimal(7.2)) == 0);
                v1 = await session.Query<T>().Where(a => a.Text == "name").Select(r => r.DecimalNull).ToListAsync();
                Execute.Log(2, Decimal.Compare(v1.Sum(a => a.Value), new decimal(7.2)) == 0);
                var su1 = session.Query<T>().Where(a => a.Text == "name").Sum(r => r.DecimalNull);
                Execute.Log(3, Decimal.Compare(su1.Value, new decimal(7.2)) == 0);
                su1 = await session.Query<T>().Where(a => a.Text == "name").SumAsync(r => r.DecimalNull);
                Execute.Log(4, Decimal.Compare(su1.Value, new decimal(7.2)) == 0);
                su1 = session.Query<T>().Where(a => a.Text == "name").Average(r => r.DecimalNull);
                Execute.Log(5, Decimal.Compare(su1.Value, new decimal(3.6)) == 0);
                su1 = await session.Query<T>().Where(a => a.Text == "name").AverageAsync(r => r.DecimalNull);
                Execute.Log(6, Decimal.Compare(su1.Value, new decimal(3.6)) == 0);
                su1 = session.Query<T>().Where(a => a.Text == "name").Min(r => r.DecimalNull);
                Execute.Log(7, Decimal.Compare(su1.Value, new decimal(3.4)) == 0);
                su1 = await session.Query<T>().Where(a => a.Text == "name").MinAsync(r => r.DecimalNull);
                Execute.Log(8, Decimal.Compare(su1.Value, new decimal(3.4)) == 0);
                su1 = session.Query<T>().Where(a => a.Text == "name").Max(r => r.DecimalNull);
                Execute.Log(9, Decimal.Compare(su1.Value, new decimal(3.8)) == 0);
                su1 = await session.Query<T>().Where(a => a.Text == "name").MaxAsync(r => r.DecimalNull);
                Execute.Log(10, Decimal.Compare(su1.Value, new decimal(3.8)) == 0);
            }

            {
                var v1 = session.Query<T>().Where(a => a.Text == "name").Select(r => r.FloatNull).ToList();

                Execute.Log(11, v1.Sum(a => a.Value)==7.2f);
                v1 = await session.Query<T>().Where(a => a.Text == "name").Select(r => r.FloatNull).ToListAsync();
                Execute.Log(12, v1.Sum(a => a.Value) == 7.2f);
                var su1 = session.Query<T>().Where(a => a.Text == "name").Sum(r => r.FloatNull);
                Execute.Log(13, su1.Value==7.2f);
                su1 = await session.Query<T>().Where(a => a.Text == "name").SumAsync(r => r.FloatNull);
                Execute.Log(14, su1.Value == 7.2f);
                su1 = session.Query<T>().Where(a => a.Text == "name").Average(r => r.FloatNull);
                Execute.Log(15, su1.Value==3.6f);
                su1 = await session.Query<T>().Where(a => a.Text == "name").AverageAsync(r => r.FloatNull);
                Execute.Log(16, su1.Value==3.6f);
                su1 = session.Query<T>().Where(a => a.Text == "name").Min(r => r.FloatNull);
                Execute.Log(17, su1.Value==3.4f);
                su1 = await session.Query<T>().Where(a => a.Text == "name").MinAsync(r => r.FloatNull);
                Execute.Log(18, su1.Value==3.4f);
                su1 = session.Query<T>().Where(a => a.Text == "name").Max(r => r.FloatNull);
                Execute.Log(19, su1.Value==3.8f);
                su1 = await session.Query<T>().Where(a => a.Text == "name").MaxAsync(r => r.FloatNull);
                Execute.Log(20, su1.Value==3.8f);
            }

            {
                var v1 = session.Query<T>().Where(a => a.Text == "name").Select(r => r.DoubleNull).ToList();

                Execute.Log(21, Math.Round(v1.Sum(a => a.Value),2) == 7.2);
                v1 = await session.Query<T>().Where(a => a.Text == "name").Select(r => r.DoubleNull).ToListAsync();
                Execute.Log(22, Math.Round(v1.Sum(a => a.Value),2) == 7.2d);
                var su1 = session.Query<T>().Where(a => a.Text == "name").Sum(r => r.DoubleNull);
                Execute.Log(23, Math.Round(su1.Value,2) == 7.2d);
                su1 = await session.Query<T>().Where(a => a.Text == "name").SumAsync(r => r.DoubleNull);
                Execute.Log(24, Math.Round(su1.Value,2) == 7.2d);
                su1 = session.Query<T>().Where(a => a.Text == "name").Average(r => r.DoubleNull);
                Execute.Log(25, Math.Round(su1.Value,2) == 3.6d);
                su1 = await session.Query<T>().Where(a => a.Text == "name").AverageAsync(r => r.DoubleNull);
                Execute.Log(26, Math.Round(su1.Value,2) == 3.6d);
                su1 = session.Query<T>().Where(a => a.Text == "name").Min(r => r.DoubleNull);
                Execute.Log(27, su1.Value == 3.4d);
                su1 = await session.Query<T>().Where(a => a.Text == "name").MinAsync(r => r.DoubleNull);
                Execute.Log(28, su1.Value == 3.4d);
                su1 = session.Query<T>().Where(a => a.Text == "name").Max(r => r.DoubleNull);
                Execute.Log(29, su1.Value == 3.8d);
                su1 = await session.Query<T>().Where(a => a.Text == "name").MaxAsync(r => r.DoubleNull);
                Execute.Log(30, su1.Value == 3.8d);
            }

            {
                var v1 = session.Query<T>().Where(a => a.Text == "name").Select(r => r.LongNull).ToList();

                Execute.Log(31, v1.Sum(a => a.Value) == 7L);
                v1 = await session.Query<T>().Where(a => a.Text == "name").Select(r => r.LongNull).ToListAsync();
                Execute.Log(32, v1.Sum(a => a.Value) == 7L);
                var su1 = session.Query<T>().Where(a => a.Text == "name").Sum(r => r.LongNull);
                Execute.Log(33, su1.Value == 7L);
                su1 = await session.Query<T>().Where(a => a.Text == "name").SumAsync(r => r.LongNull);
                Execute.Log(34, su1.Value == 7L);
                su1 = (long?)session.Query<T>().Where(a => a.Text == "name").Average(r => r.LongNull);
                Execute.Log(35, su1.Value == 3L);
                
                su1 = await session.Query<T>().Where(a => a.Text == "name").AverageAsync(r => r.LongNull);
                if (s.GetProviderName() == ProviderName.MsSql)
                {
                    Execute.Log(36, su1.Value == 3L);
                }
                else
                {
                    Execute.Log(36, su1.Value == 4L);
                }
                
                su1 = session.Query<T>().Where(a => a.Text == "name").Min(r => r.LongNull);
                Execute.Log(37, su1.Value == 3L);
                su1 = await session.Query<T>().Where(a => a.Text == "name").MinAsync(r => r.LongNull);
                Execute.Log(38, su1.Value == 3L);
                su1 = session.Query<T>().Where(a => a.Text == "name").Max(r => r.LongNull);
                Execute.Log(39, su1.Value == 4L);
                su1 = await session.Query<T>().Where(a => a.Text == "name").MaxAsync(r => r.LongNull);
                Execute.Log(40, su1.Value == 4L);
            }

            var v2 = session.Query<T>().Where(a => a.Text == "name").Select(a => new { a.Id, a.Bytes, a.Guid, a.Date,a.DecimalNull }).ToList();
             foreach (var v in v2)
             {
                 Console.WriteLine(v);
             }


        }
    }
}
