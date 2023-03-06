using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using ORM_1_21_;
using TestLibrary;


namespace TestPostgres
{
    internal class Program
    {
        private const ProviderName ProviderName = ORM_1_21_.ProviderName.Postgresql;

        static async Task Main(string[] args)
        {
            string s1 = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=audi124;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            string s2 = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=test;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

            switch (ProviderName)
            {
                case ProviderName.MsSql:
                    Starter.Run(s1, ProviderName);
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
            
            ISession session = Configure.Session;
            if (session.TableExists<TestList>())
            {
                session.DropTable<TestList>();
            }
            session.TableCreate<TestList>();
            session.Save(new TestList());
            var rr=session.Query<TestList>().First();
            rr.MyTest.Name="sasasas";
            session.Save(rr);
            var errrr = session.Query<TestList>().ToList();






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
        public MyTest MyTest { get; set; } = new MyTest(){Name = "123"};

        [MapColumnName("list")]
        public List<int> List { get; set; } = new List<int>() { 1, 2, 3 };

        [MapColumnName("testuser")]
        public TestUser TestUser { get; set; } = new TestUser { Id = 23, Name = "23" };

    }

   

    [MapTableName("image_my")]
    class MyImage
    {
        [MapPrimaryKey("id",Generator.Native)]
        public long Id{ get; set; }
        [MapColumnName("image")]
        public Image Image { get; set; }
    }

   
}
