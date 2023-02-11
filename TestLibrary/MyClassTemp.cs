using ORM_1_21_;

using System;

namespace TestLibrary
{
    [MapTableName("nomap")]
    class MyClassTemp
    {
        [MapColumnName("id")]
        public Guid Id { get; set; } = Guid.NewGuid();
        [MapColumnName("enum1")]
        public MyEnum MyEnum { get; set; }
        [MapColumnName("age")]
        public int Age { get; set; }
        public override string ToString()
        {
            return $" Id={Id}, MyEnum={MyEnum}, Age={Age}";
        }
    }
}