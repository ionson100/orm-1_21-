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
             //Execute.RunOtherSession();
             //Execute.RunThread();
             //Console.ReadKey();
             //Console.ReadKey();
             // Execute.TotalTest();
             // Execute.TestNativeInsert();
             // Execute.TestAssignetInsert();
             // Execute2.TestTimeStamp();
             // 
             // await Execute3.TotalTestAsync();
             // await ExecuteLinqAll.Run();
             // ExecutePrimaryKey.Run();
             // await ExecuteFree.Run();
             // await ExecuteSp.Run();

            Stopwatch stopwatch = new Stopwatch();


            using (ISession session = Configure.Session)
            {
                // await session.DropTableIfExistsAsync<MyClassJoinPostgres>();
                //
                //
                // await session.TableCreateAsync<MyClassJoinPostgres>();
                //
                // session.InsertBulk(new List<MyClassJoinPostgres>
                //     {
                //         new MyClassJoinPostgres() { Age = 40, Name = "name" },
                //         new MyClassJoinPostgres() { Age = 401, Name = "name" },
                //         new MyClassJoinPostgres() { Age = 201, Name = "name" },
                //         new MyClassJoinPostgres() { Age = 202, Name = "name" },
                //         new MyClassJoinPostgres() { Age = 203, Name = "name" },
                //         new MyClassJoinPostgres() { Age = 40, Name = "name" },
                //         new MyClassJoinPostgres() { Age = 40, Name = "name" },
                //         new MyClassJoinPostgres() { Age = 40, Name = "name" },
                //
                //     }
                // );

                var sql = $"select * from {session.TableName<MyClass>()} where {session.ColumnName<MyClass>(a => a.Age)} > {session.GetSymbolParam()}1" +
                          $" and name <> @2";
                // for Postgres: select * from "my_class5" where "age" > @1
                // for MSSQL   : select * from [my_class5] where [age] > @1
                // for MySql   : select * from `my_class5` where `age` > ?1
                // for SqLite  : select * from "my_class5" where "age" > @1
                var res = session.FreeSql<MyClass>(sql, 10,"asas");



            }





        }



    }

    [MapTable("select")]
    class TT34
    {
        [MapPrimaryKey("id", Generator.Native)]
        public int Id { get; set; } 
       
        [MapIndex]
        [MapColumn("select")]
        public string Name { get; set; }
    }

    [MapTable]
    class TT23
    {
       
        [MapPrimaryKey(Generator.Native)]
        public int Id { get; set; }
        [MapDefaultValue("UNIQUE")]
        [MapColumn]
        public string Name { get; set; }
    }
    [MapTable("test_sub_class")]//Mandatory table name
    class TSuperClass
    {
        [MapPrimaryKey(Generator.Assigned)]
        public Guid Id { get; set; } = Guid.NewGuid();
      
        [MapColumn]
        public string Name { get; set; }

    }

    class TSubClass : TSuperClass
    {
        [MapColumn]
        public int Age { get; set; }
    }
   
    class TCoreClass : TSubClass
    {
        [MapColumn]
        public string Description { get; set; }
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
        [MapDefaultValue("default (lower(hex(randomblob(4))) || '-' || " +
                         "lower(hex(randomblob(2))) || '-4' || substr(lower(hex(randomblob(2))),2)" +
                         " || '-' || substr('89ab',abs(random()) % 4 + 1, 1)" +
                         " || substr(lower(hex(randomblob(2))),2) || '-' || " +
                         "lower(hex(randomblob(6))))")]
        [MapPrimaryKey("id", Generator.NativeNotReturningId)]
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
        [MapPrimaryKey("id", Generator.NativeNotReturningId)]
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
