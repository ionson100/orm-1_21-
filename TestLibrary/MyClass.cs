using ORM_1_21_;


namespace TestLibrary
{
    
    [MapTable("my_class5")]
    public class MyClass : MyClassBase
    {
        public MyClass(int t)
        {

        }
    }
    [MapTable("my_class5")]
   public class MyClassMysql : MyClassBase
    {
       
    }
    [MapTable("my_class5")]
    class MyClassPostgres : MyClassBase
    {
    
    }

    [MapTable()]
    class MyClassMsSql : MyClassBase
    {

    }
    [MapTable("my_class5")]
    public class MyClassSqlite : MyClassBase
    {

    }

    [MapTable("my_class5_join")]
    public class MyClassJoinPostgres : MyClassBase
    {
        public MyClassJoinPostgres()
        {

        }
    }


}