using ORM_1_21_.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ORM_1_21_.Linq
{
    internal class MsSqlConstructorSql
    {
        private List<OneComposite> _listOne;
        // private int i = 0;

        private bool PingComposite(Evolution eval)
        {
            return _listOne.Any(a => a.Operand == eval);
        }

        public string GetStringSql<T>(List<OneComposite> listOne, ProviderName providerName) //, JoinCapital joinCapital
        {
            
            _listOne = listOne;
            var sqlBody = UtilsCore.CheckAny(listOne);
            if (sqlBody != null)
            {
                return sqlBody;
            }
            if (!string.IsNullOrWhiteSpace(AttributesOfClass<T>.SqlWhere))
            {
                _listOne.Add(new OneComposite
                { Operand = Evolution.Where, Body = $"({AttributesOfClass<T>.SqlWhere})" });
            }


            if (PingComposite(Evolution.Update))
                return AttributesOfClass<T>.CreateCommandUpdateFreeForMsSql(_listOne, providerName);

           
            if (PingComposite(Evolution.All))
            {
                StringBuilder builder = new StringBuilder("SELECT COUNT(*),(SELECT COUNT(*) FROM ");
                builder.Append(AttributesOfClass<T>.TableName(providerName)).Append(' ');
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
                    var geoList = listOne.Where(a => a.Operand == Evolution.ListGeo).Select(a => a.Body.Trim());
                    var enumerable = geoList as string[] ?? geoList.ToArray();
                    var val = listOne.First(a => a.Operand == Evolution.Select).Body.Trim();
                   
                    if (enumerable.Any() )
                    {
                        HashSet<string> hashSet = new HashSet<string>();
                        foreach (string s in enumerable)
                        {
                            if (hashSet.Contains(s) == false)
                            {
                                val = val.Replace(s, $" {UtilsCore.SqlConcat(s, providerName)} ");
                                //val= val.Replace(s, $"{s}.STAsText()");
                                hashSet.Add(s);
                            }
                        }
                        sbb.Append(val);

                    }
                    else
                    {
                        sbb.Append(val);
                    } 
                    
                }
                else
                {
                    if (PingComposite(Evolution.Count)) sbb.Append(" COUNT(*) ");
                    if (PingComposite(Evolution.Any))
                    {
                        sbb.Clear();
                        sbb.Append(" IF  EXISTS( " + StringConst.Select);
                    }


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


                    else if (_listOne.All(a => a.Operand != Evolution.Count))
                    {
                        sbb.Clear();
                        var str1 = AttributesOfClass<T>.SimpleSqlSelect(providerName);
                        sbb.Append(str1.Substring(0,
                            str1.IndexOf("FROM", StringComparison.Ordinal)));
                    }

                    var str = sbb.ToString().TrimEnd(',');
                    sbb.Length = 0;
                    sbb.Append(str);
                }
            }

            sbb.Append(AttributesOfClass<T>.SimpleSqlSelect(providerName).Substring(
                AttributesOfClass<T>.SimpleSqlSelect(providerName).IndexOf(" FROM ", StringComparison.Ordinal)));

            var ss = listOne.Where(a => a.Operand == Evolution.Where);
            foreach (var i in ss)
            {
                if (string.IsNullOrEmpty(i.Body)) continue;
                sbb.Append(sbb.ToString().IndexOf("WHERE", StringComparison.Ordinal) == -1 ? " WHERE " : " and ");
                sbb.Append(i.Body);
            }

            var ii = 0;

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

            if (PingComposite(Evolution.First) || PingComposite(Evolution.FirstOrDefault))
            {
                if (PingComposite(Evolution.Single) || PingComposite(Evolution.SingleOrDefault))
                    sbb = sbb.Replace(StringConst.Select,
                        string.Format("{0} {1} (2) ", StringConst.Select, StringConst.Top));
                else
                    sbb = sbb.Replace(StringConst.Select,
                        string.Format("{0} {1} (1) ", StringConst.Select, StringConst.Top));
            }

            if (PingComposite(Evolution.Last) || PingComposite(Evolution.LastOrDefault))
                sbb = sbb.Replace(StringConst.Select,
                    $"{StringConst.Select} {StringConst.Top} (1) ");




            if (PingComposite(Evolution.Any))
            {
                sbb.Insert(0, "IF EXISTS (");
                sbb.Append(" ) BEGIN " + StringConst.Select + " 1; END");
            }

            if (PingComposite(Evolution.Skip))
            {
                int isk = 0;
                foreach (OneComposite composite in listOne.Where(a => a.Operand == Evolution.Skip))
                {
                    isk += int.Parse(composite.Body);
                }

                if (isk <= 0)
                {

                }
                else
                {
                    sbb.Append($"   OFFSET {isk} ROWS ");

                }
            }

            if (PingComposite(Evolution.Limit))
                sbb = new StringBuilder(AttributesOfClass<T>.CreateCommandLimitForMsSql(listOne, sbb.ToString(), providerName));
            //if (PingComposite(Evolution.Join))
            //{
            //    var whereSb = new StringBuilder();
            //    foreach (var str in _listOne.Where(a => a.Operand == Evolution.Where).Select(a => a.Body))
            //    {
            //        var str1 = str.Replace("`", "").Replace("[", "").Replace("]", "");
            //        var eew = str1;
            //        var matsup = new Regex(@"[aA-zZаА-яЯ\d[_]*]*\.[aA-zZаА-яЯ\d[_]*]*").Matches(str1);
            //        foreach (var s in matsup)
            //            eew = str1.Replace(s.ToString(),
            //                UtilsCore.TanslycatorFieldParam1(s.ToString(), UtilsCore.Table1AliasForJoin));
            //        whereSb.Append(eew + " AND ");
            //    }
            //
            //    if (!string.IsNullOrEmpty(whereSb.ToString()))
            //        whereSb.Insert(0, " WHERE ");
            //
            //    var orderBy = new StringBuilder();
            //    foreach (var str in _listOne.Where(a => a.Operand == Evolution.OrderBy).Select(a => a.Body))
            //    {
            //        if (str == _listOne.First(a => a.Operand == Evolution.OrderBy).Body)
            //        {
            //            orderBy.AppendFormat(" ORDER BY {0},",
            //                UtilsCore.TanslycatorFieldParam1(str, UtilsCore.Table1AliasForJoin));
            //            continue;
            //        }
            //
            //        orderBy.AppendFormat(" {0},", UtilsCore.TanslycatorFieldParam1(str, UtilsCore.Table1AliasForJoin));
            //    }
            //
            //
            //    var sing = _listOne.Single(a => a.Operand == Evolution.Join);
            //    sbb = new StringBuilder(sing.Body + whereSb.ToString().Trim("AND ".ToArray()) +
            //                            orderBy.ToString().Trim(','));
            //}

            return sbb.ToString().Replace("  ", " ").Trim(' ', ',').Replace("Average", "AVG")
                .Replace("LongCount", "Count") + "; ";
        }
    }
}