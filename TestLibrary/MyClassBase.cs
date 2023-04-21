using System;
using ORM_1_21_;

namespace TestLibrary
{
    //[MapUsageActivator]
    public class MyClassBase
    {
        public MyClassBase()
        {
            Issa = 101;
        }

        public int Issa { get; set; } = 100;


        [MapPrimaryKey("id",Generator.Assigned)] public Guid Id { get; set; } = Guid.NewGuid();


        [MapColumn("name")] public string Name { get; set; }

        [MapDefaultValue("NOT NULL DEFAULT '5'")]
        [MapColumn("age")]
        public int Age { get; set; }

        [MapColumn("desc")]
        [MapColumnType("TEXT")]
        public string Description { get; set; }

        [MapColumn("enum")] public MyEnum MyEnum { get; set; } = MyEnum.First;

        [MapColumn("date")] public DateTime DateTime { get; set; } = DateTime.Now;

        [MapColumn("test1")] public int? ValInt { get; set; }

        [MapColumn("test3")] public bool? Valbool { get; set; }

        [MapColumn("test4")] public double? Valdouble { get; set; }

        [MapColumn] public decimal? Valdecimal { get; set; }

        [MapColumn("test6")] public float? Valfloat { get; set; }

        [MapColumn] public short? ValInt16 { get; set; }

        [MapColumn] public long? ValInt4 { get; set; }

        [MapColumn("test9")] public Guid? ValGuid { get; set; }

        [MapColumn("testuser")] public string TestUser { get; set; } ="asas";
    }
}