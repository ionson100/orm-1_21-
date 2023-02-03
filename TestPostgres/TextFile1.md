# Simple Orm for Android sqlite
=======
Quickstart

**1.**  -Availability of the database file in the folder assets.

**2.**  -Initialization ORM at the entry point of the program:

```java
 new Configure(
 getApplicationInfo().dataDir + "/test_data.pdb", //path file data
 getBaseContext(),// current contex
 true  // you have to have assets test_data.sqlite base
       //true - at each start, it will be overwritten.
       //false - at the start, if it is not to be written, rewriting is not allowed
 );
```

**3.**  -Create class - map database tables.
```java
@Table(name = "test1")
public class Test1 implements IActionOrm<Test1>{
    @PrimaryKey(name = "id")
    public int id;

    @Column(name = "name")
    public  String name;

    @Column(name = "longs")
    public long longs;
    
    public void actionBeforeUpdate(Test1 test1) {
    }

    @Override
    public void actionAfterUpdate(Test1 test1) {
    }

    @Override
    public void actionBeforeInsert(Test1 test1) {
    }

    @Override
    public void actionAfterInsert(Test1 test1) {
    }

    @Override
    public void actionBeforeDelete(Test1 test1) {
    }

    @Override
    public void actionAfterDelete(Test1 test1) {
    }
}
```

**4.**  -Program realization:
```java
  ISession ses=Configure.getSession();
  Object dds=  ses.executeScalar("SELECT name FROM sqlite_master WHERE type='table' AND name='test1';", null);
  if(dds==null){// проверка на существование таблицы
      Configure.createTable(Test1.class);// Создание таблицы
   }
   Test1 dd=new Test1();
   dd.name="sdsdsd";
   dd.longs=12132388;
   dd.aShort=34;
   dd.aByte=45;
   byte[] rr=new byte[2];
   rr[0]=3;
   rr[1]=45;
   dd.aBlob=rr;
   dd.inte = 35;
   Configure.getSession().insert(dd);
   
   List<Test1> test1List=Configure.getSession().getList(Test1.class,null);
   List<Test1> test1List1=Configure.getSession().getList(Test1.class," id =? ",1);
   Test1 test1=Configure.getSession().get(Test1.class,1);
   int res= (int) Configure.getSession().executeScalar(" select count(*) from test1",null);
```

```java
    <T> int update(T item);
    <T> int insert(T item);
    <T> int delete(T item);
    <T> List<T> getList(Class<T> tClass, String where, Object... objects) ;
    <T> T get(Class<T> tClass, Object id);
    <T> Object executeScalar(String sql, Object ... objects);
    void execSQL(String sql, Object ... objects);
    void beginTransaction();
    void commitTransaction();
    void endTransaction();
    void close();
```

**5** implements IActionOrm - optional

 При старте приложения - активация орм.
 ```java
     new Configure(getApplicationInfo().dataDir + "/" + Utils.BASE_NAME, getApplicationContext(), Utils.RELOAD_BASE);
 ```
  где:\
 getApplicationInfo().dataDir + "/" + Utils.BASE_NAME - путь до файла базы\
 getApplicationContext() - контекст приложения\
 Utils.RELOAD_BASE - булевая величина, которая определяет будет ли переписываться файл базы при старте приложения\
 Работа с орм.\
 Модель подразумевает маппинг на атрибутах.\
 Пример:

```java
 @Table("user") //имя таблицы в базе
public class User   {

  @PrimaryKey("id") //первичный ключ, обязательная величина. id - название поля таблицы
   public int _id;

   @Column("name")  //поле таблицы
   public String name_user;
}
````
 Запрос на выборку  массива данных:
```java
 List<User> users=Configure.getSession().getList(User.class,null);
 // sql: select id,name from user;
       
 List<User> users=Configure.getSession().getList(User.class,"name ='"+"ion"+"'");
 List<User> users=Configure.getSession().getList(User.class,"name =?","ion");
 //sql:select id,name from user where name='ion'
       
 List<User> users=Configure.getSession().getList(User.class,"id =?",34);
 //sql:select id,name from user where id=34
