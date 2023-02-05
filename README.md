#### ORM-1_21
ОРМ ( MySql,PostgreSQL,MSSQL,Sqlite)\
Допускает обращение к разным базам данных (MSSQL,Postgresql,MySQL,Sqlite) из одного контекста приложения.\
Реализация в стиле HiberNate;\
При старте инициализируем Configure,базой по умолчанию, проверяем создание таблиц (CodeFirst)\
Потом из любого места обращаемся к базе.\
Статическая инициализация:
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
```
В режиме отладки создается файл лога:SqlLog.txt\
Внимание: Файл не лимитирован по длине!\
Куда пишутся запросы к базе, и информация о ошибках.
```sql
INSERT INTO "my_class" ("name", "age", "desc", "enum", "date", "test")VALUES (@p1,@p2,@p3,@p4,@p5,@p6) RETURNING "id"; params:  @p1 - ion100  @p2 - 11  @p3 - simple  @p4 - 1  @p5 - 21.01.2023 11:39:42  @p6 - [{"Name":"simple"}] 
INSERT INTO "my_class" ( "name","age","desc","enum","date","test") VALUES('ion100',12,'simple',1,'2023-01-21 11:39:42.703','[{"Name":"simple"}]'),
('ion100',13,'simple',1,'2023-01-21 11:39:42.703','[{"Name":"simple"}]')
SELECT "my_class"."age"  FROM  "my_class" WHERE ((("my_class"."age" > @p1) or ("my_class"."name" LIKE CONCAT(@p2,'%'))) and ("my_class"."name" LIKE CONCAT('%',@p3,'%'))) ORDER BY "my_class"."age" Limit 2 OFFSET 0 params:  @p1 - 5  @p2 - ion100  @p3 - 100 
SELECT "my_class"."id", "my_class"."name", "my_class"."age", "my_class"."desc", "my_class"."enum", "my_class"."date", "my_class"."test"  FROM  "my_class" WHERE ("my_class"."name" is not null) LIMIT 1
```
Выбор типа базы по умолчанию, осуществляется вторым параметром конструктора.\
Первый параметр - строка подключения: [ConnectionString](https://www.connectionstrings.com)\
ОРМ предполагает автономное развертывание, исходя их этого требуется добавлении пакетов провайдера для\
выбранной базы данных, через NuGet (Npgsql,Mysql.Data,System.Data.SQLite,System.Data.SqlClient)\
**Внимание:Ограничения для PostgreSQL.**\
Хранение даты осуществляется в старом режиме.\
Корректировку:
```C#
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
```
ОРМ добавляет сама.
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
        public int Age { get; set; }

        [MapColumnName("desc")]
        [MapColumnType("TEXT NULL")]
        public string Description { get; set; }

        [MapColumnName("enum")] 
        public MyEnum MyEnum { get; set; } = MyEnum.First;

        [MapColumnName("date")] 
        public DateTime DateTime { get; set; } = DateTime.Now;

        [MapColumnName("test")] 
        public List<MyTest> Test23 { get; set; } = new List<MyTest>() { new MyTest() { Name = "simple" }
    };
```
Внимание! Свойство IList, проецируются в базу как Text.И сохраняются как Json,\
что естественно сказывается на низкую производительность.
Свойства не отмеченные атрибутом, в таблицу не проецируются.\
Свойства помеченные индексом встают в таблицу одним собирательным индексом,\
если нужно отдельно - нужно строить отделенный запрос при создании таблицы.\
Наличие свойства первичного ключа - обязательно, и только одно.\
Генератор первичного ключа: Generator.Native, создает автоинкрементное поле в таблице.\
Минимальная длина (int)\
Для типа базы SQLite, генератор должен быть - Generator.Native.\
Данный тип мапится в PosgreSql  в виде:
```sql
CREATE TABLE IF NOT EXISTS "my_class" (
 "id" UUID  PRIMARY KEY,
 "name" VARCHAR(256) NULL ,
 "age" INTEGER NOT NULL DEFAULT '0' ,
 "desc" TEXT NULL ,
 "enum" INTEGER NOT NULL DEFAULT '0' ,
 "date" TIMESTAMP NULL ,
 "test" TEXT NULL );
CREATE INDEX IF NOT EXISTS INDEX_my_class_age ON "my_class" ("age");
```
###### Ling To SQL.
Отложенное выполнение или запрос по требованию.\
Внимание! Не все конструкции реализованы в Визиторе, особенно для SQlite
Пример запроса:
```C#
 var list = Configure.Session.Querion<MyClass>().
 Where(a => (a.Age > 5||a.Name.StartsWith("ion100"))&&a.Name.Contains("100")).
 OrderBy(d=>d.Age).
 Select(f=>new {age=f.Age}).
 Limit(0,2).
 ToList(); 
```
real sql:
```sql
SELECT "my_class"."age"  FROM  "my_class" 
WHERE ((("my_class"."age" > @p1) or ("my_class"."name" LIKE CONCAT(@p2,'%'))) and ("my_class"."name" LIKE CONCAT('%',@p3,'%'))) 
ORDER BY "my_class"."age" Limit 2 OFFSET 0 
params:  @p1 - 5  @p2 - ion100  @p3 - 100 
```
###### native sql.
```C#
var ses1 = Configure.Session;
var list = ses1.FreeSql<MyClass>($"select * from {ses1.TableName<MyClass>()} where \"name\" LIKE CONCAT(@p1,'%')",
           new Parameter("@p1", "ion100")).ToList();

var dataTable = ses1.GetDataTable($"select * from {ses1.TableName<MyClass>()}");//or
var dataTable = ses1.GetDataTable($"select * from {ses1.TableName<MyClass>()} where age = @1",-1,12);
var coutn = dataTable.Rows.Count;
```
FreeSql - быстрый запрос к базе ( не отложенный)\
Запрос по одному полю:
```C#
 foreach (var r in Configure.Session.FreeSql<MyEnum>("select enum as enum1 from my_class"))
   Console.WriteLine($" enum={r}");
```
запрос по нескольким полям:
```C#
 foreach (var r in Configure.Session.FreeSql<dynamic>("select enum as enum1,age from my_class"))
   Console.WriteLine($" enum1={r.enum1} age={r.age}");
```
Типизированный запрос по нескольким полям или джойн.\
Обязательно нужно определить тип, свойства замапить атрибутами, имена колонок должны соответствовать\
именам полей в запросе, равно и как количество.
```C#
 [MapTableName("nomap")]
    class MyClassTemp
    {
        [MapColumnName("id")]
        public int Id { get; set; }
        [MapColumnName("enum1")]
        public MyEnum MyEnum { get; set; }
        [MapColumnName("age")]
        public int Age { get; set; }
        public override string ToString()
        {
            return $" Id={Id}, MyEnum={MyEnum}, Age={Age}";
        }
    }
 foreach (var r in Configure.Session.FreeSql<MyClassTemp>("select enum as enum1, age, id from my_class"))
    Console.WriteLine($"{r}");
```
Перечисление анонимных типов, можно сделать через метод обертку:
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

        void IValidateDal<T>.Validate(T item){}// действе перед вставкой и обновлением
    }
