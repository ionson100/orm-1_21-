using System.Collections.Generic;
using ORM_1_21_;

namespace Example
{
    internal static class Starter
    {
        public const string ConnectionString = "Server=localhost;Port=5432;Database=testorm;User Id=user;Password=postgres;";
        public const string Sqlite = "Data Source=mydb.db;Version=3;BinaryGUID=False;";

        public static void Start()
        {
            _ = new Configure(Sqlite, ProviderName.SqLite);//start at the beginning of the application;
            using (var session=Configure.Session)
            {
                using (session.BeginTransaction())
                {
                    session.DropTableIfExists<User>();
                    session.TableCreate<User>();
                    session.InsertBulk(new List<User>
                    {
                        new User(20), new User(30), new User(40), new User(50)
                    });
                }
                
            }
        }
    }
}
