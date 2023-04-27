using System;
using System.Collections.Generic;
using System.Linq;
using ORM_1_21_;

namespace Example
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Starter.Start();
            using (var session = Configure.Session)
            {

                var users = session.Query<User>().Where(a => a.Age > 10 && a.Name != null).OrderBy(s => s.Age).ToList();
                
                Console.WriteLine(users.Count == 4);
                users.ForEach(Console.WriteLine);

                users = session.Query<User>().Where(a => a.Name != null).Between(s => s.Age, 30, 50).OrderByDescending(d => d.Age).ToList();
                Console.WriteLine(users.Count == 3);
                users.ForEach(Console.WriteLine);

                var user = session.Query<User>().Single(a => a.Age == 20);
                user.Name += " update";
              
                var i = session.Update(user);
                Console.WriteLine(i == 1);

                user = session.Query<User>().Single(a => a.Name.Contains("upd"));
                Console.WriteLine(true);
                Console.WriteLine(user);

                string sql = $"select * from {session.TableName<User>()} where age > @1 and age < @2";
                users = session.FreeSql<User>(sql, 0, 100).ToList();
                Console.WriteLine(users.Count == 4);
                users.ForEach(Console.WriteLine);

                var du = session.FreeSql<dynamic>(sql, 0, 100).ToList();
                Console.WriteLine(du.Count == 4);
                du.ForEach(Console.WriteLine);

                var ages = session.FreeSql<int>($"select age from {session.TableName<User>()} where age > @1 and age < @2 order by age", 0, 100).ToList();
                Console.WriteLine(du.Count == 4);
                ages.ForEach(Console.WriteLine);

                i = session.Query<User>().Where(a => a.Age == 50).Update(u => new Dictionary<object, object>()
                {
                    { u.Age, 60 }, { u.Name, string.Concat("Name update ",60) }
                });
                Console.WriteLine(i == 1);
                user = session.Query<User>().Single(a => a.Age == 60);
                Console.WriteLine(user);

               // session.TruncateTable<User>();
                var count = session.Query<User>().Count();
                Console.WriteLine(count == 0);

            }
            Console.ReadKey();
        }
      
    }

  

}
