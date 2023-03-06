using System;
using ORM_1_21_;

namespace TestLibrary
{
    [MapTableName("null_table")]
    class MyClassNullBase
    {
        [MapPrimaryKey("id", Generator.Native)]
        public long Id { get; set; }


        //[MapColumnName("v2")] public uint? V2 { get; set; } = 1;
        //[MapColumnName("v3")]public ulong? V3 { get; set; }
        //[MapColumnName("v4")]public ushort? V4 { get; set; }
        //[MapColumnName("v14")]public sbyte? V14 { get; set; }


        [MapColumnName("v1")] public string V1 { get; set; }

        [MapColumnName("v5")] public bool? V5 { get; set; } = true;

        [MapColumnName("v6")] public byte? V6 { get; set; } = 56;

        [MapDefaultValue("NOT NULL DEFAULT '0'")]
        [MapColumnName("v7")] public char V7 { get; set; } = 'y';
        [MapColumnName("v8")] public DateTime? V8 { get; set; } = DateTime.Now;
        [MapColumnName("v9")] public decimal? V9 { get; set; }
        [MapColumnName("v10")] public double? V10 { get; set; }
        [MapColumnName("v11")] public short? V11 { get; set; }
        [MapColumnName("v12")] public int? V12 { get; set; }
        [MapColumnName("v13")] public long? V13 { get; set; }
        //
        [MapColumnName("v15")] public float? V15 { get; set; }
        [MapColumnName("v16")] public Guid V16 { get; set; }


        //[MapColumnName("v19")] public byte[] V21 { get; set; } = new byte[] { 5 };
    }

}