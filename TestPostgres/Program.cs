using ORM_1_21_;
using ORM_1_21_.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
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
            //Execute.TotalTest();
            //Execute.TotalTestNull();


            ISession session = Configure.Session;
            session.TruncateTable<MyClass>();
            for (int j = 0; j < 10; j++)
            {
                session.Save(new MyClass() { Age = 10, Name = "name1" });
            }
            session.Save(new MyClass() { Age = 20, Name = "name1" });
            session.Save(new MyClass() { Age = 5, Name = "name1" });
            var r1 = session.Query<MyClass>().Where(a => a.Age > 0).FirstAsync().Result;
            var r2 = session.Query<MyClass>().Where(a => a.Age > 0).FirstAsync(s => s.Name.StartsWith("name")).Result;
            
            var r3 = session.Query<MyClass>().Where(a => a.Age > 0).FirstOrDefaultAsync().Result;
            var r4 = session.Query<MyClass>().Where(a => a.Age > 0).FirstOrDefaultAsync(s => s.Name.StartsWith("name")).Result;
            
            var r5 = session.Query<MyClass>().Where(a => a.Age > 0).LastAsync().Result;
            var r6 = session.Query<MyClass>().Where(a => a.Age > 0).LastAsync(s => s.Name.StartsWith("name")).Result;
            
            var r7 = session.Query<MyClass>().Where(a => a.Age > 0).LastOrDefaultAsync().Result;
            var r8 = session.Query<MyClass>().Where(a => a.Age > 0).LastOrDefaultAsync(s => s.Name.StartsWith("name")).Result;
            
            var r9 = session.Query<MyClass>().Where(a => a.Age == 20).SingleAsync().Result;
            var r10 = session.Query<MyClass>().Where(a => a.Age == 20).SingleAsync(s => s.Name.StartsWith("name")).Result;
            
            var r11 = session.Query<MyClass>().Where(a => a.Age == 20).SingleOrDefaultAsync().Result;
            var r12 = session.Query<MyClass>().Where(a => a.Age == 20).SingleOrDefaultAsync(s => s.Name.StartsWith("name")).Result;
            
            var r13 = session.Query<MyClass>().Where(a => a.Age == 20).AnyAsync().Result;
            var r14 = session.Query<MyClass>().Where(a => a.Age == 20).AnyAsync(s => s.Name.StartsWith("name")).Result;
            
            
            var r16 = session.Query<MyClass>().Where(a => a.Age == 20).AllAsync(s => s.Name.StartsWith("name")).Result;
            
            var r17 = session.Query<MyClass>().Where(a => a.Age == 20).CountAsync().Result;
            var r18 = session.Query<MyClass>().Where(a => a.Age == 20).CountAsync(s => s.Name.StartsWith("name")).Result;
            
            var r19 = session.Query<MyClass>().Where(a => a.Age == 20).LongCountAsync().Result;
            var r20 = session.Query<MyClass>().Where(a => a.Age == 20).LongCountAsync(s => s.Name.StartsWith("name")).Result;
            
            var r22 = session.Query<MyClass>().MinAsync(a => a.Age).Result;
            
            var r24 = session.Query<MyClass>().MaxAsync(a => a.Age).Result;
           
            
            var r25 = session.Query<MyClass>().SumAsync(a => a.Age).Result;
           

            var r27 = session.Query<MyClass>().SumAsync(a => a.Valdecimal).Result;
           
            var r30 = await session.Query<MyClass>().Where(a=>a.Age==5).UpdateAsync(a => new Dictionary<object,object>()
            {
                { a.Age, 122 }
            });
            var ass = session.Query<MyClassMysql>().Where(a => a.Age == 122).SplitQueryableAsync(3).Result;

            var sum = session.Query<MyClass>().Average(a => a.Valdecimal);
          
          





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
