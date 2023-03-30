using ORM_1_21_;
using System;

namespace TestLibrary
{
  
    class MyClassNullBase
    {
        [MapPrimaryKey("id", Generator.Native)]
        public long Id { get; set; }


        //[MapColumn("v2")] public uint? V2 { get; set; } = 1;
        //[MapColumn("v3")]public ulong? V3 { get; set; }
        //[MapColumn("v4")]public ushort? V4 { get; set; }
        //[MapColumn("v14")]public sbyte? V14 { get; set; }


        [MapColumn("v1")] public string V1 { get; set; }

        [MapColumn("v5")] public bool? V5 { get; set; } = true;

        [MapColumn("v6")] public byte? V6 { get; set; } = 56;

        [MapDefaultValue("NOT NULL DEFAULT '0'")]
        [MapColumn("v7")] public char V7 { get; set; } = 'y';
        [MapColumn("v8")] public DateTime? V8 { get; set; } = DateTime.Now;
        [MapColumn("v9")] public decimal? V9 { get; set; }
        [MapColumn("v10")] public double? V10 { get; set; }
        [MapColumn("v11")] public short? V11 { get; set; }
        [MapColumn("v12")] public int? V12 { get; set; }
        [MapColumn("v13")] public long? V13 { get; set; }
        //
        [MapColumn("v15")] public float? V15 { get; set; }
        [MapColumn("v16")] public Guid? V16 { get; set; }


        //[MapColumnName("v19")] public byte[] V21 { get; set; } = new byte[] { 5 };
    }
    [MapTable("null_table1")]
    class ClassNullPostgres : MyClassNullBase
    {

    }
    [MapTable("null_table2")]
    class ClassNullMsSql : MyClassNullBase
    {

    }
    [MapTable("null_table3")]
    class ClassNullMysql : MyClassNullBase
    {
        [MapColumn("v2")] public uint? V2 { get; set; } 
        [MapColumn("v3")] public ulong? V3 { get; set; }
        [MapColumn("v4")] public ushort? V4 { get; set; }
        [MapColumn("v14")] public sbyte? V14 { get; set; }
    }
    [MapTable("null_table4")]
    class ClassNullSqlite : MyClassNullBase
    {
        [MapColumn("v2")] public uint? V2 { get; set; } 
        [MapColumn("v3")] public ulong? V3 { get; set; }
        [MapColumn("v4")] public ushort? V4 { get; set; }
        [MapColumn("v14")] public sbyte? V14 { get; set; }
    }

}