﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using ORM_1_21_.geo;

namespace ORM_1_21_.Utils
{
    internal class UtilsBulkMsSql
    {
        private readonly ProviderName _providerName;

        public UtilsBulkMsSql(ProviderName providerName)
        {
            _providerName = providerName;
        }

        public string GetSql<T>(IEnumerable<T> list, string fileCsv, string fieldterminator)
        {
            if (fileCsv != null)
            {
                return GetSqlFile(list, fileCsv, fieldterminator);
            }
            return GetSimpleSql2New(list);
        }

        public string GetSql<T>(IEnumerable<T> list)
        {
            return GetSimpleSql2New(list);
        }

        private string GetSqlFile<T>(IEnumerable<T> list, string fileCsv, string fieldterminator)
        {
            var tableName = AttributesOfClass<T>.TableName(_providerName);
            StringBuilder sql = new StringBuilder("bulk insert " + tableName);
            sql.AppendLine($"from '{fileCsv}'");
            sql.AppendLine(" with(");
            sql.AppendLine("FIRSTROW = 2,");
            sql.AppendLine($"FIELDTERMINATOR = '{fieldterminator}',");
            sql.AppendLine("ROWTERMINATOR = '\n'");
            sql.AppendLine(")");
            StringBuilder builder = new StringBuilder();
            bool isAddPk = AttributesOfClass<T>.PkAttribute(_providerName).Generator == Generator.Assigned;

            StringBuilder rowHead = new StringBuilder();
            if (isAddPk)
            {
                rowHead.Append(AttributesOfClass<T>.PkAttribute(_providerName).GetColumnNameRaw())
                    .Append(';');
            }
            foreach (var map in AttributesOfClass<T>.CurrentTableAttributeDal(_providerName))
            {
                rowHead.Append(map.GetColumnNameRaw()).Append(';');
            }

            builder.Append(rowHead.ToString().Substring(0, rowHead.ToString().LastIndexOf(';')))
                .Append(Environment.NewLine);

            foreach (var ob in list)
            {
                StringBuilder row = new StringBuilder();
                if (isAddPk)
                {
                    row.Append(AttributesOfClass<T>.GetValueE(_providerName, AttributesOfClass<T>.PkAttribute(_providerName).PropertyName, ob))
                        .Append(';');
                }
                foreach (var keyValuePair in AttributesOfClass<T>.PropertyInfoList.Value)
                {
                    row.Append(AttributesOfClass<T>.GetValueE(_providerName, keyValuePair.Value.Name, ob)).Append(';');
                }

                string s = row.ToString().Substring(0, row.ToString().LastIndexOf(",", StringComparison.Ordinal)) + "\n";
                builder.Append(s);
            }
            File.WriteAllText(fileCsv, builder.ToString());
            return sql.ToString();
        }


        private string GetSimpleSql2<T>(IEnumerable<T> list)
        {
            bool isAddPk = AttributesOfClass<T>.PkAttribute(_providerName).Generator == Generator.Assigned;
            StringBuilder builder = new StringBuilder("INSERT INTO");
            builder.Append(AttributesOfClass<T>.TableName(_providerName));
            builder.Append('(');
            StringBuilder headBuilder = new StringBuilder();
            if (isAddPk)
            {
                headBuilder.Append(AttributesOfClass<T>.PkAttribute(_providerName).GetColumnName(_providerName)).Append(',');
            }
            foreach (var map in AttributesOfClass<T>.CurrentTableAttributeDal(_providerName))
            {
                if ( map.PropertyType == typeof(byte[]))
                {
                    continue;
                }
                headBuilder.Append(map.GetColumnName(_providerName)).Append(',');
            }

            builder.Append(headBuilder.ToString().Substring(0, headBuilder.ToString().LastIndexOf(',')));
            builder.Append(") ");

            foreach (var ob in list)
            {
                StringBuilder row = new StringBuilder("SELECT ");
                if (isAddPk)
                {
                    var o = AttributesOfClass<T>.GetValueE(_providerName, AttributesOfClass<T>.PkAttribute(_providerName).PropertyName, ob);
                    Type type = AttributesOfClass<T>.PropertyInfoList.Value[AttributesOfClass<T>.PkAttribute(_providerName).PropertyName].PropertyType;
                    row.Append(new UtilsBulkMySql(_providerName).GetValue(o, type)).Append(',');
                }
                foreach (var map in AttributesOfClass<T>.CurrentTableAttributeDal(_providerName))
                {
                    var o = AttributesOfClass<T>.GetValueE(_providerName, map.PropertyName, ob);
                    Type type = AttributesOfClass<T>.PropertyInfoList.Value[map.PropertyName].PropertyType;
                    if ( type == typeof(byte[]))
                    {
                        continue;
                    }
                    string str = new UtilsBulkMySql(_providerName).GetValue(o, type);
                    row.Append(str).Append(',');
                }
                builder.AppendLine(row.ToString().Substring(0, row.ToString().LastIndexOf(',')));
                builder.AppendLine("UNION ALL");
            }
            var res = builder.ToString().Substring(0, builder.ToString().LastIndexOf("UNION ALL", StringComparison.Ordinal));
            return "SET DATEFORMAT YMD;" + res;
        }


