using MySql.Data.MySqlClient;
using ORM_1_21_;
using System;
using System.Data.Common;

namespace TestLibrary
{
   public static class ConnectionStrings
    {
        public const string Sqlite = "Data Source=mydb.db;Version=3;BinaryGUID=False;";
        public const string Mysql = "Server=localhost;Database=test;Uid=root;Pwd=12345;";
        public const string Postgesql =
            "Server=localhost;Port=5432;Database=testorm;User Id=postgres;Password=ion100312873;";
        public const string MsSql= "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=test;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
    }
  public  class MyDbMySql : IOtherDataBaseFactory
    {
        private static readonly Lazy<DbProviderFactory> DbProviderFactory = 
            new Lazy<DbProviderFactory>(() => new MySqlClientFactory());
        public ProviderName GetProviderName()
        {
            return ProviderName.MySql;
        }
        public string GetConnectionString()
        {
           //var e= new MySqlClientFactory().CreateConnection().ConnectionString = "";
            return ConnectionStrings.Mysql;
        }

        public DbProviderFactory GetDbProviderFactories()
        {
            return DbProviderFactory.Value;
        }
    }
  public class MyDbPostgres : IOtherDataBaseFactory
  {
      private static readonly Lazy<DbProviderFactory> DbProviderFactory = 
          new Lazy<DbProviderFactory>(() => Npgsql.NpgsqlFactory.Instance);
      public ProviderName GetProviderName()
      {
          return ProviderName.PostgreSql;
      }
      public string GetConnectionString()
      {
          return ConnectionStrings.Postgesql;
      }

      public DbProviderFactory GetDbProviderFactories()
      {
          return DbProviderFactory.Value;
      }
  }
  public class MyDbMsSql : IOtherDataBaseFactory
  {
      private static readonly Lazy<DbProviderFactory> DbProviderFactory = 
          new Lazy<DbProviderFactory>(() => System.Data.SqlClient.SqlClientFactory.Instance);
      public ProviderName GetProviderName()
      {
          return ProviderName.MsSql;
      }
      public string GetConnectionString()
      {
          return ConnectionStrings.MsSql;
      }

      public DbProviderFactory GetDbProviderFactories()
      {
          return DbProviderFactory.Value;
      }
  }
  public class MyDbSqlite : IOtherDataBaseFactory
  {
      private static readonly Lazy<DbProviderFactory> DbProviderFactory = 
          new Lazy<DbProviderFactory>(() => System.Data.SQLite.SQLiteFactory.Instance);
      public ProviderName GetProviderName()
      {
          return ProviderName.SqLite;
      }
      public string GetConnectionString()
      {
          return ConnectionStrings.Sqlite;
      }

      public DbProviderFactory GetDbProviderFactories()
      {
          return DbProviderFactory.Value;
      }
  }
}