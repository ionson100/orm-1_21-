using ORM_1_21_;
using ORM_1_21_.Attribute;
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
            string s1 = "Server=localhost;Database=test;Uid=root;Pwd=12345;";
            Starter.Run(s1, ProviderName.MySql);



            var myClass = new MyClass
            {
                Age = 12,
                Description = "simple",
                MyEnum = MyEnum.First,
                MyTest = new MyTest { Name = "ass" },
                Test23 = { new MyTest(), new MyTest() }
            };
            ISession session = Configure.Session;
            session.Save(myClass);
            var ee = session.Querion<MyClass>().ToList();
            Console.WriteLine(ee);
            await Execute.RunExecute();

            Console.ReadKey();
            Console.ReadKey();


        }
    }

   

}
