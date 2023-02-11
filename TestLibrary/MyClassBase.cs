using ORM_1_21_;
using ORM_1_21_.Attribute;
using System;
using System.Collections.Generic;
namespace TestLibrary
{
    [MapUssageActivator]
    public class MyClassBase
    {
        public MyClassBase()
        {
            Issa = 101;
        }
        
        public int Issa { get; set; } = 100;




#if true
        [MapPrimaryKey("id", Generator.Native)]
        public long Id { get; set; }
#else
        [MapPrimaryKey("id", Generator.Assigned)]
        public Guid Id { get; set; } = Guid.NewGuid();
#endif


        [MapColumnName("name")]
        public string Name { get; set; }


        [MapColumnName("age")]
        [MapIndex]
        public int Age { get; set; }

        [MapColumnName("desc")]
        [MapColumnType("TEXT")]
        public string Description { get; set; }

        [MapColumnName("enum")]
        public MyEnum MyEnum { get; set; } = MyEnum.First;

        [MapColumnName("date")]
        public DateTime DateTime { get; set; } = DateTime.Now;

        [MapColumnName("test")]
        public List<MyTest> Test23 { get; set; } = new List<MyTest>() { new MyTest() { Name = "simple" }
        };


    }
}