using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ORM_1_21_;

namespace TestLibrary
{
    class TestInsertBaseNative
    {
        [MapPrimaryKey("id", Generator.Native)]
        public int Id { get; set; }
        [MapColumnName("name")]
        public string Name { get; set; }
    }
    class TestInsertBaseAssignet
    {
        [MapPrimaryKey("id", Generator.Assigned)]
        public Guid Id { get; set; } = Guid.NewGuid();
        [MapColumnName("name")]
        public string Name { get; set; }
    }

    [MapTableName("TT1")]
    class TiPostgresNative:TestInsertBaseNative
    {
       
    }
    [MapTableName("TT2")]
    class TiPostgresAssignet : TestInsertBaseAssignet
    {
        [MapPrimaryKey("id",Generator.Assigned)]
        public Guid Id { get; set; }=Guid.NewGuid();
    }
    [MapTableName("TT11")]
    class TiMysqlNative : TestInsertBaseNative
    {
       
    }
    [MapTableName("TT21")]
    class TiMysqlAssignet : TestInsertBaseAssignet
    {
        [MapPrimaryKey("id", Generator.Assigned)]
        public Guid Id { get; set; } = Guid.NewGuid();
    }

    [MapTableName("TT111")]
    class TiMsSqlNative : TestInsertBaseNative
    {
      
    }
    [MapTableName("TT211")]
    class TiMsSqlAssignet : TestInsertBaseAssignet
    {
        [MapPrimaryKey("id", Generator.Assigned)]
        public Guid Id { get; set; } = Guid.NewGuid();
    }

    [MapTableName("TT1111")]
    class TiSqliteNative : TestInsertBaseNative
    {
      
    }
    [MapTableName("TT2111")]
    class TiSqliteAssignet : TestInsertBaseAssignet
    {
      
    }
}
