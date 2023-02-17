using ORM_1_21_;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TestLibrary;

namespace TestMysql
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
           
            Starter.Run("Server=localhost;Database=test;Uid=root;Pwd=12345;", ProviderName.MySql);
            Execute.NewExe(ProviderName.MySql);
            //var myClass = new MyClass
            //{
            //    Age = 12,
            //    Name = "simpled ",
            //    MyEnum = MyEnum.First,
            //    // MyTest = new MyTest { Name = "ass" },
            //    //Test23 =new List<MyTest>() { new MyTest(), new MyTest() }
            //};
            //ISession session = Configure.Session;
            //session.Save(myClass);
            //var ass = session.Querion<MyClass>().Count();
            //var ee = session.Querion<MyClass>().Where(a => a.Name.TrimEnd('d',' ') == "simple").ToList();
            // Execute.NewExe(ProviderName.MySql);
            Console.ReadKey();


        }
    }

   

}
