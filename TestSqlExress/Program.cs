using ORM_1_21_;
using ORM_1_21_.Attribute;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TestLibrary;

namespace TestSqlExress
{
    class Program
    {

        static async Task Main(string[] args)
        {
            string s1 = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=audi124;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            string s2 = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=test;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

            Starter.Run(s2, ProviderName.MsSql);



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
