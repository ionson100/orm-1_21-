using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ORM_1_21_;
using ORM_1_21_.Attribute;

namespace TestSqlExress
{
    class Program
    {

        static void Main(string[] args)
        {
            Starter.Run();
            MyClass myClass = new MyClass()
            {
                Age = 12,
                Description = "simple",
                Name = "ion100FROMfromFrom ass"
            };
            Configure.GetSession().Save(myClass);
            List<MyClass> classes = new List<MyClass>()
            {
                new MyClass()
                {
                    Age = 12,
                    Description = "simple",
                    Name = "ion100FROMfromFrom ass",
                    DateTime = DateTime.Now
                },
                new MyClass()
                {
                    Age = 12,
                    Description = "simple",
                    Name = "ion100FROMfromFrom ass",
                    DateTime = DateTime.Now
                },
                new MyClass()
                {
                    Age = 12,
                    Description = "simple",
                    Name = "ion100FROMfromFrom ass",
                    DateTime = DateTime.Now
                }
            };
            Configure.GetSession().InsertBulk(classes);
            var list = Configure.GetSession().Querion<MyClass>().Where(a => a.Age > 5).ToList();
            var list1 = Configure.GetSession()
                .FreeSql<MyClass>($"select * from {Configure.GetSession().TableName<MyClass>()}");
            var list2 = Configure.GetSession().Querion<MyClass>().Select(a => new { ageCore = a.Age }).ToList();
        }
    }

    static class Starter
    {
        public static void Run()
        {

            string path = null;
#if DEBUG
            path = "SqlLog.txt";
#endif
            _ = new Configure("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=audi124;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False",
                ProviderName.MsSql, path);
            using (var ses = Configure.GetSession())
            {
                if (ses.TableExists<MyClass>())
                {
                    ses.DropTable<MyClass>();
                }
                if (ses.TableExists<MyClass>() == false)
                {
                    ses.TableCreate<MyClass>();
                }
            }



        }
    }

    [MapTableName("my_class")]
    class MyClass
    {
        [MapPrimaryKey("id", Generator.Assigned)]
        public Guid Id { get; set; } = Guid.NewGuid();

        [MapColumnName("name")] public string Name { get; set; }
        [MapColumnName("age")] [MapIndex] public int Age { get; set; }

        [MapColumnName("desc")]
        [MapColumnType("TEXT")]
        public string Description { get; set; }

        [MapColumnName("age1")] [MapIndex] public int? Age1 { get; set; }


        [MapColumnName("price")]
        public decimal? Price { get; set; }

        [MapColumnName("enum")] public MyEnum MyEnum { get; set; } = MyEnum.First;
        [MapColumnName("date")] public DateTime DateTime { get; set; } = DateTime.Now;
        [MapColumnName("test")]
        public List<Test23> Test23 { get; set; } = new List<Test23>() { new Test23() { Name = "simple" }
            };



    }

    class Test23
    {
        public string Name { get; set; }
    }

    enum MyEnum
    {
        Def = 0, First = 1
    }
}
