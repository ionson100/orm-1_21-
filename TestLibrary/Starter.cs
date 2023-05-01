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

            string path = null; ;
#if DEBUG
            path = @"myLog.txt";
#endif
            _ = new Configure(conSre,provider,path);
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
                ses.InsertBulk(new List<MyClass>
                    {
                        new MyClass{ Age = 40, Name = "name40" },
                        new MyClass{ Age = 40, Name = "name40" },
                        new MyClass{ Age = 20, Name = "name20" },
                        new MyClass{ Age = 20, Name = "name20" },
                        new MyClass{ Age = 20, Name = "name20" },
                        new MyClass{ Age = 20, Name = "name20" },
               
                    }
                );
            }
        }

        public static MyClass GetMyClass(int age, string name)
        {
            return new MyClass(1){ Age = age, Name = name };
        }
    }
}
