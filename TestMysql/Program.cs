using ORM_1_21_;
using ORM_1_21_.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace TestMysql
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Starter.Run();

            new Thread(async () =>
            {
                while (true)
                {
                    var ses = Configure.Session;
                    var ts = ses.BeginTransaction();
                    MyClass c = new MyClass();
                    ses.Save(c);

                    var s = Configure.Session.Querion<MyClass>().Where(a => a.Age != -1);
                    var ees = await s.ToListAsync();
                    Console.WriteLine("1 -- " + ees.Count());
                    ts.Commit();

                }

            }).Start();

            new Thread(() =>
            {
                while (true)
                {
                    var ses = Configure.Session;
                    var ts = ses.BeginTransaction();
                    MyClass c = new MyClass();
                    ses.Save(c);

                    var s = Configure.Session.Querion<MyClass>().Where(a => a.Age != -1).ToList();
                    Console.WriteLine("2 -- " + s.Count());
                    ts.Commit();
                }

            }).Start();
            Console.ReadKey();
            var tss = Configure.Session.GetTableColumns(Configure.Session.TableName<MyClass>() ).ToList();
            MyClass myClass = new MyClass()
            {
                Age = 12,
                Description = "simple",
                Name = "ion100FROMfromFrom ass",
                DateTime = DateTime.Now
            };
            Configure.Session.Save(myClass);
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
            Configure.Session.InsertBulk(classes);
            var sss = Configure.Session

                .ExecuteScalar("SELECT table_name FROM information_schema.tables WHERE table_schema = 'test';");
            var i = Configure.Session.Querion<MyClass>().Where(a => a.Age == 12).
                Update(s => new Dictionary<object, object> { { s.Age, 100 }, { s.Name, "simple" } });
            var @calss = Configure.Session.GetList<MyClass>("age =100 order by age ").FirstOrDefault();
            var list = Configure.Session.Querion<MyClass>().Where(a => a.Age > 5).ToList();
            var list1 = Configure.Session
                .FreeSql<MyClass>($"select * from {Configure.Session.TableName<MyClass>()}");
            var list2 = Configure.Session.Querion<MyClass>().Select(a => new { ageCore = a.Age }).ToList();

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
            _ = new Configure("Server=localhost;Database=test;Uid=root;Pwd=12345;",
                ProviderName.MySql, path);
            using (var ses = Configure.Session)
            {
                if (ses.TableExists<MyClass>() == true)
                    ses.DropTable<MyClass>();
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
        [MapColumnName("age")][MapIndex] public int Age { get; set; }
        [MapColumnName("age1")][MapIndex] public int? Age1 { get; set; }

        [MapColumnName("desc")]
        [MapColumnType("TEXT")]
        public string Description { get; set; }

        [MapColumnName("enum")] public MyEnum MyEnum { get; set; } = MyEnum.First;
        [MapColumnName("date")] public DateTime? DateTime { get; set; }
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
