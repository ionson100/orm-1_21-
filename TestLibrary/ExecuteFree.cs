using ORM_1_21_;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace TestLibrary
{
    public static class ExecuteFree
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
            ISession session = Configure.GetSession<TB>();
            session.DropTableIfExists<T>();
            session.TableCreate<T>();
            session.InsertBulk(new List<T>
            {
                new T { Age = 100, Name = "100",DateTime = DateTime.Now},
                new T { Age = 10, Name = "10",DateTime = DateTime.Now},
            });
           var list = session.FreeSql<T>($"select * from {session.TableName<T>()}");
           Execute.Log(1, list.Count() == 2);
           if (s.GetProviderName() == ProviderName.MySql)
           {
               list = session.FreeSql<T>($"select * from {session.TableName<T>()} where {session.ColumnName<T>(a => a.Age)} = ?1", 100);
               Execute.Log(2, list.Count() == 1);
           }
           else
           {
           
               list = session.FreeSql<T>($"select * from {session.TableName<T>()} where {session.ColumnName<T>(a => a.Age)} = @1", 100);
               Execute.Log(2, list.Count() == 1);
           }
           /*-----  2 ------*/
           if (s.GetProviderName() == ProviderName.MySql)
           {
               list = session.FreeSql<T>($"select * from {session.TableName<T>()} " +
                                         $"where {session.ColumnName<T>(a => a.Age)} = ?1 or " +
                                         $" {session.ColumnName<T>(a => a.Age)}=?2", 100, 10);
               Execute.Log(2, list.Count() == 2);
           }
           else
           {
           
               list = session.FreeSql<T>($"select * from {session.TableName<T>()} " +
                                         $"where {session.ColumnName<T>(a => a.Age)} = @1 or " +
                                         $" {session.ColumnName<T>(a => a.Age)}=@2", 100, 10);
               Execute.Log(2, list.Count() == 2);
           }
           
           /*-----  datetime ------*/
           if (s.GetProviderName() == ProviderName.MySql)
           {
               list = await session.FreeSqlAsync<T>($"select * from {session.TableName<T>()} " +
                                         $"where date >?1 and date<?2", DateTime.Now.AddDays(-1), DateTime.Now.AddDays(1));
               Execute.Log(3, list.Count() == 2);
           }
           else
           {
           
               list = await session.FreeSqlAsync<T>($"select * from {session.TableName<T>()} " +
                                         $"where date > @1 and date <@2", DateTime.Now.AddDays(-1), DateTime.Now.AddDays(1));
               Execute.Log(3, list.Count() == 2);
           }
           
           if (s.GetProviderName() == ProviderName.MySql)
           {
               list =  session.FreeSql<T>($"select * from {session.TableName<T>()} " +
                                                    $"where date >?1 and date<?2", DateTime.Now.AddDays(-1), DateTime.Now.AddDays(1));
               Execute.Log(4, list.Count() == 2);
           }
           else
           {
           
               list =  session.FreeSql<T>($"select * from {session.TableName<T>()} " +
                                                    $"where date > @1 and date <@2", DateTime.Now.AddDays(-1), DateTime.Now.AddDays(1));
               Execute.Log(4, list.Count() == 2);
           }
           
            var listInt = session.FreeSql<int>($"select age from {session.TableName<T>()}").ToList();
            foreach (int i in listInt)
            {
                Console.WriteLine(i);
            } 
            
            listInt = (List<int>)await session.FreeSqlAsync<int>($"select age from {session.TableName<T>()}");
            foreach (int i in listInt)
            {
                Console.WriteLine(i);
            }
            var tt=await session.FreeSqlAsync<Guid>($"select {session.ColumnName<T>(a=>a.Id)} from {session.TableName<T>()}");
            foreach (Guid i in tt)
            {
                Console.WriteLine(i);
            }

            var ttU = await session.FreeSqlAsync<dynamic>($"select testuser from {session.TableName<T>()}");
            foreach (dynamic i in ttU)
            {
                Console.WriteLine(i.testuser);
            }

            var ttUp = await session.FreeSqlAsync<ProxyFreeSql>($"select id,age,testuser  from {session.TableName<T>()}");
            foreach (ProxyFreeSql i in ttUp)
            {
                Console.WriteLine(i.ToString());
            }

        }
        [MapReceiverFreeSql]
        private class ProxyFreeSql
        {
            public ProxyFreeSql(Guid id, int age, TestUser user)
            {
                Id = id;
                Age = age;
                User = user;
            }

            private Guid Id { get; }
            private int Age { get;  }
            private TestUser User { get;  }
            public override string ToString()
            {
                return $"id=\"{Id}\"  Age=\"{Age}\"   UserName=\"{User.Name}\"";
            }
        }
    }
}
