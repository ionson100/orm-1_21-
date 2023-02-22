using ORM_1_21_;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using TestLibrary;
using System.Drawing;

namespace TestPostgres
{
    internal class Program
    {
        private static ProviderName _providerName = ProviderName.MsSql;

        static async Task Main(string[] args)
        {
            string s1 = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=audi124;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            string s2 = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=test;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

            switch (_providerName)
            {
                case ProviderName.MsSql:
                    Starter.Run(s1, _providerName);
                    break;
                case ProviderName.MySql:
                    Starter.Run("Server=localhost;Database=test;Uid=root;Pwd=12345;", _providerName);
                    break;
                case ProviderName.Postgresql:
                    Starter.Run("Server=localhost;Port=5432;Database=testorm;User Id=postgres;Password=ion100312873;", _providerName);
                    break;
                case ProviderName.Sqlite:
                    Starter.Run("Data Source=mydb.db;Version=3;BinaryGUID=False;", _providerName);//
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            Execute.RunThread();
            Console.ReadKey();
            Execute.TotalTest();

            ISession session = Configure.Session;

            if (session.TableExists<MyImage>() )
            {
                session.DropTable<MyImage>();
              
            } 
            session.TableCreate<MyImage>();

            string a = session.TableName<MyClass>();

            MyImage sd = new MyImage();
            sd.Image=Image.FromFile("1.png");
            session.Save(sd);
            var sdsd = session.Querion<MyImage>().ToList();
            sdsd[0].Image.Save("assa.png");


            
            var myClass = new MyClass
            {
                Age = 12,
                Name = "name",
                MyEnum = MyEnum.First,
               // MyTest = new MyTest { Name = "ass" },
                //Test23 =new List<MyTest>() { new MyTest(), new MyTest() }
            };
           
            session.Save(myClass);
            session.Save(new MyClass{Age = 10,Name = "name1"});
            session.Save(new MyClass { Age = 10, Name = "name1" });
            session.Save(new MyClass { Age = 10, Name = "name2" });
            var err = session.Querion<MyClass>().Where(sw => sw.Age == 10).Update(d => new Dictionary<object, object>
            {
                { d.Name,string.Concat(d.Name,d.Age)},
                { d.DateTime,DateTime.Now}
            });

            Console.ReadKey();
        }

       

      


    }
    [MapTableName("image_my")]
    class MyImage
    {
        [MapPrimaryKey("id",Generator.Assigned)]
        public Guid Id { get; set; }=Guid.NewGuid();
        [MapColumnName("image")]
        public Image Image { get; set; }

    }
}