        private string GetSimpleSql2New<T>(IEnumerable<T> list)
        {
            var pk = AttributesOfClass<T>.PkAttribute(_providerName);
            var listDal = AttributesOfClass<T>.CurrentTableAttributeDal(_providerName);
            bool isAddPk = pk.Generator == Generator.Assigned;
            
            StringBuilder builder = new StringBuilder();
            builder.Append("INSERT INTO ").Append(AttributesOfClass<T>.TableName(_providerName)).AppendLine("");

            StringBuilder partBuilder = new StringBuilder("(");
            if (isAddPk)
            {
                partBuilder.Append(pk.GetColumnName(_providerName)).Append(", ");
            }
            foreach (MapColumnAttribute mapColumnAttribute in listDal)
            {
                partBuilder.Append(mapColumnAttribute.GetColumnName(_providerName)).Append(", ");
            }

            builder.Append(partBuilder.ToString().Trim(' ', ',')).Append(')');
            builder.AppendLine(" VALUES ");

            var rt = new UtilsBulkMySql(_providerName);
            foreach (T ob in list)
            {  partBuilder.Clear();
                partBuilder.Append('(');
                if (isAddPk)
                {
                    var o = AttributesOfClass<T>.GetValueE(_providerName, pk.PropertyName, ob);
                    Type type = AttributesOfClass<T>.PropertyInfoList.Value[pk.PropertyName].PropertyType;
                    string str = rt.GetValue(o, type);
                    partBuilder.Append(str).Append(", ");
                }
                foreach (var column in listDal)
                {
                    var o = AttributesOfClass<T>.GetValueE(_providerName, column.PropertyName, ob);
                    if (column.IsInheritIGeoShape)
                    {

                        if (o == null)
                        {
                            partBuilder.Append("null, ");
                        }
                        else
                        {
                            partBuilder.Append($"geometry::STGeomFromText('{((IGeoShape)o).StAsText()}', {((IGeoShape)o).StSrid()}), ");
                        }
                        
                    } else if (column.IsJson)
                    {
                        if (o is string)
                        {
                            partBuilder.Append($"'{o}'").Append(", ");
                        }
                        else
                        {
                            partBuilder.Append($"'{JsonConvert.SerializeObject(o)}'").Append(", ");
                        }
                        
                    }
                    else
                    {
                       
                        Type type = AttributesOfClass<T>.PropertyInfoList.Value[column.PropertyName].PropertyType;
                        string str = rt.GetValue(o, type);
                        partBuilder.Append(str).Append(", ");
                    }
                   
                }

                string b = partBuilder.ToString().Trim(' ', ',') + "),";
                builder.AppendLine(b);
              
            }

            string sql = builder.ToString();
            return sql.Substring(0,sql.LastIndexOf(",",StringComparison.CurrentCulture));
        }

        public static string InsertFile<T>(string fileCsv, string fieldterminator, ProviderName providerName)
        {
            StringBuilder sql = new StringBuilder("bulk insert " + AttributesOfClass<T>.TableName(providerName));
            sql.AppendLine($" from '{fileCsv}'");
            sql.AppendLine(" with( ");
           
            sql.AppendLine("FIRSTROW = 2, CODEPAGE = '65001',");
            sql.AppendLine($"FIELDTERMINATOR = '{fieldterminator}', ");
            sql.AppendLine("ROWTERMINATOR = '\n'");//
            sql.AppendLine(")");
            return sql.ToString();
        }
    }
}
