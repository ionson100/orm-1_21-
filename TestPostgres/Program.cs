using ORM_1_21_;
using ORM_1_21_.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
            Execute.TotalTest();
            Execute.TestNativeInsert();
            Execute.TestAssignetInsert();
            Execute2.TestTimeStamp();
             
            await Execute3.TotalTestAsync();
            await ExecuteLinqAll.Run();

            Stopwatch stopwatch = new Stopwatch();


            using (ISession session = Configure.Session)
            {
                session.InsertBulk(new List<MyClass>
                    {
                        new MyClass(1) { Age = 40, Name = "name" },
                        new MyClass(1) { Age = 401, Name = "name" },
                        new MyClass(1) { Age = 201, Name = "name" },
                        new MyClass(1) { Age = 202, Name = "name" },
                        new MyClass(1) { Age = 203, Name = "name" },
                        new MyClass(1) { Age = 40, Name = "name" },
                        new MyClass(1) { Age = 40, Name = "name" },
                        new MyClass(1) { Age = 40, Name = "name" },

                    }
                );
                if (session.TableExists<MyClassJoinPostgres>())
                {
                    session.DropTable<MyClassJoinPostgres>();
                }

                session.TableCreate<MyClassJoinPostgres>();

                session.InsertBulk(new List<MyClassJoinPostgres>
                    {
                        new MyClassJoinPostgres { Age = 40, Name = "name11" },
                        new MyClassJoinPostgres { Age = 40, Name = "name11" },
                        new MyClassJoinPostgres { Age = 20, Name = "name11" },
                        new MyClassJoinPostgres { Age = 20, Name = "name11" },
                        new MyClassJoinPostgres { Age = 20, Name = "name11" },
                        new MyClassJoinPostgres { Age = 20, Name = "name11" },

                    }
                );
                //var sss = session.Query<MyClassJoinPostgres>(). 

                var wer = session.Query<MyClassJoinPostgres>().Where(a => a.Age > 0).
                    GroupByCore(a => a.Age, null).ToList();
                var asas = session.Query<MyClass>().SelectCore(a => new { a.Name, a.Age }).Select(s => s.Age).ToList();
                var tt = session.Query<MyClassJoinPostgres>().Where(a => a.Age == 20);
                var rer = session.Query<MyClassJoinPostgres>().UnionCore(tt).ToList();
                var list1 = session.Query<MyClass>().ToList();
                var list2 = session.Query<MyClassJoinPostgres>();
                var enumerable = list1.GroupBy(a => new { a.Age, a.Name }, d => d);
                //session.Query<MyClass>().

                var groupBy = session.Query<MyClass>().Where(a => a.Age > 0)
                    .GroupByCore(a => new { a.Age, a.DateTime }, d => new { d.Age, d.Description }).ToList();

                //  var myClasses =
                //      session.Query<MyClass>().Where(a => a.Name=="name").Join(
                //          session.Query<MyClassJoinPostgres>().Where(d => d.Name == "name11"),
                //          a => a.Age,
                //          d => d.Age,
                //          (w, e) => new { s = w.Age, d = e.Age, dd = e.Name }).ToList();
                //  var list1 = session.Query<MyClass>().ToList();
                //  var list2 = session.Query<MyClassJoinPostgres>();
                //
                var groupJoin = session.Query<MyClass>().Where(a => a.Age > 0).GroupJoinCore
                    (session.Query<MyClassJoinPostgres>(), a => a.Age, b => b.Age, (ff, dd) => new { ff.Age, dd }).ToList();
                var sas = session.Query<MyClass>().GroupJoinCore(session.Query<MyClassJoinPostgres>(),
                    a => a.Age,
                    b => b.Age,
                    (m, ss) => ss);
                var ssas = session.Query<MyClass>().GroupByCore(a => a.Age);
                var ssas1 = await session.Query<MyClass>().GroupByCoreAsync(a => a.Age, d => d.DateTime);
                var ssas2 = session.Query<MyClass>().GroupByCore(a => a.Age,
                    (i, classes) => classes.Sum(f => f.Age));

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
