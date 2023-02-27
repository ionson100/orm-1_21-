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
        private static ProviderName _providerName = ProviderName.Sqlite;

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
            //Execute.RunThread();
            //Console.ReadKey();
            //Execute.TotalTest();
            
            ISession session = Configure.Session;

            session.Save(new MyClass()
            {
                Age = 22,
                MyTest = new MyTest() { Name = "asas" }
            });
            session.Save(new MyClass()
            {
                MyTest = new MyTest() { Name = "asas" }
            });
            session.Save(new MyClassSqlite());
            var anons = Execute.TempSql(new { age = 3, name = "asss" }, session, $"select getint32(age) as age,name from {session.TableName<MyClass>()}");
            var anon = Execute.TempSql(new { age = 3, name = "" }, session, session.TableName<MyClass>());
           

            
            session.Save(new MyClass()
            {
                MyTest = new MyTest(){Name = "asas"}
            });
            session.Save(new MyClass()
            {
                MyTest = new MyTest() { Name = "asas" }
            });
            session.Save(new MyClass()
            {
                MyTest = new MyTest() { Name = "asas" }
            });

            // if (session.TableExists<MyClassNull>() )
            // {
            //     session.DropTable<MyClassNull>();
            //   
            // } 
            // session.TableCreate<MyClassNull>();
            // MyClassNull @null = new MyClassNull()
            // {
            //    // V5 = true
            // };
            // session.Save(@null);
            var ss = session.Query<MyClassNull>().ToList();
            var iis = session.Query<MyClass>().Where(a => a.Age<1000).CacheUsage().ToList();
             iis = session.Query<MyClass>().Where(a => a.Age < 1000).CacheUsage().ToList();
             var sasskey = session.Query<MyClass>().Where(a => a.Age < 1000).CacheGetKey();
             iis = (List<MyClass>)session.CacheGetValue<MyClass>(sasskey);

            //var asas = session.Query<MyClass>().Where(a => a.Age == 12).CacheUsage().ToList();
            //asas = session.Query<MyClass>().Where(a => a.Age == 12).CacheUsage().ToList();

            //tring a = session.TableName<MyClass>();
            //
            //   MyImage sd = new MyImage();
            //   sd.Image = Image.FromFile("1.png");
            //   session.Save(sd);
            //
            //
            //   MyImage sd = new MyImage();
            //   sd.Image = Image.FromFile("1.png");
            //   session.Save(sd);
            //
            //
            //
            //ar sdsd = session.Querion<MyImage>().ToList();
            //dsd[0].Image.Save("assa.png");
            //
            //
            //
            //ar myClass = new MyClass
            //
            //   Age = 12,
            //   Name = "name",
            //   MyEnum = MyEnum.First,
            //   MyTest = new MyTest { Name = "ass" },
            //   Test23 =new List<MyTest>() { new MyTest(), new MyTest() }
            //;
            //
            //ession.Save(myClass);
            session.Save(new MyClass{Age = 10,Name = "name1"});
            session.Save(new MyClass { Age = 10, Name = "name1" });
            session.Save(new MyClass { Age = 10, Name = "name2" });
            var erer=session.ProcedureCall<object>("getList");
            var ass=
            
            //ar err = session.Querion<MyClass>().Where(sw => sw.Age == 10).Update(d => new Dictionary<object, object>
            //
            //   { d.Name,string.Concat(d.Name,d.Age)},
            //   { d.DateTime,DateTime.Now}
            //);
            //ar ss = session.Querion<MyClass>().ToList();

            Console.ReadKey();
        }

       

      


    }
    [MapTableName("image_my")]
    class MyImage
    {
        [MapPrimaryKey("id",Generator.Native)]
        public long Id{ get; set; }
        [MapColumnName("image")]
        public Image Image { get; set; }
    }

    [MapTableName("null_table")]
    class MyClassNull
    {
        [MapPrimaryKey("id", Generator.Native)]
        public long Id { get; set; }


            //[MapColumnName("v2")] public uint? V2 { get; set; } = 1;
            //[MapColumnName("v3")]public ulong? V3 { get; set; }
            //[MapColumnName("v4")]public ushort? V4 { get; set; }
            //[MapColumnName("v14")]public sbyte? V14 { get; set; }
           

          [MapColumnName("v1")]public string V1 { get; set; }
      
         [MapColumnName("v5")] public bool? V5 { get; set; } = true;
         
         [MapColumnName("v6")] public byte? V6 { get; set; } = 56;
         
         [MapDefaultValue("NOT NULL DEFAULT '0'")]
         [MapColumnName("v7")] public char V7 { get; set; } = 'y';
         [MapColumnName("v8")]public DateTime? V8 { get; set; }= DateTime.Now;
         [MapColumnName("v9")]public decimal? V9 { get; set; }
         [MapColumnName("v10")]public double? V10 { get; set; }
         [MapColumnName("v11")]public short? V11 { get; set; }
         [MapColumnName("v12")]public int? V12 { get; set; }
         [MapColumnName("v13")]public long? V13 { get; set; }
         //
         [MapColumnName("v15")]public float? V15 { get; set; }
         [MapColumnName("v16")]public Guid V16 { get; set; }
       
        
          //[MapColumnName("v19")] public byte[] V21 { get; set; } = new byte[] { 5 };
    }
}
