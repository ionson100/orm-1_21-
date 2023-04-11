using ORM_1_21_;
using ORM_1_21_.Extensions;
using System;
using System.Collections.Generic;
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
            //Execute.TestNativeInsert();
            //Execute.TestAssignetInsert();
            //Execute2.TestTimeStamp();

            // await Execute3.TotalTestAsync();
            //  await ExecuteLinqAll.Run();

            Stopwatch stopwatch = new Stopwatch();


            using (ISession session = Configure.Session)
            {
                session.DropTableIfExists<MyClassJoinPostgres>();


                session.TableCreate<MyClassJoinPostgres>();

                session.InsertBulk(new List<MyClassJoinPostgres>
                    {
                        new MyClassJoinPostgres() { Age = 40, Name = "name" },
                        new MyClassJoinPostgres() { Age = 401, Name = "name" },
                        new MyClassJoinPostgres() { Age = 201, Name = "name" },
                        new MyClassJoinPostgres() { Age = 202, Name = "name" },
                        new MyClassJoinPostgres() { Age = 203, Name = "name" },
                        new MyClassJoinPostgres() { Age = 40, Name = "name" },
                        new MyClassJoinPostgres() { Age = 40, Name = "name" },
                        new MyClassJoinPostgres() { Age = 40, Name = "name" },

                    }
                );




                session.DropTableIfExists<MyClass>();
                session.TableCreate<MyClass>();
                session.InsertBulk(new List<MyClass>
                    {
                        new MyClass{ Age = 40, Name = "name40" },
                        new MyClass{ Age = 40, Name = "name40" },
                        new MyClass{ Age = 20, Name = "name20" },
                        new MyClass{ Age = 20, Name = "name20" },
                        new MyClass{ Age = 20, Name = "name20" },
                        new MyClass{ Age = 20, Name = "name20" },

                    }
                );



                var ee = session.Query<MyClass>().ToList().Join(session.Query<MyClass>(), a => a.Age, b => b.Age,
                     (aa, bb) => new { name1 = aa.Name, name2 = bb.Name }).ToList();


            }





        }



        static int GetInt()
        {
            return 10;
        }






    }

    static class Help
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="func"></param>
        /// <typeparam name="TSource"></typeparam>
        public static IEnumerable<object> Test<TSource>(this IQueryable<TSource> source, Func<TSource, object> func)
        {

            var res = source.Provider.Execute<IEnumerable<TSource>>(source.Expression);
            foreach (var re in res)
            {
                yield return func(re);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="func"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TSource"></typeparam>
        /// <returns></returns>
        public static async Task<IEnumerable<object>> TestAsync<TSource>(this IQueryable<TSource> source,
            Func<TSource, object> func, CancellationToken cancellationToken = default)
        {

            var res = await source.Provider.ExecuteAsync<IEnumerable<TSource>>(source.Expression, cancellationToken);
            return res.ToList().Select(func);


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
