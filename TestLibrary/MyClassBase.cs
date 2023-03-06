using ORM_1_21_;

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace TestLibrary
{
    [MapUsageActivator]
    public class MyClassBase
    {
        public MyClassBase()
        {
            Issa = 101;
        }
        
        public int Issa { get; set; } = 100;





        [MapPrimaryKey("id", Generator.Assigned)]
        public Guid Id { get; set; } = Guid.NewGuid();


        [MapColumnName("my_test")]
        public MyTest MyTest { get; set; }

        [MapColumnName("name")]
        public string Name { get; set; }

        [MapDefaultValue("NOT NULL DEFAULT '5'")]
        [MapColumnName("age")]
        public int Age { get; set; }

        [MapColumnName("desc")]
        [MapColumnType("TEXT")]
        public string Description { get; set; }

        [MapColumnName("enum")]
        public MyEnum MyEnum { get; set; } = MyEnum.First;

        [MapColumnName("date")]
        public DateTime DateTime { get; set; } = DateTime.Now;

        [MapColumnName("test")]
        public List<MyTest> Test23 { get; set; } = new List<MyTest>() { new MyTest() { Name = "simple" } };
        [MapColumnName("test1")]
        public int? ValInt { get; set; }
        [MapColumnName("test3")]
        public bool? Valbool { get; set; }
        [MapColumnName("test4")]
        public double? Valdouble{ get; set; }
        [MapColumnName("test5")]
        public decimal? Valdecimal { get; set; }
        [MapColumnName("test6")]
        public float? Valfloat { get; set; }
        [MapColumnName("test7")]
        public Int16? ValInt16 { get; set; }
        [MapColumnName("test8")]
        public Int64? ValInt4 { get; set; }
        [MapColumnName("test9")]
        public Guid? ValGuid { get; set; }
        [MapColumnName("testuser")]
        public TestUser TestUser { get; set; } = new TestUser { Id = 23, Name = "23" };

    }
}