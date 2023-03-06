using Newtonsoft.Json;
using ORM_1_21_;

namespace TestLibrary
{
    public class TestUser : IMapSerializable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }

        public void Deserialize(string str)
        {
            var o = JsonConvert.DeserializeObject<TestUser>(str);
            Id = o.Id;
            Name = o.Name;
        }
    }
}