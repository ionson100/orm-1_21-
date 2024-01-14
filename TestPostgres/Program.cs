
using ORM_1_21_;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TestLibrary;

//DESKTOP - GTR4O03
namespace TestPostgres
{
    internal class Program
    {
        private static ProviderName ProviderNamee = ORM_1_21_.ProviderName.SqLite;

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
                    Starter.Run(ConnectionStrings.Sqlite, ProviderNamee); //
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
            
            ExeGeo.Run();
            Console.WriteLine("finish");
            Console.ReadKey();






            ISession session = Configure.Session;
            {

               



            }

        }

        [MapTable("Order")]
        public class Order1
        {
            [MapPrimaryKey("id", Generator.Assigned)]public Guid Id { get; set; } = System.Guid.NewGuid();
            
            [MapColumn("number")] public int Number { get; set; }
            // [MapColumn] public byte[] Bytes { get; set; } = { 1, 2 };
           [MapColumn] public string Text { get; set; } = "рвыроврывор";
           //[MapColumn] public int CustomerId { get; set; }
           //
           //[MapColumn] public int? IntNull { get; set; }
           //
           //
           //
           //[MapColumn] public long Long { get; set; } = 33;
           //[MapColumn] public long? LongNull { get; set; }
           //
           //[MapColumn] public short Short { get; set; } = 33;
           //[MapColumn] public short? ShortNull { get; set; }
           //
           //[MapColumn] public decimal Decimal { get; set; } = 33;
           //[MapColumn] public decimal? DecimalNull { get; set; }
           //
           //[MapColumn] public float Float { get; set; } = 33;
           //[MapColumn] public float? FloatNull { get; set; }
           //
           //[MapColumn] public char Char { get; set; } = '1';
           //[MapColumn] public char? CharNull { get; set; }
           ///
           ///
           //[MapColumn] public bool Bool { get; set; }
           //[MapColumn] public bool? BoolNull { get; set; }
           //
           //[MapColumn] public byte Byte { get; set; } = 33;
           //[MapColumn] public byte? ByteNull { get; set; }



           // [MapColumn] public Guid Guid { get; set; } = System.Guid.NewGuid();
           // [MapColumn] public Guid? GuidNull { get; set; }
           //
           [MapColumn] public DateTime? Date { get; set; } = DateTime.Now;
           // [MapColumn] public DateTime? DateNull { get; set; }
           //
           //
           //
           //[MapColumn] public double? DoubleNull { get; set; }
           //[MapColumn] public double Double { get; set; }

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
