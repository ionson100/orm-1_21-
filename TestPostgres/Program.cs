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
                Age = 12,
                Description = "simple",
                Name = "ion100FROMfromFrom ass"
            };
            Configure.GetSession().Save(myClass);
            var list = Configure.GetSession().Querion<MyClass>().Where(a => a.Age > 5).ToList();
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
            using (var ses=Configure.GetSession())
            {
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

        [MapColumnName("enum")] public MyEnum MyEnum { get; set; } = MyEnum.First;
        [MapColumnName("date")] public DateTime DateTime { get; set; } = DateTime.Now;
        [MapColumnName("test")] public List<Test23> Test23 { get; set; } = new List<Test23>() { new Test23() { Name = "simple" }
    };



}

    class Test23
    {
        public string Name { get; set; }
    }

    enum MyEnum
    {
        Def=0,First=1
    }
}
