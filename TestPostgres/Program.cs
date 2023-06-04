
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ORM_1_21_;
using TestLibrary;


namespace TestPostgres
{
    internal class Program
    {
        private static ProviderName ProviderNamee = ORM_1_21_.ProviderName.PostgreSql;
        static async Task Main(string[] args)
        {

            switch (ProviderNamee)
            {
                case ProviderName.MsSql:
                    Starter.Run(ConnectionStrings.MsSql, ProviderNamee);
                    break;
                case ProviderName.MySql:
                    Starter.Run(ConnectionStrings.Mysql, ProviderNamee);
                    break;
                case ProviderName.PostgreSql:
                    Starter.Run(ConnectionStrings.Postgesql, ProviderNamee);
                    break;
                case ProviderName.SqLite:
                    Starter.Run(ConnectionStrings.Sqlite, ProviderNamee);//
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            //  Execute.RunOtherSession();
            //  Execute.RunThread();
            //  //Console.ReadKey();
            //  Console.ReadKey();
             Execute.TotalTest();
             Execute.TestNativeInsert();
             Execute.TestAssignetInsert();
             Execute2.TestTimeStamp();
             await Execute3.TotalTestAsync();
             await ExecuteLinqAll.Run();
             ExecutePrimaryKey.Run();
             await ExecuteFree.Run();
             await ExecuteSp.Run();
             TestCapacity.Run();
             await TestSelector.Run();
             InsertUpdate.Run();
             await ExecAdd.Run();
             Console.ReadKey();





            ISession session = Configure.Session;
            {

                session.DropTableIfExists<Order1>();
                session.TableCreate<Order1>();
                session.InsertBulk(new List<Order1>
                {
                    new Order1()
                    {
                        Date = DateTime.Now,
                        BoolNull = true,
                        ByteNull = 1,
                        CharNull = 'c',
                        DateNull = DateTime.Now,
                        DecimalNull = new Decimal(1),
                        DoubleNull = 1.2d,
                        FloatNull = 1.2f,
                        GuidNull = Guid.Empty,
                        IntNull = 11,
                        LongNull = 11, 
                        ShortNull = 11,
                        Text = "assa"
                    }

                });
                var ss1 = session.Query<Order1>().First();
               
                ss1.BoolNull = null;
                ss1.ByteNull = null;
                ss1.CharNull = null;
                ss1.DateNull = null;
                ss1.DecimalNull = null;
                ss1.DoubleNull = null;
                ss1.FloatNull = null;
                ss1.GuidNull = null;
                ss1.IntNull = null;
                ss1.LongNull = null;
                ss1.ShortNull = null;
                ss1.Text = "assa";
                session.Update(ss1);
                

                ss1.Date = DateTime.Now;
                ss1.BoolNull = true;
                ss1.ByteNull = 1;
                ss1.CharNull = 'c';
                ss1.DateNull = DateTime.Now;
                ss1.DecimalNull = new Decimal(1);
                ss1.DoubleNull = 1.2d;
                ss1.FloatNull = 1.2f;
                ss1.GuidNull = Guid.Empty;
                ss1.IntNull = 11;
                ss1.LongNull = 11;
                ss1.ShortNull = 11;
                ss1.Text = "assa";
                session.Update(ss1);
                var ss = session.FreeSql<Order1>($"select * from {session.TableName<Order1>()}").ToList();


            }



        }
        [MapTable("Order")]
        public class Order1
        {
            [MapPrimaryKey("id", Generator.Assigned)]
            public Guid Id { get; set; } = System.Guid.NewGuid();
            [MapColumn("number")]
            public int Number { get; set; }
            [MapColumn]
            public string Text { get; set; }
            [MapColumn]
            public int CustomerId { get; set; }

            [MapColumn] public int? IntNull { get; set; }



            [MapColumn] public long Long { get; set; } = 33;
            [MapColumn] public long? LongNull { get; set; }

            [MapColumn] public short Short { get; set; } = 33;
            [MapColumn] public short? ShortNull { get; set; }

            //[MapColumn] public uint Uint { get; set; } = 33;
            //[MapColumn] public uint? UintNull { get; set; }
            //
            //[MapColumn] public ushort Ushort { get; set; } = 33;
            //[MapColumn] public ushort? UshortNull { get; set; }
            //
            //[MapColumn] public ulong Ulong { get; set; } = 33;
            //[MapColumn] public ulong? UlongNull { get; set; }

            [MapColumn] public decimal Decimal { get; set; } = 33;
            [MapColumn] public decimal? DecimalNull { get; set; }

            [MapColumn] public float Float { get; set; } = 33;
            [MapColumn] public float? FloatNull { get; set; }

            [MapColumn] public char Char { get; set; } = '1';
            [MapColumn] public char? CharNull { get; set; }


            [MapColumn] public bool Bool { get; set; }
            [MapColumn] public bool? BoolNull { get; set; }

            [MapColumn] public byte Byte { get; set; } = 33;
            [MapColumn] public byte? ByteNull { get; set; }

            [MapColumn] public byte[] Bytes { get; set; } = { 1, 2 };

            [MapColumn] public Guid Guid { get; set; } = System.Guid.NewGuid();
            [MapColumn] public Guid? GuidNull { get; set; }

            [MapColumn] public DateTime Date { get; set; }// = DateTime.Now;
            [MapColumn] public DateTime? DateNull { get; set; }

            [MapColumn] public double Double { get; set; }

            [MapColumn] public double? DoubleNull { get; set; }


        }

        class OrderFree
        {

            public Guid id { get; set; }

            public int number { get; set; }

            public string Text { get; set; }

            public int CustomerId { get; set; }

            public int? IntNull { get; set; }

            public long Long { get; set; }

            public long? LongNull { get; set; }

            public Int16 Short { get; set; }

            public Int16? ShortNull { get; set; }

            public decimal? Decimal { get; set; }

            public decimal? DecimalNull { get; set; }

            public Int16 Float { get; set; }

            public Int16? FloatNull { get; set; }

            public char Char { get; set; }

            public char? CharNull { get; set; }

             public bool? Bool { get; set; }

            public bool? BoolNull { get; set; }

            public bool Byte { get; set; }

            public bool? ByteNull { get; set; }

            public byte[] Bytes { get; set; }

            public Guid Guid { get; set; }

            public Guid? GuidNull { get; set; }

            public DateTime Date { get; set; }

            public DateTime? DateNull { get; set; }

            public double Double { get; set; }

            public double? DoubleNull { get; set; }
        }





    }













}
