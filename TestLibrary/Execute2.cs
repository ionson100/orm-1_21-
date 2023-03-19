using System;
using System.Linq;
using ORM_1_21_;

namespace TestLibrary
{
    public static class Execute2
    {
        public static void TestTimeStamp()
        {
            TestTimeStampPostgresql();
            TestTimeStampMsSql();
            TestTimeStampMySql();
            TestTimeStampSqlie();
        }
         static void TestTimeStampPostgresql()
        {
            Console.WriteLine($@"**********************Test TimeStamp Postgresql***********************");
            var session = Configure.GetSession<MyDbPostgres>();
            if (session.TableExists<TSPostgres>()) 
                session.DropTable<TSPostgres>();

            session.TableCreate<TSPostgres>();
            var postgres = new TSPostgres { Name = "123" };
            session.Save(postgres);
            var t = session.Query<TSPostgres>().Single();

            var s = session.Update(t, new AppenderWhere(session.ColumnName<TSPostgres>(d => d.Ts), t.Ts));
            Console.WriteLine($@"1 {s == 1}");
            t.Ts = DateTime.Now.AddMilliseconds(1);
            s = session.Update(t, new AppenderWhere(session.ColumnName<TSPostgres>(d => d.Ts), t.Ts));
            Console.WriteLine($@"2 {s == 0}");
        }

         static void TestTimeStampMsSql()
        {
            Console.WriteLine($@"**********************Test TimeStamp MsSql rowversion***********************");
            var session = Configure.GetSession<MyDbMsSql>();
            if (session.TableExists<TSMsSql>()) 
                session.DropTable<TSMsSql>();

            session.TableCreate<TSMsSql>();
            var msSql = new TSMsSql { Name = "123" };
            session.Save(msSql);
            var t = session.Query<TSMsSql>().Single();

            var s = session.Update(t, new AppenderWhere(session.ColumnName<TSMsSql>(d => d.Ts), t.Ts));
            Console.WriteLine($@"1 {s == 1}");
           
            s = session.Update(t, new AppenderWhere(session.ColumnName<TSMsSql>(d => d.Ts), t.Ts));
            Console.WriteLine($@"2 {s == 0}");
        }

         static void TestTimeStampMySql()
        {
            Console.WriteLine($@"**********************Test TimeStamp MySql ***********************");
            var session = Configure.GetSession<MyDbMySql>();
            if (session.TableExists<TSMySql>())
                session.DropTable<TSMySql>();

            session.TableCreate<TSMySql>();
            var msSql = new TSMySql { Name = "123" };
            session.Save(msSql);
            var t = session.Query<TSMySql>().Single();

            t.Name = "111";
            var s = session.Update(t, new AppenderWhere(session.ColumnName<TSMySql>(d => d.Ts), t.Ts));
            Console.WriteLine($@"1 {s == 1}");
            t.Name = "222";
            s = session.Update(t, new AppenderWhere(session.ColumnName<TSMySql>(d => d.Ts), t.Ts));
            Console.WriteLine($@"2 {s == 0}");
        }

         static void TestTimeStampSqlie()
        {
            Console.WriteLine($@"**********************Test TimeStamp Sqlite ***********************");
            var session = Configure.GetSession<MyDbSqlite>();
            if (session.TableExists<TSSqlite>())
                session.DropTable<TSSqlite>();

            session.TableCreate<TSSqlite>();
            var msSql = new TSSqlite { Name = "123" };
            session.Save(msSql);
            var t = session.Query<TSSqlite>().Single();

            t.Name = "111";
            var s = session.Update(t, new AppenderWhere(session.ColumnName<TSSqlite>(d => d.Ts), t.Ts));
            Console.WriteLine($@"1 {s == 1}");
            t.Name = "222";
            t.Ts = t.Ts.AddMilliseconds(1);
            s = session.Update(t, new AppenderWhere(session.ColumnName<TSSqlite>(d => d.Ts), t.Ts));
            Console.WriteLine($@"2 {s == 0}");
        }
    }


    public class TestTSBase
    {
        [MapPrimaryKey("id", Generator.Assigned)]
        public Guid Id { get; set; } = Guid.NewGuid();

        [MapIndex]
        [MapColumnName("name")] public string Name { get; set; }
    }

    [MapTableName("TS1")]
    public class TSPostgres : TestTSBase
    {
        [MapColumnName("ts")]
        [MapColumnType("timestamp ")]
        [MapDefaultValue("")]
        public DateTime Ts { get; set; } = DateTime.Now;
    }

    [MapTableName("TS2")]
    public class TSMsSql : TestTSBase
    {
        [MapNotInsertUpdate]
        [MapColumnName("ts")]
        [MapColumnType("rowversion")]
        [MapDefaultValue("")]
        public byte[] Ts { get; set; } 
    }

    [MapTableName("TS3")]
    public class TSMySql : TestTSBase
    {
        [MapNotInsertUpdate]
        [MapColumnName("ts")]
        [MapColumnType("TIMESTAMP ")]
        [MapDefaultValue(" DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP")]
        public DateTime Ts { get; set; }
    }
    [MapTableName("TS4")]
    public class TSSqlite : TestTSBase
    {
        [MapColumnName("ts")]
        [MapColumnType("TIMESTAMP ")]
        [MapDefaultValue(" DEFAULT CURRENT_TIMESTAMP")]
        public DateTime Ts { get; set; }
    }
}