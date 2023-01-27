using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ORM_1_21_.Attribute;
using ORM_1_21_;

namespace TestPostgres
{
    internal class Program
    {
        static void Main(string[] args)
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
            using (var ses = Configure.GetSession())
            {
                var tr = ses.BeginTransaction();
                try
                {
                    
                    ses.Save(myClass);
                    var s = ses.Clone(myClass);
                    ses.Save(s);
                    var res = ses.Get<MyClass>(myClass.Id);
                    if(res != null)
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
            var ses1=Configure.GetSession();
            string t = ses1.TableName<MyClass>();
            var i=  Configure.GetSession().Querion<MyClass>().Where(a => a.Age == 12).
            Update(s => new Dictionary<object, object> { { s.Age, 100 },{s.Name,"simple"} });
            var @calss = ses1.GetList<MyClass>("age =100 order by age ").FirstOrDefault();
            
            var list = ses1.Querion<MyClass>().
                Where(a => (a.Age > 5||a.Name.StartsWith("ion100"))&&a.Name.Contains("100")).
                OrderBy(d=>d.Age).
                Select(f=>new {age=f.Age}).
                Limit(0,2).
                ToList();
            
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
            _ = new Configure("Server=localhost;Port=5432;Database=testorm;User Id=assa;Password=posgres;",
                ProviderName.Postgresql, path);
            using (var ses=Configure.GetSession())
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
    class MyClass:IValidateDal<MyClass>, IActionDal<MyClass>
    {
        [MapPrimaryKey("id", Generator.Native)]
        public int Id { get; set; }

        [MapColumnName("name")] 
        public string Name { get; set; }

        
        [MapColumnName("age")] [MapIndex] 
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

        public void AfterDelete(MyClass item)
        {
            //throw new NotImplementedException();
        }

        public void AfterInsert(MyClass item)
        {
            //throw new NotImplementedException();
        }

        public void AfterUpdate(MyClass item)
        {
            //throw new NotImplementedException();
        }

        public void BeforeDelete(MyClass item)
        {
            //throw new NotImplementedException();
        }

        public void BeforeInsert(MyClass item)
        {
            //throw new NotImplementedException();
        }

        public void BeforeUpdate(MyClass item)
        {
            //throw new NotImplementedException();
        }

        public void Validate(MyClass item)
        {
            //throw new NotImplementedException();
        }
    }

    class MyTest
    {
        public string Name { get; set; }
    }

    enum MyEnum
    {
        Def=0,First=1
    }
}
