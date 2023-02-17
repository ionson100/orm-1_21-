using System.Collections.Generic;
using System.Data.Common;
using ORM_1_21_;


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
    [MapTableName("ion1")]
    class SqliteModel
    {
        [MapPrimaryKey("id",Generator.Native)]
        public int Id { get; set; }


        [MapColumnName("base_name")]
        public int BaseName { get; set; } = 0;

        [MapColumnName("hash_str")]
        public int HashStr { get; set; } = 0;

        [MapColumnName("tab_list")]
        public List<ModelData> list { get; set; }=new List<ModelData>();
        public List<ModelData> list2 { get; set; } = new List<ModelData>();
        public int sd { get; set; } = 45;
    }

    public class ModelData
    {
        public double Position { get; set; }
        public string Sql { get; set; }
    }
}
