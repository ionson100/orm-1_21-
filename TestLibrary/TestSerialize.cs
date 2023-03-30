using ORM_1_21_;

namespace TestLibrary
{
    [MapTable("test_list")]
    public class TestSerialize
    {
        [MapPrimaryKey("id", Generator.Native)]
        public long Id { get; set; }
        [MapColumn("sdd")]
        public TestUser User { get; set; }

    }
}