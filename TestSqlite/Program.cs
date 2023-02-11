using ORM_1_21_;
using ORM_1_21_.Attribute;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TestLibrary;

namespace TestSqlite
{

    internal class Program
    {
        static async Task Main(string[] args)
        {
            string s1 = "Data Source=mydb.db;Version=3";
            Starter.Run(s1, ProviderName.Sqlite);



            var myClass = new MyClass
            {
                Age = 12,
                Description = "simple",
                Name = "1ffd2726-1022-41c7-bc82-f0f0364906f7",
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

        }
    }

   

}
