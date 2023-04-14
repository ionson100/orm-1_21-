using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ORM_1_21_;

namespace TestLibrary
{
   public class TestInsertBaseNative
    {
        [MapPrimaryKey("id", Generator.Native)]
        public int Id { get; set; }
        [MapColumn("name")]
        public string Name { get; set; }
    }
    class TestInsertBaseAssignet
    {
        [MapPrimaryKey("id", Generator.Assigned)]
        public Guid Id { get; set; } = Guid.NewGuid();
        [MapColumn("name")]
        public string Name { get; set; }
    }

    [MapTable("TT1")]
    public class TiPostgresNative:TestInsertBaseNative
    {
       
    }
    [MapTable("TT2")]
    class TiPostgresAssignet : TestInsertBaseAssignet
    {
        [MapPrimaryKey("id",Generator.Assigned)]
        public Guid Id { get; set; }=Guid.NewGuid();
    }
    [MapTable("TT11")]
    class TiMysqlNative : TestInsertBaseNative
    {
       
    }
    [MapTable("TT21")]
    class TiMysqlAssignet : TestInsertBaseAssignet
    {
        [MapPrimaryKey("id", Generator.Assigned)]
        public Guid Id { get; set; } = Guid.NewGuid();
    }

    [MapTable("TT111")]
    class TiMsSqlNative : TestInsertBaseNative
    {
      
    }
    [MapTable("TT211")]
    class TiMsSqlAssignet : TestInsertBaseAssignet
    {
        [MapPrimaryKey("id", Generator.Assigned)]
        public Guid Id { get; set; } = Guid.NewGuid();
    }

    [MapTable("TT1111")]
    class TiSqliteNative : TestInsertBaseNative
    {
      
    }
    [MapTable("TT2111")]
    class TiSqliteAssignet : TestInsertBaseAssignet
    {
      
    }
}