```
 Выбор одиночного значения по первичному ключу\
```java
 User user=Configure.getSession().get(User.class,34);
 //sql:select id,name from user where id=34
```
Иногда нужно работать с полями UUID как суррогатными ключами.\
Наследуем модель от IUsingGuidId
```java
@Table("user")
class User implements IUsingGuidId {

    @PrimaryKey("id")
    public int _id;

    @Column("uuid")
    public String uuid;


    @Column("name")
    public String name_user;

    @Override
    public String get_id() {
        return uuid;
    }
}
```
Выборку одиночного объекта можно производить в этом случае:
```java
 String uuid="9df35d16-a36b-4515-b03f-2dfe4331216d";
 User user=Configure.getSession().get(User.class,uuid);
```
Иногда в одной таблице базы возникает желание хранить несколько  объектов разных modelel сущностей.
```java
@Where("ping = 2")
@Table("user")
class User1  {

    @PrimaryKey("id")
    public int _id;

    @Column("name")
    public String name_user;

    @Column("ping")
    public int ping=2;
}

@Where("ping = 3")
@Table("user")
class User2  {
    @PrimaryKey("id")
    public int _id;

    @Column("uuid")

    public String name_user;

    @Column("ping")
    public int ping=3;
}
```
Количество и поля должны совпадать.\
Работаем как с разными объектами орм только будет в запросы добавлять where ping = ?\
запросы будут происходить к одной таблице.\
Иногда возникает хранить в поле сериализованный объект как jason
```java
@Table("user")
class User1  {
    @PrimaryKey("id")
    public int _id;

    @Column("name")
    public String name_user;

    @UserField(IUserType = MyUsers2Field.class)
    @Column("users2") - поле в таблице user название users2 тип поля - строка
    public  List<User2> user2List=new ArrayList<>();
}


class User2 implements Serializable { // - наследование от Serializable

    public int _id;
    public String uuid;
    public String name_user;
}
//вспомогательный тип для сериализации и десериализации поля в объекте
public class MyUsers2Field implements IUserType {
    @Override
    public Object getObject(String str) {
        Gson gson = new Gson();
        List<User2> actionList = gson.fromJson(str, new TypeToken<List<User2>>() {
        }.getType());
        return actionList;
    }

    @Override
    public String getString(Object ojb) {
        String string = new Gson().toJson(ojb);
        return string;
    }
}
```
Если есть нужда отслеживать  события перед модификацией объекта.\
Наследуем объект от IActionOrm;
```java
@Table("user")
class User1 implements IActionOrm {
    @PrimaryKey("id")
    public int _id;
    @Column("uuid")
    public String uuid;
    @Column("name")
    public String name_user;

    @Override
    public void actionBeforeUpdate(Object o) {}

    @Override
    public void actionAfterUpdate(Object o) {}

    @Override
    public void actionBeforeInsert(Object o) {}

    @Override
    public void actionAfterInsert(Object o) {}

    @Override
    public void actionBeforeDelete(Object o) {}

    @Override
    public void actionAfterDelete(Object o) {}
}
```
Сортировка без условия поиска:
```java
 List<User> users=Configure.getSession().getList(User.class," 1 = 1 order by 'name' ");
 //sql:select id,name from user where 1=1 order by 'name'
```

Пакетная вставка:
```java
 List<User> users= ......
 ISession ses= Configure.getSession();
 Configure.bulk(User.class, users, ses);*/
```

       
 Поддерживаются типы Date и BigDecimal  поля  в таблице должны быть соответственно integer  и string.
```java
 createTable(Class<?> aClass) - авто создание таблицы
 getStringCreateTable(Class<?> aClass) - строка для создания таблицы
 getStringCreateAllTable(Context contex) - строка создания всех таблиц.
```
Все метаданные для рефлексии кешируются в момент первого обращения к типу.