```
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
если потом этот объект попытаться сохранить: ses.Save(o), то ОРМ попытается сделать запрос UPDATE.\
При выполнении асинхронных запросов,иногда приведение типов не очень удачно работает,\
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
###### Обращение к дугой базе данных.
Для обращения к другой DB, есть метод получения сессии 
```C#
var session=Configure.GetSession<TS>()
```
Где TS тип который реализует интерфейс
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
Если обращений к базе будет много , можно реализовать как singleton
```C#
class MyDbMySql : IOtherDataBaseFactory
 {
     private static readonly Lazy<DbProviderFactory> dbProviderFactory = new Lazy<DbProviderFactory>(() =>
     {
        return new MySqlClientFactory();
     });
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
         return dbProviderFactory.Value;
     }
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
var r1 = session.Querion<MyClassMysql>().Where(d => d.Age != 123)
                .Select(f => new { name = f.Name });
var task = r1.ToListAsync();
var mysql = await task;
```
Сессия открыта только для обращения к базе MySql.\
Следует учитывать. ВАЖНО.\
Если вы использовали тип, в примере MyClassMysql, к обращению к одной базе, к другой базе\
этот тип в контексте App, уже обращаться не может (кэширование)\
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
Один класс для обращения к Postgresql (TablePostgres)\
Другой класс (TableMysql) для обращения к MySql.



