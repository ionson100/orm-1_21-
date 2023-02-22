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




#if false
        [MapPrimaryKey("id", Generator.Native)]
        public long Id { get; set; }
#else
        [MapPrimaryKey("id", Generator.Assigned)]
        public Guid Id { get; set; } = Guid.NewGuid();
#endif

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


    }
}