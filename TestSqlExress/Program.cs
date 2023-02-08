using ORM_1_21_;
using ORM_1_21_.Attribute;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TestSqlExress
{
    class Program
    {

        static async Task Main(string[] args)
        {
            Starter.Run();

            new Thread( () =>
            {
                while (true)
                {
                    try
                    {
                        var ses = Configure.Session;
                        var ts = ses.BeginTransaction(IsolationLevel.ReadCommitted);
                        MyClass c = new MyClass();
                        ses.Save(c);

                        var s = ses.Querion<MyClass>().Where(a => a.Age != -1);
                        var ees = s.ToList();
                        Console.WriteLine("1 -- " + ees.Count());
                        ts.Commit();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                   

                }

            }).Start();


            new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        var ses = Configure.Session;
                        var ts = ses.BeginTransaction(IsolationLevel.ReadCommitted);
                        MyClass c = new MyClass();
                        ses.Save(c);

                        var s = ses.Querion<MyClass>().Where(a => a.Age != -1).ToList();
                        Console.WriteLine("2 -- " + s.Count());
                        ts.Commit();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                  
                }

            }).Start();
            Console.ReadKey();
            var sas = Configure.Session.GetTableColumns(Configure.Session.TableName<MyClass>()).ToList();
            MyClass myClass = new MyClass()
            {
                Age = 12,
                Description = "simple",
                Name = "ion100FROMfromFrom ass"
            };
            Configure.Session.Save(myClass);
            List<MyClass> classes = new List<MyClass>()
            {
                new MyClass()
                {
                    Age = 121,
                    Description = "simple",
                    Name = "ion100FROMfromFrom ass",
                    DateTime = DateTime.Now
                },
                new MyClass()
                {
                    Age = 122,
                    Description = "simple",
                    Name = "ion100FROMfromFrom ass",
                    DateTime = DateTime.Now
                },
                new MyClass()
                {
                    Age = 123,
                    Description = "simple",
                    Name = "ion100FROMfromFrom ass",
                    DateTime = DateTime.Now
                }
            };
            Configure.Session.InsertBulk(classes);
            //var iResUpdate = Configure.GetSession().Querion<MyClass>().Where(a => a.Age == 12).
            //    Update(s => new Dictionary<object, object> { { s.Age, 100 }, { s.Name, "oldBoy" } });
            //var @calss = Configure.GetSession().GetList<MyClass>("age =100 order by age ").FirstOrDefault();
            //var list22 = Configure.GetSession().Querion<MyClass>().Select(a => new { ageCore = a.Age, name = a.Name }).ToList();
            var list = Configure.Session.Querion<MyClass>();
            var resList = await list.ToListAsync();

            var countDelete = Configure.Session.Querion<MyClass>().Where(s => s.Age == 100).Delete();


            var list2 = Configure.Session.Querion<MyClass>().Select(a => new { ageCore = a.Age }).ToList();


        }
    }

    class Assa
    {
        public int AgeMy { get; set; }
    }

    static class Starter
    {
        //Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=audi124;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"
        //Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=test;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False
        public static void Run()
        {

            string path = null;
#if DEBUG
            path = "SqlLog.txt";
#endif
            _ = new Configure("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=audi124;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False",
                ProviderName.MsSql, path,true);
            using (var ses = Configure.Session)
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
        [MapColumnName("age")][MapIndex] public int Age { get; set; }

        [MapColumnName("desc")]
        [MapColumnType("TEXT")]
        public string Description { get; set; }

        [MapColumnName("age1")][MapIndex] public int? Age1 { get; set; }


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
