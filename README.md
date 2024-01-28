#### ORM-1_21
+ [Table Map](#tablemap)
+ [Attribute](#attribute)
+ [Linq to SQL](#linqtosql)
+ [Native SQL](#nativesql)
+ [Transaction](#transaction)
+ [Interfaces](#interfaces)
+ [Accessing another database](#anotherdatabase)
+ [Work with subclasses](#subclass)
+ [Working with geometry 2d model](#geometry)
+ [Working with the Json type](#json)
+ [Method: WhereIn,WhereNotIn,SelectSql,SelectSqlE,WhereSql, FromSql](#wheresql)
+ [The concept of persistence](#persistence)
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
<a name="wheresql"></a> 
##### Method: WhereIn,WhereNotIn,SelectSql,SelectSqlE,WhereSql, FromSql.
Table:
```c#
[MapTable("m_sql")]
class MSql
{
    [MapPrimaryKey("id", Generator.Assigned)]
    public Guid Id { get; set; } = Guid.NewGuid();

    [MapColumn("name")]
    public string Name { get; set; }

    [MapColumn("age")]
    public int Age { get; set; }
}
```
WhereIn:
```c#
session.Query<MSql>().WhereIn(a => a.Age, 20, 30).ForEach(a=>Console.WriteLine($"{a.Name}-{a.Age}"));
//SELECT "m_sql"."id", "m_sql"."name", "m_sql"."age" FROM "m_sql" WHERE ("m_sql"."age" IN (20, 30));
session.Query<MSql>().WhereIn(a => a.Name, "Jon", "Ion").ForEach(a => Console.WriteLine($"{a.Name}-{a.Age}"));
//SELECT "m_sql"."id", "m_sql"."name", "m_sql"."age" FROM "m_sql" WHERE("m_sql"."name" IN(@p1, @p2)); params:  @p1 - Jon  @p2 - Ion
```
WhereNotIn:
```c#
session.Query<MSql>().WhereNotIn(a => a.Age, 20, 30).ForEach(a=>Console.WriteLine($"{a.Name}-{a.Age}"));
//SELECT "m_sql"."id", "m_sql"."name", "m_sql"."age" FROM "m_sql" WHERE ("m_sql"."age" NOT IN (20, 30));
session.Query<MSql>().WhereNotIn(a => a.Name, "Jon", "Ion").ForEach(a => Console.WriteLine($"{a.Name}-{a.Age}"));
//SELECT "m_sql"."id", "m_sql"."name", "m_sql"."age" FROM "m_sql" WHERE ("m_sql"."name" NOT IN (@p1, @p2)); params:  @p1 - Jon  @p2 - Ion 
```
WhereSql:
```c#
session.Query<MSql>().WhereSql(a => $"{a.Age}=20 and {a.Name} notnull").ForEach(a=>Console.WriteLine($"{a.Name}-{a.Age}"));
//SELECT "m_sql"."id", "m_sql"."name", "m_sql"."age" FROM "m_sql" WHERE "m_sql"."age"=20 and "m_sql"."name" notnull
session.Query<MSql>().WhereSql(a => $"{a.Age}=@v1 and {a.Name} notnull",new SqlParam("@v1",20)).ForEach(a => Console.WriteLine($"{a.Name}-{a.Age}"));
//SELECT "m_sql"."id", "m_sql"."name", "m_sql"."age" FROM "m_sql" WHERE "m_sql"."age"=@v1 and "m_sql"."name" notnull; params:  @v1 - 20 
```
FromSql
```c#
session.Query<MSql>().FromSql("(select id, name, age from m_sql where age>30) as m_sql").ForEach(a=>Console.WriteLine($"{a.Name}-{a.Age}"));
//SELECT "m_sql"."id", "m_sql"."name", "m_sql"."age" FROM (select id, name, age from m_sql where age>30) as m_sql;
```
SelectSql
```c#
session.Query<MSql>().SelectSql<int>(" age*2 ").ForEach(a=>Console.WriteLine(a));
//SELECT age*2 FROM "m_sql";
```
SelectSqlE
```c#
List<object> res=session.Query<MSql>().SelectSqlE(a => $"Concat('Name:',{a.Name},' - Age:',{a.Age})").ToList();
//SELECT Concat('Name:',"m_sql"."name",' - Age:',"m_sql"."age") FROM "m_sql";
List<object> res=session.Query<MSql>().SelectSqlE(a => $"Concat(@v1,{a.Name},' - @v2',{a.Age})",
new SqlParam("@v1","Name:"),
new SqlParam("@v2","Age:")).ToList();
//SELECT Concat(@v1,"m_sql"."name",' - @v2',"m_sql"."age") FROM "m_sql"; params:  @v1 - Name:  @v2 - Age: 
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
###### Example MySql.
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
###### Example PostgreSQl.
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
###### Example MsSql.
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
###### Example SQLite.
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

<a name="persistence"></a> 
##### The concept of persistence
Attention! Attribute use: MapUsagePersistentAttribute, lengthens the execution of the select command.\
To work with persistence, you need the type to be marked with an attribute:\
```[MapUsagePersistentAttribute]```\
This allows the code to know where the object was received from.
```C#
[MapUsagePersistentAttribute]//For derived types only
[MapTable]
class Foo
{
  [MapPrimaryKey("id",Generator.Assigned)] 
   public Guid Id { get; set; } = Guid.NewGuid();
}
Foo o=new Foo();
bool r = session.IsPersistent(o);
//r -false The object is not from the database


session.Save(o);// Only for working with objects 
                //where the type is marked with an attribute:MapUsagePersistentAttribute
                //The ORM itself determines what to do with the object, insert or update.


r = session.IsPersistent(o);
//r -true The object from the database
r = session.Get<Foo>(o.Id);//Get by primary key
r = session.IsPersistent(o);
//r -true The object from the database
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

<a name="geometry"></a> 
##### Working with geometry 2d model.
Implemented via the IGeoShape interface.\
Attention only: PostgreSql,MySql, MsSql.\
Type Geo object:
```c#
 public enum GeoType
 {
     None,
     Point,
     LineString,
     Polygon,
     MultiPoint,
     MultiLineString,
     MultiPolygon,
     GeometryCollection,
     CircularString,
     PolygonWithHole,
     Empty
 }
```
Table creation example:
```c#
 [MapTable("m_geo")]
  class MGeo
  {
      [MapPrimaryKey("id", Generator.Assigned)]
      public Guid Id { get; set; } = Guid.NewGuid();
  
      [MapColumn("name")]
      public string Name { get; set; }
       
      [MapIndex]
      [MapColumn("my_geo")]
      public IGeoShape MyGeo { get; set; } = FactoryGeo.Empty(GeoType.GeometryCollection);

  }
 // Postgres
 //CREATE TABLE IF NOT EXISTS "m_geo" (
 //"id" UUID  PRIMARY KEY,
 //"name" VARCHAR(256) NULL ,
 //"my_geo" geometry NULL );
 //CREATE INDEX IF NOT EXISTS  "idx_m_geo_my_geo_geom" ON "m_geo" USING gist ("my_geo");
```
Example insert:
```C#
session.DropTableIfExists<MGeo>();
session.TableCreate<MGeo>();

session.Insert(new MGeo
    { Name = "f1", MyGeo = FactoryGeo.Polygon("POLYGON((0 0, 0 5, 10 5, 10 0,0 0))") });
session.Insert(new MGeo
    { Name = "f1", MyGeo = FactoryGeo.Point(1,1) });
session.Insert(new MGeo
    { Name = "f1", MyGeo = FactoryGeo.Point(-1, -1) });

//INSERT INTO "m_geo" ("id", "name", "my_geo") VALUES (@p1,@p2,ST_GeomFromText(@p3, @srid3)) ; params:  @p1 - cbc6bf29-a2fe-4387-8596-0e8d96183ec5  @p2 - f1  @srid3 - 4326  @p3 - POLYGON((0 0, 0 5, 10 5, 10 0,0 0)) 
//INSERT INTO "m_geo" ("id", "name", "my_geo") VALUES (@p1,@p2,ST_GeomFromText(@p3, @srid3)) ; params:  @p1 - 30bbdc30-c3f8-4347-80a9-f368ad4a85fa  @p2 - f1  @srid3 - 4326  @p3 - POINT(1 1) 
//INSERT INTO "m_geo" ("id", "name", "my_geo") VALUES (@p1,@p2,ST_GeomFromText(@p3, @srid3)) ; params:  @p1 - 409400dc-57b2-4dfd-b2b2-89b8730b514a  @p2 - f1  @srid3 - 4326  @p3 - POINT(-1 -1)
```
Default Srid appointed:```FactoryGeo.DefaultSrid=value```\
Instance assignment:
```C#
var shape = FactoryGeo.Point(1, 1).StSetSRID(0); \\or
var shape = FactoryGeo.Point(1, 1).SetSrid(0);
```
Select only points from the table.
```C#
var point = session.Query<MGeo>().Where(a => a.MyGeo.StGeometryType() == "ST_Point").ToList();
// SELECT "m_geo"."id", "m_geo"."name", coalesce(CONCAT('SRID=',ST_SRID("m_geo"."my_geo"),';',ST_AsText("m_geo"."my_geo")),null) as "my_geo" FROM "m_geo" WHERE ( ST_GeometryType("m_geo"."my_geo") = @p1); params:  @p1 - ST_Point 
```
Find only points that lie inside a given polygon.
```C#
var res = session.Query<MGeo>()
 .Where(a => a.MyGeo.StWithin(FactoryGeo.CreateGeo("POLYGON((0 0, 0 5, 20 5, 10 0,0 0))").SetSrid(4326)) == true).
 Where(s=>s.MyGeo.StGeometryType()== "ST_Point").ToList();
//SELECT "m_geo"."id", "m_geo"."name", coalesce(CONCAT('SRID=',ST_SRID("m_geo"."my_geo"),';',ST_AsText("m_geo"."my_geo")),null) as "my_geo" FROM "m_geo" 
//WHERE ( ST_Within("m_geo"."my_geo",ST_GeomFromText(@p1, 4326)) = True) and ( ST_GeometryType("m_geo"."my_geo") = @p2); params:  @p1 - POLYGON((0 0, 0 5, 20 5, 10 0,0 0))  @p2 - ST_Point 
```
Combining geometries (Union).\
Please note that for instances using methods of geographic functions,\
it is required to initialize an instance of the current open ORM session ```SetSession()```
```C#
var geo1 = FactoryGeo.CreateGeo("POINT(1 2)").SetSrid(4326);
var gei2 = FactoryGeo.CreateGeo("POINT(-2 3)").SetSrid(4326);
var uGeo = geo1.SetSession(session).StUnion(gei2);
string str = uGeo.StAsText();//MULTIPOINT(1 2,-2 3)
```
Create GeoJson:
```C#
var geo1 = FactoryGeo.CreateGeo("POINT(1 2)").SetSrid(4326);
var gei2 = FactoryGeo.CreateGeo("POINT(-2 3)").SetSrid(4326);
var gei3 = FactoryGeo.CreateGeo("POLYGON((0 0, 0 5, 20 5, 10 0,0 0))").SetSrid(4326);
var col = FactoryGeo.GeometryCollection(geo1, gei2, gei3);
var jObject = col.GetGeoJson(new { id = Guid.NewGuid(), name = "gc1" });
string json = JsonConvert.SerializeObject(jObject, Formatting.Indented);
```
```json
{
  "type": "GeometryCollection",
  "geometries": [
    {
      "type": "Point",
      "coordinates": [
        1.0,
        2.0
      ]
    },
    {
      "type": "Point",
      "coordinates": [
        -2.0,
        3.0
      ]
    },
    {
      "type": "Polygon",
      "coordinates": [
        [
          [
            0.0,
            0.0
          ],
          [
            0.0,
            5.0
          ],
          [
            20.0,
            5.0
          ],
          [
            10.0,
            0.0
          ],
          [
            0.0,
            0.0
          ]
        ]
      ]
    }
  ],
  "properties": {
    "id": "83af8b9c-101f-44c2-99f0-22bd461f1781",
    "name": "gc1"
  }
}
```
For geometric objects, the following methods are implemented.\
Attention, not all methods work with all databases; this is most complete for PostgreSql.\
Asynchronous methods cannot be used in expression trees, to build a ff query, only to work with an instance object.
```C#
IGeoShape SetSrid(int srid);
GeoType GeoType { get; }
List<GeoPoint> ListGeoPoints { get; }
object GetGeoJson(object properties = null);
List<IGeoShape> MultiGeoShapes { get; }
string StAsText();
string StGeometryType();
Task<string> StGeometryTypeAsync(CancellationToken cancellationToken = default);
double? StArea();
Task<double?> StAreaAsync(CancellationToken cancellationToken = default);
bool? StWithin(IGeoShape shape);
Task<bool?> StWithinAsync(IGeoShape shape, CancellationToken cancellationToken = default);
byte[] StAsBinary();
Task<byte[]> StAsBinaryAsync(CancellationToken cancellationToken = default);
IGeoShape StBoundary();
Task<IGeoShape> StBoundaryAsync(CancellationToken cancellationToken = default);
IGeoShape StBuffer(float distance);
Task<IGeoShape> StBufferAsync(float distance, CancellationToken cancellationToken = default);
IGeoShape StCentroid();
Task<IGeoShape> StCentroidAsync(CancellationToken cancellationToken = default);
IGeoShape StEndPoint();
Task<IGeoShape> StEndPointAsync(CancellationToken cancellationToken = default);
IGeoShape StEnvelope();
Task<IGeoShape> StEnvelopeAsync(CancellationToken cancellationToken = default);
IGeoShape StStartPoint();
Task<IGeoShape> StStartPointAsync(CancellationToken cancellationToken = default);
IGeoShape StSymDifference(IGeoShape shape);
Task<IGeoShape> StSymDifferenceAsync(IGeoShape shape, CancellationToken cancellationToken = default);
IGeoShape StUnion(IGeoShape shape);
Task<IGeoShape> StUnionAsync(IGeoShape shape, CancellationToken cancellationToken = default);
bool? StContains(IGeoShape shape);
Task<bool?> StContainsAsync(IGeoShape shape, CancellationToken cancellationToken=default);
bool? StCrosses(IGeoShape shape);
Task<bool?> StCrossesAsync(IGeoShape shape, CancellationToken cancellationToken = default);
IGeoShape StDifference(IGeoShape shape);
Task<IGeoShape> StDifferenceAsync(IGeoShape shape, CancellationToken cancellationToken = default);
int? StDimension();
Task<int?> StDimensionAsync(CancellationToken cancellationToken = default);
bool? StDisjoint(IGeoShape shape);
Task<bool?> StDisjointAsync(IGeoShape shape, CancellationToken cancellationToken = default);
double? StDistance(IGeoShape shape);
Task<double?> StDistanceAsync(IGeoShape shape, CancellationToken cancellationToken = default);
bool? StEquals(IGeoShape shape);
Task<bool?> StEqualsAsync(IGeoShape shape, CancellationToken cancellationToken = default);
bool? StIntersects(IGeoShape shape);
Task<bool?> StIntersectsAsync(IGeoShape shape, CancellationToken cancellationToken = default);
bool? StOverlaps(IGeoShape shape);
Task<bool?> StOverlapsAsync(IGeoShape shape, CancellationToken cancellationToken = default);
int? StSrid();
bool? StTouches(IGeoShape shape);
Task<bool?> StTouchesAsync(IGeoShape shape, CancellationToken cancellationToken = default);
int? StNumGeometries();
Task<int?> StNumGeometriesAsync(CancellationToken cancellationToken = default);
int? StNumInteriorRing();
Task<int?> StNumInteriorRingAsync(CancellationToken cancellationToken = default);
bool? StIsSimple();
Task<bool?> StIsSimpleAsync(CancellationToken cancellationToken = default);
bool? StIsValid();
Task<bool?> StIsValidAsync(CancellationToken cancellationToken = default);
double? StLength();
Task<double?> StLengthAsync(CancellationToken cancellationToken = default);
bool? StIsClosed();
Task<bool?> StIsClosedAsync(CancellationToken cancellationToken = default);
int? StNumPoints();
Task<int?> StNumPointsAsync(CancellationToken cancellationToken = default);
double? StPerimeter();
Task<double?> StPerimeterAsync(CancellationToken cancellationToken = default);
IGeoShape StTranslate(float deltaX, float deltaY);
Task<IGeoShape> StTranslateAsync(float deltaX, float deltaY, CancellationToken cancellationToken = default);
IGeoShape SetSession(ISession session);
IGeoShape StConvexHull();
Task<IGeoShape> StConvexHullAsync(CancellationToken cancellationToken = default);
IGeoShape StCollect(params IGeoShape[] shapes);
IGeoShape StPointN(int n);
Task<IGeoShape> StPointNAsync(int n, CancellationToken cancellationToken = default);
IGeoShape StPointOnSurface();
Task<IGeoShape> StPointOnSurfaceAsync(CancellationToken cancellationToken = default);
IGeoShape StInteriorRingN(int n);
Task<IGeoShape> StInteriorRingNAsync(int n, CancellationToken cancellationToken = default);
double? StX();
Task<double?> StXAsync(CancellationToken cancellationToken = default);
double? StY();
Task<double?> StYAsync(CancellationToken cancellationToken=default);
IGeoShape StTransform(int srid);
Task<IGeoShape> StTransformAsync(int srid, CancellationToken cancellationToken = default);
IGeoShape StSetSRID(int srid);
string StAsLatLonText(string format =null );
Task<object> StAsLatLonTextAsync(string format, CancellationToken cancellationToken = default);
IGeoShape StReverse();
Task<IGeoShape> StReverseAsync(CancellationToken cancellationToken = default);
string StIsValidReason();
Task<string> StIsValidReasonAsync(CancellationToken cancellationToken = default);
IGeoShape StMakeValid();
Task<IGeoShape> StMakeValidAsync(CancellationToken cancellationToken = default);
string StAsGeoJson();
Task<string> StAsGeoJsonAsync(CancellationToken cancellationToken = default);
```
<a name="json"></a> 
##### Working with the Json type.
Table creation examples: (PostgreSql)
```C#
[MapTable("m_json")]
class MJson
{
    [MapPrimaryKey("id", Generator.Assigned)]
    public Guid Id { get; set; } = Guid.NewGuid();

    [MapColumn("name")]
    public string Name { get; set; }
   
    [MapColumn("json")]
    [MapColumnTypeJson]
    public JsonBody Body { get; set; } 
}

class JsonBody
{
    public string Name { get; set; }
    public string Description { get; set;}
}
// CREATE TABLE IF NOT EXISTS "m_json" (
// "id" UUID  PRIMARY KEY,
// "name" VARCHAR(256) NULL ,
// "json" jsonb NULL );
```
Insert:
```c#
session.Insert(new MJson { Name = "j1", Body = new JsonBody { Name = "n1", Description = "simple" } });

```
Update:
```c#
session.Query<MJson>().Update(a => new Dictionary<object, object>
{
    { a.Body, new JsonBody { Name = "u1" } }
});
session.Query<MJson>().Update(a => new Dictionary<object, object>
{
    { a.Body, JsonConvert.SerializeObject(new JsonBody { Name = "u2" }) }
});
session.Query<MJson>().UpdateSql(a => $"{a.Body}='{JsonConvert.SerializeObject(new JsonBody { Name = "u3" })}'");
var json= session.Query<MJson>().First().Body.Name;//u3
```
Select:\
Select all records where the Json field contains the property  "Name" equal to u3
```c#
var u3 = session.Query<MJson>().WhereSql(a => $"{a.Body} @> '"+"{\"Name\":\"u3\"}'").ToList();
```
Option with type Object.\
Please note that the argument ```[MapColumnTypeJson(TypeReturning.AsString)]```\
can be used with an object; in this case, all requests will be returned as a json string
```C#
[MapTable("m_json")]
class MJson
{
    [MapPrimaryKey("id", Generator.Assigned)]
    public Guid Id { get; set; } = Guid.NewGuid();

    [MapColumn("name")]
    public string Name { get; set; }
   
    [MapColumn("json")]
    [MapColumnTypeJson(TypeReturning.AsString)]
    public object Body { get; set; } 
}
```
Insert:
```c#
session.Insert(new MJson { Name = "j1", Body = new { Name = "n0", Description = "simple" } });
session.Insert(new MJson { Name = "j2", Body = new JsonBody { Name = "n1", Description = "simple" } });
//or
session.Insert(new MJson { Name = "j3", Body = JsonConvert.SerializeObject(new JsonBody { Name = "n1", Description = "simple" }) });

```






