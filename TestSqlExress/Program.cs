using ORM_1_21_;
using System;
using System.Linq;
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

            Starter.Run(s1, ProviderName.MsSql);

            //var myClass = new MyClass
            //{
            //    Age = 12,
            //    Name = "simple",
            //    MyEnum = MyEnum.First,
            //    MyTest = new MyTest { Name = "ass" },
            //    Test23 = { new MyTest(), new MyTest() }
            //};
            //ISession session = Configure.Session;
            //session.Save(myClass);
            //var ee = Configure.Session.Querion<MyClass>().Where(a => a.Name.Length == 6).ToList();
            //var aa = session.Querion<MyClass>().Select(a => new { ass = a.Name.Length }).ToList();

            Execute.NewExe(ProviderName.MsSql);

            Console.ReadKey();
           

        }
    }

   
}
