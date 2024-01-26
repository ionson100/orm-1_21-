
using ORM_1_21_;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TestLibrary;

//DESKTOP - GTR4O03
namespace TestPostgres
{
    internal class Program
    {
        static string Test()
        {
            return "number";
        }
        private static ProviderName ProviderNamee = ORM_1_21_.ProviderName.PostgreSql;

        static async Task Main(string[] args)
        {

            switch (ProviderNamee)
            {
                case ProviderName.MsSql:
                    Starter.Run(ConnectionStrings.MsSql, ProviderNamee);
                    break;
                case ProviderName.MySql:
                    Starter.Run(ConnectionStrings.Mysql, ProviderNamee);
                    break;
                case ProviderName.PostgreSql:
                    Starter.Run(ConnectionStrings.Postgesql, ProviderNamee);
                    break;
                case ProviderName.SqLite:
                    Starter.Run(ConnectionStrings.Sqlite, ProviderNamee); //
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            //  Execute.RunOtherSession();
            //  Execute.RunThread();
            //  //Console.ReadKey();
            //  Console.ReadKey();
            Execute.TotalTest();
            Execute.TestNativeInsert();
            Execute.TestAssignetInsert();
            Execute2.TestTimeStamp();
            await Execute3.TotalTestAsync();
            //await ExecuteLinqAll.Run();
           // ExecutePrimaryKey.Run();
           // await ExecuteFree.Run();
           // await ExecuteSp.Run();
           // TestCapacity.Run();
           // await TestSelector.Run();
           // InsertUpdate.Run();
           // await ExecAdd.Run();
            //
            ExeGeo.Run();
            await ExeGeo.RunAsync();
            Console.WriteLine("finish");
            Console.ReadKey();






            ISession session = Configure.Session;
            {
                session.DropTableIfExists<JPosS>();
                session.TableCreate<JPosS>();
                JPosS o = new JPosS();
                o.Name = "as";
                o.Ion100 = JsonConvert.SerializeObject(new Ion100 { Age = 100, Name = "100" });
                session.InsertBulk(new List<JPosS>(){o});
               session.Query<JPosS>().Update(a => new Dictionary<object, object>
               {
                   { a.Ion100, new Ion100() }
               });
                List<JPosS> list = session.Query<JPosS>().ToList();
                var ssas = list[0].Ion100;
                if (ssas == null)
                {

                }
                var res = session.Query<JPosS>().WhereSql("ion100 @> '{\"Age\":100}'").Select(a => a.Ion100).ToList();
                var res2 = session.Query<JPosS>().WhereSql("ion100 @> '{\"Age\":100}'").ToList();
                //var type = res2[0].Ion100.GetType();
                //dynamic ss =((dynamic) res2[0].Ion100).Age;
                //
                //string sql = $"select {session.StarSql<JPosS>()} from {session.TableName<JPosS>()}";
                //list = session.FreeSql<JPosS>(sql).ToList();
                //var sas = ((Newtonsoft.Json.Linq.JObject) res2[0].Ion100).ToObject(typeof(dynamic));
                using (var ses = Configure.GetSession<MyDbMySql>())
                {
                    ses.DropTableIfExists<JsonMysql>();
                    ses.TableCreate<JsonMysql>();
                    

                }
            }

        }

        [MapTable("jsonTest")]
        public class JsonMysql
        {
            [MapPrimaryKey("id", Generator.Assigned)]
            public Guid Id { get; set; } = Guid.NewGuid();
            //[MapIndex]
            [MapColumn("ion100")]
            [MapColumnTypeJson(TypeReturning.AsObject)]
            public object Ion100 { get; set; }

            [MapColumn]
            public string Name { get; set; } = "name";
        }






    }













}
