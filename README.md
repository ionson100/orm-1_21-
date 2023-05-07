#### ORM-1_21
+ [Table Map](#tablemap)
+ [Attribute](#attribute)
+ [Linq to SQL](#linqtosql)
+ [Native SQL](#nativesql)
+ [Transaction](#transaction)
+ [Interfaces](#interfaces)
+ [Accessing another database](#anotherdatabase)
+ [Work with subclasses](#subclass)
+ [License](./LICENSE.md)


Simple micro ОРМ ( MySql, PostgreSQL, MSSQL, Sqlite).\
Allows access to different databases (MSSQL, Postgresql, MySQL, Sqlite) from one application context.\
CodeFirst, Linq to sql, free sql.
#### Restrictions.
All bases must be created before use, with the exception of Sqlite,\
 if the file does not exist, the ORM will create it.\
Write to log file=debug mode only.\
install database provider from NuGet: (Npgsql, Mysql.Data, System.Data.SQLite, System.Data.SqlClient).\
Enum  type is stored in the database as an integer.\
Primary key is Allowed on one field.
#### Quick start
```C#
using ORM_1_21_;

string path = null;
#if DEBUG
    path = "SqlLog.txt";
#endif
_ = new Configure("ConnectionString",ProviderName.PostgreSql, path);
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
    using (ISession session = Configure.Session)
    {
        session.InsertBulk(new List<MyClass>(){
                new MyClass{ Age = 40, Name = "name40"},
                new MyClass{ Age = 20, Name = "name20" },
                new MyClass{ Age = 30, Name = "name30" },
                new MyClass{ Age = 50, Name = "name" },
                new MyClass{ Age = 60, Name = "name" },
                new MyClass{ Age = 10, Name = "name" },
            }
        );
        session.Query<MyClass>().Where(a => a.Age < 50).ForEach(s =>
        {
            Console.WriteLine($@"{s.Name} - {s.Age}");
        });
    } 
```


**Please note: Restrictions for PostgreSQL.**\
The date is stored in the olden mode.\
Correction:
```C#
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
```
ORM adds itself.
<a name="tablemap"></a> 
#### Tables Map.
```C#
using ORM_1_21_;

 [MapTable("my_class")]// or [MapTable]
 class MyClass
 {
     [MapPrimaryKey("id", Generator.Assigned)]//or [MapPrimaryKey(Generator.Assigned)]
      public Guid Id { get; set; } = Guid.NewGuid();

     [MapColumn("name")] //or [MapColumn]
     public string Name { get; set; }

     [MapColumn("age")] //or [MapColumn]
     [MapIndex]
     [MapDefaultValue("NOT NULL DEFAULT '5'")] 
      public int Age { get; set; }

     [MapColumn("desc")] //or [MapColumn]
     [MapColumnType("TEXT")]
      public string Description { get; set; }

     [MapColumn("date")] //or [MapColumn] 
      public DateTime DateTime { get; set; } = DateTime.Now;
 }

```
<a name="attribute"></a> 
#### Attribute.
###### Table name:
```C#
using ORM_1_21_;


//using Postgresql
class PeopleAllBase
{
    [MapPrimaryKey("id", Generator.Native)]
    public long Id { get; set; }
    [MapColumn("name")]
    public string Name { get; set; }
    [MapColumn("age")]
    public int Age { get; set; }
}

[MapTable("People")]
class PeopleAll : PeopleAllBase {}

[MapTable("People"," \"age\" < 55")]
class PeopleYoung :PeopleAllBase {}

[MapTable("People", " \"age\" >= 55")]
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
      session.Insert(new PeopleAll() { Age = i * 10, Name = "simpleName" });
  }
  session.Query<PeopleAll>().ForEach(a =>
  {
      Console.WriteLine($@" {a.Name} {a.Age}");
  });
  //sql:SELECT "People"."id", "People"."name", "People"."age" FROM "People"

   session.Query<PeopleYoung>().Where(a => a.Name.StartsWith("simple")).ForEach(b =>
  {
      Console.WriteLine($@" Young {b.Name} {b.Age}");
  });
  //sql:SELECT "People"."id", "People"."name", "People"."age" FROM "People" 
  //WHERE ("People"."name" LIKE CONCAT(@p1,'%')) and ( "age" < 55) params:  @p1 - simple 

   session.Query<PeopleOld>().Where(a=>a.Name.StartsWith("simple")).ForEach(b =>
  {
      Console.WriteLine($@" Old {b.Name} {b.Age}");
  });
  //sql:SELECT "People"."id", "People"."name", "People"."age" FROM "People" 
  //WHERE ("People"."name" LIKE CONCAT(@p1,'%')) and ( "age" >= 55) params:  @p1 - simple 
```
###### Column Name,Index,Not Insert and Update,Type,Default value
```C#
using ORM_1_21_;


public class TestTSBase
{
    [MapPrimaryKey("id", Generator.Assigned)]
    public Guid Id { get; set; } = Guid.NewGuid();
    [MapIndex]
    [MapColumn("name")] 
    public string Name { get; set; }
}

[MapTable("TS2")]
public class TSMsSql : TestTSBase
{
    [MapNotInsertUpdate]
    [MapColumn("ts")]
    [MapColumnType("rowversion")]
    [MapDefaultValue("")]
    public byte[] Ts { get; set; } 
}
 var session=Configure.Session;
 session.TableCreate<TSMsSql>();
 session.Insert(new TSMsSql { Name = "123" });
 var t=session.Query<TSMsSql>().Single();
 session.Update(t);
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

```[MapPrimaryKey("id", Generator.Assigned)]``` - Assigned by user.\
```[MapPrimaryKey("id", Generator.Native)]``` - Designates a database as an auto-increment column.\
```[MapPrimaryKey(Generator.Assigned)]``` - Assigned by user. Column name from name property 
Example: Postgres uuid primary key auto generate.
```C#
// postgres
[MapTable("test_uuid")]
class TestUuid
{
    [MapColumnType(" uuid")]
    [MapDefaultValue("DEFAULT uuid_generate_v4() PRIMARY KEY")]
    [MapPrimaryKey("id",Generator.Native)]
    public Guid id { get; set; }
    [MapColumn("name")]
    public string Name { get; set; }   
}
```
```sql
CREATE TABLE IF NOT EXISTS "test_uuid"
(
 "id" uuid NOT NULL DEFAULT uuid_generate_v4() PRIMARY KEY,
 "name" character varying(256) ,
)
```
```[MapPrimaryKey("id", Generator.NativeNotReturningId,)]``` - Designates a database,At insertion record, id record is not referred on client.\
Example: Mysql uuid primary key auto generate.
```C#
[MapTable]
class TestMySqlUUid
{
    [MapColumnType("binary(16) default (uuid_to_bin(uuid()))")]
    [MapDefaultValue("not null primary key")]
    [MapPrimaryKey("id", Generator.NativeNotReturningId)]
    public Guid Id { get; set; }
    [MapColumn]
    public string Name { get; set; }
}
```
###### Serialization to JSON



```C#
class Foo
{
  [MapColumn("my_test")]
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
 
*By default, no constructor and initializer is called for map type.\
To make this work, the type must be marked ```MapUsageActivatorAttribute```\
Having a default constructor*
```C#
[MapUsageActivator]
class Foo
{
  public Fo(){}
  public int IntFoo{get;set;}=5;
}
```

<a name="linqtosql"></a> 
#### Ling To SQL.

![alt text](https://github.com/ionson100/ORM-1_21-/blob/master/Linq.png)

Methods: Join, GroupJoin, Concat, Cast, Select, SelectMany, Aggregate, GroupBy, Except\
have no implementation, they are overridden method name Core ending.
```C#
Example:
session.Query<MyClass>().Join(session.Query<MyClass>(), a => a.Age, b => b.Age,
                    (aa, bb) => new { name1=aa.Name, name2=bb.Name }).ToList();
// Error: Method Join for IQueryable is not implemented, use method JoinCore or ...toList().Join()
Alternative:
session.Query<MyClass>().JoinCore(session.Query<MyClass>(), a => a.Age, b => b.Age,
                     (aa, bb) => new { name1 = aa.Name, name2 = bb.Name }).ToList();
Or:
session.Query<MyClass>().ToList().Join(session.Query<MyClass>(), a => a.Age, b => b.Age,
                     (aa, bb) => new { name1 = aa.Name, name2 = bb.Name }).ToList();
```

```C#
using ORM_1_21_;


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
*Replace expression in queries ``` "str1"+"str2"``` to ```string.Concat("str1","str2")```*
###### Sql Builder:
```C#
ISession session = Configure.Session;

var sql = $"select * from {session.TableName<MyClass>()} where {session.ColumnName<MyClass>(a => a.Age)} > {session.SymbolParam}1";
// for Postgres: select * from "my_class5" where "age" > @1
// for MSSQL   : select * from [my_class5] where [age] > @1
// for MySql   : select * from `my_class5` where `age` > ?1
// for SqLite  : select * from "my_class5" where "age" > @1
var res=session.FreeSql<MyClass>(sql, 10);
```

###### Extensions:
```C#
static class Help
{
    public static IEnumerable<object> Foo<TSource>(this IQueryable<TSource> source, Func<TSource, object> func)
    {
        var res = source.Provider.Execute<IEnumerable<TSource>>(source.Expression);
        foreach (var re in res)
        {
            yield return func(re);
        }
    }

    public static async Task<IEnumerable<object>> FooAsync<TSource>(this IQueryable<TSource> source,
        Func<TSource, object> func, CancellationToken cancellationToken = default)
    {
        var res = await source.Provider.ExecuteAsync<IEnumerable<TSource>>(source.Expression, cancellationToken);
        return res.Select(func);
    }
}

 foreach (var o in await session.Query<MyClass>().Select(a => new { a.Age }).FooAsync(a => a.Age + 30))
 {
     Console.WriteLine(o);
 }
```


<a name="nativesql"></a> 
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
###### example: (using attribute:```MapReceiverFreeSqlAttribute```) the presence of a constructor with parameters
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
###### example: simple class 
```c#

 сlass MySimpleFreeSql
 {
     public Guid id { get; }
     public string name { get; }
   
 }
.....

var tempFree = session.FreeSql<MyFreeSql>($"select id, name  {session.TableName<MyClass>()}");
//Caution!
//Table column names in sql query ( id ,name)
//Must match the name of the public properties of the type

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
###### example: (anonymous type)
```C#
  var res = Configure.Session.FreeSqlAsTemplate(new { id = 1, number = 1 },
  $"select id, number from {session.TableName<MyClass>()}").ToList();
```
<a name="interfaces"></a> 
#### Interfaces.
```C#
 class MyClass : IMapAction<MyClass>
 {
   public void ActionCommand(MyClass item, CommandMode mode)
   {
       
   }
 }
....
 public enum CommandMode
 {
     /// <summary>
     /// None
     /// </summary>
     None,
     /// <summary>
     /// Before Update
     /// </summary>
     BeforeUpdate,
     /// <summary>
     /// Before Insert
     /// </summary>
     BeforeInsert,
     /// <summary>
     /// Before Delete
     /// </summary>
     BeforeDelete,
     /// <summary>
     /// After Update
     /// </summary>
     AfterUpdate,
     /// <summary>
     /// After Insert
     /// </summary>
     AfterInsert,
     /// <summary>
     /// After Delete
     /// </summary>
     AfterDelete,
 }
```
<a name="transaction"></a> 
#### Transaction.
```C#
 ISession session = Configure.Session;
 var tr=session.BeginTransaction();
 try
 {
    session.Insert(new MyClass { Age=6,DateTime=DateTime.Now,Description="bla"});
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
 using (ISession session = await Configure.SessionAsync)
 {
    using (var tr = await session.BeginTransactionAsync())
    {
        // insert, update 
    }
 }
```
<a name="anotherdatabase"></a> 
#### Accessing another database.
Getting a session
```C#
var session=Configure.GetSession<TS>() //where TS : IOtherDataBaseFactory ,new()
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
*The session is opened only for this database\
Type to use only for a specific database*
##### Stored Procedures
Example Mysql:
```sql
DROP procedure IF EXISTS `getCountList`;
CREATE  PROCEDURE `getCountList`(IN maxAge int,out myCount int)
BEGIN
  select count(*) INTO myCount from my_class5 where age < maxAge;
  select * from my_class5  where age < maxAge;
END;
DROP procedure IF EXISTS `getList`;

CREATE PROCEDURE `getList`()
BEGIN
    select * from my_class5;
END;
```
```C#
var list =  (IEnumerable<MyClass>)session.ProcedureCall<MyClass>("getList");
var par1 = new ParameterStoredPr("maxAge", 100, ParameterDirection.Input);
var par2 = new ParameterStoredPr("myCount", 120, ParameterDirection.Output);
list = (IEnumerable<MyClass>) session.ProcedureCallParam<MyClass>("getCountList", par1, par2);
var count=par2.Value;
```

<a name="subclass"></a> 
##### Work with subclasses

```c#
[MapTable("test_sub_class")]//Mandatory table name
class TSuperClass
{
    [MapPrimaryKey(Generator.Assigned)]
    public Guid Id { get; set; } = Guid.NewGuid();
    [MapColumn]
    public string Name { get; set; }
}

class TSubClass : TSuperClass
{
    [MapColumn]
    public int Age { get; set; }
}

class TCoreClass : TSubClass
{
    [MapColumn]
    public string Description { get; set; }
}
...
session.DropTableIfExists<TCoreClass>();
session.TableCreate<TCoreClass>();
session.InsertBulk(new List<TCoreClass>()
{
    new TCoreClass(){Age = 20,Name = "20",Description = "20"},
    new TCoreClass(){Age = 20,Name = "20",Description = "20"},
    new TCoreClass(){Age = 20,Name = "20",Description = "20"},
});
var s1 = session.Query<TCoreClass>().ToList();
var s2 = session.Query<TSubClass>().ToList();
var s3 = session.Query<TSuperClass>().ToList();

var tableName = session.TableName<TSubClass>();
var s4 = session.FreeSql<TSuperClass>($"select * from {tableName}");
var s5 = session.FreeSql<TSubClass>($"select * from {tableName}");
var s6 = session.FreeSql<TCoreClass>($"select * from {tableName}");
```





