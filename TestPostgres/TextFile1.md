# Simple Orm for Android 
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
        if(dds==null){
            Configure.createTable(Test1.class);
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

   Орм  sqlite
   при старте приложения - активация орм.\
    ```java
        new Configure(getApplicationInfo().dataDir + "/" + Utils.BASE_NAME, getApplicationContext(), Utils.RELOAD_BASE);
     ```
     где - getApplicationInfo().dataDir + "/" + Utils.BASE_NAME путь до файла базы
        getApplicationContext()  контекст приложения
        Utils.RELOAD_BASE булевая величина, которая определяет будет ли переписываться файл базы при старте приложения
        ВАЖНО: файл базы должен лежать в assets, название должно соответствовать названию при активации орм.

        Работа с орм.
        Модель подразумевает маппинг на атрибутах.
        пример:
@Table("user") - имя таблицы в базе
public class User   {

    @PrimaryKey("id") - первичный ключ, обязательная величина. id - название поля таблицы
    public int _id;


    @Column("name") - обыкновенное поле типа, name поле таблицы
    public String name_user;
}
    запрос на выборку  массива данных:
        List<User> users=Configure.getSession().getList(User.class,null);
        as select id,name from user;
        или
        List<User> users=Configure.getSession().getList(User.class,"name ='"+"ion"+"'");
        или
        List<User> users=Configure.getSession().getList(User.class,"name =?","ion");
        as select id,name from user where name='ion'
        или
        List<User> users=Configure.getSession().getList(User.class,"id =?",34);
        as select id,name from user where id=34

        Выбор одиночного значения по первичному ключу
        User user=Configure.getSession().get(User.class,34);
        as select id,name from user where id=34
        Иногда нужно работать с полями UUID как суррогатными ключами.
        Наследуем модель от IUsingGuidId
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
    Выборку одиночного объекта можно производить в этом случае:

        String uuid="fhdsjfhjsdhfjdshffhdsf";
        User user=Configure.getSession().get(User.class,uuid);

        Иногда в одной таблице базы возникает желание хранить несколько  объектов разных моделей сущности.

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
    Работаем как с разными объектами орм только будет в запросы добавлять where ping = &&&

        иногда возникает довольно часто такая ситуация

@Table("user")
class User1  {
    @PrimaryKey("id")
    public int _id;
    @Column("uuid")

    public String name_user;

    public  List<User2> user2List=new ArrayList<>();
}

class User2  {

    public int _id;
    public String uuid;
    public String name_user;
}
    единственно правильное решение как обычно раскинуть все это на две таблицы и иностранными ключами.
        но не спешите
        можно применить сериализацию через json

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

class User2 implements Serializable { // это очень важно, наследование от Serializable

    public int _id;
    public String uuid;
    public String name_user;
}

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
    теперь массив пользователей 2 хранится сериализованным в виде json в таблице user
        орм сама знает что с ним делать, при сохранении изменении и удалении.

        если есть нужда отслеживать  события перед модификацией объекта   и после оной.
        наследуем объект от IActionOrm;

@Table("user")
class User1 implements IActionOrm {
    @PrimaryKey("id")
    public int _id;
    @Column("uuid")
    public String uuid;
    @Column("name")
    public String name_user;

    @Override
    public void actionBeforeUpdate(Object o) {

    }

    @Override
    public void actionAfterUpdate(Object o) {

    }

    @Override
    public void actionBeforeInsert(Object o) {

    }

    @Override
    public void actionAfterInsert(Object o) {

    }

    @Override
    public void actionBeforeDelete(Object o) {

    }

    @Override
    public void actionAfterDelete(Object o) {

    }
}
     Фитча, иногда нужна сортировка без условия поиска
             можно решить таким способом
             List<User> users=Configure.getSession().getList(User.class," 1 = 1 order by 'name' ");
        as select id,name from user where 1=1 order by 'name'

        Убыстрение вставки больших массивов
        Пакетная вставка:

        List<User> users= ......
        ISession ses= Configure.getSession();
        Configure.bulk(User.class, users, ses);*/
 Поддерживаются типы Date и BigDecimal  поля  в таблице должны быть соответственно integer  и string.
```java
 createTable(Class<?> aClass) - автосоздание таблицы
 getStringCreateTable(Class<?> aClass) - строка для создания таблицы
 getStringCreateAllTable(Context contex) - строка создания всех таблиц.
```
 Исходя из того что тип должен быть помечен @Table("tableName")
