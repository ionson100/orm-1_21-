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
        private static ProviderName _providerName = ProviderName.Postgresql;

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
           
            List<MyClass> list22 = new List<MyClass>();
            for (int i = 0; i < 500; i++)
            {
                list22.Add(new MyClass() { Age = 30, Name = "name1", MyTest = new MyTest { Name = "simple" } });
                list22.Add(new MyClass() { Age = 10, Name = "name2", MyTest = new MyTest { Name = "simple" } });
            }

          
            var inter = session.InsertBulk(list22);

            List<IGrouping<string, MyClass>> list = await Configure.Session.Query<MyClass>().GroupBy(a => a.Name)
                .ToLisAsync();
            var asss = session.Query<MyClass>().Where(s => s.Name != null).ToListAsync().Result;




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
