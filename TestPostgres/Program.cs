using ORM_1_21_;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TestLibrary;

namespace TestPostgres
{
    internal class Program
    {
        private static ProviderName _providerName = ProviderName.Sqlite;

        static async Task Main(string[] args)
        {
            string s1 = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=audi124;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            string s2 = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=test;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

            switch (_providerName)
            {
                case ProviderName.MsSql:
                    Starter.Run(s1, _providerName);
                    break;
                case ProviderName.MySql:
                    Starter.Run("Server=localhost;Database=test;Uid=root;Pwd=12345;", _providerName);
                    break;
                case ProviderName.Postgresql:
                    Starter.Run("Server=localhost;Port=5432;Database=testorm;User Id=postgres;Password=ion100312873;", _providerName);
                    break;
                case ProviderName.Sqlite:
                    Starter.Run("Data Source=mydb.db;Version=3;BinaryGUID=False;", _providerName);//
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            //Execute.RunThread();
            //Console.ReadKey();
            Execute.NewExe(_providerName);

            ISession session = Configure.Session;
          


            
            var myClass = new MyClass
            {
                Age = 12,
                Name = "name",
                MyEnum = MyEnum.First,
               // MyTest = new MyTest { Name = "ass" },
                //Test23 =new List<MyTest>() { new MyTest(), new MyTest() }
            };
           
            session.Save(myClass);
            var ss = session.Querion<MyClass>().Where(a => string.IsNullOrEmpty(a.Description.Replace("''","'"))==true).ToList();
            // var so = session.Querion<MyClass>().OrderBy(a => a.Age).SingleOrDefault();
            //
            //
            //
            // List<MyClass> list = new List<MyClass>()
            // {
            //     Starter.GetMyClass(10, "MyName1"),
            //     Starter.GetMyClass(30, "MyName3"),
            //     Starter.GetMyClass(20, "MyName2"),
            // };
            // session.InsertBulk(list);
            //
            // var o = session.Querion<MyClass>().OrderBy(a => a.Age).FirstOrDefault();
            //
            // var asss = session.Querion<MyClass>().ToList();
            // var single = session.Get<MyClass>(myClass.Id);
            // string wass = myClass.Id.ToString();
            // var asas = session.Querion<MyClass>().Where(a => a.Id == new Guid(myClass.Id.ToString())).ToList();
            //
            // var count = session.Querion<MyClass>().Count();

            //var ass = session.Querion<MyClass>().Where(a => a.Name == new MyClass() { Name = "name" }.Name).ToList();
            //var ee = session.Querion<MyClass>().Where(a => a.Name.ToLower()==myClass.Name.Trim()).ToList();
            //
            // var eeeClasses = session.Querion<MyClass>().Where(a => a.DateTime.AddMilliseconds(-1).Minute==10).ToList();
            //
            //
            //Console.WriteLine(ee);
            //await Execute.RunExecute();
            //Execute.RunThread();



            Console.ReadKey();
        }

       

      


    }

}
