using ORM_1_21_.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ORM_1_21_.Linq.MySql
{
    internal class MySqlConstructorSql
    {
        private readonly ProviderName _providerName;
        private List<OneComposite> _listOne;

        public MySqlConstructorSql(ProviderName providerName)
        {
            _providerName = providerName;
        }
        // private int i = 0;

        private bool PingComposite(Evolution eval)
        {
            return _listOne.Any(a => a.Operand == eval);
        }

        

        public string GetStringSql<T>(List<OneComposite> listOne,ProviderName providerName)
        {
         
            _listOne = listOne;
            var sqlBody = UtilsCore.CheckAny(listOne, Evolution.FreeSql, 
                Evolution.TableCreate, Evolution.TableExists,Evolution.ExecuteScalar, Evolution.TruncateTable,Evolution.ExecuteNonQuery, Evolution.DataTable);
            if (sqlBody != null)
            {
                return sqlBody;
            }
            if (!string.IsNullOrWhiteSpace(AttributesOfClass<T>.SqlWhere))
            {
                _listOne.Add(new OneComposite
                    { Operand = Evolution.Where, Body = $"({AttributesOfClass<T>.SqlWhere})" });
            }


            //if (PingComposite(Evolution.FreeSql)) return _listOne.Single(a => a.Operand == Evolution.FreeSql).Body;

            if (PingComposite(Evolution.Update)) return AttributesOfClass<T>.CreateCommandLimitForMySql(_listOne,providerName);
           
            if (PingComposite(Evolution.All))
            {
                StringBuilder builder = new StringBuilder("SELECT COUNT(*),(SELECT COUNT(*) FROM ");
                builder.Append(AttributesOfClass<T>.TableName(providerName)).Append(" ");
                var f = _listOne.First(a => a.Operand == Evolution.All).Body;
                bool addAll = false;
                if (string.IsNullOrWhiteSpace(f) == false)
                {
                    addAll = true;
                    builder.Append(" WHERE ").Append(f);
                }

                if (_listOne.Any(s => s.Operand == Evolution.Where))
                {
                    if (addAll == false)
                    {
                        builder.Append(" WHERE ");
                    }
                    else
                    {
                        builder.Append(" AND ");
                    }
                    foreach (OneComposite composite in listOne.Where(a => a.Operand == Evolution.Where))
                    {
                        builder.Append(composite.Body).Append(" AND ");
                    }
                }
                string at = builder.ToString().Trim(" AND ".ToCharArray());
                builder.Clear().Append(at).Append(" )");

                builder.Append(" FROM ").Append(AttributesOfClass<T>.TableName(providerName));

                if (_listOne.Any(s => s.Operand == Evolution.Where))
                {
                    builder.Append(" WHERE ");
                    foreach (OneComposite composite in listOne.Where(a => a.Operand == Evolution.Where))
                    {
                        builder.Append(composite.Body).Append(" AND ");
                    }
                }

                string sql = builder.ToString().TrimEnd(" AND ".ToCharArray());
                return sql;


            }
            if (PingComposite(Evolution.All))
            {
                var sb = new StringBuilder(listOne.First(a => a.Operand == Evolution.All).Body);
                // <> - = #7#
                sb.Replace("<>", "#7#");
                // >= - <=  #1#
                sb.Replace(">=", "#1#");
                // >  -  <  #2#
                sb.Replace(">", "#2#");
                // <  - >   #3#
                sb.Replace("<", "#3#");
                // <= - >=  #4# 
                sb.Replace("<=", "#4#");
                // =  - !=  #5#
                sb.Replace("=", "#5#");
                // != - =   #6#
                sb.Replace("!=", "#6#");
                listOne.First(a => a.Operand == Evolution.All).Body = sb.ToString().Replace("#1#", "<=")
                    .Replace("#2#", "<").Replace("#3#", ">").Replace("#4#", ">=").Replace("#5#", "!=")
                    .Replace("#6#", "=").Replace("#7#", "=");
                listOne.Add(new OneComposite
                { Operand = Evolution.Where, Body = listOne.First(a => a.Operand == Evolution.All).Body });
            }

            var ii = 0;
            var sbb = new StringBuilder();
            if (PingComposite(Evolution.Delete))
            {
                sbb.Append("DELETE ");
            }
            else
            {
                sbb.Append(StringConst.Select + " ");


                if (PingComposite(Evolution.Select))
                {
                    sbb.Append(listOne.First(a => a.Operand == Evolution.Select).Body);
                }
                else
                {
                    if (PingComposite(Evolution.Count)) sbb.Append(" COUNT(*) ");
                    if (PingComposite(Evolution.Any)) sbb.AppendFormat(" EXISTS ( " + StringConst.Select);

                    if (PingComposite(Evolution.DistinctCore))
                    {
                        string s = listOne.First(a => a.Operand == Evolution.DistinctCore).Body;
                        if (PingComposite(Evolution.SelectNew))
                        {

                            sbb.Append(" DISTINCT " + s.TrimStart(new char[] { ' ', ',' }));
                        }
                        else
                        {
                            sbb.Append(" DISTINCT " + s);
                        }

                    }

                    else
                    {
                        if (!PingComposite(Evolution.Count))
                            foreach (var i in AttributesOfClass<T>.CurrentTableAttributeDall(_providerName))
                            {
                                if (ii == 0)
                                    sbb.Append(string.Format(CultureInfo.CurrentCulture, "{1} {0},",
                                        AttributesOfClass<T>.TableName(providerName) + "." +
                                        AttributesOfClass<T>.PkAttribute(_providerName).GetColumnName(_providerName),
                                        listOne.Any(a => a.Operand == Evolution.DistinctCore &&
                                                         a.Body == AttributesOfClass<T>.PkAttribute(_providerName).GetColumnName(_providerName))
                                            ? " Distinct "
                                            : ""));
                                sbb.Append(string.Format(CultureInfo.CurrentCulture, "{1} {0},",
                                    AttributesOfClass<T>.TableName(providerName) + "." + i.GetColumnName(_providerName),
                                    listOne.Any(a => a.Operand == Evolution.DistinctCore && a.Body == i.GetColumnName(_providerName))
                                        ? " Distinct "
                                        : ""));

                                ii++;
                            }
                    }


                    var str = sbb.ToString().TrimEnd(',');
                    sbb.Length = 0;
                    sbb.Append(str);
                }
            }

            sbb.Append(" FROM ");
            sbb.Append(AttributesOfClass<T>.TableName(providerName));

            var ss = listOne.Where(a => a.Operand == Evolution.Where);
            foreach (var i in ss)
            {
                if (string.IsNullOrEmpty(i.Body)) continue;
                sbb.Append(sbb.ToString().IndexOf("WHERE", StringComparison.Ordinal) == -1 ? " WHERE " : " and ");
                sbb.Append(i.Body);
            }

          
            ii = 0;

            if (PingComposite(Evolution.GroupBy))
            {
                sbb.Append(" ");
                var sbo = new StringBuilder();
                foreach (var i in listOne.Where(a => a.Operand == Evolution.GroupBy)) sbo.Append(" " + i.Body + ",");
                if (listOne.Any(a => a.Operand == Evolution.GroupBy && a.ExpressionDelegate != null) &&
                    listOne.Any(s => s.Operand == Evolution.Count))
                {
                    sbb.AppendFormat(" GROUP BY {0} ", sbo.ToString().TrimEnd(','));
                }
            }


            foreach (var i in listOne.Where(a => a.Operand == Evolution.OrderBy))
            {
                if (ii == 0)
                    sbb.Append(" ORDER BY ");

                sbb.Append(" " + i.Body + ",");
                ii++;
            }

            if (PingComposite(Evolution.Reverse))
            {
                sbb.Append("  ");
                sbb.Append(listOne.Single(a => a.Operand == Evolution.Reverse).Body);
            }


            var ee = sbb.ToString().Trim(' ', ',');
            sbb.Length = 0;
            sbb.Append(ee);
            if (PingComposite(Evolution.ElementAt))
            {
                if (_providerName == ProviderName.PostgreSql || _providerName == ProviderName.SqLite)
                    sbb.AppendFormat(" LIMIT 1 OFFSET {0}", listOne.First(a => a.Operand == Evolution.ElementAt).Body);
                else
                    sbb.AppendFormat(" LIMIT {0},1", listOne.First(a => a.Operand == Evolution.ElementAt).Body);
            }

            if (PingComposite(Evolution.ElementAtOrDefault))
            {
                if (_providerName == ProviderName.PostgreSql || _providerName == ProviderName.SqLite)
                    sbb.AppendFormat(" LIMIT 1  OFFSET {0} ", listOne.First(a => a.Operand == Evolution.ElementAtOrDefault).Body);
                else
                    sbb.AppendFormat(" LIMIT {0},1",
                        listOne.First(a => a.Operand == Evolution.ElementAtOrDefault).Body);
            }

            if (PingComposite(Evolution.First))
            {
                if (_providerName == ProviderName.PostgreSql || _providerName == ProviderName.SqLite)
                {
                    if (PingComposite(Evolution.Single) || PingComposite(Evolution.SingleOrDefault))
                        sbb.Append(" LIMIT 2 ");
                    else
                        sbb.Append(" LIMIT 1 ");
                }
                else
                {
                    if (PingComposite(Evolution.Single) || PingComposite(Evolution.SingleOrDefault))
                        sbb.Append(" LIMIT 0,2 ");
                    else
                        sbb.Append(" LIMIT 0,1 ");
                }
            }

            if (PingComposite(Evolution.Skip))
            {
                int isk = 0;
                foreach (OneComposite composite in listOne.Where(a=>a.Operand==Evolution.Skip))
                {
                    isk += int.Parse(composite.Body);
                }

                if (isk <= 0)
                {

                }
                else
                {
                    if (_providerName == ProviderName.PostgreSql)
                    {
                        sbb.Append(" OFFSET ").Append(isk).Append(" ");
                    }
                    else
                    {
                        sbb.Append($" LIMIT {int.MaxValue} OFFSET {isk} ").Append(" ");
                    }
                    
                }
            }



            if (PingComposite(Evolution.Limit)) sbb.Append(listOne.First(a => a.Operand == Evolution.Limit).Body);
         
            if (PingComposite(Evolution.Any)) sbb.Append(" ) ");

            

            if (PingComposite(Evolution.Join))
            {
                var whereSb = new StringBuilder();
                foreach (var str in _listOne.Where(a => a.Operand == Evolution.Where).Select(a => a.Body))
                {
                    var str1 = str.Replace("`", "").Replace("[", "").Replace("]", "");
                    var eew = str1;
                    var matsup = new Regex(@"[aA-zZаА-яЯ\d[_]*]*\.[aA-zZаА-яЯ\d[_]*]*").Matches(str1);
                    foreach (var s in matsup)
                        eew = str1.Replace(s.ToString(),
                            UtilsCore.TanslycatorFieldParam1(s.ToString(), UtilsCore.Table1AliasForJoin));
                    whereSb.Append(eew + " AND ");
                }

                if (!string.IsNullOrEmpty(whereSb.ToString()))
                    whereSb.Insert(0, " WHERE ");

                var orderby = new StringBuilder();
                foreach (var str in _listOne.Where(a => a.Operand == Evolution.OrderBy).Select(a => a.Body))
                {
                    if (str == _listOne.First(a => a.Operand == Evolution.OrderBy).Body)
                    {
                        orderby.AppendFormat(" ORDER BY {0},",
                            UtilsCore.TanslycatorFieldParam1(str, UtilsCore.Table1AliasForJoin));
                        continue;
                    }

                    orderby.AppendFormat(" {0},", UtilsCore.TanslycatorFieldParam1(str, UtilsCore.Table1AliasForJoin));
                }


                var dfggf = _listOne.Single(a => a.Operand == Evolution.Join);
                sbb = new StringBuilder(dfggf.Body + whereSb.ToString().Trim("AND ".ToArray()) +
                                        orderby.ToString().Trim(','));
            }
            // todo ion100 Replace("''", "'")
            return sbb.ToString().Replace("  ", " ").Replace("Average", "AVG")
                .Replace("LongCount", "Count")+";";
        }
    }
}