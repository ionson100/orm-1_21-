using System.Collections.Generic;
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
    public class MyClassJoinPostgres : MyClassBase, IEqualityComparer<MyClassJoinPostgres>
    {
        public MyClassJoinPostgres()
        {

        }

        public bool Equals(MyClassJoinPostgres x, MyClassJoinPostgres y)
        {
            if (y != null && x != null && x.Age == y.Age)
            {
                return true;
            }

            return false;
        }

        public int GetHashCode(MyClassJoinPostgres obj)
        {
            return 0;
        }
    }


}