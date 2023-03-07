using ORM_1_21_;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using ORM_1_21_.Extensions;
using TestLibrary;


namespace TestPostgres
{
    internal class Program
    {
        private const ProviderName ProviderName = ORM_1_21_.ProviderName.MsSql;

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
            //Execute.TotalTest();
            //Execute.TotalTestNull();


            ISession session = Configure.Session;
            session.TruncateTable<MyClass>();
            for (int j = 0; j < 10; j++)
            {
                session.Save(new MyClass() { Age = 10 * j, Name = "name" + j, DateTime = DateTime.Now, Valfloat = 123.3f});
                session.Save(new MyClass() { Age = 10 * j, Name = "name" + j, DateTime = DateTime.Now });
            }

            //ssion.Query<MyClass>().OrderBy(s=>s.Age).ToList();
            //
            //"select {session.ColumnName<MyClass>(ss => ss.Age)} from {session.TableName<MyClass>()} where  " +
            //ssion.ColumnName<MyClass>(ss => ss.Name)}=@1 and " +
            //ssion.ColumnName<MyClass>(ss => ss.Valdecimal)} = '123.3' and" +
            //ession.ColumnName<MyClass>(d => d.Age)} = @3";
            //int)session.ExecuteScalar("select [age] from [my_class5] where  [name]=@1 and [test5] = '123.3' and [age] = '10'", "name1");
            //= (int)session.ExecuteScalar(sql,
            //ameter("@1", "name1"),
            //ameter("@2", new decimal(123.3),DbType.Double),
            //ameter("@3", 10));

            

           
            var ee = session.FreeSql<MyClass>("select * from my_class5 where test6 = @1",new Parameter("@1",123.3f));
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
