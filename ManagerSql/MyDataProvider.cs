using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ORM_1_21_;
using ORM_1_21_.Attribute;

namespace ManagerSql
{
    class MyDataProvider:IOtherDataBaseFactory
    {
        public ProviderName GetProviderName()
        {
            return ProviderName.Sqlite;
        }

        public DbProviderFactory GetDbProviderFactories()
        {
           return System.Data.SQLite.SQLiteFactory.Instance;
        }

        public string GetConnectionString()
        {
            return "Data Source=mydbSqlite.db;Version=3";
        }

    }
    [MapTableName("ion100")]
    class SqliteModel
    {
        [MapPrimaryKey("id",Generator.Native)]
        public int Id { get; set; }
        [MapColumnName("tab_join")]
        public string Join { get; set; }
    }

    public class ModelData
    {
        public double Position { get; set; }
        public string Sql { get; set; }
    }
}
