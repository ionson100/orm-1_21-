#### ORM-1_21
ОРМ ( MySql,PostgreSQL,MSSQL,Sqlite)\
Allows access to different databases (MSSQL,Postgresql,MySQL,Sqlite) from one application context.\
CodeFirst, Linq to sql,Query caching.
#### Restrictions.
All bases must be created before use, with the exception of Sqlite,\
 if the file does not exist, the ORM will create it.\
Write to log file=debug mode only.\
install database provider from NuGet (Npgsql,Mysql.Data,System.Data.SQLite,System.Data.SqlClient)
#### Quick start
```C#
using ORM_1_21_;
using ORM_1_21_.Extensions;


string path = null;
#if DEBUG
    path = "SqlLog.txt";
#endif
_ = new Configure("ConnectionString",ProviderName.Postgresql, path);
    using (var ses=Configure.Session)
    {
      if (ses.TableExists<MyClass>())
       {
         ses.DropTable<MyClass>();
       }
      if (ses.TableExists<MyClass>() == false)
       {
         ses.TableCreate<MyClass>();
       }
    } 
   ISession session = Configure.Session;
   ssion.InsertBulk(new List<MyClass>()
     new MyClass() { Age = 40, Name = "name" ,MyTest = new MyTest{Name = "simple"}},
     new MyClass() { Age = 20, Name = "name1",MyTest = new MyTest{Name = "simple"}},
     new MyClass() { Age = 30, Name = "name2",MyTest = new MyTest{Name = "simple"}},
     new MyClass() { Age = 50, Name = "name3",MyTest = new MyTest{Name = "simple"}},
     new MyClass() { Age = 60, Name = "name4",MyTest = new MyTest{Name = "simple"}},
     new MyClass() { Age = 10, Name = "name5",MyTest = new MyTest{Name = "simple"}},
   ); 
   session.Query<MyClass>().Where(a=>a.Age<50).ForEach(s =>
   {
       Console.WriteLine($@"{s.Name} - {s.Age}");
   });
   session.Dispose();   
```


