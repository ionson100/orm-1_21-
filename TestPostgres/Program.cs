using ORM_1_21_;
using ORM_1_21_.Attribute;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Npgsql;

namespace TestPostgres
{
    internal class Program
    {


        static async Task Main(string[] args)
        {
            Starter.Run();

            MyClass myClass = new MyClass()
            {
                Age = 11,
                Description = "simple",
                Name = "ion100"
            };
            List<MyClass> list1 = new List<MyClass>
            {
                new MyClass()
                {
                Age = 12,
                Description = "simple",
                Name = "ion100"
                },
                new MyClass()
                {
                    Age = 13,
                    Description = "simple",
                    Name = "ion100"
                }, new MyClass()
                {
                    Age = 13,
                    Description = "simple",
                    Name = "ion100"
                }
            };
            using (var ses = Configure.Session)
            {
                var tra = ses.BeginTransaction();
                try
                {
                    
                    ses.Save(myClass);
                   
                    myClass.Description = "ss";
                    ses.Save(myClass);
                  
                    
                    

                    ses.InsertBulk(list1);
                    tra.Commit();
                }
                catch (Exception e)
                {
                    tra.Rollback();
                    Console.WriteLine(e);
                    throw;
                }
            }
            ISession session = Configure.Session;
            var tr = session.BeginTransaction();
            try
            {
                var ee12 = await session.Querion<MyClass>().Distinct().ToListAsync();
                session.Save(new MyClass { Age = 6, DateTime = DateTime.Now, Description = "bla" });
                tr.Commit();
            }
            catch (Exception)
            {
                tr.Rollback();
                throw;
            }
            finally
            {
                session.Dispose();
            }

            var sd5 = Configure.Session.GetListOtherBase<MyClass>(new MyDb(), "select * from my_class where name<> @1","100");

            foreach (var r in Configure.Session.FreeSql<MyEnum>("select enum as enum1 from my_class"))
            {
                Console.WriteLine($" enum={r}");
            }
             foreach (var r in Configure.Session.FreeSql<dynamic>("select enum as enum1,age from my_class"))
                 Console.WriteLine($" enum1={r.enum1} age={r.age}");
            
            
             foreach (var r in Configure.Session.FreeSql<MyClassTemp>("select enum as enum1, age, id from my_class"))
                 Console.WriteLine($"{r}");
            
             foreach (var r in TempSql(new { enum1 = 1, age = 2 }))
                 Console.WriteLine($"{r}");
            
            
             var ses1 = Configure.Session;
             string t = ses1.TableName<MyClass>();
             var i0 = Configure.Session.Querion<MyClass>().Where(a => a.Age == 12).ToList();
             var i = Configure.Session.Querion<MyClass>().Where(a => a.Age == 12).
             Update(s => new Dictionary<object, object> { { s.Age, 100 }, { s.Name, "simple" } });
             var @calss = ses1.GetList<MyClass>("age =100 order by age ").FirstOrDefault();
            
             var eri = Configure.Session.Querion<MyClass>().ToList();
             var list = ses1.Querion<MyClass>().
                 Where(a => (a.Age > 5 || a.Name.StartsWith("ion100")) && a.Name.Contains("100")).
                 OrderBy(d => d.Age).
                 Select(f => f.Age).
                 Limit(0, 2);
             await list.ToListAsync().ContinueWith(r =>
             {
                 Console.WriteLine(r.Result.Count);
             });
             var ee = Configure.Session.Querion<MyClass>().DistinctCore(s => new { ass = s.Age, name = s.Name }).ToList();
             foreach (var s in ee)
             {
            
             }
             try
             {
                 await Configure.Session.Querion<MyClass>().Where(s => !s.Name.Contains("sdFROMUU")).SetTimeOut(20).GroupBy(f => f.Age).ToListAsync().ContinueWith(f =>
                 {
                     Console.WriteLine(f.Result.Count());
                 });
            
             }
             catch (Exception ex)
             {
                 Console.WriteLine(ex);
             }
            
            
             var rs = await ses1.Querion<MyClass>().Where(a => a.Name != null).Select(d => new { sd = d.Age, name = d.Name }).ToListAsync();
             foreach (var r in rs)
             {
                 Console.WriteLine(r);
             }
             var val1 = ses1.FreeSql<MyClass>($"select * from {ses1.TableName<MyClass>()} where \"name\" LIKE CONCAT(@p1,'%')",
                     new Parameter("@p1", "ion100")).ToList();
             var isP = ses1.IsPersistent(val1[0]);
            
             var dataTable = ses1.GetDataTable($"select * from {ses1.TableName<MyClass>()} ");
             var coutn = dataTable.Rows.Count;
        }

        static IEnumerable<T> TempSql<T>(T t)
        {
            return Configure.Session.FreeSql<T>("select enum as enum1,age from my_class");
        }


    }

    class MyDb:IOtherBaseCommandFactory
    {
        public IDbCommand GetDbCommand()
        {
            
            return new MySqlCommand("SET character_set_results=utf8;", new MySqlConnection("Server=localhost;Database=test;Uid=root;Pwd=12345;"));
        }
    }
    [MapTableName("nomap")]
    class MyClassTemp
    {
        [MapColumnName("id")]
        public Guid Id { get; set; } = Guid.NewGuid();
        [MapColumnName("enum1")]
        public MyEnum MyEnum { get; set; }
        [MapColumnName("age")]
        public int Age { get; set; }
        public override string ToString()
        {
            return $" Id={Id}, MyEnum={MyEnum}, Age={Age}";
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
            _ = new Configure("Server=localhost;Port=5432;Database=testorm;User Id=postgres;Password=ion100312873;",
                ProviderName.Postgresql, path);
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
    class MyClass : IValidateDal<MyClass>, IActionDal<MyClass>
    {
        [MapPrimaryKey("id", Generator.Assigned)]
        public Guid Id { get; set; } = Guid.NewGuid();

        [MapColumnName("name")]
        public string Name { get; set; }


        [MapColumnName("age")]
        [MapIndex]
        public int Age { get; set; }

        [MapColumnName("desc")]
        [MapColumnType("TEXT NULL")]
        public string Description { get; set; }

        [MapColumnName("enum")]
        public MyEnum MyEnum { get; set; } = MyEnum.First;

        [MapColumnName("date")]
        public DateTime DateTime { get; set; } = DateTime.Now;

        [MapColumnName("test")]
        public List<MyTest> Test23 { get; set; } = new List<MyTest>() { new MyTest() { Name = "simple" }
    };

        void IActionDal<MyClass>.AfterDelete(MyClass item)
        {

        }

        void IActionDal<MyClass>.AfterInsert(MyClass item)
        {

        }

        void IActionDal<MyClass>.AfterUpdate(MyClass item)
        {

        }

        void IActionDal<MyClass>.BeforeDelete(MyClass item)
        {

        }

        void IActionDal<MyClass>.BeforeInsert(MyClass item)
        {

        }

        void IActionDal<MyClass>.BeforeUpdate(MyClass item)
        {

        }

        void IValidateDal<MyClass>.Validate(MyClass item)
        {

        }
    }

    class MyTest
    {
        public string Name { get; set; }
    }

    enum MyEnum
    {
        Def = 0, First = 1
    }
}
