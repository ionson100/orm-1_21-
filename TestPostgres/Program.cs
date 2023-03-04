using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ORM_1_21_;
using ORM_1_21_.Extensions;
using TestLibrary;


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
                    Starter.Run(s2, _providerName);
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
            

            var d = new MyTest() { Name = "simple" };
            var g = new Guid("87ae6aba-086e-49e3-b569-1145b0a2744e");
            var data = new DateTime(2023, 3, 4);
            List<MyClass> list22 = new List<MyClass>();
            for (int i = 0; i < 10; i++)
            {
                list22.Add(new MyClass() { Age = 30, Name = "name1", MyTest = new MyTest { Name = "simple" } });
                list22.Add(new MyClass() { Age = 10, Name = "name2", MyTest = new MyTest { Name = "simple" } });
            }
            session.InsertBulk(list22);
            var err = session.Query<MyClass>().Select(a => new { sd = a.Name.Length }).ToList();

            var yee = new decimal(100);
            var asss = session.Query<MyClass>().ToListAsync().Result;


           //session.InsertBulk(list22);
           session.Query<MyClass>().Update(a => new Dictionary<object, object>()
           {
               { a.Age,  new int()}
           });
           session.Query<MyClass>().Delete(a => a.Age == new int()); 
            asss = session.Query<MyClass>().ToListAsync().Result;
            ////////////////////////////
           // session.InsertBulk(list22);
           // session.Query<MyClass>().Update(a => new Dictionary<object, object>()
           // {
           //     { a.MyTest,  new MyTest() { Name = "simple" }}
           // });
           // session.Query<MyClass>().Delete(a => a.MyTest == new MyTest() { Name = "simple" });
           //  asss = session.Query<MyClass>().ToListAsync().Result;
           // ////////////////////////////
            session.InsertBulk(list22);

            session.Query<MyClass>().Update(a => new Dictionary<object, object>()
            {
                { a.MyTest,  d}
            });
            session.Query<MyClass>().Delete(a => a.MyTest == d);
             asss = session.Query<MyClass>().ToListAsync().Result;
////////////////////////////
            session.InsertBulk(list22);

            session.Query<MyClass>().Update(a => new Dictionary<object, object>()
            {
                { a.ValGuid,  g}
            });
            session.Query<MyClass>().Delete(a => a.ValGuid == g); 
            asss = session.Query<MyClass>().ToListAsync().Result;
            /////////////////////
            session.InsertBulk(list22);

            session.Query<MyClass>().Update(a => new Dictionary<object, object>()
            {
                { a.ValGuid,  new Guid("87ae6aba-086e-49e3-b569-1145b0a2744e")}
            });
            session.Query<MyClass>().Delete(a => a.ValGuid == new Guid("87ae6aba-086e-49e3-b569-1145b0a2744e"));
            asss = session.Query<MyClass>().ToListAsync().Result;
            /////////////////////
            session.InsertBulk(list22);

            session.Query<MyClass>().Update(a => new Dictionary<object, object>()
            {
                { a.ValGuid,  g}
            });
            session.Query<MyClass>().Delete(a => a.ValGuid == g);
            asss = session.Query<MyClass>().ToListAsync().Result;
            /////////////////////
            session.InsertBulk(list22);

            session.Query<MyClass>().Update(a => new Dictionary<object, object>()
            {
                { a.DateTime,  data}
            });
            asss = session.Query<MyClass>().Where(a => a.DateTime == data).ToList();
            session.Query<MyClass>().Delete(a => a.DateTime == data);
            asss = session.Query<MyClass>().ToListAsync().Result;
            /////////////////////
            session.InsertBulk(list22);

            session.Query<MyClass>().Update(a => new Dictionary<object, object>()
            {
                { a.DateTime,   new DateTime(2023, 3, 4)}
            });
            session.Query<MyClass>().Delete(a => a.DateTime == new DateTime(2023, 3, 4));
            asss = session.Query<MyClass>().ToListAsync().Result;
            /////////////////////










            Console.ReadKey();
        }

        static int GetInt()
        {
            return 10;
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
