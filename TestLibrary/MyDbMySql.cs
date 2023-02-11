using MySql.Data.MySqlClient;
using ORM_1_21_;
using System;
using System.Data.Common;

namespace TestLibrary
{
    class MyDbMySql : IOtherDataBaseFactory
    {
        private static readonly Lazy<DbProviderFactory> dbProviderFactory = new Lazy<DbProviderFactory>(() =>
        {
            return new MySqlClientFactory();
        });
        public ProviderName GetProviderName()
        {
            return ProviderName.MySql;
        }
        public string GetConnectionString()
        {
            return "Server=localhost;Database=test;Uid=root;Pwd=12345;";
        }

        public DbProviderFactory GetDbProviderFactories()
        {
            return dbProviderFactory.Value;
        }
    }
}