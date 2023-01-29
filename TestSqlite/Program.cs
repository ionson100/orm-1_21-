using ORM_1_21_;
using ORM_1_21_.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TestSqlite
{

    internal class Program
    {
        static void Main(string[] args)
        {
            Starter.Run();
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
                   Age = 121,
                   Description = "simple",
                   Name = "ion100FROMfromFrom ass",
                   DateTime = DateTime.Now
               },
               new MyClass()
               {
                   Age = 121,
                   Description = "simple",
                   Name = "ion100FROMfromFrom ass",
                   DateTime = DateTime.Now
               }
           };
            Configure.Session.InsertBulk(classes);
            var i = Configure.Session.Querion<MyClass>().Where(a => a.Age == 12).
                Update(s => new Dictionary<object, object> { { s.Age, 100 }, { s.Name, "simple" } });
            var @calss = Configure.Session.GetList<MyClass>("age =100 order by age ").FirstOrDefault();
            var eee = Configure.Session.ExecuteScalar("SELECT name FROM sqlite_temp_master WHERE type='table';");
            var list = Configure.Session.Querion<MyClass>().Where(a => a.Age > 5).ToList();
            var list1 = Configure.Session
                .FreeSql<MyClass>($"select * from {Configure.Session.TableName<MyClass>()}");
            var list2 = Configure.Session.Querion<MyClass>().Select(a => new { ageCore = a.Age, name = a.Name }).ToList();
            var list3 = Configure.Session.Querion<MyClass>().
               Where(a => (a.Age > 5 || a.Name.StartsWith("ion100")) && a.Name.Contains("100")).
               OrderBy(d => d.Age).
               Select(f => new { age = f.Age }).
               Limit(0, 2).
               ToList();

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
            _ = new Configure("Data Source=mydb.db;Version=3",
                ProviderName.Sqlite, path);
            using (var ses = Configure.Session)
            {
                if (ses.TableExists<MyClass>())
                    ses.DropTable<MyClass>();
                if (!ses.TableExists<MyClass>())
                {
                    ses.TableCreate<MyClass>();
                }
            }



        }
    }

    [MapTableName("my_class")]
    class MyClass
    {
        [MapPrimaryKey("id", Generator.Native)]
        public int Id { get; set; }

        [MapColumnName("name")] public string Name { get; set; }
        [MapColumnName("age")][MapIndex] public int Age { get; set; }
        [MapColumnName("age1")][MapIndex] public int? Age1 { get; set; }

        [MapColumnName("desc")]
        [MapColumnType("TEXT")]
        public string Description { get; set; }
        [MapColumnName("price")]
        public decimal? Price { get; set; }

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
