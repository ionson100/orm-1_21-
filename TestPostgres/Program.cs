using ORM_1_21_;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TestLibrary;


namespace TestPostgres
{
    internal class Program
    {
        private const ProviderName ProviderName = ORM_1_21_.ProviderName.PostgreSql;
        static async Task Main(string[] args)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            switch (ProviderName)
            {
                case ProviderName.MsSql:
                    Starter.Run(ConnectionStrings.MsSql, ProviderName);
                    break;
                case ProviderName.MySql:
                    Starter.Run(ConnectionStrings.Mysql, ProviderName);
                    break;
                case ProviderName.PostgreSql:
                    Starter.Run(ConnectionStrings.Postgesql, ProviderName);
                    break;
                case ProviderName.SqLite:
                    Starter.Run(ConnectionStrings.Sqlite, ProviderName);//
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            //Execute.RunOtherSession();
            //Execute.RunThread();
            //Console.ReadKey();
            //Console.ReadKey();
            Execute.TotalTest();
            Execute.TestNativeInsert();
            Execute.TestAssignetInsert();
            Execute2.TestTimeStamp();
            await Execute3.TotalTestAsync();
            await ExecuteLinqAll.Run();
            ExecutePrimaryKey.Run();
            await ExecuteFree.Run();
            await ExecuteSp.Run();




            ISession session = Configure.Session;
            {
                await session.DropTableIfExistsAsync<MyClassJoinPostgres>();


                await session.TableCreateAsync<MyClassJoinPostgres>();

                //session.Insert(new MyClassJoinPostgres() { Age = 40, Name = "name" });
                //session.Insert(new MyClassJoinPostgres() { Age = 40, Name = "name" });
                //session.Insert(new MyClassJoinPostgres() { Age = 40, Name = "name" });
                //session.Insert(new MyClassJoinPostgres() { Age = 40, Name = "name" });
                //session.InsertBulk(new List<MyClassJoinPostgres>
                //    {
                //        new MyClassJoinPostgres() { Age = 40, Name = "name" },
                //        new MyClassJoinPostgres() { Age = 401, Name = "name" },
                //        new MyClassJoinPostgres() { Age = 201, Name = "name" },
                //        new MyClassJoinPostgres() { Age = 202, Name = "name" },
                //        new MyClassJoinPostgres() { Age = 203, Name = "name" },
                //        new MyClassJoinPostgres() { Age = 40, Name = "name" },
                //        new MyClassJoinPostgres() { Age = 40, Name = "name" },
                //        new MyClassJoinPostgres() { Age = 40, Name = "name" },
                //
                //    }
                //);


                //var ee = session.Query<MyClassJoinPostgres>().First();

                //var sas = session.Update(ee);
                session.DropTableIfExists<Order>();
                session.TableCreate<Order>();
                var s = session.Insert(new Order());
                var order = session.Query<Order>().First();
                //order.DecimalNull = 100;
                session.Update(order);
                // var sas = session.Query<Order>().Where(a => a.DecimalNull == 100).First();

                // var ss = session.Query<MyClassJoinPostgres>().ToList();
                //  ss = session.Query<MyClassJoinPostgres>().ToList();



                session.Insert(new MyClassJoinPostgres());
                session.Insert(new MyClassJoinPostgres());
                session.InsertBulk(new List<Order>() { new Order() });
                {
                    var sss = session.FreeSql<Order>($"select * from {session.TableName<Order>()}");
                    Stopwatch stopwatch = new Stopwatch();



                    stopwatch.Start();

                    var ssr = session.Query<Order>().ToList();

                    stopwatch.Stop();
                    var v2 = stopwatch.ElapsedMilliseconds;

                }





            }


        }

        [MapTable("Order")]
        public partial class Order
        {
            [MapPrimaryKey("Id", Generator.Native)]
            public int Id { get; set; }
            [MapColumn]
            public int Number { get; set; }
            // [MapColumn]
            // public string Text { get; set; }
            // [MapColumn]
            // public int CustomerId { get; set; }
            //
            // [MapColumn] public int IntNull { get; set; }
            //
            //
            //
            // [MapColumn] public long Long { get; set; } = 33;
            // [MapColumn] public long? LongNull { get; set; }
            //
            // [MapColumn] public short Short { get; set; } = 33;
            // [MapColumn] public short? ShortNull { get; set; }

            // [MapColumn] public uint Uint { get; set; } = 33;
            // [MapColumn] public uint? UintNull { get; set; }
            //
            // [MapColumn] public ushort Ushort { get; set; } = 33;
            // [MapColumn] public ushort? UshortNull { get; set; }
            //
            // [MapColumn] public ulong Ulong { get; set; } = 33;
            // [MapColumn] public ulong? UlongNull { get; set; }

            // [MapColumn] public decimal Decimal { get; set; } = 33;
            // [MapColumn] public decimal? DecimalNull { get; set; }
            //
            // [MapColumn] public float Float { get; set; } = 33;
            // [MapColumn] public float? FloatNull { get; set; }
            //
            // [MapColumn] public char Char { get; set; } = '1';
            // [MapColumn] public char? CharNull { get; set; }
            //
            //
            // [MapColumn] public bool Bool { get; set; }
            // [MapColumn] public bool? BoolNull { get; set; }
            //
            // [MapColumn] public byte Byte { get; set; } = 33;
            // [MapColumn] public byte? ByteNull { get; set; }
            //
            // [MapColumn] public byte[] Bytes { get; set; } = { 1, 2 };
            //
            // [MapColumn] public Guid GuidA { get; set; } = System.Guid.NewGuid();
            // [MapColumn] public Guid? Guid { get; set; }
            //
            // [MapColumn] public DateTime Date { get; set; } = DateTime.Now;
            // [MapColumn] public DateTime? DateNull { get; set; }


        }
    }













}
