using ORM_1_21_;
using System;
using System.Linq;
using System.Threading.Tasks;
using TestLibrary;

namespace TestPostgres
{
    internal class Program
    {


        static async Task Main(string[] args)
        {

            Starter.Run("Server=localhost;Port=5432;Database=testorm;User Id=postgres;Password=ion100312873;"
                , ProviderName.Postgresql);
            


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
             var ee=session.Querion<MyClass>().ToList();
          
            Console.WriteLine(ee);
            await Execute.RunExecute();
            Execute.RunThread();



            Console.ReadKey();
        }

       

      


    }

}
