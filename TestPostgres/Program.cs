using ORM_1_21_;
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
        private const ProviderName ProviderName = ORM_1_21_.ProviderName.SqLite;

        static async Task Main(string[] args)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
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
            Execute.RunOtherSession();
            //Execute.RunThread();
            Console.ReadKey();
            //Console.ReadKey();
           Execute.TotalTest();
           Execute.TestNativeInsert();
           Execute.TestAssignetInsert();
           Execute2.TestTimeStamp();
           
           await Execute3.TotalTestAsync();
           await ExecuteLinqAll.Run();
           ExecutePrimaryKey.Run();
           await ExecuteFree.Run();
           await ExecuteSp.Run();

            Stopwatch stopwatch = new Stopwatch();


            using (ISession session = Configure.Session)
            {
                await session.DropTableIfExistsAsync<MyClassJoinPostgres>();


                await session.TableCreateAsync<MyClassJoinPostgres>();

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


                var listInt = session.FreeSql<int>($"select age from {session.TableName<MyClassJoinPostgres>()}").ToList();
                foreach (int iw in listInt)
                {
                    Console.WriteLine(iw);
                }


                session.DropTableIfExists<TT>();
                session.TableCreate<TT>();
                session.InsertBulk(new List<TT>()
                {
                    new TT()
                });

                var t = new TT() { Name = "[assa'\"" };
                var i = session.Save(t);

                var ii = session.Delete(t);
                var list = session.Query<TT>().ToList();
                //var p = list.First();
                //p.Name = "ion100";
                //var i=session.Save(p);
                //var rr=session.Query<TT>().Where(a => a.Name == "ion100").FirstOrDefault();
                //
                await session.DropTableIfExistsAsync<MyClass>();
                await session.TableCreateAsync<MyClass>();
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

                //await session.DropTableIfExistsAsync<TestSqliteUUid>();
                //await session.TableCreateAsync<TestSqliteUUid>();
                //
                //var t = new TestSqliteUUid() { Name = "asas" };
                //session.Save(t);
                //var ll = session.Query<TestSqliteUUid>().ToList();
                //  await session.DropTableIfExistsAsync<TestMySqlUUid>();
                //  await session.TableCreateAsync<TestMySqlUUid>();
                //  var t = new TestMySqlUUid(){Name = "ass"};
                //  await session.SaveAsync(t);
                //
                // var re = session.Query<TestMySqlUUid>().ToList();

                //session.DropTableIfExists<TestUuid2>();
                //session.TableCreate<TestUuid2>();
                //TestUuid2 testUuid = new TestUuid2();
                //testUuid.Name = "asas";
                //session.Save(testUuid);
                //TestUuid2 testUuid1 = new TestUuid2();
                //testUuid1.Name = "asas";
                //session.Save(testUuid1);
                //var s = session.Query<TestUuid2>().ToList();

                // var l = session.Query<MyClass>().Select(a => a.Age).ToList();
                // var e1e = await session.Query<MyClass>().Select(a => a.Age).JoinCoreAsync(l, a => a, b => b,
                //     (aa, bb) => new { name1 = aa, name2 = bb });
                //
                // var ee = session.Query<MyClass>().JoinCore(session.Query<MyClass>(), a => a.Age, b => b.Age,
                //      (aa, bb) => new { name1 = aa.Name, name2 = bb.Name }).ToList();

                //foreach (var o in await session.Query<MyClass>().Select(a => new { a.Age }).FooAsync(a => a.Age + 30))
                //{
                //    Console.WriteLine(o);
                //}
                //
                //var res3 = await session.ProcedureCallAsync<dynamic>("getList");
                //var res4 = session.ProcedureCall<dynamic>("getList");
                //
                //var res13 = await session.ProcedureCallAsync<MyClass>("getList");
                //var res14 = session.ProcedureCall<MyClass>("getList");
                //
                //{
                //    var par1 = new ParameterStoredPr("maxAge", 100, ParameterDirection.Input);
                //    var par2 = new ParameterStoredPr("myCount", 120, ParameterDirection.Output);
                //    var res5 = session.ProcedureCallParam<dynamic>("getCountList", par1, par2);
                //    var par2Value = par2.Value;
                //}
                //
                //{
                //    var par1 = new ParameterStoredPr("maxAge", 100, ParameterDirection.Input);
                //    var par2 = new ParameterStoredPr("myCount", 120, ParameterDirection.Output);
                //    var res5 = await session.ProcedureCallParamAsync<MyClass>("getCountList", new[] { par1, par2 });
                //    var par2Value = par2.Value;
                //}

            }





        }



    }
    [MapTable("tt")]
    class TT : IMapAction<TT>
    {
        [MapPrimaryKey("id", Generator.Assigned)]
        public Guid Id { get; set; } = Guid.NewGuid();
        [MapColumn("name")]
        public string Name { get; set; }

        public void ActionCommand(TT item, CommandMode mode)
        {

        }
    }
    [MapTable]
    class TestSqliteUUid
    {
        [MapColumnType("text")]
        [MapDefaultValue("default (lower(hex(randomblob(4))) || '-' || lower(hex(randomblob(2))) || '-4' || substr(lower(hex(randomblob(2))),2) || '-' || substr('89ab',abs(random()) % 4 + 1, 1) || substr(lower(hex(randomblob(2))),2) || '-' || lower(hex(randomblob(6))))")]
        [MapPrimaryKey("id", Generator.NativeNotLastInsert)]
        public Guid Id { get; set; }
        [MapColumn]
        public string Name { get; set; }
    }


    [MapTable]
    class TestMySqlUUid
    {
        [MapColumnType("binary(16) default (uuid_to_bin(uuid()))")]
        [MapDefaultValue("not null primary key")]
        [MapPrimaryKey("id", Generator.Native)]
        public Guid Id { get; set; }
        [MapColumn]
        public string Name { get; set; }
    }

    [MapTable("test_uuid")]
    class TestUuid
    {
        [MapColumnType(" uuid")]
        [MapDefaultValue("DEFAULT uuid_generate_v4() PRIMARY KEY")]
        [MapPrimaryKey("id", Generator.NativeNotLastInsert)]
        public Guid id { get; set; }
        [MapColumn("name")]
        public string Name { get; set; }

    }

    [MapTable("test_uuid2")]
    class TestUuid2
    {

        [MapPrimaryKey("id", Generator.Native)]
        public long id { get; set; }
        [MapColumn("name")]
        public string Name { get; set; }

    }

    static class Help
    {
        public static IEnumerable<object> Foo<TSource>(this IQueryable<TSource> source, Func<TSource, object> func)
        {
            var res = source.Provider.Execute<IEnumerable<TSource>>(source.Expression);
            foreach (var re in res)
            {
                yield return func(re);
            }
        }

        public static async Task<IEnumerable<object>> FooAsync<TSource>(this IQueryable<TSource> source,
            Func<TSource, object> func, CancellationToken cancellationToken = default)
        {
            var res = await source.Provider.ExecuteAsync<IEnumerable<TSource>>(source.Expression, cancellationToken);
            return res.Select(func);
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
