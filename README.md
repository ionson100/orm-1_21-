#### ORM-1_21
ОРМ ( MySql,PostgreSQL,MSSQL,Sqlite)\
Реализация в стиле HiberNate;\
При старте инициализируем Configure, проверяем создание таблиц (CodeFirst)\
Потом из любого места обращаемся к базе.\
Статическая инициализация:
```
string path = null;
#if DEBUG
    path = "SqlLog.txt";
#endif
_ = new Configure("ConnectionString",
    ProviderName.Postgresql, path);
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
```
INSERT INTO "my_class" ("name", "age", "desc", "enum", "date", "test")VALUES (@p1,@p2,@p3,@p4,@p5,@p6) RETURNING "id"; params:  @p1 - ion100  @p2 - 11  @p3 - simple  @p4 - 1  @p5 - 21.01.2023 11:39:42  @p6 - [{"Name":"simple"}] 
INSERT INTO "my_class" ( "name","age","desc","enum","date","test") VALUES('ion100',12,'simple',1,'2023-01-21 11:39:42.703','[{"Name":"simple"}]'),
('ion100',13,'simple',1,'2023-01-21 11:39:42.703','[{"Name":"simple"}]')
SELECT "my_class"."age"  FROM  "my_class" WHERE ((("my_class"."age" > @p1) or ("my_class"."name" LIKE CONCAT(@p2,'%'))) and ("my_class"."name" LIKE CONCAT('%',@p3,'%'))) ORDER BY "my_class"."age" Limit 2 OFFSET 0 params:  @p1 - 5  @p2 - ion100  @p3 - 100 
SELECT "my_class"."id", "my_class"."name", "my_class"."age", "my_class"."desc", "my_class"."enum", "my_class"."date", "my_class"."test"  FROM  "my_class" WHERE ("my_class"."name" is not null) LIMIT 1
```
Выбор типа базы, осуществляется вторым параметром конструктора.\
Первый параметр - строка подключения: [ConnectionString](https://www.connectionstrings.com)\
ОРМ предполагает автономное разветывание, исходя их этого требуется добавлении пакетов провайдера для\
выбранной базы данных, через NuGet (Npgsql,Mysql.Data,System.Data.SQLite,System.Data.SqlClient)\
Внимание: Для PostgreSQL, хранение даты осуществляется в старом режиме.
###### Мапинг таблиц.
```
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
что ествественно сказывается на низкую производительность.
Свойства не отмеченные атрибутом, в таблицу не проецируются.\
Свойства помеченные индексом встают в таблицу одним собирательным индексом,\
если нужно отдельно - нужно строить отделный запрос при создании таблицы.\
Наличие свойства первичного ключа - обязательно, и только одно.\
Генератор первичного ключа: Generator.Native, создает автоинкрементное поле в таблице.\
Минимальная длина (int)\
Для типа базы SQLite, генеатор должен быть - Generator.Native.\
Данный тип мапится в PosgreSql  в виде:
```
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
Внимание! Не все конструкции реализованы в Визиторе, особенно для SQlite
Пример запроса:\
```
 var list = Configure.Session.Querion<MyClass>().
 Where(a => (a.Age > 5||a.Name.StartsWith("ion100"))&&a.Name.Contains("100")).
 OrderBy(d=>d.Age).
 Select(f=>new {age=f.Age}).
 Limit(0,2).
 ToList();
 nativ sql:
 SELECT "my_class"."age"  FROM  "my_class" 
 WHERE ((("my_class"."age" > @p1) or ("my_class"."name" LIKE CONCAT(@p2,'%'))) and ("my_class"."name" LIKE CONCAT('%',@p3,'%'))) 
 ORDER BY "my_class"."age" Limit 2 OFFSET 0 
 params:  @p1 - 5  @p2 - ion100  @p3 - 100 
 
```
###### native sql.
```
var ses1 = Configure.Session;
var list = ses1.FreeSql<MyClass>($"select * from {ses1.TableName<MyClass>()} where \"name\" LIKE CONCAT(@p1,'%')",
           new Parameter("@p1", "ion100")).ToList();

var dataTable = ses1.GetDataTable($"select * from {ses1.TableName<MyClass>()}");//or
var dataTable = ses1.GetDataTable($"select * from {ses1.TableName<MyClass>()} where age = @1",-1,12);
var coutn = dataTable.Rows.Count;
```
Рефликсия для собирания результата запроса, рализована на компиляции деревьев выражений\
[Тынц](https://github.com/ionson100/AccessGetSet)\
Что дает хороший прирост производительности.
###### Интерфейсы.
```
 class T : IActionDal<T>,IValidateDal<T>
    {
        void IActionDal<T>.AfterDelete(T item){}

        void IActionDal<T>.AfterInsert(T item){}

        void IActionDal<T>.AfterUpdate(T item){}

        void IActionDal<T>.BeforeDelete(T item){}

        void IActionDal<T>.BeforeInsert(T item){}

        void IActionDal<T>.BeforeUpdate(T item){}

        void IValidateDal<T>.Validate(T item){}// действуе перед вставкой и обновлением
    }
```
Объект ( на основе маппинга) полученный из базы,\
или сохраненный удачно в базе, имеет признак персистентности:
```C#
var isP = ses.IsPersistent(list[0]);
```
сделать объект персистентным ( как бы получен из базы):
```C#
var o=new MyClass();
ses.ToPersistent(o);
```
если потом этот объект попытаться сохранить: ses.Save(o), то ОРМ попытается сделать запрос UPDATE.\
При выполнении аснихронных запросов,иногда приведение типов не очень удачно работает,\
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
Зпрос к базе не изменится.
```sql
SELECT "my_class"."id", "my_class"."name", "my_class"."age", "my_class"."desc", "my_class"."enum", "my_class"."date", "my_class"."test" FROM "my_class" WHERE ("my_class"."name" is not null)
```

