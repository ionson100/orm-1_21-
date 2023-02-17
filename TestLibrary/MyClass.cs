using ORM_1_21_;


namespace TestLibrary
{
    [MapTableName("my_class")]
    public class MyClass : MyClassBase
    {
        [MapColumnName("My_test")]
        public MyTest MyTest { get; set; }
    }
    [MapTableName("my_class")]
   public class MyClassMysql : MyClassBase
    {
        [MapColumnName("my_test")]
        public MyTest MyTest { get; set; }
    }
    [MapTableName("my_class2")]
    class MyClassMysql2 : MyClassBase
    {
        [MapColumnName("my_test")]
        public MyTest MyTest { get; set; }
    }
}