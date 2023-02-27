using ORM_1_21_;


namespace TestLibrary
{
    
    [MapTableName("my_class5")]
    public class MyClass : MyClassBase
    {
       
    }
    [MapTableName("my_class5")]
   public class MyClassMysql : MyClassBase
    {
       
    }
    [MapTableName("my_class5")]
    class MyClassPostgres : MyClassBase
    {
    
    }

    [MapTableName("my_class5")]
    class MyClassMsSql : MyClassBase
    {

    }
    [MapTableName("my_class5")]
    public class MyClassSqlite : MyClassBase
    {

    }
}