using ORM_1_21_;
using ORM_1_21_.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TestLibrary;


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
         //await Execute3.TotalTestAsync();

            Stopwatch stopwatch = new Stopwatch();

            using (ISession session = Configure.Session)
            {
                session.InsertBulk(new List<MyClass>(){
                        new MyClass(1) { Age = 40, Name = "name" },
                        new MyClass(1) { Age = 401, Name = "name" },
                        new MyClass(1) { Age = 201, Name = "name" },
                        new MyClass(1) { Age = 202, Name = "name" },
                        new MyClass(1) { Age = 203, Name = "name" },
                        new MyClass(1) { Age = 40, Name = "name" },
                       
                    }
                );
                if (session.TableExists<MyClassJoinPostgres>())
                {
                    session.DropTable<MyClassJoinPostgres>();
                }

                session.TableCreate<MyClassJoinPostgres>();

                session.InsertBulk(new List<MyClassJoinPostgres>(){
                        new MyClassJoinPostgres() { Age = 40, Name = "name11" },
                        new MyClassJoinPostgres() { Age = 40, Name = "name11" },
                        new MyClassJoinPostgres() { Age = 40, Name = "name11" },
                        new MyClassJoinPostgres() { Age = 40, Name = "name11" },
                        new MyClassJoinPostgres() { Age = 40, Name = "name11" },
                        new MyClassJoinPostgres() { Age = 40, Name = "name11" },

                    }
                );

                var groupBy = session.Query<MyClass>().GroupBy(a => a.Age).ToList();

                var myClasses =
                    session.Query<MyClass>().Where(a => a.Name=="name").Join(
                        session.Query<MyClassJoinPostgres>().Where(d => d.Name == "name11"),
                        a => a.Age,
                        d => d.Age,
                        (w, e) => new { s = w.Age, d = e.Age, dd = e.Name }).ToList();
                var list1 = session.Query<MyClass>().ToList();
                var list2 = session.Query<MyClassJoinPostgres>();
                //
                //var groupJoin = session.Query<MyClass>().Where(a=>a.Age>0).GroupJoin
                //    (session.Query<MyClassJoinPostgres>(), a => a.Age, b => b.Age, (ff, dd) => new { ff.Age, dd }).ToList();
                //var groupJoin = list1.GroupJoin(list2, a => a.Age, b => b.Age, (ff, dd) => new { ff.Age, dd });
            }





        }

    

        static int GetInt()
        {
            return 10;
        }






    }

    [MapTable]
    public class TestSerialize1
    {
        [MapPrimaryKey(Generator.Native)]
        public long Id { get; set; }
        [MapColumn]
        public TestUser User { get; set; }
        [MapColumn]
        public int IdCore { get; set; }
       

    }

    [MapTable]
    public class TestSerialize2
    {
        [MapPrimaryKey(Generator.Native)]
        public long Id { get; set; }
        [MapColumn]
        public TestUser User { get; set; }
        [MapColumn]
        public int IdCore { get; set; }

    }







}
