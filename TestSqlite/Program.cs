using ORM_1_21_;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestLibrary;

namespace TestSqlite
{

    internal class Program
    {
        static async Task Main(string[] args)
        {
           
            Starter.Run("Data Source=mydb.db;Version=3", ProviderName.Sqlite);

           // //Execute.NewExe(ProviderName.Sqlite);
           // var myClass = new MyClass
           // {
           //     Age = 12,
           //     Name = "dsimpled ",
           //     MyEnum = MyEnum.First,
           //     // MyTest = new MyTest { Name = "ass" },
           //     //Test23 =new List<MyTest>() { new MyTest(), new MyTest() }
           // };
           // ISession session = Configure.Session;
           // session.Save(myClass);
           // var ass = session.Querion<MyClass>().Count();
           // var ee = session.Querion<MyClass>().Select(a => new {aaa=a.Name.TrimEnd(' ','d')}).ToList();
             Execute.NewExe(ProviderName.MySql);
            Console.ReadKey();



            Console.ReadKey();

        }
    }

   

}