**Please note: Restrictions for PostgreSQL.**\
The date is stored in the olden mode.\
Correction:
```C#
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
```
ORM adds itself.
#### Tables Map.
```C#
using ORM_1_21_;
using ORM_1_21_.Extensions;


 [MapTableName("my_class")]
 class MyClass
 {
     [MapPrimaryKey("id", Generator.Assigned)]
      public Guid Id { get; set; } = Guid.NewGuid();

     [MapColumnName("name")] 
     public string Name { get; set; }

     [MapColumnName("age")] 
     [MapIndex]
     [MapDefaultValue("NOT NULL DEFAULT '5'")] 
      public int Age { get; set; }

     [MapColumnName("desc")]
     [MapColumnType("TEXT")]
      public string Description { get; set; }

     [MapColumnName("enum")] 
      public MyEnum MyEnum { get; set; } = MyEnum.First;

     [MapColumnName("date")] 
      public DateTime DateTime { get; set; } = DateTime.Now;

     [MapColumnName("test")] 
      public List<MyTest> Test23 { get; set; } = new List<MyTest>() { new MyTest() { Name = "simple" }
      
     [MapColumnName("my_test")]
      public MyTest MyTest { get; set; }
 }

 [MapSerializable]
 class MyTest 
 {
     public string Name { get; set; }
 }
```
#### Attribute.
###### Table name:
```C#
using ORM_1_21_;
using ORM_1_21_.Extensions;

//using Postgresql
class PeopleAllBase
{
    [MapPrimaryKey("id", Generator.Native)]
    public long Id { get; set; }
    [MapColumnName("name")]
    public string Name { get; set; }
    [MapColumnName("age")]
    public int Age { get; set; }
}

[MapTableName("People")]
class PeopleAll : PeopleAllBase {}

[MapTableName("People"," \"age\" < 55")]
class PeopleYoung :PeopleAllBase {}

[MapTableName("People", " \"age\" >= 55")]
class PeopleOld : PeopleAllBase { }
.......
  ISession session = Configure.Session;
  if (session.TableExists<PeopleAll>())
  {
      session.DropTable<PeopleAll>();
  }

  session.TableCreate<PeopleAll>();
  for (int i = 0; i < 10; i++)
  {
      session.Save(new PeopleAll() { Age = i * 10, Name = "simpleName" });
  }
  session.Query<PeopleAll>().ForEach(all =>
  {
      Console.WriteLine($@" {all.Name} {all.Age}");
  });
  //sql:SELECT "People"."id", "People"."name", "People"."age" FROM "People"

   session.Query<PeopleYoung>().Where(a => a.Name.StartsWith("simple")).ForEach(all =>
  {
      Console.WriteLine($@" Young {all.Name} {all.Age}");
  });
  //sql:SELECT "People"."id", "People"."name", "People"."age" FROM "People" 
  //WHERE ("People"."name" LIKE CONCAT(@p1,'%')) and ( "age" < 55) params:  @p1 - simple 

   session.Query<PeopleOld>().Where(a=>a.Name.StartsWith("simple")).ForEach(all =>
  {
      Console.WriteLine($@" Old {all.Name} {all.Age}");
  });
  //sql:SELECT "People"."id", "People"."name", "People"."age" FROM "People" 
  //WHERE ("People"."name" LIKE CONCAT(@p1,'%')) and ( "age" >= 55) params:  @p1 - simple 
```
###### Column Name,Index,Not Insert and Update,Type,Default value
```C#
using ORM_1_21_;
using ORM_1_21_.Extensions;

public class TestTSBase
{
    [MapPrimaryKey("id", Generator.Assigned)]
    public Guid Id { get; set; } = Guid.NewGuid();
    [MapIndex]
    [MapColumnName("name")] 
    public string Name { get; set; }
}

[MapTableName("TS2")]
public class TSMsSql : TestTSBase
{
    [MapNotInsertUpdate]
    [MapColumnName("ts")]
    [MapColumnType("rowversion")]
    [MapDefaultValue("")]
    public byte[] Ts { get; set; } 
}
 var session=Configure.Session;
 session.TableCreate<TSMsSql>();
 session.Save(new TSMsSql { Name = "123" });
 var t=session.Query<TSMsSql>().Single();
 session.Save(t);
 var res = session.Update(t, new AppenderWhere(session.ColumnName<TSMsSql>(d => d.Ts), t.Ts));
```
```sql
// using MsSql
IF not exists (select 1 from information_schema.tables where table_name = 'TS2')CREATE TABLE [dbo].[TS2](
[id] uniqueIdentifier default (newId()) primary key,
 [ts] rowversion ,
 [name] [NVARCHAR] (256) NULL,
);
CREATE INDEX [INDEX_TS2_name] ON [TS2] (name);

INSERT INTO [TS2] ([TS2].[id], [TS2].[name]) VALUES (@p1,@p2) ; params:  @p1 - 03ced090-affd-4cf5-be48-3329267b2b98  @p2 - 123 
SELECT TOP (2)  [TS2].[ts] AS ts2____ts, [TS2].[name] AS ts2____name, [TS2].[id] AS ts2____id FROM [TS2]; 
UPDATE [TS2] SET  [TS2].[name] = @p1 WHERE [TS2].[id] = @p2; params:  @p1 - 123  @p2 - 03ced090-affd-4cf5-be48-3329267b2b98 
UPDATE [TS2] SET  [TS2].[name] = @p1 WHERE [TS2].[id] = @p2   AND [ts] = @p3; params:  @p1 - 123  @p2 - 03ced090-affd-4cf5-be48-3329267b2b98  @p3 - System.Byte[] 
 
```
###### Primary Key

```[MapPrimaryKey("id", Generator.Assigned)]``` - Assigned by user\
```[MapPrimaryKey("id", Generator.Native)]``` - Designates a database as an auto-increment column

###### Serialization to JSON
```C#
 class Foo
 {
     [MapColumnName("mylist")]
     public List<MyItem> MyList { get; set; } = new List<MyItem>() { new MyItem() { Name = "simple" }
 }
ORM serializes the property type List<> as a JSON into a table with a text column
```

```C#
class Foo
{
  [MapColumnName("my_test")]
  public MyTest MyTest { get; set; }
}

[MapSerializable]
class MyTest 
{
  public string Name { get; set; }
}
The ORM serializes the type marked with MapSerializableAttribute as JSON into a table with a text column.
```

```C#
class Foo
{
  [MapColumnName("my_test")]
  public F MyTest { get; set; }
}

public class F: IMapSerializable
{ 
  public string Serialize()
  {    
  }
  public void Deserialize(string str)
  {          
  }
}
User Serialization. Using the Interface IMapSerializable.
```

<span style="color:red">Important</span>.
 
