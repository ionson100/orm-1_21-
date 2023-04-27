using MySql.Data.MySqlClient;
using ORM_1_21_;
using System;
using System.Data.Common;
using System.IO;

namespace TestLibrary
{
   public static class ConnectionStrings
    {
        public const string Sqlite = "Data Source=mydb.db;Version=3;BinaryGUID=fALSE;";//
        public const string Mysql = "Server=localhost;Database=test;Uid=root;Pwd=12345;";//OldGuids=true;
        public const string Postgesql =
            "Server=localhost;Port=5432;Database=testorm;User Id=user;Password=postgres;";
        public static string MsSql =
            $"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=" +
            $"{Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test.mdf")};Integrated Security=True";

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
          //return Npgsql.NpgsqlFactory.Instance;
         
          
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
          // return System.Data.SqlClient.SqlClientFactory.Instance;
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