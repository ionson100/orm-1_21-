using System;
using ORM_1_21_;

namespace TestLibrary
{
    [MapTable("Order")]
    public class Order
    {
        [MapPrimaryKey("id", Generator.Assigned)]
        public Guid Id { get; set; }= System.Guid.NewGuid();
        [MapColumn("number")]
        public int Number { get; set; }
        [MapColumn]
        public string Text { get; set; }
        [MapColumn]
        public int CustomerId { get; set; }

        [MapColumn] public int IntNull { get; set; }



        [MapColumn] public long Long { get; set; } = 33;
        [MapColumn] public long? LongNull { get; set; }

        [MapColumn] public short Short { get; set; } = 33;
        [MapColumn] public short? ShortNull { get; set; }

        //[MapColumn] public uint Uint { get; set; } = 33;
        //[MapColumn] public uint? UintNull { get; set; }
        //
        //[MapColumn] public ushort Ushort { get; set; } = 33;
        //[MapColumn] public ushort? UshortNull { get; set; }
        //
        //[MapColumn] public ulong Ulong { get; set; } = 33;
        //[MapColumn] public ulong? UlongNull { get; set; }

        [MapColumn] public decimal Decimal { get; set; } = 33;
        [MapColumn] public decimal? DecimalNull { get; set; }

        [MapColumn] public float Float { get; set; } = 33;
        [MapColumn] public float? FloatNull { get; set; }

        [MapColumn] public char Char { get; set; } = '1';
        [MapColumn] public char? CharNull { get; set; }


        [MapColumn] public bool Bool { get; set; }
        [MapColumn] public bool? BoolNull { get; set; }

        [MapColumn] public byte Byte { get; set; } = 33;
        [MapColumn] public byte? ByteNull { get; set; }

        [MapColumn] public byte[] Bytes { get; set; } = { 1, 2 };

        [MapColumn] public Guid Guid { get; set; } = System.Guid.NewGuid();
        [MapColumn] public Guid? GuidNull { get; set; }

        [MapColumn] public DateTime Date { get; set; }// = DateTime.Now;
        [MapColumn] public DateTime? DateNull { get; set; }

        [MapColumn] public double Double { get; set; }

        [MapColumn] public double? DoubleNull { get; set; }


    }

   public class OrderPostgres:Order { }
   public class OrderMysql : Order { }
   public class OrderMsSql : Order { }
   public class OrderSqlite : Order { }
}