By default, no constructor and initializer is called for map type.\
To make this work, the type must be marked ```MapUsageActivatorAttribute```\
Having a default constructor
```C#
[MapUsageActivator]
class Foo
{
  public Fo(){}
  public int IntFoo{get;set;}=5;
}
```
#### Persistence
All objects received or stored in the database are persistent.\
On this marker, the method chooses to insert or update command.
```C#
MyClass myClass = new MyClass();
ISession  session = Configure.Session;
bool isPer = session.IsPersistent(myClass);// false
session.Save(myClass); // Magic Insert command
isPer = session.IsPersistent(myClass); //trye
myClass=session.Query<MyClass>().First();
isPer =session.IsPersistent(myClass); //trye
session.Save(myClass);// Magic  update command
isPer = session.IsPersistent(myClass); //trye
```





















#### Ling To SQL.

```C#
using ORM_1_21_;
using ORM_1_21_.Extensions;


var expSql = Configure.Session.Query<MyClass>().
Where(a => (a.Age > 5 || a.Name.StartsWith("nam")) && a.Name.Contains("1")).
OrderBy(d => d.Age).
Select(f => new { age = f.Age }).
Limit(0, 2);
var list=expSql.ToList();
```
real sql:
```sql
// Postgresql
SELECT "my_class5"."age" FROM "my_class5" WHERE ((("my_class5"."age" > 5) 
or ("my_class5"."name" LIKE CONCAT(@p1,'%'))) and 
("my_class5"."name" LIKE CONCAT('%',@p2,'%'))) ORDER BY "my_class5"."age" Limit 2 OFFSET 0;
params:  @p1 - nam  @p2 - 1 
```
###### Update:
```C#
var count = session.Query<MyClass>().Where(a => a.Age == 10).Update(d => new Dictionary<object, object>
{
  { d.Name, string.Concat(d.Name, d.Age) },
  { d.DateTime, DateTime.Now }
});
```
###### Delete
```C#
session.Query<MyClass>().Delete(a => a.Age == 10);
```
###### Insert Bulk:
```C#
var count=session.InsertBulk(new List<MyClass>()
{
   new MyClass { Age = 40, Name = "name", MyTest = new MyTest { Name = "simple" } },
   new MyClass { Age = 20, Name = "name1", MyTest = new MyTest { Name = "simple" } },
});
```
###### Distinct:
```C#
var d1 = Configure.Session.Query<MyClass>().Distinct(a => a.Name);
var d2 = Configure.Session.Query<MyClass>().Distinct(a => new {a.Age,a.Name});
```
###### SetTimeOut сurrent request:
```C#
var f = await session.Query<MyClass>().Where(a => a.Age > 0).SetTimeOut(30).SingleOrDefaultAsync();
```
<span style="color:red">Important</span>\
Replace expression in queries ``` "str1"+"str2"``` to ```string.Concat("str1","str2")```
######  Caching:
```C#
var res = session.Query<MyClass>().Where(a => a.Age = 10).CacheUsage().ToList();//First call to create cache
res = session.Query<MyClass>().Where(a => a.Age = 10).CacheUsage().ToList();//Next calls - get from cache
res = session.Query<MyClass>().Where(a => a.Age = 10).CacheOver().ToList();//If cache exists, will be overwritten
session.CacheClear<MyClass>();//Removing all caches for a type MyClass.
```
<span style="color:red">Important</span>\
Removing all caches for a type MyClass, when calling any command Insert or Update.
###### Sql Builder:
```C#
ISession session = Configure.Session;
// select * from my_class where age=10;
string sql = $" select * from {session.TableName<MyClass>()} where {session.ColumnName<MyClass>(a=>a.Age)} = @1";
var res=session.FreeSql<MyClass>(sql, 10);
```



#### Native SQL.
###### example class map native:
```C#
var session = Configure.Session;
var list = session.FreeSql<MyClass>($"select * from {session.TableName<MyClass>()} where \"name\" LIKE CONCAT(@p1,'%')","name").ToList();
// where MyClass have MapAttribute
```
###### example: (dynamic)
```C#
 foreach (var r in Configure.Session.FreeSql<dynamic>("select enum as enum1,age from my_class"))
   Console.WriteLine($" enum1={r.enum1} age={r.age}");
```
###### example: (one column)
```C#
 foreach (var r in Configure.Session.FreeSql<MyEnum>("select enum as enum1 from my_class"))
   Console.WriteLine($" enum={r}");
```
###### example: (using attribute:```MapReceiverFreeSqlAttribute```)
```c#
[MapReceiverFreeSql]
 lass MyFreeSql
 {
     public Guid IdGuid { get; }
     public string Name { get; }
     public int Age { get; }
     public MyEnum MyEnum { get; }
     public MyFreeSql(Guid idGuid, string name, int age, MyEnum @enum)
     {
         IdGuid = idGuid;
         Name = name;
         Age = age;
         MyEnum = (MyEnum)@enum;
     }
 }
.....

var tempFree = session.FreeSql<MyFreeSql>($"select id,name,age,enum from {session.TableName<MyClass>()}");
//Caution!
//Sequence of types from select: select id,name,age,enum 
//Must match the sequence of constructor parameter types.: MyFreeSql(Guid idGuid, string name, int age, MyEnum @enum)

```
###### example: (anonymous type)
```C#
 static IEnumerable<T> TempSql<T>(T t)
 {
   return Configure.Session.FreeSql<T>("select enum as enum1,age from my_class");
 }
 foreach (var r in TempSql(new { enum1 = 1, age = 2 }))
    Console.WriteLine($"{r}");
```

