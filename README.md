#### ORM-1_21
ОРМ ( MySql,PostgreSQL,MSSQL,Sqlite)\
Allows access to different databases (MSSQL,Postgresql,MySQL,Sqlite) from one application context.\
CodeFirst, Linq to sql,Query caching.
###### Restrictions.
All bases must be created before use, with the exception of Sqlite,\
 if the file does not exist, the ORM will create it.\
Write to log file=debug mode only.\
install database provider from NuGet (Npgsql,Mysql.Data,System.Data.SQLite,System.Data.SqlClient)
###### Quick start
```C#
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
###### Мапинг таблиц.
```C#
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

###### Ling To SQL.
Отложенное выполнение или запрос по требованию.\
Внимание! Не все конструкции реализованы в Визиторе, особенно для SQlite
Пример запроса:
```C#
var list = Configure.Session.Query<MyClass>().
Where(a => (a.Age > 5 || a.Name.StartsWith("nam")) && a.Name.Contains("1")).
OrderBy(d => d.Age).
Select(f => new { age = f.Age }).
Limit(0, 2).
ToList();
```
real sql:
```sql
SELECT "my_class5"."age" FROM "my_class5" WHERE ((("my_class5"."age" > 5) 
or ("my_class5"."name" LIKE CONCAT(@p1,'%'))) and 
("my_class5"."name" LIKE CONCAT('%',@p2,'%'))) ORDER BY "my_class5"."age" Limit 2 OFFSET 0;
params:  @p1 - nam  @p2 - 1 
```
###### native sql.
example:
```C#
var ses1 = Configure.Session;
var list = ses1.FreeSql<MyClass>($"select * from {ses1.TableName<MyClass>()} where \"name\" LIKE CONCAT(@p1,'%')",
    new Parameter("@p1", "ion100")).ToList();
// where MyClass have MapAttribute
```
example: (dynamic)
```C#
 foreach (var r in Configure.Session.FreeSql<dynamic>("select enum as enum1,age from my_class"))
   Console.WriteLine($" enum1={r.enum1} age={r.age}");
```
example: (one field)
```C#
 foreach (var r in Configure.Session.FreeSql<MyEnum>("select enum as enum1 from my_class"))
   Console.WriteLine($" enum={r}");
```
example: (using attribute:```MapReceiverFreeSqlAttribute```)
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
example: (anonymous type)
```C#
 static IEnumerable<T> TempSql<T>(T t)
 {
   return Configure.Session.FreeSql<T>("select enum as enum1,age from my_class");
 }
 foreach (var r in TempSql(new { enum1 = 1, age = 2 }))
    Console.WriteLine($"{r}");
```
Рефлексия для собирания результата запроса, реализована на компиляции деревьев выражений\
[Тынц](https://github.com/ionson100/AccessGetSet)\
Что дает хороший прирост производительности.
###### Интерфейсы.
```C#
 class T : IActionDal<T>,IValidateDal<T>
 {
    void IActionDal<T>.AfterDelete(T item){}

    void IActionDal<T>.AfterInsert(T item){}

    void IActionDal<T>.AfterUpdate(T item){}

    void IActionDal<T>.BeforeDelete(T item){}

    void IActionDal<T>.BeforeInsert(T item){}

    void IActionDal<T>.BeforeUpdate(T item){}

    void IValidateDal<T>.Validate(T item){}// вызывается перед вставкой и обновлением
 }
```
Хорошее место сделать медиатор или декоратор.\
\
Объект ( на основе маппинга) полученный из базы,\
или сохраненный удачно в базе, имеет признак персистентный:
```C#
var isP = ses.IsPersistent(list[0]);
```
сделать объект персистентным ( как бы он получен из базы):
```C#
var o=new MyClass();
ses.ToPersistent(o);
```
если потом этот объект попытаться сохранить: ```ses.Save(o)```, то ОРМ попытается сделать запрос UPDATE.\
При выполнении асинхронных запросов, иногда приведение типов не очень удачно работает,\
например:
```C#
await Configure.Session.Querion<MyClass>().Where(s=>s.Name!=null).GroupBy(f=>f.Age).ToListAsync().ContinueWith(f =>
       {
         Console.WriteLine(f.Result.Count());
       });
```
Error...\
Всегда можно заменить:
```C#
 await Configure.Session.Querion<MyClass>().Where(s=>s.Name!=null).ToListAsync().ContinueWith(f =>
 {
   var res=f.Result.GroupBy(s => s.Age);
   Console.WriteLine(res.Count());
 });
```
Запрос к базе не изменится.
```sql
SELECT "my_class"."id", "my_class"."name", "my_class"."age", "my_class"."desc", "my_class"."enum", "my_class"."date", "my_class"."test" 
FROM "my_class" WHERE ("my_class"."name" is not null)
```
###### sql transaction.
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
При работе с Sqlite и MsSql express, обращайте внимание на изоляцию транзакций,\
из за специфики файловой структуры базы.
###### Обращение к дугой базе данных.
Для обращения к другой DB, есть метод получения сессии 
```C#
var session=Configure.GetSession<TS>()
```
Где TS тип который реализует интерфейс,```IOtherDataBaseFactory```\
тип должен иметь открытый конструктор.
```C#
  /// <summary>
  ///Интерфейс для  обращение к чужой базе данных
  /// </summary>
 public interface IOtherDataBaseFactory
 {
    /// <summary>
    /// Тип базы данных 
    /// </summary>
    ProviderName GetProviderName();

    /// <summary>
    /// Получение Провайдера для выбранной базы данных
    /// </summary>
    DbProviderFactory GetDbProviderFactories();

    /// <summary>
    /// Строка подключения к базе данных
    /// </summary>
    string GetConnectionString();
 }
```

Пример реализации для приложения где орм настроена на Postgresql, а есть желание\
обращаться и к MySql.
```C#
 class MyDbMySql : IOtherDataBaseFactory
 {
    public ProviderName GetProviderName()
    {
        return ProviderName.MySql;
    }
    public string GetConnectionString()
    {
        return "Server=localhost;Database=test;Uid=root;Pwd=12345;";
    }
    
    public DbProviderFactory GetDbProviderFactories()
    {
        return MySql.Data.MySqlClient.MySqlClientFactory.Instance;
    }
 }
/*******************/
var session=Configure.GetSession<MyDbMySql>();
var r1 = session.Querion<MyClassMysql>()
                .Where(d => d.Age != 123)
                .Select(f => new { name = f.Name });
var task = r1.ToListAsync();
var mysql = await task;
```
Сессия открыта только для обращения к базе MySql.\
Следует учитывать. ВАЖНО.\
Если вы использовали тип, в примере MyClassMysql, к обращению к одной базе, к другой базе\
этот тип в контексте приложения, уже обращаться не может (кэширование).\
Может возникнуть ошибка нарушения синтаксиса SQL.\
Если структура таблиц в разных базах одинакова можно сделать трюк с наследованием
```C#
class BaseMap
{
  [MapColumnName("id")]
  public Guid Id { get; set; } = Guid.NewGuid();

  [MapColumnName("name")]
  public string Name { get; set; }
}
[MapTableName("my_class")]
class TablePostgres:BaseMap{}

[MapTableName("my_class")]
class TableMysql:BaseMap{}
```
Один тип, для обращения к Postgresql (TablePostgres)\
Другой тип (TableMysql), для обращения к MySql.
###### Что такое сессия.
Легковесный объект, реализующий IDisposable.\
Является прокси для доступа к базе, в нем создаются объекты для доступа к базе.\
Хороший тон - вызывать Dispose (using).\
Если не вызывать, а писать в простом стиле : ``` Configure.Session.FreeSql... .```\
Ошибки не будет, все объекты для доступа к базе получат своевременно Dispose.\
Но будет вызван финализатор для сессии, когда подойдет время для очистки кучи.\
Если вы используете транзакцию, лучше все же использовать using.(try/finaly)\
Все метаданные типов, для маппинга таблиц, кэшируются при первом обращении.\
Тип ```IOtherDataBaseFactory```   инстанцируется и кэшируется при первом обращении,\
если менять его реализацию на лету, то за кэшируется первая реализация, изменения учитываться не будут.\
Если сессия получила Dispose - этой сессии пользоваться нельзя, возникнет исключение.\
Сессий много не бывает.




