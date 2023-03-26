using ORM_1_21_;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using ORM_1_21_.Extensions;
using TestLibrary;
using static System.Collections.Specialized.BitVector32;


namespace TestPostgres
{
    internal class Program
    {
        private const ProviderName ProviderName = ORM_1_21_.ProviderName.PostgreSql;

        static async Task Main(string[] args)
        {

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
            //Execute.RunThread();
            //Console.ReadKey();
            //Console.ReadKey();
           // Execute.TotalTest();
           // //Execute.TotalTestAsync();
           // Execute.TestNativeInsert();
           // Execute.TestAssignetInsert();
           // Execute2.TestTimeStamp();
           // await Execute3.TotalTestAsync();
           
            Stopwatch stopwatch = new Stopwatch();
            //засекаем время начала операции
            var dt = DateTime.Now;
            using (ISession session = Configure.Session)
            {
                try
                {
                    using (var tr = await session.BeginTransactionAsync())
                    {
                        await session.SaveAsync(new MyClass(1));
                        await session.SaveAsync(new MyClass(1));
                        await tr.CommitAsync();
                        await tr.CommitAsync();
                    }
                }
                catch (Exception e)
                {
                    Execute.Log(38, true, e.Message);
                }
            }

          
        }

        static int GetInt()
        {
            return 10;
        }






    }
    [MapTableName("test_list")]
    class TestList
    {
        [MapPrimaryKey("id", Generator.Native)]
        public long Id { get; set; }




        [MapColumnName("list")]
        public List<int> List { get; set; } = new List<int>() { 1, 2, 3 };

        [MapColumnName("testuser")]
        public TestUser TestUser { get; set; } = new TestUser { Id = 23, Name = "23" };




        [MapNotInsertUpdate]
        [MapColumnType("rowversion")]
        [MapDefaultValue(" ")]
        [MapColumnName("ts")]
        public byte[] Date { get; set; } = new byte[] { 0 };

    }



    [MapTableName("image_my")]
    class MyImage
    {
        [MapPrimaryKey("id", Generator.Native)]
        public long Id { get; set; }
        [MapColumnName("image")]
        public Image Image { get; set; }
    }


}
