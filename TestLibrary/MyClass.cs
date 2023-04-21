using ORM_1_21_;
using System.Collections.Generic;


namespace TestLibrary
{

    [MapTable("my_class5")]
    public class MyClass : MyClassBase
    {
        public MyClass(int t)
        {

        }

        public MyClass()
        {
        }
    }
    [MapTable("my_class5")]
    public class MyClassMysql : MyClassBase { }
    [MapTable("my_class5")]
    public class MyClassPostgres : MyClassBase { }

    [MapTable()]
    class MyClassMsSql : MyClassBase { }
    [MapTable("my_class5")]
    public class MyClassSqlite : MyClassBase { }


    [MapTable("my_class51")]
    public class MyClassMysql1 : MyClassBase { }
    [MapTable("my_class51")]
    class MyClassPostgres1 : MyClassBase { }

    [MapTable()]
    class MyClassMsSql1 : MyClassBase { }
    [MapTable("my_class51")]
    public class MyClassSqlite1 : MyClassBase { }

    [MapTable("my_class5_join")]
    public class MyClassJoinPostgres : MyClassBase
    {
        public MyClassJoinPostgres()
        {

        }

    }


}