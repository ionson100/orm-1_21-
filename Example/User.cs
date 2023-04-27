using System;
using ORM_1_21_;

namespace Example
{
    [MapTable("simple_user")]
    class User
    {
        public User(int age)
        {
            Age=age;
            Name = $"Name {age}";
            DateBirth = DateTime.Now.AddYears(-age);
        }
        [MapPrimaryKey("id",Generator.Assigned)]
        public Guid Id { get; set; }  =Guid.NewGuid();
        [MapColumn("age")]
        public int Age { get; set; }
        [MapColumn("name")]
        public string Name { get; set; }
        [MapColumn("date")]
        public DateTime DateBirth { get; set; }

        public override string ToString()
        {
            return $"   User: {Name}  Age: {Age}  DateBirth: {DateBirth}";
        }
    }
}