
using ORM_1_21_;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ORM_1_21_.geo;
using TestLibrary;
using System.Drawing;
using System.Windows.Forms;
using System.Configuration.Provider;

//DESKTOP - GTR4O03
namespace TestPostgres
{
    internal class Program
    {
        static string Test()
        {
            return "number";
        }
        private static ProviderName ProviderNamee = ORM_1_21_.ProviderName.SqLite;

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
         // Execute.TotalTest();
         // Execute.TestNativeInsert();
         // Execute.TestAssignetInsert();
         // Execute2.TestTimeStamp();
         // await Execute3.TotalTestAsync();
         // await ExecuteLinqAll.Run();
         // ExecutePrimaryKey.Run();
         // await ExecuteFree.Run();
         // await ExecuteSp.Run();
         // TestCapacity.Run();
         // await TestSelector.Run();
         // InsertUpdate.Run();
         // await ExecAdd.Run();
           
         //  ExeGeo.Run();
         //  await ExeGeo.RunAsync();
         //  Console.WriteLine("finish");
          // Console.ReadKey();






            ISession session = Configure.Session;
            {

                session.DropTableIfExists<JPosO>();
                session.TableCreate<JPosO>();
                JPosO jsonPos = new JPosO
                {
                    Name = "1'",
                    Ion100 = new Ion100 { Age = 10, Name = "10" }
                };
                session.Insert(jsonPos);



                //JSON_EXTRACT(c, "$.id") > 1

               string ssqlr = JsonConvert.SerializeObject(new { bb = 1 });
                session.Query<JPosO>().Update(a => new Dictionary<object, object>
                {
                    { a.Name, "name" },
                    { a.Ion100, "{ \"bb\": 10 }" }
                });

                if (session.ProviderName == ProviderName.MsSql)
                {
                    var o = session.Query<JPosO>().WhereSql(a => "JSON_VALUE(ion100, '$.bb') = 1").Select(a => a.Ion100).ToList();
                }
                if (session.ProviderName == ProviderName.PostgreSql)
                {
                    var o = session.Query<JPosO>().WhereSql(a => "ion100 @> '{\"bb\":1}'").Select(a => a.Ion100).ToList();
                }
                if (session.ProviderName == ProviderName.MySql)
                {
                    var o = session.Query<JPosO>().WhereSql(a => $"JSON_EXTRACT({a.Ion100}, \"$.bb\") > 1").Select(a => a.Ion100).ToList();
                }

                if (session.ProviderName == ProviderName.SqLite)
                {
                    var o1 = session.Query<JPosO>().WhereSql(a => $"{a.Ion100}->>'$.bb' = 10").Select(a => a.Ion100).ToList();
                    var o = session.Query<JPosO>().SelectSqlE(a=>$"json_object({a.Ion100}, '$.bb') ").ToList();
                }





                //session.DropTableIfExists<TT>();
                //session.TableCreate<TT>();
                //TT o = new TT();
                //o.Name = "as";
                //o.Age = 12;
                //
                //TT o1 = new TT();
                //o1.Name = "ass";
                //o1.Age = 12;
                //session.InsertBulk(new List<TT>(){o,o1});
                //
                ////var asws = session.Query<TT>().FromSql(" (select * from mygeo) as tt").ToList();
                //string ss = "@4";
                //var цу = session.Query<TT>().SelectSql<int>(" age");
                //await цу.ForEachAsync(Console.WriteLine);
                //var res56 = session.Query<TT>().SelectSqlE(a => $"Concat('Age:',{a.Age},' ',{ss},{a.Name})",
                //    new SqlParam("@4","Name:")).First();
                ////var sd = res56.Cast<int>().ToList();
                //
                //
                //
                //
                //var res = session.Query<TT>().WhereSql(a => $"{a.Name}=@1", new SqlParam("@1","as")).ToList();
                //JsonMysql jsonMysql =new JsonMysql();
                //var o2 = session.Query<TT>().WhereSql(ass => "name='as'").Select(d=>d.Age).ToList();
                //var i = session.Query<TT>().Where(s => s.Age == 12).
                //    UpdateSql(a => $"{a.Name}={session.SymbolParam}e1",new SqlParam($"{session.SymbolParam}e1",jsonMysql.GetAssa("1224")));
                //var list = session.Query<TT>().ToList();
                // i = session.Query<TT>().Where(s => s.Age == 12).UpdateSql(a => $"\"name\"='"+jsonMysql.GetAssa("sas")+"'");
                // i = session.Query<TT>().Where(s=>s.Age==12).UpdateSql(a => $"\"name\"='sasas'");
                //i = session.Query<JPosS>().UpdateSql(a => $"{a.Name}='qqwqw',  " + $"{a.Ion100}=null");
                //i = session.Query<TT>().Where(s => s.Age == 12).UpdateSql(a => $"{a.Name}='{jsonMysql.Assa}'");
                //i = session.Query<TT>().Where(s=>s.Age==12).UpdateSql(a => $"{a.Name}='{jsonMysql.Name}'");
                //i = session.Query<TT>().Where(s=>s.Age==12).UpdateSql(a => $"{a.Name}='{DateTime.Now:D}'");
                //i = session.Query<TT>().Where(s=>s.Age==12).UpdateSql(a => $"{a.Name}='{Guid.NewGuid()}'");
                //i = session.Query<TT>().Where(s => s.Age == 12).UpdateSql(a => $"{a.Name}='234'");

            }

        }




        [MapTable("m_sql")]
        class MSql
        {
            [MapPrimaryKey("id", Generator.Assigned)]
            public Guid Id { get; set; } = Guid.NewGuid();

            [MapColumn("name")]
            public string Name { get; set; }

            [MapColumn("age")]
            public int Age { get; set; }
        }

       

        class JsonBody
        {
            public string Name { get; set; }
            public string Description { get; set;}
        }

        [MapTable("jsonTest")]
        public class JsonMysql
        {
            public string GetAssa(string s)
            {
                return s;
            }
            public string Assa = "asas";
            [MapPrimaryKey("id", Generator.Assigned)]
            public Guid Id { get; set; } = Guid.NewGuid();
            //[MapIndex]
            [MapColumn("ion100")]
            [MapColumnTypeJson(TypeReturning.AsObject)]
            public object Ion100 { get; set; }

            [MapColumn]
            public string Name { get; set; } = "name";
        }
        [MapTable("TT")]
        public class TT
        {
            [MapPrimaryKey("id", Generator.Assigned)]
            public Guid Id { get; set; }= Guid.NewGuid();
            [MapColumn("name")]
            public string Name { get; set; }
            [MapColumn("age")]
            public int Age { get; set; }
        }






    }













}
