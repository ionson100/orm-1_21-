using ORM_1_21_;
using ORM_1_21_.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using TestLibrary;


namespace TestPostgres
{
    internal class Program
    {
        private const ProviderName ProviderName = ORM_1_21_.ProviderName.MsSql;

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
                case ProviderName.Postgresql:
                    Starter.Run(ConnectionStrings.Postgesql, ProviderName);
                    break;
                case ProviderName.Sqlite:
                    Starter.Run(ConnectionStrings.Sqlite, ProviderName);//
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            //Execute.RunThread();
            //Console.ReadKey();
            //Console.ReadKey();
            Execute.TotalTest();
            Execute.TotalTestNull();
            Execute.TestNativeInsert();
            Execute.TestAssignetInsert();
            
            Execute2.TestTimeStamp();


            ISession session = Configure.Session;
            session.TruncateTable<MyClass>();
            for (int j = 0; j < 10; j++)
            {
                session.Save(new MyClass() { Age = 10, Name = "name1" });
            }
            session.Save(new MyClass() { Age = 20, Name = "name1" });
            session.Save(new MyClass() { Age = 5, Name = "name1" });


            var list = session.Query<TestList>().ToList();
            Console.ReadKey();
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


        [MapColumnName("mytest")]
        public MyTest MyTest { get; set; } = new MyTest() { Name = "123" };

        [MapColumnName("list")]
        public List<int> List { get; set; } = new List<int>() { 1, 2, 3 };

        [MapColumnName("testuser")]
        public TestUser TestUser { get; set; } = new TestUser { Id = 23, Name = "23" };



       
        [MapNotInsertUpdate]
        [MapColumnType("rowversion")]   
        [MapDefaultValue(" ")]
        [MapColumnName("ts")]
        public byte[] Date { get; set; } =new byte[]{0};

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
