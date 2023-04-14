using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ORM_1_21_;

namespace TestLibrary
{
   public class ExecuteSp
    {

        public static async Task Run()
        {

            await InnerRun<MyClassPostgres, MyDbPostgres>();
            await InnerRun<MyClassMysql, MyDbMySql>();
            await InnerRun<MyClassMsSql, MyDbMsSql>();
            await InnerRun<MyClassSqlite, MyDbSqlite>();
        }

        private static async Task InnerRun<T, TB>() where T : MyClassBase, new() where TB : IOtherDataBaseFactory, new()
        {
            var s = Activator.CreateInstance<TB>();
            Console.WriteLine($"**************************{s.GetProviderName()}*****************************");
            ISession session = await Configure.GetSessionAsync<TB>();
            await session.DropTableIfExistsAsync<T>();
            await session.TableCreateAsync<T>();
            session.InsertBulk(new List<T>
            {
                new T { Age = 100, Name = "100",DateTime = DateTime.Now},
                new T { Age = 10, Name = "10",DateTime = DateTime.Now},
            });
            if (s.GetProviderName() == ProviderName.MySql)
            {
                {
                    session.ExecuteNonQuery(File.ReadAllText("spMysql.txt"));
                    var r = await session.ProcedureCallAsync<T>("getList");
                    var count = ((IEnumerable<T>)r).Count();
                    Execute.Log(1, count == 2);
                    var par1 = new ParameterStoredPr("maxAge", 100, ParameterDirection.Input);
                    var par2 = new ParameterStoredPr("myCount", 120, ParameterDirection.Output);
                    var res5 = await session.ProcedureCallParamAsync<T>("getCountList", new[] { par1, par2 });
                    var par2Value = par2.Value;
                    count = ((IEnumerable<T>)res5).Count();
                    Execute.Log(2, count == 1 && par2Value.ToString() == "1");
                }

                {
                    session.ExecuteNonQuery(File.ReadAllText("spMysql.txt"));
                    var r =  session.ProcedureCall<T>("getList");
                    var count = ((IEnumerable<T>)r).Count();
                    Execute.Log(3, count == 2);
                    var par1 = new ParameterStoredPr("maxAge", 100, ParameterDirection.Input);
                    var par2 = new ParameterStoredPr("myCount", 120, ParameterDirection.Output);
                    var res5 =  session.ProcedureCallParam<T>("getCountList", new[] { par1, par2 });
                    count = ((IEnumerable<T>)res5).Count();
                    Execute.Log(4, count == 1 && par2.Value.ToString() == "1");
                }

                {
                    session.ExecuteNonQuery(File.ReadAllText("spMysql.txt"));
                    var r = session.ProcedureCall<dynamic>("getList");
                    var count = ((IEnumerable<dynamic>)r).Count();
                    Execute.Log(5, count == 2);
                    var par1 = new ParameterStoredPr("maxAge", 100, ParameterDirection.Input);
                    var par2 = new ParameterStoredPr("myCount", 120, ParameterDirection.Output);
                    var res5 = session.ProcedureCallParam<dynamic>("getCountList", par1, par2);
                    count = ((IEnumerable<dynamic>)res5).Count();
                    Execute.Log(6, count == 1 && par2.Value.ToString() == "1");
                }

            }
        }
    }
}
