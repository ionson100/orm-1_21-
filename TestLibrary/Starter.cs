using ORM_1_21_;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestLibrary
{
   public static class Starter
    {
        public static void Run(string conSre,ProviderName provider )
        {

            string path = @"myLog.txt"; ;
#if DEBUG
            path = @"myLog.txt";
#endif
            _ = new Configure(conSre,provider, path);
            using (var ses = Configure.Session)
            {
                if (ses.TableExists<MyClass>())
                {
                    ses.DropTable<MyClass>();
                }
                if (ses.TableExists<MyClass>() == false)
                {
                   var e= ses.TableCreate<MyClass>();
                }
            }
        }

        public static MyClass GetMyClass(int age, string name)
        {
            return new MyClass { Age = age, Name = name };
        }
    }
}
