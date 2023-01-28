using ORM_1_21_;
using ORM_1_21_.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                }
            };
            using (var ses = Configure.Session)
            {
                var tr = ses.BeginTransaction();
                try
                {
                    bool d=ses.IsPersistent(myClass);
                    ses.Save(myClass);
                    bool d2 = ses.IsPersistent(myClass);
                    myClass.Description = "ss";
                    ses.Save(myClass);
                    var s = ses.Clone(myClass);
                    ses.Save(s);
                    var res = ses.Get<MyClass>(myClass.Id);
                    if (res != null)
                    {
                        res.Age = 100;
                        ses.Save(res);
                        ses.Delete(res);
                    }

                    ses.InsertBulk(list1);
                    tr.Commit();
                }
                catch (Exception e)
                {
                    tr.Rollback();
                    Console.WriteLine(e);
                    throw;
                }
            }
            var ses1 = Configure.Session;
            //string t = ses1.TableName<MyClass>();
            //var i0 = Configure.Session.Querion<MyClass>().Where(a => a.Age == 12).ToList();
            //var i = Configure.Session.Querion<MyClass>().Where(a => a.Age == 12).
            //Update(s => new Dictionary<object, object> { { s.Age, 100 }, { s.Name, "simple" } });
            //var @calss = ses1.GetList<MyClass>("age =100 order by age ").FirstOrDefault();
            //
            //var eri = Configure.Session.Querion<MyClass>().ToList();
            //var list = ses1.Querion<MyClass>().
            //    Where(a => (a.Age > 5 || a.Name.StartsWith("ion100")) && a.Name.Contains("100")).
            //    OrderBy(d => d.Age).
            //    Select(f => f.Age).
            //    Limit(0, 2);
            //await list.ToListAsync().ContinueWith(r =>
            //{
            //    Console.WriteLine(r.Result.Count);
            //});
            try
            {
                await Configure.Session.Querion<MyClass>().Where(s=>s.Name!=null).GroupBy(f=>f.Age).ToListAsync().ContinueWith(f =>
                {
                    Console.WriteLine(f.Result.Count());
                });

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }


            var myClassCore = ses1.Querion<MyClass>().Where(a => a.Name != null).First();
            var val1 = ses1.FreeSql<MyClass>($"select * from {ses1.TableName<MyClass>()} where \"name\" LIKE CONCAT(@p1,'%')",
                    new Parameter("@p1", "ion100")).ToList();
            var isP = ses1.IsPersistent(val1[0]);

            var dataTable = ses1.GetDataTable($"select * from {ses1.TableName<MyClass>()} ");
            var coutn = dataTable.Rows.Count;
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
        [MapPrimaryKey("id", Generator.Native)]
        public int Id { get; set; }

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
