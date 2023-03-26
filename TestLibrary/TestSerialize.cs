using ORM_1_21_;

namespace TestLibrary
{
    [MapTableName("test_list")]
    public class TestSerialize
    {
        [MapPrimaryKey("id", Generator.Native)]
        public long Id { get; set; }
        [MapColumnName("sdd")]
        public TestUser User { get; set; }

    }
}