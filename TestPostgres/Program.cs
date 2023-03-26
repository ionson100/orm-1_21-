using ORM_1_21_;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using ORM_1_21_.Extensions;
using TestLibrary;
using static System.Collections.Specialized.BitVector32;


namespace TestPostgres
{
    internal class Program
    {
        private const ProviderName ProviderName = ORM_1_21_.ProviderName.PostgreSql;

        static async Task Main(string[] args)
        {

            switch (ProviderName)
            {
                case ProviderName.MsSql:
                    Starter.Run(ConnectionStrings.MsSql, ProviderName);
                    break;
                case ProviderName.MySql:
                    Starter.Run(ConnectionStrings.Mysql, ProviderName);
                    break;
                case ProviderName.PostgreSql:
                    Starter.Run(ConnectionStrings.Postgesql, ProviderName);
                    break;
                case ProviderName.SqLite:
                    Starter.Run(ConnectionStrings.Sqlite, ProviderName);//
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            //Execute.RunThread();
            //Console.ReadKey();
            //Console.ReadKey();
            //Execute.TotalTest();
            ////Execute.TotalTestAsync();
            //Execute.TestNativeInsert();
            //Execute.TestAssignetInsert();
            //Execute2.TestTimeStamp();
            await Execute3.TotalTestAsync();
           
            Stopwatch stopwatch = new Stopwatch();
           
            var dt = DateTime.Now;
            using (ISession session = await Configure.SessionAsync)
            {
                if (await session.TableExistsAsync<TestSerialize>())
                {
                    await session.DropTableAsync<TestSerialize>();
                }

                await session.TableCreateAsync<TestSerialize>();
                await session.InsertBulkAsync(new List<TestSerialize>()
                {
                    new TestSerialize()
                },30);
                var u = await session.Query<TestSerialize>().SingleAsync();
                u.User = new TestUser() { Name = "asas", Id = 2 };
                await session.SaveAsync(u);
                 u = await session.Query<TestSerialize>().SingleAsync();
            }

          
        }

        static int GetInt()
        {
            return 10;
        }






    }
  



   


}
