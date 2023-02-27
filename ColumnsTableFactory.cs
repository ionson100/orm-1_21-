using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ORM_1_21_
{
    static class ColumnsTableFactory
    {
        private static List<TableColumn> ForMsSql(string tableName, IDbCommand command, ProviderName providerName)
        {
            string sql =
                "SELECT  TableName   = OBJECT_NAME(C.object_id), ColumnName  = C.name, ColumnType  = TYPE_NAME(C.system_type_Id),  " +
                "IsPK  = IIF(IC.object_id IS NULL, 0, 1) FROM sys.columns  C  LEFT JOIN sys.key_constraints  " +
                " KC  ON KC.parent_object_id = C.object_id AND kc.type = 'PK'LEFT JOIN sys.index_columns    " +
                " IC  ON KC.parent_object_id = IC.object_id  AND KC.unique_index_id = " +
                $"IC.index_id AND IC.column_id = C.column_id WHERE C.object_id = OBJECT_ID('{tableName}');";
            return NewMethod(command, providerName, sql);
        }

        private static List<TableColumn> ForMysql(string tableName, IDbCommand command, ProviderName providerName)
        {
            string conStr = command.Connection.ConnectionString;
            var r = conStr.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                .SingleOrDefault(a => a.ToUpper().Contains("DATABASE"));
            if (r == null)
            {
                throw new Exception("Can't determine database schema");
            }

            var i = r.IndexOf("=", StringComparison.Ordinal);
            var t = r.Substring(++i).Trim();

            string sql = "SELECT 1, `COLUMN_NAME` as name,`DATA_TYPE` as type, INSTR(`COLUMN_KEY`, 'PRI') as ispk " +
                         $"FROM `INFORMATION_SCHEMA`.`COLUMNS` WHERE `TABLE_SCHEMA`='{t}' AND `TABLE_NAME`='{tableName}';";
            return NewMethod(command, providerName, sql);
        }

        private static List<TableColumn> ForSqlite(string tableName, IDbCommand command, ProviderName providerName)
        {
            string sql = $"PRAGMA table_info('{tableName}')";
            return NewMethod(command, providerName, sql);
        }

        private static List<TableColumn> ForPostgres(string tableName, IDbCommand command, ProviderName providerName)
        {
            string sql =
                $"select 1, column_name,data_type from information_schema.columns where table_name =  '{tableName}';" +
                $"select column_name from information_schema.key_column_usage where table_name='{tableName}'";
            return NewMethod(command, providerName, sql);
        }

        private static List<TableColumn> NewMethod(IDbCommand command, ProviderName providerName, string sql)
        {
            command.CommandType = CommandType.Text;
            command.CommandText = sql;
            command.Connection.Open();
            var reader = command.ExecuteReader();
            try
            {
                List<TableColumn> tc = new List<TableColumn>();
                string key = null;
                if (providerName == ProviderName.Postgresql)
                {
                    while (reader.Read())
                    {
                        tc.Add(new TableColumn { ColumnName = reader.GetString(1), ColumnType = reader.GetString(2) });
                    }

                    try
                    {
                        reader.NextResult();
                        while (reader.Read())
                        {
                            key = reader.GetString(0);
                        }

                        if (key != null)
                        {
                            var r = tc.SingleOrDefault(a => a.ColumnName == key);
                            if (r != null)
                            {
                                r.IsPk = true;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        //ignored
                    }

                }
                else
                {
                    while (reader.Read())
                    {
                        TableColumn ssd = new TableColumn
                        {
                            ColumnName = reader.GetString(1),
                            ColumnType = reader.GetString(2)
                        };
                        if (providerName == ProviderName.MySql || providerName == ProviderName.MsSql)
                        {
                            int d = reader.GetInt32(3);
                            if (d == 1)
                            {
                                ssd.IsPk = true;
                            }
                        }

                        if (providerName == ProviderName.Sqlite)
                        {
                            int d = reader.GetInt32(5);
                            if (d == 1)
                            {
                                ssd.IsPk = true;
                            }
                        }

                        tc.Add(ssd);
                    }
                }

                return tc;
            }
            finally
            {
                reader.Dispose();
                command.Dispose();
            }
        }

        public static IEnumerable<TableColumn> GeTableColumns(ProviderName providerName, IDbCommand command,
            string tableName)
        {
            switch (providerName)
            {
                case ProviderName.Postgresql:
                    {
                        return ForPostgres(tableName, command, providerName);
                    }
                case ProviderName.MySql:
                    {
                        return ForMysql(tableName, command, providerName);
                    }
                case ProviderName.MsSql:
                    {
                        return ForMsSql(tableName, command, providerName);
                    }
                case ProviderName.Sqlite:
                    {
                        return ForSqlite(tableName, command, providerName);
                    }
                default:
                    throw new ArgumentOutOfRangeException(nameof(providerName), providerName, null);
            }

        }
    }
}