#### Interfaces.
```C#
 class MyClass : IMapAction<MyClass>
 {
   public void ActionCommand(MyClass item, CommandMode mode)
   {
       
   }
 }
```

#### Transaction.
```C#
 ISession session = Configure.Session;
 var tr=session.BeginTransaction();
 try
 {
    session.Save(new MyClass { Age=6,DateTime=DateTime.Now,Description="bla"});
    tr.Commit();
 }catch(Exception)
 {
    tr.Rollback();
    throw;
 }
 finally
 {
    session.Dispose();
 }  
```
```C#
 using (ISession session = Configure.Session)
 {
   using (var tr = session.BeginTransaction())
   {
       // insert update 
   }
 }
```

#### Accessing another database.
Getting a session
```C#
var session=Configure.GetSession<TS>()
```
The type that implements the interface:```IOtherDataBaseFactory```\
the type must have a public constructor.
```C#
 /// <summary>
 /// Interface for accessing a foreign database
 /// </summary>
 public interface IOtherDataBaseFactory
 {
     /// <summary>
     /// Type database
     /// </summary>
     ProviderName GetProviderName();

     /// <summary>
     /// Getting the DbProviderFactory for the selected database
     /// </summary>
     DbProviderFactory GetDbProviderFactories();

     /// <summary>
     /// Database connection string
     /// </summary>
     string GetConnectionString();
 }

```
###### example MySql
```C#
 public  class MyDbMySql : IOtherDataBaseFactory
 {
   private static readonly Lazy<DbProviderFactory> DbProviderFactory = 
       new Lazy<DbProviderFactory>(() => new MySqlClientFactory());
   public ProviderName GetProviderName()
   {
       return ProviderName.MySql;
   }
   public string GetConnectionString()
   {
       return ConnectionStrings.Mysql;
   }

   public DbProviderFactory GetDbProviderFactories()
   {
       return DbProviderFactory.Value;
   }
 }
```
###### example Postgres
```C#
public class MyDbPostgres : IOtherDataBaseFactory
{
  private static readonly Lazy<DbProviderFactory> DbProviderFactory = 
      new Lazy<DbProviderFactory>(() => Npgsql.NpgsqlFactory.Instance);
  public ProviderName GetProviderName()
  {
      return ProviderName.Postgresql;
  }
  public string GetConnectionString()
  {
      return ConnectionStrings.Postgesql;
  }

  public DbProviderFactory GetDbProviderFactories()
  {
      return DbProviderFactory.Value;
  }
}
```
###### example MsSql
```C#
public class MyDbMsSql : IOtherDataBaseFactory
{
    private static readonly Lazy<DbProviderFactory> DbProviderFactory = 
        new Lazy<DbProviderFactory>(() => System.Data.SqlClient.SqlClientFactory.Instance);
    public ProviderName GetProviderName()
    {
        return ProviderName.MsSql;
    }
    public string GetConnectionString()
    {
        return ConnectionStrings.MsSql;
    }

    public DbProviderFactory GetDbProviderFactories()
    {
        return DbProviderFactory.Value;
    }
}
```
###### example SQLite
```C#
public class MyDbSqlite : IOtherDataBaseFactory
{
    private static readonly Lazy<DbProviderFactory> DbProviderFactory = 
        new Lazy<DbProviderFactory>(() => System.Data.SQLite.SQLiteFactory.Instance);
    public ProviderName GetProviderName()
    {
        return ProviderName.Sqlite;
    }
    public string GetConnectionString()
    {
        return ConnectionStrings.Sqlite;
    }

    public DbProviderFactory GetDbProviderFactories()
    {
        return DbProviderFactory.Value;
    }
}
```
<span style="color:red">Important</span>\
The session is opened only for this database\
Type to use only for a specific database\






