using ORM_1_21_;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using ORM_1_21_.Extensions;
using TestLibrary;
using System.Diagnostics.Metrics;


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
                    Starter.Run(MyDbMySql.s2, ProviderName);
                    break;
                case ProviderName.MySql:
                    Starter.Run("Server=localhost;Database=test;Uid=root;Pwd=12345;", ProviderName);
                    break;
                case ProviderName.Postgresql:
                    Starter.Run("Server=localhost;Port=5432;Database=testorm;User Id=postgres;Password=ion100312873;", ProviderName);
                    break;
                case ProviderName.Sqlite:
                    Starter.Run("Data Source=mydb.db;Version=3;BinaryGUID=False;", ProviderName);//
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            //Execute.RunThread();
            //Console.ReadKey();
            //Console.ReadKey();
            Execute.TotalTest();
            Execute.TotalTestNull();


            ISession session = Configure.Session;
            session.TruncateTable<MyClass>();
            for (int j = 0; j < 10; j++)
            {
                session.Save(new MyClass() { Age =  j, Name = "name" + j, DateTime = DateTime.Now, Valfloat = 123.3f});
               
            }

            var res = session.Query<MyClass>().OrderByDescending(a => a.Age).Limit(0, 1).ToList();

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
