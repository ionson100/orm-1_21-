namespace ORM_1_21_
{
    class V
    {
        public V(string sql)
        {
            _sql = sql;

        }
        private readonly string _sql;
        public string FreeSql()
        {
            return _sql;
        }

        public string TableCreate()
        {
            return _sql;
        }

        public string DropTable()
        {
           return _sql;
        }

        public string TableExists()
        {
            return _sql;
        }

        public string ExecuteScalar()
        {
            return _sql;
        }

        public string TruncateTable()
        {
            return _sql;
        }

        public string ExecuteNonQuery()
        {
            return _sql;
        }

        public string DataTable()
        {
            return _sql;
        }
    }
}