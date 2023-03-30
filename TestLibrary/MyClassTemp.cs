using ORM_1_21_;

using System;

namespace TestLibrary
{
    [MapTable("nomap")]
    class MyClassTemp
    {
        [MapColumn("id")]
        public Guid Id { get; set; } = Guid.NewGuid();
        [MapColumn("enum1")]
        public MyEnum MyEnum { get; set; }
        [MapColumn("age")]
        public int Age { get; set; }
        public override string ToString()
        {
            return $" Id={Id}, MyEnum={MyEnum}, Age={Age}";
        }
    }
}