using ORM_1_21_.geo;
using ORM_1_21_.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ORM_1_21_.Linq
{
    internal sealed class QueryTranslator<T> : ExpressionVisitor, ITranslate
    {
        //private bool isAddPost=false;
        private readonly List<PostExpression> _postExpressions = new List<PostExpression>();
        private readonly ProviderName _providerName;

        private string _currentMethod;
        private int _paramIndex;

        public QueryTranslator(ProviderName name, int paramIndex = 0)
        {
            _providerName = name;
            Param = new Dictionary<string, object>();
            _paramIndex = paramIndex;
        }

        private string ParamName => string.Format("{0}{1}",
            string.Format("{1}{0}", ParamStringName, UtilsCore.PrefParam(_providerName)), ++_paramIndex);

        private Evolution CurrentEvolution { get; set; }

        private string ParamStringName { get; set; } = "p";

        private StringBuilder StringB { get; set; } = new StringBuilder();

        public Dictionary<string, object> Param { get; set; }

        public List<OneComposite> ListOne { get; } = new List<OneComposite>();

        public string Translate(Expression expression, out Evolution ev)
        {
            CurrentEvolution = 0;
            Visit(expression);
            ev = CurrentEvolution;
            if (_providerName == ProviderName.MsSql)
            {
                var dd = new MsSqlConstructorSql().GetStringSql<T>(ListOne, _providerName);
                return dd;
            }
            else
            {
                var dd = new MySqlConstructorSql(_providerName).GetStringSql<T>(ListOne, _providerName); //,_joinCapital
                return dd;
            }
        }

        public void Translate(Expression expression, Evolution ev, List<object> paramList)
        {
            CurrentEvolution = ev;
            Visit(expression);

            if (ev == Evolution.Between)
            {
                StringB.Append(" BETWEEN ");
                Visit((Expression)paramList[0]);
                StringB.Append(" AND ");
                Visit((Expression)paramList[1]);
                var str = StringB.ToString();
                AddListOne(new OneComposite { Operand = Evolution.Where, Body = StringB.ToString() });
            }

            if (ev == Evolution.Delete)
            {
                AddListOne(new OneComposite { Operand = Evolution.Delete });
                AddListOne(new OneComposite { Operand = Evolution.Where, Body = StringB.ToString() });
            }

            if (ev == Evolution.DistinctCore)
                AddListOne(new OneComposite { Operand = Evolution.DistinctCore, Body = StringB.ToString() });

            if (ev == Evolution.Limit)
                if (paramList != null && paramList.Count == 2)
                    switch (_providerName)
                    {
                        case ProviderName.MsSql:
                            {
                                AddListOne(new OneComposite
                                {
                                    Operand = Evolution.Limit,
                                    Body =
                                        $"LIMIT {paramList[0]},{paramList[1]}"
                                });
                            }
                            break;
                        case ProviderName.MySql:
                            {
                                AddListOne(new OneComposite
                                {
                                    Operand = Evolution.Limit,
                                    Body = string.Format(" Limit {0},{1}", paramList[0], paramList[1])
                                });
                            }
                            break;
                        case ProviderName.PostgreSql:
                        case ProviderName.SqLite:
                            {
                                AddListOne(new OneComposite
                                {
                                    Operand = Evolution.Limit,
                                    Body = string.Format(" Limit {1} OFFSET {0}", paramList[0], paramList[1])
                                });
                            }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

            if (ev == Evolution.Update)
                AddListOne(new OneComposite { Operand = Evolution.Update, Body = StringB.ToString() });

           
            StringB.Length = 0;
        }

        public void Translate(Expression expression)
        {
            Visit(expression);
            
        }
        


        public List<OneComposite> GetListOne()
        {
            var list = new List<OneComposite>();
            foreach (var item in ListOne) list.Add(item);
            return list;
        }

        public List<PostExpression> GetListPostExpression()
        {
            return _postExpressions;
        }

        public string Translate(Expression expression, out Evolution ev1, string par)
        {
            ParamStringName = par;
            return Translate(expression, out ev1);
        }

        public void AddPostExpression(Evolution evolution, MethodCallExpression expression)
        {
            _postExpressions.Add(new PostExpression(evolution, expression));
        }

        private bool PingComposite(Evolution eval)
        {
            return ListOne.Any(a => a.Operand == eval);
        }

        private void AddListOne(OneComposite composite)
        {
            ListOne.Add(composite);
        }

        public int GetParamIndex()
        {
            return _paramIndex;
        }

        public string FieldOne()
        {
            return StringB.ToString();
        }

        private string GetColumnName(string member, Type type)
        {
            var ss = AttributesOfClass<T>.GetNameFieldForQuery(member, type, _providerName);
            return ss;
        }

        public int GetSrid(string member)
        {
            var ss = AttributesOfClass<T>.GetSrid(member,  _providerName);
            return ss;
        }
        private string GetColumnNameSimple(string member, Type type)
        {
            var ss = AttributesOfClass<T>.GetNameSimpleColumnForQuery(member, _providerName);
            return ss;
        }

        private static Expression StripQuotes(Expression e)
        {
            while (e.NodeType == ExpressionType.Quote) e = ((UnaryExpression)e).Operand;
            return e;
        }


        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            //if (m.Method.Name == "GroupByCore")
            //{
            //    var list = new List<string>();
            //
            //    Visit(m.Arguments[1]);
            //    var rr = StringB.ToString();
            //}

            // if (m.Method.Name == "Join")
            // {
            //     var list = new List<string>();
            //
            //     var tt = (NewExpression)((LambdaExpression)((UnaryExpression)m.Arguments[4]).Operand).Body;
            //     foreach (var argument in tt.Arguments)
            //     {
            //         var et = ((MemberExpression)argument).Expression.Type;
            //         var fieldName = StorageTypeAttribute.DictionaryAttribute[et].Translation(_providerName, argument, 0)
            //             .FieldName;
            //         list.Add(fieldName);
            //     }
            //
            //     AddListOne(new OneComposite { Operand = Evolution.SelectJoin, Body = string.Join(",", list) });
            //     var builder = new StringBuilder();
            //
            //     builder.AppendLine().Append(" JOIN ");
            //
            //     var ee = m.Arguments[1].Type.GenericTypeArguments[0];
            //     var tableName = StorageTypeAttribute.DictionaryAttribute[ee].GetTableName(_providerName);
            //     builder.Append(tableName).AppendLine(" ");
            //     builder.Append(" ON ( ");
            //     var field = StorageTypeAttribute.DictionaryAttribute[typeof(T)]
            //         .Translation(_providerName, m.Arguments[2], 0).FieldName;
            //     builder.Append(field).Append(" = ");
            //     field = StorageTypeAttribute.DictionaryAttribute[ee].Translation(_providerName, m.Arguments[3], 0)
            //         .FieldName;
            //     builder.Append(field).Append(") ").AppendLine();
            //     var sql = builder.ToString();
            //     AddListOne(new OneComposite { Operand = Evolution.Join, Body = sql });
            //
            //     int pi;
            //     {
            //         var r = StorageTypeAttribute.DictionaryAttribute[typeof(T)];
            //         var res = r.Translation(_providerName, m.Arguments[0], 0);
            //         ListOne.AddRange(res.ListOneComposite);
            //         Param = res.Params;
            //         pi = res.ParamsIndex;
            //     }
            //     {
            //         ee = m.Arguments[1].Type.GenericTypeArguments[0];
            //         var r = StorageTypeAttribute.DictionaryAttribute[ee];
            //         var res = r.Translation(_providerName, m.Arguments[1], pi);
            //         foreach (var composite in res.ListOneComposite)
            //             if (composite.Operand == Evolution.Where)
            //                 AddListOne(composite);
            //         foreach (var keyValuePair in res.Params)
            //             if (Param.ContainsKey(keyValuePair.Key) == false)
            //                 Param.Add(keyValuePair.Key, keyValuePair.Value);
            //     }
            //
            //     _currentMethod = "join";
            //
            //     return m;
            // }
            if (m.Method.Name == "ST_GeogFromText")
            {
                var lambda = (MemberExpression)m.Arguments[0];
                var dd = lambda.Member;
                StringB.Append($" ST_GeogFromText(ST_AsText(");
                Visit(lambda);
                StringB.Append("))::bytea, ");

                _geoAsName = null;
                return m;
            }
          
            if (m.Method.Name.StartsWith("ST_"))
            {
                var lambda = (MemberExpression)m.Arguments[0];
                var dd = lambda.Member;
                StringB.Append($" {m.Method.Name}(");
                Visit(lambda);
                StringB.Append("), ");
                _geoAsName = null;
                return m;
            }

            // if (m.Method.Name == "ST_Length")
            // {
            //
            //     var lambda = (MemberExpression)m.Arguments[0];
            //     var dd = lambda.Member;
            //     StringB.Append($" ST_Length(");
            //     Visit(lambda);
            //     StringB.Append("), ");
            //     _geoAsName = null;
            //     return m;
            // }
            //
            // if (m.Method.Name == "ST_AsText")
            // {
            //
            //     var lambda = (MemberExpression)m.Arguments[0];
            //     var dd = lambda.Member;
            //     StringB.Append($" ST_AsText(");
            //     Visit(lambda);
            //     StringB.Append("), ");
            //     _geoAsName = null;
            //     return m;
            // }

            if (m.Method.DeclaringType == typeof(V)
                && m.Method.Name == "FreeSql")
            {
                var constantExpression = (ConstantExpression)m.Object;
                if (constantExpression == null) return m;
                var val = m.Method.Invoke(constantExpression.Value, null);

                AddListOne(new OneComposite { Body = val.ToString(), Operand = Evolution.FreeSql });
                return m;
            }

            if (m.Method.DeclaringType == typeof(V)
                && m.Method.Name == "TableCreate")
            {
                var constantExpression = (ConstantExpression)m.Object;
                if (constantExpression == null) return m;
                var val = m.Method.Invoke(constantExpression.Value, null);

                AddListOne(new OneComposite { Body = val.ToString(), Operand = Evolution.TableCreate });
                return m;
            }

            if (m.Method.DeclaringType == typeof(V)
                && m.Method.Name == "DropTable")
            {
                var constantExpression = (ConstantExpression)m.Object;
                if (constantExpression == null) return m;
                var val = m.Method.Invoke(constantExpression.Value, null);

                AddListOne(new OneComposite { Body = val.ToString(), Operand = Evolution.DropTable });
                return m;
            }

            if (m.Method.DeclaringType == typeof(V)
                && m.Method.Name == "TableExists")
            {
                var constantExpression = (ConstantExpression)m.Object;
                if (constantExpression == null) return m;
                var val = m.Method.Invoke(constantExpression.Value, null);

                AddListOne(new OneComposite { Body = val.ToString(), Operand = Evolution.TableExists });
                return m;
            }

            if (m.Method.DeclaringType == typeof(V)
                && m.Method.Name == "ExecuteScalar")
            {
                var constantExpression = (ConstantExpression)m.Object;
                if (constantExpression == null) return m;
                var val = m.Method.Invoke(constantExpression.Value, null);

                AddListOne(new OneComposite { Body = val.ToString(), Operand = Evolution.ExecuteScalar });
                return m;
            }

            if (m.Method.DeclaringType == typeof(V)
                && m.Method.Name == "TruncateTable")
            {
                var constantExpression = (ConstantExpression)m.Object;
                if (constantExpression == null) return m;
                var val = m.Method.Invoke(constantExpression.Value, null);

                AddListOne(new OneComposite { Body = val.ToString(), Operand = Evolution.TruncateTable });
                return m;
            }

            if (m.Method.DeclaringType == typeof(V)
                && m.Method.Name == "ExecuteNonQuery")
            {
                var constantExpression = (ConstantExpression)m.Object;
                if (constantExpression == null) return m;
                var val = m.Method.Invoke(constantExpression.Value, null);

                AddListOne(new OneComposite { Body = val.ToString(), Operand = Evolution.ExecuteNonQuery });
                return m;
            }

            if (m.Method.DeclaringType == typeof(V)
                && m.Method.Name == "DataTable")
            {
                var constantExpression = (ConstantExpression)m.Object;
                if (constantExpression == null) return m;
                var val = m.Method.Invoke(constantExpression.Value, null);

                AddListOne(new OneComposite { Body = val.ToString(), Operand = Evolution.DataTable });
                return m;
            }


            if (m.Method.DeclaringType == typeof(Queryable)
                && m.Method.Name == "ElementAt")
            {
                Visit(m.Arguments[0]);
                StringB.Clear();
                Visit(m.Arguments[1]);
                if (_providerName == ProviderName.MsSql)
                {
                    AddListOne(new OneComposite
                    {
                        Operand = Evolution.Limit,
                        Body = string.Format(" LIMIT {0},1", StringB)
                    });
                    var o = new OneComposite
                    {
                        Operand = Evolution.ElementAt,
                        Body = StringB.ToString()
                    };
                    AddListOne(o);
                }
                else
                {
                    var o = new OneComposite
                    {
                        Operand = Evolution.ElementAt,
                        Body = StringB.ToString()
                    };
                    AddListOne(o);
                }

                StringB.Clear();
                return m;
            }

            if (m.Method.DeclaringType == typeof(Queryable)
                && m.Method.Name == "Contains")
            {
                var o = new OneComposite
                {
                    Operand = Evolution.Contains
                };
                AddListOne(o);
                Visit(m.Arguments[0]);
                StringB.Clear();
                Visit(m.Arguments[1]);

                var o1 = new OneComposite
                {
                    Operand = Evolution.Where,
                    Body = StringB.ToString()
                };
                AddListOne(o1);
                var o2 = new OneComposite
                {
                    Operand = Evolution.Count
                };
                AddListOne(o2);
                var o3 = new OneComposite
                {
                    Operand = Evolution.All
                };
                AddListOne(o3);

                StringB.Clear();
                return m;
            }


            if (m.Method.DeclaringType == typeof(Queryable)
                && m.Method.Name == "ElementAtOrDefault")
            {
                if (_providerName == ProviderName.MsSql)
                {
                    Visit(m.Arguments[0]);
                    StringB.Clear();
                    var o = new OneComposite
                    {
                        Operand = Evolution.ElementAtOrDefault
                    };
                    AddListOne(o);
                    Visit(m.Arguments[1]);
                    AddListOne(new OneComposite
                    {
                        Operand = Evolution.Limit,
                        Body = $" LIMIT {StringB},1"
                    });
                    StringB.Clear();
                    return m;
                }
                else
                {
                    Visit(m.Arguments[0]);
                    StringB.Clear();
                    Visit(m.Arguments[1]);
                    var o = new OneComposite
                    {
                        Operand = Evolution.ElementAtOrDefault,
                        Body = StringB.ToString()
                    };
                    AddListOne(o);
                    StringB.Clear();
                }


                return m;
            }

            if (m.Method.DeclaringType == typeof(Queryable)
                && m.Method.Name == "SelectMany")
            {
                if (m.Arguments.Count == 2) return BindSelectMany(m, m.Arguments[0], GetLambda(m.Arguments[1]), null);
                if (m.Arguments.Count == 3)
                    return BindSelectMany(m, m.Arguments[0], GetLambda(m.Arguments[1]), GetLambda(m.Arguments[2]));
            }

            //if ((m.Method.DeclaringType == typeof(Queryable) && m.Method.Name == "Join") ||
            //    (m.Method.DeclaringType == typeof(Queryable) && m.Method.Name == "GroupJoin"))
            //{
            //
            //    for (var i = 0; i < m.Arguments.Count; i++)
            //    {
            //        StringB.Length = 0;
            //        if (i == 0)
            //        {
            //            Visit(m.Arguments[0]);
            //        }
            //
            //        if (i == 1)
            //        {
            //            Visit(m.Arguments[1]);
            //        }
            //
            //        if (i == 2)
            //        {
            //            StringB.Length = 0;
            //            Visit(m.Arguments[2]);
            //        }
            //        if (i == 3)
            //        {
            //            StringB.Length = 0;
            //            Visit(m.Arguments[3]);
            //        }
            //
            //        if (i == 4)
            //        {
            //            StringB.Length = 0;
            //            Visit(m.Arguments[4]);
            //        }
            //
            //    }
            //
            //    return m;
            //}


            if (m.Method.DeclaringType == typeof(DateTime))
                switch (m.Method.Name)
                {
                    case "op_Subtract":
                        if (m.Arguments[1].Type == typeof(DateTime))
                        {
                            StringB.Append("DATEDIFF(");
                            Visit(m.Arguments[0]);
                            StringB.Append(", ");
                            Visit(m.Arguments[1]);
                            StringB.Append(")");
                            return m;
                        }

                        break;
                    case "AddYears":
                        switch (_providerName)
                        {
                            case ProviderName.PostgreSql:
                                {
                                    StringB.Append("(");
                                    Visit(m.Object);
                                    StringB.Append(" + INTERVAL '");
                                    StringB.Append(m.Arguments[0]);
                                    StringB.Append(" YEAR')");
                                    break;
                                }
                            case ProviderName.SqLite:
                                {
                                    StringB.Append("DATE(");
                                    Visit(m.Object);
                                    StringB.Append(", '");
                                    Visit(m.Arguments[0]);
                                    StringB.Append(" YEAR')");
                                    break;
                                }
                            case ProviderName.MySql:
                                {
                                    StringB.Append("DATE_ADD(");
                                    Visit(m.Object);
                                    StringB.Append(", INTERVAL ");
                                    Visit(m.Arguments[0]);
                                    StringB.Append(" YEAR)");
                                    break;
                                }
                            case ProviderName.MsSql:
                                {
                                    StringB.Append("DATEADD(YEAR,");
                                    Visit(m.Arguments[0]);
                                    StringB.Append(", ");
                                    Visit(m.Object);
                                    StringB.Append(" )");
                                    break;
                                }
                        }

                        return m;
                    case "AddMonths":

                        switch (_providerName)
                        {
                            case ProviderName.PostgreSql:
                                {
                                    StringB.Append("(");
                                    Visit(m.Object);
                                    StringB.Append(" + INTERVAL '");
                                    StringB.Append(m.Arguments[0]);
                                    StringB.Append(" MONTH')");
                                    break;
                                }
                            case ProviderName.SqLite:
                                {
                                    StringB.Append("DATE(");
                                    Visit(m.Object);
                                    StringB.Append(", '");
                                    Visit(m.Arguments[0]);
                                    StringB.Append(" MONTH')");
                                    break;
                                }
                            case ProviderName.MySql:
                                {
                                    StringB.Append("DATE_ADD(");
                                    Visit(m.Object);
                                    StringB.Append(", INTERVAL ");
                                    Visit(m.Arguments[0]);
                                    StringB.Append(" MONTH)");
                                    break;
                                }
                            case ProviderName.MsSql:
                                {
                                    StringB.Append("DATEADD(MONTH,");
                                    Visit(m.Arguments[0]);
                                    StringB.Append(", ");
                                    Visit(m.Object);
                                    StringB.Append(" )");
                                    break;
                                }
                        }

                        return m;
                    case "AddDays": //DATE

                        switch (_providerName)
                        {
                            case ProviderName.PostgreSql:
                                {
                                    StringB.Append("(");
                                    Visit(m.Object);
                                    StringB.Append(" + INTERVAL '");
                                    StringB.Append(m.Arguments[0]);
                                    StringB.Append(" DAY')");
                                    break;
                                }
                            case ProviderName.SqLite:
                                {
                                    StringB.Append("DATE(");
                                    Visit(m.Object);
                                    StringB.Append(", '");
                                    Visit(m.Arguments[0]);
                                    StringB.Append(" DAY')");
                                    break;
                                }
                            case ProviderName.MySql:
                                {
                                    StringB.Append("DATE_ADD(");
                                    Visit(m.Object);
                                    StringB.Append(", INTERVAL ");
                                    Visit(m.Arguments[0]);
                                    StringB.Append(" DAY)");
                                    break;
                                }
                            case ProviderName.MsSql:
                                {
                                    StringB.Append("DATEADD(DAY,");
                                    Visit(m.Arguments[0]);
                                    StringB.Append(", ");
                                    Visit(m.Object);
                                    StringB.Append(" )");

                                    break;
                                }
                        }

                        return m;
                    case "AddHours":
                        switch (_providerName)
                        {
                            case ProviderName.PostgreSql:
                                {
                                    StringB.Append("(");
                                    Visit(m.Object);
                                    StringB.Append(" + INTERVAL '");
                                    StringB.Append(m.Arguments[0]);
                                    StringB.Append(" HOUR')");
                                    break;
                                }
                            case ProviderName.SqLite:
                                {
                                    StringB.Append("DATETIME(");
                                    Visit(m.Object);
                                    StringB.Append(", '");
                                    Visit(m.Arguments[0]);
                                    StringB.Append(" HOUR')");
                                    break;
                                }
                            case ProviderName.MySql:
                                {
                                    StringB.Append("DATE_ADD(");
                                    Visit(m.Object);
                                    StringB.Append(", INTERVAL ");
                                    Visit(m.Arguments[0]);
                                    StringB.Append(" HOUR)");
                                    break;
                                }
                            case ProviderName.MsSql:
                                {
                                    StringB.Append("DATEADD(HOUR,");
                                    Visit(m.Arguments[0]);
                                    StringB.Append(", ");
                                    Visit(m.Object);
                                    StringB.Append(" )");
                                    break;
                                }
                        }

                        return m;
                    case "AddMinutes":

                        switch (_providerName)
                        {
                            case ProviderName.PostgreSql:
                                {
                                    StringB.Append("(");
                                    Visit(m.Object);
                                    StringB.Append(" + INTERVAL '");
                                    StringB.Append(m.Arguments[0]);
                                    StringB.Append(" MINUTE')");
                                    break;
                                }
                            case ProviderName.SqLite:
                                {
                                    StringB.Append("DATETIME(");
                                    Visit(m.Object);
                                    StringB.Append(", '");
                                    Visit(m.Arguments[0]);
                                    StringB.Append(" minutes')");
                                    break;
                                }
                            case ProviderName.MySql:
                                {
                                    StringB.Append("DATE_ADD(");
                                    Visit(m.Object);
                                    StringB.Append(", INTERVAL ");
                                    Visit(m.Arguments[0]);
                                    StringB.Append(" MINUTE)");
                                    break;
                                }
                            case ProviderName.MsSql:
                                {
                                    StringB.Append("DATEADD(MINUTE,");
                                    Visit(m.Arguments[0]);
                                    StringB.Append(", ");
                                    Visit(m.Object);
                                    StringB.Append(" )");
                                    break;
                                }
                        }

                        return m;
                    case "AddSeconds":

                        switch (_providerName)
                        {
                            case ProviderName.PostgreSql:
                                {
                                    StringB.Append("(");
                                    Visit(m.Object);
                                    StringB.Append(" + INTERVAL '");
                                    StringB.Append(m.Arguments[0]);
                                    StringB.Append(" SECOND')");
                                    break;
                                }
                            case ProviderName.SqLite:
                                {
                                    StringB.Append("DATETIME(");
                                    Visit(m.Object);
                                    StringB.Append(", '");
                                    Visit(m.Arguments[0]);
                                    StringB.Append(" SECOND')");
                                    break;
                                }
                            case ProviderName.MySql:
                                {
                                    StringB.Append("DATE_ADD(");
                                    Visit(m.Object);
                                    StringB.Append(", INTERVAL ");
                                    Visit(m.Arguments[0]);
                                    StringB.Append(" SECOND)");
                                    break;
                                }
                            case ProviderName.MsSql:
                                {
                                    StringB.Append("DATEADD(SECOND,");
                                    Visit(m.Arguments[0]);
                                    StringB.Append(", ");
                                    Visit(m.Object);
                                    StringB.Append(" )");
                                    break;
                                }
                        }


                        return m;
                    case "AddMilliseconds":

                        switch (_providerName)
                        {
                            case ProviderName.PostgreSql:
                                {
                                    StringB.Append("(");
                                    Visit(m.Object);
                                    StringB.Append(" + INTERVAL '");
                                    StringB.Append(m.Arguments[0]);
                                    StringB.Append(" MICROSECOND')");
                                    break;
                                }
                            case ProviderName.SqLite:
                                {
                                    StringB.Append("DATE(");
                                    Visit(m.Object);
                                    StringB.Append(", '");
                                    Visit(m.Arguments[0]);
                                    StringB.Append(" MICROSECOND')");
                                    break;
                                }
                            case ProviderName.MySql:
                                {
                                    StringB.Append("DATE_ADD(");
                                    Visit(m.Object);
                                    StringB.Append(", INTERVAL (");
                                    Visit(m.Arguments[0]);
                                    StringB.Append("* 1000) MICROSECOND)");
                                    break;
                                }
                            case ProviderName.MsSql:
                                {
                                    StringB.Append("DATEADD(MICROSECOND,");
                                    Visit(m.Arguments[0]);
                                    StringB.Append(", ");
                                    Visit(m.Object);
                                    StringB.Append(" )");
                                    break;
                                }
                        }


                        return m;
                }

            if (m.Method.ReturnType == typeof(char[]) && m.Method.Name == "ToArray")
                Visit(m.Arguments[0]); //Trin("ddas".ToArray())
            if (m.Method.DeclaringType == typeof(string) || m.Method.DeclaringType == typeof(Enumerable))
                switch (m.Method.Name)
                {
                    case "Reverse":
                        {
                            StringB.Append(" REVERSE(");
                            Visit(m.Arguments[0]);
                            StringB.Append(") ");
                            return m;
                        }
                    case "StartsWith":
                        {
                            StringB.Append("(");
                            Visit(m.Object);
                            switch (_providerName)
                            {
                                case ProviderName.SqLite:
                                    {
                                        StringB.AppendFormat(" {0} ", StringConst.Like);
                                        var x = StringB.Length;
                                        Visit(m.Arguments[0]);
                                        var s = StringB.ToString().Substring(x);
                                        var p = Param[s];
                                        if (p != null) Param[s] = $"{p}%";
                                        StringB.Append(")");
                                        break;
                                    }
                                case ProviderName.MySql:
                                case ProviderName.PostgreSql:
                                    {
                                        StringB.AppendFormat(" {0} CONCAT(", StringConst.Like);
                                        Visit(m.Arguments[0]);
                                        StringB.Append(",'%'))");
                                        break;
                                    }

                                case ProviderName.MsSql:
                                    {
                                        StringB.AppendFormat(" {0} CONCAT(", StringConst.Like);
                                        Visit(m.Arguments[0]);
                                        StringB.Append(",'%'))");
                                        break;
                                    }
                            }

                            return m;
                        }
                    case "LikeSql":
                        {
                            return m;
                        }
                    case "EndsWith":
                        {
                            switch (_providerName)
                            {
                                case ProviderName.SqLite:
                                    {
                                        StringB.Append("(");
                                        Visit(m.Object);
                                        StringB.AppendFormat(" {0} ", StringConst.Like);
                                        var x = StringB.Length;
                                        Visit(m.Arguments[0]);
                                        var s = StringB.ToString().Substring(x);
                                        StringB.Append(")");
                                        var p = Param[s];
                                        if (p != null) Param[s] = $"%{p}";
                                        break;
                                    }
                                case ProviderName.MySql:
                                case ProviderName.PostgreSql:
                                    {
                                        StringB.Append("(");
                                        Visit(m.Object);
                                        StringB.AppendFormat(" {0} CONCAT('%',", StringConst.Like);
                                        Visit(m.Arguments[0]);
                                        StringB.Append("))");
                                        break;
                                    }
                                case ProviderName.MsSql:
                                    {
                                        StringB.Append("(");
                                        Visit(m.Object);
                                        StringB.AppendFormat(" {0} CONCAT('%',", StringConst.Like);
                                        Visit(m.Arguments[0]);
                                        StringB.Append("))");
                                        break;
                                    }
                            }

                            return m;
                        }
                    case "Contains":
                        {
                            switch (_providerName)
                            {
                                case ProviderName.MsSql:
                                    StringB.Append("(");
                                    Visit(m.Object);
                                    StringB.AppendFormat(" {0} CONCAT('%',", StringConst.Like);
                                    Visit(m.Arguments[0]);
                                    StringB.Append(",'%'))");
                                    break;
                                case ProviderName.MySql:
                                    {
                                        StringB.Append("(");
                                        Visit(m.Object);
                                        StringB.AppendFormat(" {0} CONCAT('%',", StringConst.Like);
                                        Visit(m.Arguments[0]);
                                        StringB.Append(",'%'))");
                                        break;
                                    }
                                case ProviderName.PostgreSql:
                                    {
                                        StringB.Append("(");
                                        Visit(m.Object);
                                        StringB.AppendFormat(" {0} CONCAT('%',", StringConst.Like);
                                        Visit(m.Arguments[0]);
                                        StringB.Append(",'%'))");
                                        break;
                                    }
                                case ProviderName.SqLite:
                                    {
                                        StringB.Append("(");
                                        Visit(m.Object);
                                        StringB.AppendFormat(" {0} ", StringConst.Like);
                                        var x = StringB.Length;
                                        Visit(m.Arguments[0]);
                                        var s = StringB.ToString().Substring(x);
                                        StringB.Append(")");
                                        var p = Param[s];
                                        if (p != null) Param[s] = $"%{p}%";
                                        break;
                                    }
                            }

                            return m;
                        }
                    case "Concat":
                        {
                            switch (_providerName)
                            {
                                case ProviderName.PostgreSql:
                                    {
                                        IList<Expression> args = m.Arguments;
                                        if (args.Count == 1 && args[0].NodeType == ExpressionType.NewArrayInit)
                                            args = ((NewArrayExpression)args[0]).Expressions;
                                        StringB.Append("CONCAT(");
                                        for (int i = 0, n = args.Count; i < n; i++)
                                        {
                                            if (i > 0) StringB.Append(", ");
                                            Visit(args[i]);
                                        }

                                        StringB.Append(")");
                                        break;
                                    }
                                case ProviderName.MySql:
                                    {
                                        IList<Expression> args = m.Arguments;
                                        if (args.Count == 1 && args[0].NodeType == ExpressionType.NewArrayInit)
                                            args = ((NewArrayExpression)args[0]).Expressions;
                                        StringB.Append("CONCAT(");
                                        for (int i = 0, n = args.Count; i < n; i++)
                                        {
                                            if (i > 0) StringB.Append(", ");
                                            Visit(args[i]);
                                        }

                                        StringB.Append(")");
                                        break;
                                    }
                                case ProviderName.SqLite:
                                    {
                                        IList<Expression> args = m.Arguments;
                                        if (args.Count == 1 && args[0].NodeType == ExpressionType.NewArrayInit)
                                            args = ((NewArrayExpression)args[0]).Expressions;
                                        StringB.Append("(");
                                        for (int i = 0, n = args.Count; i < n; i++)
                                        {
                                            if (i > 0) StringB.Append(" || ");
                                            Visit(args[i]);
                                        }

                                        StringB.Append(")");
                                        break;
                                    }
                                case ProviderName.MsSql:
                                    {
                                        IList<Expression> args = m.Arguments;
                                        if (args.Count == 1 && args[0].NodeType == ExpressionType.NewArrayInit)
                                            args = ((NewArrayExpression)args[0]).Expressions;
                                        StringB.Append("CONCAT(");
                                        for (int i = 0, n = args.Count; i < n; i++)
                                        {
                                            if (i > 0) StringB.Append(", ");
                                            Visit(args[i]);
                                        }

                                        StringB.Append(")");
                                        break;
                                    }
                            }

                            return m;
                        }
                    case "IsNullOrEmpty":
                        {
                            switch (_providerName)
                            {
                                case ProviderName.PostgreSql:
                                    {
                                        StringB.Append("(");
                                        Visit(m.Arguments[0]);
                                        StringB.Append(" = '') IS NOT FALSE ");
                                        break;
                                    }
                                case ProviderName.MySql:
                                    {
                                        StringB.Append("(");
                                        Visit(m.Arguments[0]);
                                        StringB.Append(" IS NULL OR ");
                                        Visit(m.Arguments[0]);
                                        StringB.Append(" = '')");
                                        break;
                                    }
                                case ProviderName.SqLite:
                                    {
                                        StringB.Append("(");
                                        Visit(m.Arguments[0]);
                                        StringB.Append(" IS NULL OR ");
                                        Visit(m.Arguments[0]);
                                        StringB.Append(" = '')");
                                        break;
                                    }
                                case ProviderName.MsSql:
                                    {
                                        StringB.Append("( CONVERT(VARCHAR,");
                                        Visit(m.Arguments[0]);
                                        StringB.Append(") ");
                                        StringB.Append(" IS NULL OR CONVERT(VARCHAR,");
                                        Visit(m.Arguments[0]);
                                        StringB.Append(") = '' ");
                                        StringB.Append(")");
                                        break;
                                    }
                            }

                            return m;
                        }
                    case "ToUpper":
                        {
                            StringB.Append("UPPER(");
                            Visit(m.Object);
                            StringB.Append(")");
                            return m;
                        }
                    case "ToLower":
                        {
                            StringB.Append("LOWER(");
                            Visit(m.Object);
                            StringB.Append(")");
                            return m;
                        }
                    case "Replace":
                        {
                            StringB.Append("REPLACE(");
                            Visit(m.Object);
                            StringB.Append(", ");
                            Visit(m.Arguments[0]);
                            StringB.Append(", ");
                            Visit(m.Arguments[1]);
                            StringB.Append(")");
                            return m;
                        }
                    case "Substring":
                        {
                            if (_providerName == ProviderName.MsSql)
                            {
                                StringB.Append("SUBSTRING(");
                                Visit(m.Object);
                                StringB.Append(", ");
                                Visit(m.Arguments[0]);
                                StringB.Append(" + 1");
                                if (m.Arguments.Count == 2)
                                {
                                    StringB.Append(", ");
                                    Visit(m.Arguments[1]);
                                }

                                if (m.Arguments.Count == 1) StringB.Append(", 1000000 ");
                                StringB.Append(")");
                                return m;
                            }

                            StringB.Append("SUBSTRING(");
                            Visit(m.Object);
                            StringB.Append(", ");
                            Visit(m.Arguments[0]);
                            StringB.Append(" + 1");
                            if (m.Arguments.Count == 2)
                            {
                                StringB.Append(", ");
                                Visit(m.Arguments[1]);
                            }

                            StringB.Append(")");
                            return m;
                        }
                    case "Remove":
                        {
                            if (m.Arguments.Count == 1)
                            {
                                StringB.Append("LEFT(");
                                Visit(m.Object);
                                StringB.Append(", ");
                                Visit(m.Arguments[0]);
                                StringB.Append(")");
                            }
                            else
                            {
                                StringB.Append("CONCAT(");
                                StringB.Append("LEFT(");
                                Visit(m.Object);
                                StringB.Append(", ");
                                Visit(m.Arguments[0]);
                                StringB.Append("), SUBSTRING(");
                                Visit(m.Object);
                                StringB.Append(", ");
                                Visit(m.Arguments[0]);
                                StringB.Append(" + ");
                                Visit(m.Arguments[1]);
                                StringB.Append("))");
                            }

                            return m;
                        }
                    case "IndexOf":
                        {
                            if (_providerName == ProviderName.MsSql)
                            {
                                StringB.Append("(CHARINDEX(");
                                Visit(m.Arguments[0]);
                                StringB.Append(", ");
                                Visit(m.Object);
                                if (m.Arguments.Count == 2 && m.Arguments[1].Type == typeof(int))
                                {
                                    StringB.Append(", ");
                                    Visit(m.Arguments[1]);
                                    StringB.Append(" + 1");
                                }

                                StringB.Append(") - 1)");
                                return m;
                            }

                            StringB.Append("(LOCATE(");
                            Visit(m.Arguments[0]);
                            StringB.Append(", ");
                            Visit(m.Object);
                            if (m.Arguments.Count == 2 && m.Arguments[1].Type == typeof(int))
                            {
                                StringB.Append(", ");
                                Visit(m.Arguments[1]);
                                StringB.Append(" + 1");
                            }

                            StringB.Append(") - 1)");
                            return m;
                        }
                    case "Trim":
                        {
                            switch (_providerName)
                            {
                                case ProviderName.PostgreSql:
                                    {
                                        StringB.Append("TRIM(both from ");
                                        Visit(m.Object);
                                        if (m.Arguments.Count > 0)
                                        {
                                            var ee = (NewArrayExpression)m.Arguments[0];
                                            for (var i = 0; i < ee.Expressions.Count; i++)
                                            {
                                                if (i == 0) StringB.Append(", '");

                                                StringB.Append(ee.Expressions[i]);
                                                if (i == ee.Expressions.Count - 1) StringB.Append("'");
                                            }
                                        }

                                        StringB.Append(")");
                                        break;
                                    }
                                case ProviderName.MySql:
                                    {
                                        if (m.Arguments.Count == 0)
                                        {
                                            StringB.Append("TRIM(");
                                            Visit(m.Object);
                                            StringB.Append(")");
                                        }
                                        else if (m.Arguments.Count == 1)
                                        {
                                            StringB.Append("TRIM(LEADING ");
                                            Visit(m.Arguments[0]);
                                            StringB = StringB.Replace(",", "");
                                            StringB.Append(" FROM ");
                                            Visit(m.Object);
                                            StringB.Append(")");
                                        }

                                        break;
                                    }
                                case ProviderName.SqLite:
                                    {
                                        StringB.Append("TRIM(");
                                        Visit(m.Object);
                                        if (m.Arguments.Count > 0)
                                        {
                                            var ee = (NewArrayExpression)m.Arguments[0];
                                            for (var i = 0; i < ee.Expressions.Count; i++)
                                            {
                                                if (i == 0) StringB.Append(", '");

                                                StringB.Append(ee.Expressions[i]);
                                                if (i == ee.Expressions.Count - 1) StringB.Append("'");
                                            }
                                        }

                                        StringB.Append(")");
                                        break;
                                    }
                                case ProviderName.MsSql:
                                    {
                                        StringB.Append("RTRIM(");
                                        StringB.Append("LTRIM(");
                                        Visit(m.Object);
                                        if (m.Arguments.Count > 0)
                                        {
                                            var ee = (NewArrayExpression)m.Arguments[0];
                                            for (var i = 0; i < ee.Expressions.Count; i++)
                                            {
                                                if (i == 0) StringB.Append(", '");

                                                StringB.Append(ee.Expressions[i]);
                                                if (i == ee.Expressions.Count - 1) StringB.Append("' ");
                                            }
                                        }

                                        StringB.Append(")");
                                        if (m.Arguments.Count > 0)
                                        {
                                            var ee = (NewArrayExpression)m.Arguments[0];
                                            for (var i = 0; i < ee.Expressions.Count; i++)
                                            {
                                                if (i == 0) StringB.Append(", '");

                                                StringB.Append(ee.Expressions[i]);
                                                if (i == ee.Expressions.Count - 1) StringB.Append("' ");
                                            }
                                        }

                                        StringB.Append(")");
                                        break;
                                    }
                            }

                            return m;
                        }

                    case "TrimEnd":
                        {
                            switch (_providerName)
                            {
                                case ProviderName.PostgreSql:
                                    {
                                        StringB.Append("TRIM(trailing from ");
                                        Visit(m.Object);
                                        var ee = (NewArrayExpression)m.Arguments[0];
                                        for (var i = 0; i < ee.Expressions.Count; i++)
                                        {
                                            if (i == 0) StringB.Append(", '");

                                            StringB.Append(ee.Expressions[i]);
                                            if (i == ee.Expressions.Count - 1) StringB.Append("'");
                                        }

                                        StringB.Append(")");
                                        break;
                                    }
                                case ProviderName.MySql:
                                    {
                                        StringB.Append("TRIM(TRAILING  ");

                                        var ee = (NewArrayExpression)m.Arguments[0];
                                        for (var i = 0; i < ee.Expressions.Count; i++)
                                        {
                                            if (i == 0) StringB.Append(" '");

                                            StringB.Append(ee.Expressions[i]);
                                            if (i == ee.Expressions.Count - 1) StringB.Append("'");
                                        }

                                        StringB.Append(" FROM ");
                                        Visit(m.Object);

                                        StringB.Append(")");
                                        break;
                                    }
                                case ProviderName.SqLite:
                                    {
                                        StringB.Append("RTRIM(");
                                        Visit(m.Object);
                                        if (m.Arguments.Count > 0)
                                        {
                                            var ee = (NewArrayExpression)m.Arguments[0];
                                            for (var i = 0; i < ee.Expressions.Count; i++)
                                            {
                                                if (i == 0) StringB.Append(", '");

                                                StringB.Append(ee.Expressions[i]);
                                                if (i == ee.Expressions.Count - 1) StringB.Append("'");
                                            }
                                        }

                                        StringB.Append(")");
                                        break;
                                    }
                                case ProviderName.MsSql:
                                    {
                                        StringB.Append("RTRIM(");
                                        Visit(m.Object);
                                        if (m.Arguments.Count > 0)
                                        {
                                            var ee = (NewArrayExpression)m.Arguments[0];
                                            for (var i = 0; i < ee.Expressions.Count; i++)
                                            {
                                                if (i == 0) StringB.Append(", '");

                                                StringB.Append(ee.Expressions[i]);
                                                if (i == ee.Expressions.Count - 1) StringB.Append("' ");
                                            }
                                        }

                                        StringB.Append(")");
                                        break;
                                    }
                            }

                            return m;
                        }
                    case "TrimStart":
                        {
                            switch (_providerName)
                            {
                                case ProviderName.PostgreSql:
                                    {
                                        StringB.Append("TRIM(leading from ");
                                        Visit(m.Object);
                                        var ee = (NewArrayExpression)m.Arguments[0];
                                        for (var i = 0; i < ee.Expressions.Count; i++)
                                        {
                                            if (i == 0) StringB.Append(", '");

                                            StringB.Append(ee.Expressions[i]);
                                            if (i == ee.Expressions.Count - 1) StringB.Append("'");
                                        }

                                        StringB.Append(")");
                                        break;
                                    }
                                case ProviderName.MySql:
                                    {
                                        StringB.Append("TRIM(LEADING   ");

                                        var ee = (NewArrayExpression)m.Arguments[0];
                                        for (var i = 0; i < ee.Expressions.Count; i++)
                                        {
                                            if (i == 0) StringB.Append(" '");

                                            StringB.Append(ee.Expressions[i]);
                                            if (i == ee.Expressions.Count - 1) StringB.Append("'");
                                        }

                                        StringB.Append(" FROM ");
                                        Visit(m.Object);

                                        StringB.Append(")");
                                        break;
                                    }
                                case ProviderName.SqLite:
                                    {
                                        StringB.Append("LTRIM(");
                                        Visit(m.Object);
                                        if (m.Arguments.Count > 0)
                                        {
                                            var ee = (NewArrayExpression)m.Arguments[0];
                                            for (var i = 0; i < ee.Expressions.Count; i++)
                                            {
                                                if (i == 0) StringB.Append(", '");

                                                StringB.Append(ee.Expressions[i]);
                                                if (i == ee.Expressions.Count - 1) StringB.Append("'");
                                            }
                                        }

                                        StringB.Append(")");
                                        break;
                                    }
                                case ProviderName.MsSql:
                                    {
                                        StringB.Append("LTRIM(");
                                        Visit(m.Object);
                                        if (m.Arguments.Count > 0)
                                        {
                                            var ee = (NewArrayExpression)m.Arguments[0];
                                            for (var i = 0; i < ee.Expressions.Count; i++)
                                            {
                                                if (i == 0) StringB.Append(", '");

                                                StringB.Append(ee.Expressions[i]);
                                                if (i == ee.Expressions.Count - 1) StringB.Append("' ");
                                            }
                                        }

                                        StringB.Append(")");
                                        break;
                                    }
                            }

                            return m;
                        }
                }

            if (m.Method.DeclaringType == typeof(decimal))
                switch (m.Method.Name)
                {
                    case "Add":
                    case "Subtract":
                    case "Multiply":
                    case "Divide":
                    case "Remainder":
                        StringB.Append("(");
                        VisitValue(m.Arguments[0]);
                        StringB.Append(" ");
                        StringB.Append(GetOperator(m.Method.Name));
                        StringB.Append(" ");
                        VisitValue(m.Arguments[1]);
                        StringB.Append(")");
                        return m;
                    case "Negate":
                        StringB.Append("-");
                        Visit(m.Arguments[0]);
                        StringB.Append("");
                        return m;
                    case "Ceiling":
                    case "Floor":
                        StringB.Append(m.Method.Name.ToUpper());
                        StringB.Append("(");
                        Visit(m.Arguments[0]);
                        StringB.Append(")");
                        return m;
                    case "Round":
                        if (m.Arguments.Count == 1)
                        {
                            StringB.Append("ROUND(");
                            Visit(m.Arguments[0]);
                            StringB.Append(")");
                            return m;
                        }

                        if (m.Arguments.Count == 2 && m.Arguments[1].Type == typeof(int))
                        {
                            StringB.Append("ROUND(");
                            Visit(m.Arguments[0]);
                            StringB.Append(", ");
                            Visit(m.Arguments[1]);
                            StringB.Append(")");
                            return m;
                        }

                        break;
                    case "Truncate":
                        StringB.Append("TRUNCATE(");
                        Visit(m.Arguments[0]);
                        StringB.Append(",0)");
                        return m;
                }
            else if (m.Method.DeclaringType == typeof(Math))
                switch (m.Method.Name)
                {
                    case "Abs":
                    case "Acos":
                    case "Asin":
                    case "Atan":
                    case "Atan2":
                    case "Cos":
                    case "Exp":
                    case "Log10":
                    case "Sin":
                    case "Tan":
                    case "Sqrt":
                    case "Sign":
                    case "Ceiling":
                    case "Floor":
                        StringB.Append(m.Method.Name.ToUpper());
                        StringB.Append("(");
                        Visit(m.Arguments[0]);
                        StringB.Append(")");
                        return m;
                    case "Log":
                        if (m.Arguments.Count == 1) goto case "Log10";
                        break;
                    case "Pow":
                        StringB.Append("POWER(");
                        Visit(m.Arguments[0]);
                        StringB.Append(", ");
                        Visit(m.Arguments[1]);
                        StringB.Append(")");
                        return m;
                    case "Round":
                        if (m.Arguments.Count == 1)
                        {
                            StringB.Append("ROUND(");
                            Visit(m.Arguments[0]);
                            StringB.Append(")");
                            return m;
                        }

                        if (m.Arguments.Count == 2 && m.Arguments[1].Type == typeof(int))
                        {
                            StringB.Append("ROUND(");
                            Visit(m.Arguments[0]);
                            StringB.Append(", ");
                            Visit(m.Arguments[1]);
                            StringB.Append(")");
                            return m;
                        }

                        break;
                    case "Truncate":
                        StringB.Append("TRUNCATE(");
                        Visit(m.Arguments[0]);
                        StringB.Append(",0)");
                        return m;
                }

            if (m.Method.DeclaringType == typeof(Queryable)
                && m.Method.Name == "Where")
            {
                var o = new OneComposite { Operand = Evolution.Where };
                Visit(m.Arguments[0]);
                var lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);

                Visit(lambda.Body);
                o.Body = StringB.ToString();
                AddListOne(o);
                StringB.Length = 0;


                return m;
            }

            if (m.Method.Name == "FromString")
            {
                var u = m.Arguments[0] as ConstantExpression;
                var o = new OneComposite { Operand = Evolution.FromString,Body = u.Value.ToString()};
                AddListOne(o);
                StringB.Length = 0;
                return m;
            }
           
            if (m.Method.Name == "FromStringP")
            {
                var u = m.Arguments[0] as ConstantExpression;
                var o = new OneComposite { Operand = Evolution.FromString, Body = u.Value.ToString() };
                var p = m.Arguments[1] as ConstantExpression;
                var pv = (SqlParam[])p.Value;
                foreach (SqlParam param in pv)
                {
                    Param.Add(param.Name, param.Value);
                }
                AddListOne(o);
                StringB.Length = 0;
                return m;
            }
            
            if (m.Method.Name == "WhereString")
            {
                var t = m.Arguments[0] as ConstantExpression;
                var v=t.Value;
                StringB.Append(v);
              
                return m;
            }
           
            if (m.Method.Name == "WhereStringP")
            {
                var t = m.Arguments[0] as ConstantExpression;
                var v = t.Value;
                StringB.Append(v);

                var p = m.Arguments[1] as ConstantExpression;
                var pv = (SqlParam[])p.Value;
                foreach (SqlParam param in pv)
                {
                    Param.Add(param.Name, param.Value);
                }
               

                return m;
            }
           
            if (m.Method.Name == "WhereIn")
            {

                StringB.Append("(");
                Visit(m.Arguments[0]);
                var sp = m.Arguments[1] as ConstantExpression;
                var o = sp.Value as IEnumerable;
                StringB.Append(" IN (");
                foreach (var o1 in o)
                {
                    if (o1 == null)
                    {
                        StringB.Append("null").Append(", ");
                    }
                    else
                    {
                        var typeCore = o1.GetType();
                        if (UtilsCore.IsNumericType(typeCore))
                        {
                            StringB.Append(o1).Append(", ");

                        }
                        else if (typeCore.IsEnum)
                        {
                            StringB.Append((int)o1).Append(", ");
                        }
                        else if (typeCore == typeof(Guid) || typeCore == typeof(Guid?))
                        {
                            StringB.Append($"'{o1}'").Append(", ");
                        }
                        else
                        {
                            AddParameter(o1);
                            StringB.Append(", ");
                        }
                    }
                }

                var str = StringB.ToString().TrimEnd(',',' ');
                StringB.Clear().Append(str).Append("))");
                return m;
            }
           
            if (m.Method.Name == "WhereNotIn")
            {
                StringB.Append("(");
                Visit(m.Arguments[0]);
                var sp = m.Arguments[1] as ConstantExpression;
                var o = sp.Value as IEnumerable;
                StringB.Append(" NOT IN (");
                foreach (var o1 in o)
                {
                    if (o1 == null)
                    {
                        StringB.Append("null").Append(", ");
                    }
                    else
                    {
                        var typeCore = o1.GetType();
                        if (UtilsCore.IsNumericType(typeCore))
                        {
                            StringB.Append(o1).Append(", ");

                        }
                        else if (typeCore.IsEnum)
                        {
                            StringB.Append((int)o1).Append(", ");
                        }
                        else if (typeCore==typeof(Guid)|| typeCore == typeof(Guid?))
                        {
                            StringB.Append($"'{o1}'").Append(", ");
                        }
                        else
                        {
                            AddParameter(o1);
                            StringB.Append(", ");
                        }
                    }
                }

                var str = StringB.ToString().TrimEnd(',', ' ');
                StringB.Clear().Append(str).Append("))");
                return m;
                
            }
           
            if (m.Method.Name == "GeoST_GeometryType")
            {
                var c = m.Arguments[0] as ConstantExpression;
                var nameColumn = c.Value;

                var c1 = m.Arguments[1] as ConstantExpression;
                var enumType = c1.Value;
                GeoType type = (GeoType)enumType;

               
                string s = "none";
                switch (type)
                {
                    case GeoType.None:
                        throw new ArgumentOutOfRangeException();
                    case GeoType.Point:
                    {
                        s = "ST_Point";
                        if (_providerName == ProviderName.MySql || _providerName == ProviderName.MsSql)
                        {
                            s = "Point";
                        }
                    }
                        break;
                    case GeoType.LineString:
                    {
                        s = "ST_LineString";
                        if (_providerName == ProviderName.MySql || _providerName == ProviderName.MsSql)
                        {
                            s = "LineString";
                        }
                    }
                        break;
                    case GeoType.Polygon:
                    {
                        s = "ST_Polygon";
                        if (_providerName == ProviderName.MySql || _providerName == ProviderName.MsSql)
                        {
                            s = "Polygon";
                        }
                    }
                        break;
                    case GeoType.MultiPoint:
                    {
                        s = "ST_MultiPoint";
                        if (_providerName == ProviderName.MySql || _providerName == ProviderName.MsSql)
                        {
                            s = "MultiPoint";
                        }
                    }
                        break;
                    case GeoType.MultiLineString:
                    {
                        s = "ST_MultiLineString";
                        if (_providerName == ProviderName.MySql || _providerName == ProviderName.MsSql)
                        {
                            s = "MultiLineString";
                        }
                    }
                        break;
                    case GeoType.MultiPolygon:
                    {
                        s = "ST_MultiPolygon";
                        if (_providerName == ProviderName.MySql || _providerName == ProviderName.MsSql)
                        {
                            s = "MultiPolygon";
                        }
                    }
                        break;
                    case GeoType.GeometryCollection:
                    {
                        s = "ST_GeometryCollection";
                        if (_providerName == ProviderName.MySql || _providerName == ProviderName.MsSql)
                        {
                            s = "GeometryCollection";
                        }
                    }
                        break;
                    case GeoType.CircularString:
                    {
                        s = "ST_CircularString";
                        if (_providerName == ProviderName.MySql|| _providerName == ProviderName.MsSql)
                        {
                            s = "CircularString";
                        }
                    }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }





                var o = new OneComposite { Operand = Evolution.Where };
                StringB.Length = 0;
                if (_providerName == ProviderName.MsSql)
                {
                    
                    StringB.Append($" {nameColumn}.STGeometryType() = ");
                }
                else
                {
                    StringB.Append($" ST_GeometryType({nameColumn}) = ");
                }
               
                AddParameter(s);

                o.Body = StringB.ToString();
                StringB.Clear();
                AddListOne(o);
                return m;
            }
           
            if (m.Method.Name == "GeoST_Contains")
            {
                var c = m.Arguments[0] as ConstantExpression;
                var nameColumn = c.Value;

                var c2 = m.Arguments[1] as ConstantExpression;
                IGeoShape geoShape = (IGeoShape)c2.Value;

                var c3 = m.Arguments[2] as ConstantExpression;
                bool actionResult = c3 != null && (bool)c3.Value;
                var o = new OneComposite { Operand = Evolution.Where };
                switch (_providerName)
                {
                    case ProviderName.MsSql:
                        StringB.Length = 0;
                        StringB.Append($" {nameColumn}.STContains(geometry::STGeomFromText(");
                        AddParameter(geoShape.GeoText);
                        StringB.Append(actionResult ? $", {geoShape.Srid}))=1" : $", {geoShape.Srid})) = 0");
                        o.Body = StringB.ToString();
                        StringB.Clear();
                        break;
                    case ProviderName.MySql:
                        StringB.Length = 0;
                        StringB.Append($" ST_Contains({nameColumn},ST_GeomFromText(");
                        AddParameter(geoShape.GeoText);
                        StringB.Append(actionResult ? $", {geoShape.Srid})) = 1" : $", {geoShape.Srid})) = 0");
                        o.Body = StringB.ToString();
                        StringB.Clear();
                        break;
                    case ProviderName.PostgreSql:
                        StringB.Length = 0;
                        StringB.Append($" ST_Contains({nameColumn},ST_GeomFromText(");
                        AddParameter(geoShape.GeoText);
                        StringB.Append(actionResult ? $", {geoShape.Srid}))" : $", {geoShape.Srid})) = false");
                        o.Body = StringB.ToString();
                        StringB.Clear();
                        break;
                    case ProviderName.SqLite:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
              
                
                AddListOne(o);
                return m;
            }
           
            if (m.Method.Name == "GeoST_DWithin")
            {
                var c = m.Arguments[0] as ConstantExpression;
                var nameColumn = c.Value;

                var c2 = m.Arguments[1] as ConstantExpression;
                IGeoShape geoShape = (IGeoShape)c2.Value;

                var c3 = m.Arguments[2] as ConstantExpression;
                int dissociation = (int)c3.Value;

                var c4 = m.Arguments[3] as ConstantExpression;
                bool actionResult = c4 != null && (bool)c4.Value;
                var o = new OneComposite { Operand = Evolution.Where };
                if (_providerName == ProviderName.PostgreSql)
                {
                    StringB.Length = 0;
                    StringB.Append($" ST_DWithin({nameColumn},ST_GeomFromText(");
                    AddParameter(geoShape.GeoText);
                    StringB.Append(actionResult ? $", {geoShape.Srid}),{dissociation} )" : $", {geoShape.Srid}), {dissociation}) = false");
                    o.Body = StringB.ToString();
                    StringB.Clear();
                }
                else
                {
                    throw new Exception("Only PostgreSql");
                }
                
                AddListOne(o);
                return m;
            }
           
            if (m.Method.Name == "GeoST_Intersects")
            {
                var c = m.Arguments[0] as ConstantExpression;
                var nameColumn = c.Value;

                var c2 = m.Arguments[1] as ConstantExpression;
                IGeoShape geoShape = (IGeoShape)c2.Value;

                var c3 = m.Arguments[2] as ConstantExpression;
                bool actionResult = c3 != null && (bool)c3.Value;
                var o = new OneComposite { Operand = Evolution.Where };
                StringB.Length = 0;
                switch (_providerName)
                {
                    case ProviderName.MsSql:
                        StringB.Append($" {nameColumn}.STIntersects(geometry::STGeomFromText(");
                        AddParameter(geoShape.GeoText);
                        StringB.Append(actionResult ? $", {geoShape.Srid}))=1" : $", {geoShape.Srid})) = 0");
                        o.Body = StringB.ToString();
                        StringB.Clear();
                        break;
                    case ProviderName.MySql:
                        StringB.Append($" ST_Intersects({nameColumn}, ST_GeomFromText(");
                        AddParameter(geoShape.GeoText);
                        StringB.Append(actionResult ? $", {geoShape.Srid})) = 1" : $", {geoShape.Srid})) = 0");
                        o.Body = StringB.ToString();
                        StringB.Clear();
                        break;
                    case ProviderName.PostgreSql:
                        StringB.Append($" ST_Intersects({nameColumn}::geometry,ST_GeomFromText(");
                        AddParameter(geoShape.GeoText);
                        StringB.Append(actionResult ? $", {geoShape.Srid}))" : $"{geoShape.Srid})) = false");
                        o.Body = StringB.ToString();
                        StringB.Clear();
                        break;
                    case ProviderName.SqLite:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                AddListOne(o);
                return m;
            }
           
            if (m.Method.Name == "GeoST_Disjoint")
            {
                var c = m.Arguments[0] as ConstantExpression;
                var nameColumn = c.Value;

                var c2 = m.Arguments[1] as ConstantExpression;
                IGeoShape geoShape = (IGeoShape)c2.Value;

                var c3 = m.Arguments[2] as ConstantExpression;
                bool actionResult = c3 != null && (bool)c3.Value;
                var o = new OneComposite { Operand = Evolution.Where };
                switch (_providerName)
                {
                    case ProviderName.MsSql:
                        StringB.Length = 0;
                        StringB.Append($" {nameColumn}.STDisjoint(geometry::STGeomFromText(");
                        AddParameter(geoShape.GeoText);
                        StringB.Append(actionResult ? $", {geoShape.Srid}))=1" : $", {geoShape.Srid})) = 0");
                        o.Body = StringB.ToString();
                        StringB.Clear();
                        break;
                    case ProviderName.MySql:
                        StringB.Length = 0;
                        StringB.Append($" ST_Disjoint({nameColumn},ST_GeomFromText(");
                        AddParameter(geoShape.GeoText);
                        StringB.Append(actionResult ? $", {geoShape.Srid})) = 1" : $", {geoShape.Srid})) = 0");
                        o.Body = StringB.ToString();
                        StringB.Clear();
                        break;
                    case ProviderName.PostgreSql:
                        StringB.Length = 0;
                        StringB.Append($" ST_Disjoint({nameColumn},ST_GeomFromText(");
                        AddParameter(geoShape.GeoText);
                        StringB.Append(actionResult ? $", {geoShape.Srid}))" : $", {geoShape.Srid})) = false");
                        o.Body = StringB.ToString();
                        StringB.Clear();
                        break;
                    case ProviderName.SqLite:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
               
                AddListOne(o);
                return m;
            }
           
            if (m.Method.Name == "GeoST_Crosses")
            {
                var c = m.Arguments[0] as ConstantExpression;
                var nameColumn = c.Value;

                var c2 = m.Arguments[1] as ConstantExpression;
                IGeoShape geoShape = (IGeoShape)c2.Value;

                var c3 = m.Arguments[2] as ConstantExpression;
                bool actionResult = c3 != null && (bool)c3.Value;
                var o = new OneComposite { Operand = Evolution.Where };
                switch (_providerName)
                {
                    case ProviderName.MsSql:
                        StringB.Length = 0;
                        StringB.Append($" {nameColumn}.STCrosses(geometry::STGeomFromText(");
                        AddParameter(geoShape.GeoText);
                        StringB.Append(actionResult ? $", {geoShape.Srid})) = 1" : $", {geoShape.Srid})) = 0");
                        o.Body = StringB.ToString();
                        StringB.Clear();
                        break;
                    case ProviderName.MySql:
                        StringB.Length = 0;
                        StringB.Append($" ST_Crosses({nameColumn},ST_GeomFromText(");
                        AddParameter(geoShape.GeoText);
                        StringB.Append(actionResult ? $", {geoShape.Srid})) = 1" : $", {geoShape.Srid})) = 0");
                        o.Body = StringB.ToString();
                        StringB.Clear();
                        break;
                    case ProviderName.PostgreSql:
                        StringB.Length = 0;
                        StringB.Append($" ST_Crosses({nameColumn},ST_GeomFromText(");
                        AddParameter(geoShape.GeoText);
                        StringB.Append(actionResult ? $", {geoShape.Srid}))" : $", {geoShape.Srid})) = false");
                        o.Body = StringB.ToString();
                        StringB.Clear();
                        break;
                    case ProviderName.SqLite:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
               

                AddListOne(o);
                return m;
            }
          
            if (m.Method.Name == "GeoST_Equals")
            {
                var c = m.Arguments[0] as ConstantExpression;
                var nameColumn = c.Value;

                var c2 = m.Arguments[1] as ConstantExpression;
                IGeoShape geoShape = (IGeoShape)c2.Value;

                var c3 = m.Arguments[2] as ConstantExpression;
                bool actionResult = c3 != null && (bool)c3.Value;
                var o = new OneComposite { Operand = Evolution.Where };
                switch (_providerName)
                {
                    case ProviderName.MsSql:
                        StringB.Length = 0;
                        StringB.Append($" {nameColumn}.STEquals(geometry::STGeomFromText(");
                        AddParameter(geoShape.GeoText);
                        StringB.Append(actionResult ? $", {geoShape.Srid})) = 1" : $", {geoShape.Srid})) = 0");
                        o.Body = StringB.ToString();
                        StringB.Clear();
                        break;
                    case ProviderName.MySql:
                        StringB.Length = 0;
                        StringB.Append($" ST_Equals({nameColumn},ST_GeomFromText(");
                        AddParameter(geoShape.GeoText);
                        StringB.Append(actionResult ? $", {geoShape.Srid})) = 1" : $", {geoShape.Srid})) = 0");
                        o.Body = StringB.ToString();
                        StringB.Clear();
                        break;
                    case ProviderName.PostgreSql:
                        StringB.Length = 0;
                        StringB.Append($" ST_Equals({nameColumn},ST_GeomFromText(");
                        AddParameter(geoShape.GeoText);
                        StringB.Append(actionResult ? $", {geoShape.Srid}))" : $", {geoShape.Srid})) = false");
                        o.Body = StringB.ToString();
                        StringB.Clear();
                        break;
                    case ProviderName.SqLite:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
               
                AddListOne(o);
                return m;
            }
          
            if (m.Method.Name == "GeoST_Overlaps")
            {
                var c = m.Arguments[0] as ConstantExpression;
                var nameColumn = c.Value;

                var c2 = m.Arguments[1] as ConstantExpression;
                IGeoShape geoShape = (IGeoShape)c2.Value;

                var c3 = m.Arguments[2] as ConstantExpression;
                bool actionResult = c3 != null && (bool)c3.Value;
                var o = new OneComposite { Operand = Evolution.Where };
                switch (_providerName)
                {
                    case ProviderName.MsSql:
                    {
                        StringB.Length = 0;
                        StringB.Append($" {nameColumn}.STOverlaps(geometry::STGeomFromText(");
                        AddParameter(geoShape.GeoText);
                        StringB.Append(actionResult ? $", {geoShape.Srid})) = 1" : $", {geoShape.Srid})) = 0");
                        o.Body = StringB.ToString();
                        StringB.Clear();
                            break;
                    }
                    case ProviderName.MySql:
                    {
                        StringB.Length = 0;
                        StringB.Append($" ST_Overlaps({nameColumn},ST_GeomFromText(");
                        AddParameter(geoShape.GeoText);
                        StringB.Append(actionResult ? $", {geoShape.Srid})) = 1" : $", {geoShape.Srid})) = 0");
                        o.Body = StringB.ToString();
                        StringB.Clear();
                            break;
                    }
                    case ProviderName.PostgreSql:
                    {
                        StringB.Length = 0;
                        StringB.Append($" ST_Overlaps({nameColumn},ST_GeomFromText(");
                        AddParameter(geoShape.GeoText);
                        StringB.Append(actionResult ? $", {geoShape.Srid}))" : $", {geoShape.Srid})) = false");
                        o.Body = StringB.ToString();
                        StringB.Clear();
                            break;
                    }
                    case ProviderName.SqLite:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }


                AddListOne(o);
                return m;
            }
          
            if (m.Method.Name == "GeoST_Touches")
            {
                var c = m.Arguments[0] as ConstantExpression;
                var nameColumn = c.Value;

                var c2 = m.Arguments[1] as ConstantExpression;
                IGeoShape geoShape = (IGeoShape)c2.Value;

                var c3 = m.Arguments[2] as ConstantExpression;
                bool actionResult = c3 != null && (bool)c3.Value;
                var o = new OneComposite { Operand = Evolution.Where };
                switch (_providerName)
                {
                    case ProviderName.MsSql:
                        StringB.Length = 0;
                        StringB.Append($" {nameColumn}.STTouches(geometry::STGeomFromText(");
                        AddParameter(geoShape.GeoText);
                        StringB.Append(actionResult ? $", {geoShape.Srid})) = 1" : $", {geoShape.Srid})) = 0");
                        o.Body = StringB.ToString();
                        StringB.Clear();
                        break;
                    case ProviderName.MySql:
                        StringB.Length = 0;
                        StringB.Append($" ST_Touches({nameColumn},ST_GeomFromText(");
                        AddParameter(geoShape.GeoText);
                        StringB.Append(actionResult ? $", {geoShape.Srid})) = 1" : $", {geoShape.Srid})) = 0");
                        o.Body = StringB.ToString();
                        StringB.Clear();
                        break;
                    case ProviderName.PostgreSql:
                        StringB.Length = 0;
                        StringB.Append($" ST_Touches({nameColumn},ST_GeomFromText(");
                        AddParameter(geoShape.GeoText);
                        StringB.Append(actionResult ? $", {geoShape.Srid}))" : $", {geoShape.Srid})) = false");
                        o.Body = StringB.ToString();
                        StringB.Clear();
                        break;
                    case ProviderName.SqLite:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
             

                AddListOne(o);
                return m;
            }
          
            if (m.Method.Name == "GeoST_IsValid")
            {
                var c = m.Arguments[0] as ConstantExpression;
                var nameColumn = c.Value;
                var c1 = m.Arguments[1] as ConstantExpression;
                bool actionResult = (bool)c1.Value;

                var o = new OneComposite { Operand = Evolution.Where };
                if (_providerName == ProviderName.MsSql)
                {
                    StringB.Append($"{nameColumn}.STIsValid() = ");
                    AddParameter(actionResult);
                    o.Body = StringB.ToString();
                    AddListOne(o);
                }
                else
                {
                    StringB.Append($"ST_IsValid({nameColumn}) = ");
                    AddParameter(actionResult);
                    o.Body = StringB.ToString();
                    AddListOne(o);
                }
               
                return m;

            }
          
            if (m.Method.Name == "GeoST_Within")
            {
                var c = m.Arguments[0] as ConstantExpression;
                var nameColumn = c.Value;

                var c2 = m.Arguments[1] as ConstantExpression;
                IGeoShape geoShape = (IGeoShape)c2.Value;

                var c3 = m.Arguments[2] as ConstantExpression;
                bool actionResult = c3 != null && (bool)c3.Value;
                var o = new OneComposite { Operand = Evolution.Where };
                switch (_providerName)
                {
                    case ProviderName.MsSql:
                        StringB.Length = 0;
                        StringB.Append($" {nameColumn}.STWithin(geometry::STGeomFromText(");
                        AddParameter(geoShape.GeoText);
                        StringB.Append(actionResult ? $", {geoShape.Srid})) = 1" : $", {geoShape.Srid})) = 0");
                        o.Body = StringB.ToString();
                        StringB.Clear();
                        break;
                    case ProviderName.MySql:
                        StringB.Length = 0;
                        StringB.Append($" ST_Within({nameColumn},ST_GeomFromText(");
                        AddParameter(geoShape.GeoText);
                        StringB.Append(actionResult ? $", {geoShape.Srid})) = 1" : $", {geoShape.Srid})) = 0");
                        o.Body = StringB.ToString();
                        StringB.Clear();
                        break;
                    case ProviderName.PostgreSql:
                        StringB.Length = 0;
                        StringB.Append($" ST_Within({nameColumn},ST_GeomFromText(");
                        AddParameter(geoShape.GeoText);
                        StringB.Append(actionResult ? $", {geoShape.Srid}))" : $", {geoShape.Srid})) = false");
                        o.Body = StringB.ToString();
                        StringB.Clear();
                        break;
                    case ProviderName.SqLite:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                AddListOne(o);
                return m;
            }
          
            if (m.Method.Name == "GeoST_Area")
            {
                return m;
            }
          
            if (m.Method.Name == "GetData")
            {
                
                return m;

            }
            
            if (m.Method.DeclaringType == typeof(Queryable))
            {
                switch (m.Method.Name)
                {
                    case "Join":
                    case "GroupJoin":
                    case "Concat":
                    //case "Cast":
                    //case "Select":
                    case "SelectMany":
                    case "Aggregate":
                    case "GroupBy":
                    case "Except":
                        {
                            throw new Exception($"Method {m.Method.Name} for IQueryable is not implemented, use method {m.Method.Name}Core or ...toList().{m.Method.Name}()");

                        }


                }

            }

            if (m.Method.Name == "Union")
            {
                Visit(m.Arguments[0]);
                return m;
            }


            if (m.Method.DeclaringType == typeof(Queryable)
                && (m.Method.Name == "OrderBy" || m.Method.Name == "ThenBy"))
            {
                Visit(m.Arguments[0]);
                var lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);
                Visit(lambda.Body);
                var o = new OneComposite { Operand = Evolution.OrderBy, Body = StringB.ToString().Trim(' ', ',') };
                var tSelect = ListOne.Where(a => a.Operand == Evolution.OrderBy).Select(d => d.Body);
                if (tSelect.Contains(o.Body) == false) AddListOne(o);

                StringB.Length = 0;
                return m;
            }

            if (m.Method.DeclaringType == typeof(Queryable)
                && m.Method.Name == "Reverse")
            {
                Visit(m.Arguments[0]);
                var sb = new StringBuilder();
                if (ListOne.Any(a => a.Operand == Evolution.OrderBy))
                    ListOne.Where(a => a.Operand == Evolution.OrderBy).ToList().ForEach(a =>

                    {
                        sb.AppendFormat(" {0},", a.Body);
                        ListOne.Remove(a);
                    });
                else
                    sb.AppendFormat("{0}.{1}", AttributesOfClass<T>.TableName(_providerName),
                        AttributesOfClass<T>.PkAttribute(_providerName).GetColumnName(_providerName));


                var o = new OneComposite
                {
                    Operand = Evolution.Reverse,
                    Body = string.Format("ORDER BY {0} DESC ", sb.ToString().TrimEnd(','))
                };
                AddListOne(o);
                StringB.Length = 0;
                return m;
            }

            if ((m.Method.DeclaringType == typeof(Queryable)
                 && m.Method.Name == "OrderByDescending") || m.Method.Name == "ThenByDescending")
            {
                Visit(m.Arguments[0]);
                var lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);
                Visit(lambda.Body);
                var o = new OneComposite
                { Operand = Evolution.OrderBy, Body = StringB.ToString().Trim(' ', ',') + " DESC " };

                AddListOne(o);
                StringB.Length = 0;
                return m;
            }


            if (m.Method.DeclaringType == typeof(Queryable)
                && m.Method.Name == "Select")
            {
                _currentMethod = "Select";
                Visit(m.Arguments[0]);
                var lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);
                AddPostExpression(Evolution.Select, m);
                

                Visit(lambda.Body);
               
                var o = new OneComposite { Operand = Evolution.Select, 
                    Body = StringB.ToString().Trim(' ', ',') };
                if (!string.IsNullOrEmpty(StringB.ToString())) AddListOne(o);
                StringB.Length = 0;
                _currentMethod = null;
                return m;
            }

            if ((m.Method.DeclaringType == typeof(Queryable) && m.Method.Name == "First") ||
                (m.Method.DeclaringType == typeof(Helper) && m.Method.Name == "FirstAsync"))
            {
                Visit(m.Arguments[0]);
                if (m.Arguments.Count == 2)
                {
                    var lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);
                    Visit(lambda.Body);

                    var o1 = new OneComposite { Operand = Evolution.Where, Body = StringB.ToString() };
                    AddListOne(o1);
                    StringB.Length = 0;
                    var o2 = new OneComposite { Operand = Evolution.First };
                    AddListOne(o2);
                    StringB.Length = 0;
                    return m;
                }

                var o = new OneComposite { Operand = Evolution.First, Body = StringB.ToString() };
                AddListOne(o);
                StringB.Length = 0;

                return m;
            }

            if ((m.Method.DeclaringType == typeof(Queryable) && m.Method.Name == "FirstOrDefault") ||
                (m.Method.DeclaringType == typeof(Helper) && m.Method.Name == "FirstOrDefaultAsync"))
            {
                Visit(m.Arguments[0]);
                if (m.Arguments.Count == 2)
                {
                    var lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);
                    Visit(lambda.Body);

                    var o1 = new OneComposite { Operand = Evolution.Where, Body = StringB.ToString() };
                    AddListOne(o1);
                    StringB.Length = 0;
                    var o2 = new OneComposite { Operand = Evolution.FirstOrDefault };
                    AddListOne(o2);
                    StringB.Length = 0;
                    return m;
                }

                var o = new OneComposite { Operand = Evolution.FirstOrDefault, Body = StringB.ToString() };
                AddListOne(o);
                StringB.Length = 0;

                return m;
            }

            if (m.Method.DeclaringType == typeof(Queryable)
                && m.Method.Name == "All")
            {
                Visit(m.Arguments[0]);
                var lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);
                Visit(lambda.Body);
                var o1 = new OneComposite { Operand = Evolution.All, Body = StringB.ToString() };
                AddListOne(o1);
                return m;
            }


            if (m.Method.DeclaringType == typeof(Queryable)
                && m.Method.Name == "Any")
            {
                Visit(m.Arguments[0]);
                if (m.Arguments.Count == 2)
                {
                    var lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);
                    Visit(lambda.Body);
                    var o1 = new OneComposite { Operand = Evolution.Where, Body = StringB.ToString() };
                    AddListOne(o1);
                    StringB.Length = 0;
                    var o2 = new OneComposite { Operand = Evolution.Any };
                    AddListOne(o2);
                    StringB.Length = 0;
                    return m;
                }

                var o = new OneComposite { Operand = Evolution.Where, Body = StringB.ToString() };
                AddListOne(o);
                AddListOne(new OneComposite { Operand = Evolution.Any });
                StringB.Length = 0;

                return m;
            }

            if ((m.Method.DeclaringType == typeof(Queryable)
                 && m.Method.Name == "Last") || m.Method.Name == "LastOrDefault")
            {
                if (_providerName == ProviderName.MsSql)
                {
                    var pizda = Evolution.Last;
                    if (m.Method.Name == "LastOrDefault")
                        pizda = Evolution.LastOrDefault;
                    Visit(m.Arguments[0]);

                    if (m.Arguments.Count == 2)
                    {
                        var lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);
                        Visit(lambda.Body);
                        var o1 = new OneComposite
                        {
                            Operand = Evolution.Where,
                            Body = StringB.ToString()
                        };
                        AddListOne(o1);
                        StringB.Length = 0;
                    }

                    if (ListOne.Any(a => a.Operand == Evolution.OrderBy))
                    {
                        foreach (var body in ListOne.Where(a => a.Operand == Evolution.OrderBy))
                            if (body.Body.IndexOf("DESC", StringComparison.Ordinal) != -1)
                                body.Body = body.Body.Replace("DESC", string.Empty);
                            else
                                body.Body = body.Body + " DESC";
                    }
                    else
                    {
                        var o = new OneComposite
                        {
                            Operand = Evolution.OrderBy,
                            Body = $" {AttributesOfClass<T>.TableName(_providerName)}." +
                                   $"{AttributesOfClass<T>.PkAttribute(_providerName).GetColumnName(_providerName)} DESC "
                        };
                        AddListOne(o);
                    }

                    var o13 = new OneComposite
                    {
                        IsAggregate = true,
                        Operand = pizda,
                        Body = "1"
                    };
                    AddListOne(o13);

                    return m;
                }

                var ee = (Evolution)Enum.Parse(typeof(Evolution), m.Method.Name);
                Visit(m.Arguments[0]);

                if (m.Arguments.Count == 2)
                {
                    var lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);
                    Visit(lambda.Body);
                    var o1 = new OneComposite
                    {
                        Operand = Evolution.Where,
                        Body = StringB.ToString()
                    };
                    AddListOne(o1);
                    StringB.Length = 0;
                }

                if (ListOne.Any(a => a.Operand == Evolution.OrderBy))
                {
                    foreach (var body in ListOne.Where(a => a.Operand == Evolution.OrderBy))
                        if (body.Body.IndexOf("DESC", StringComparison.Ordinal) != -1)
                            body.Body = body.Body.Replace("DESC", string.Empty);
                        else
                            body.Body += " DESC";
                    if (PingComposite(Evolution.Limit) == false)
                        ListOne.Last(a => a.Operand == Evolution.OrderBy).Body += " LIMIT 1";
                }
                else
                {
                    var o = new OneComposite
                    {
                        Operand = Evolution.OrderBy,
                        Body = string.Format(" {0}.{1} DESC LIMIT 1", AttributesOfClass<T>.TableName(_providerName),
                            AttributesOfClass<T>.PkAttribute(_providerName).GetColumnName(_providerName))
                    };
                    AddListOne(o);
                }

                var os = new OneComposite
                {
                    Operand = ee
                };
                AddListOne(os);

                return m;
            }

            if (m.Method.Name == "Skip")
            {
                Visit(m.Arguments[0]);
                if (m.Arguments.Count == 2)
                {
                    var lambda = (ConstantExpression)StripQuotes(m.Arguments[1]);

                    var o1 = new OneComposite { Operand = Evolution.Skip, Body = lambda.Value.ToString() };
                    AddListOne(o1);
                    StringB.Length = 0;
                    return m;
                }

                var o = new OneComposite { Operand = Evolution.Count, Body = StringB.ToString() };
                AddListOne(o);
                StringB.Length = 0;
                return m;
            }


            if (m.Method.DeclaringType == typeof(Queryable)
                && m.Method.Name == "Count")
            {
                Visit(m.Arguments[0]);
                if (m.Arguments.Count == 2)
                {
                    var lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);
                    Visit(lambda.Body);
                    var o1 = new OneComposite { Operand = Evolution.Where, Body = StringB.ToString() };
                    AddListOne(o1);
                    var o2 = new OneComposite { Operand = Evolution.Count };
                    AddListOne(o2);
                    StringB.Length = 0;
                    return m;
                }

                var o = new OneComposite { Operand = Evolution.Count, Body = StringB.ToString() };
                AddListOne(o);
                StringB.Length = 0;
                return m;
            }

            if (m.Method.DeclaringType == typeof(Queryable)
                && m.Method.Name == "LongCount")
            {
                Visit(m.Arguments[0]);
                if (m.Arguments.Count == 2)
                {
                    var lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);
                    Visit(lambda.Body);
                    var o1 = new OneComposite { Operand = Evolution.Where, Body = StringB.ToString() };
                    AddListOne(o1);
                    var o2 = new OneComposite { Operand = Evolution.LongCount };
                    AddListOne(o2);
                    o2 = new OneComposite { Operand = Evolution.Count };
                    AddListOne(o2);
                    StringB.Length = 0;
                    return m;
                }

                var o = new OneComposite { Operand = Evolution.LongCount, Body = StringB.ToString() };
                AddListOne(o);
                o = new OneComposite { Operand = Evolution.Count, Body = StringB.ToString() };
                AddListOne(o);
                StringB.Length = 0;
                return m;
            }


            if (m.Method.DeclaringType == typeof(Queryable)
                && m.Method.Name == "Single")
            {
                AddListOne(new OneComposite { Operand = Evolution.Single, Body = "", IsAggregate = true });
                Visit(m.Arguments[0]);
                if (m.Arguments.Count == 2)
                {
                    var o = new OneComposite { Operand = Evolution.Where };
                    Visit(m.Arguments[0]);
                    var lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);
                    Visit(lambda.Body);
                    o.Body = StringB.ToString();
                    AddListOne(o);
                    StringB.Length = 0;
                }

                AddListOne(new OneComposite { Operand = Evolution.First, Body = "" });
                return m;
            }

            if (m.Method.DeclaringType == typeof(Queryable)
                && m.Method.Name == "SingleOrDefault")
            {
                AddListOne(new OneComposite { Operand = Evolution.SingleOrDefault, Body = "", IsAggregate = true });
                AddListOne(new OneComposite { Operand = Evolution.First, Body = "" });
                Visit(m.Arguments[0]);

                if (m.Arguments.Count == 2)
                {
                    var o = new OneComposite { Operand = Evolution.Where };
                    var lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);
                    Visit(lambda.Body);
                    o.Body = StringB.ToString();
                    AddListOne(o);
                    StringB.Length = 0;
                }

                return m;
            }

            if (m.Method.MemberType == MemberTypes.Method)
            {
                var mcs = m;
                if (mcs.Method.DeclaringType == typeof(Queryable)
                    || mcs.Method.DeclaringType == typeof(Enumerable))
                {
                    switch (mcs.Method.Name)
                    {
                        //case "Count":
                        //case "LongCount":
                        case "Sum":
                        case "Min":
                        case "Max":
                        case "Average":
                            {
                                // var e = mcs.Arguments;
                                Visit(mcs.Arguments[0]);
                                var lambda = (LambdaExpression)StripQuotes(mcs.Arguments[1]);
                                StringB.Length = 0;
                                StringB.Append(mcs.Method.Name + "(");
                                Visit(lambda.Body);
                                StringB.Append(")");
                                AddListOne(new OneComposite
                                { Operand = Evolution.Select, Body = StringB.ToString(), IsAggregate = true });
                                break;
                            }
                    }

                    return m;
                }

                if (m.Method.MemberType == MemberTypes.Field)
                {
                    var df = m.Arguments.Select(a => a.GetType().GetProperty("Value").GetValue(a, null));
                    var values = m.Method.Invoke(m, df.ToArray());
                    AddParameter(values);
                    return m;
                }

                if (m.Method.MemberType == MemberTypes.Property)
                {
                    var df = m.Arguments.Select(a => a.GetType().GetProperty("Value").GetValue(a, null));
                    var value = m.Method.Invoke(m, df.ToArray());
                    AddParameter(value);
                    return m;
                }

                if (m.Method.MemberType == MemberTypes.Method && m.Object != null)
                {
                    Visit(m.Object);
                    return m;
                }

                if (m.Method.Name == "Query") return m;

                if (m.Method.Name == "get_Item")
                {
                    if (m.Object != null)
                    {
                        var value = Expression.Lambda<Func<object>>(m.Object).Compile()();
                        var dddd = m.Arguments[0].GetType().GetProperty("Value").GetValue(m.Arguments[0], null);
                        var tt = m.Method.Invoke(value, new[] { dddd });
                        AddParameter(tt);
                    }

                    return m;
                }

                if (m.Method.Name == "WhereOn")
                {
                    return m;
                }

               


                var la = new List<object>();
                foreach (var i in m.Arguments)
                    if (i.GetType().GetProperty("Value") ==
                        null) //System.Linq.Expressions.InstanceMethodCallExpressionN
                    {
                        var value = Expression.Lambda<Func<object>>(i).Compile()();
                        la.Add(value);
                    }
                    else
                    {
                        la.Add(i.GetType().GetProperty("Value").GetValue(i, null));
                    }

                var value3 = m.Method.Invoke(m, la.ToArray());
                AddParameter(value3);
                return m;
            }

            throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture,
                "The method '{0}' is not supported", m.Method.Name));
        }

        private static LambdaExpression GetLambda(Expression e)
        {
            while (e.NodeType == ExpressionType.Quote) e = ((UnaryExpression)e).Operand;
            if (e.NodeType == ExpressionType.Constant) return ((ConstantExpression)e).Value as LambdaExpression;
            return e as LambdaExpression;
        }

        protected override Expression VisitUnary(UnaryExpression u)
        {
            switch (u.NodeType)
            {
                case ExpressionType.Not:
                    StringB.Append(" NOT ");
                    Visit(u.Operand);
                    break;

                case ExpressionType.Convert:
                    Visit(u.Operand);
                    break;

                case ExpressionType.Quote:
                    Visit(u.Operand);
                    break;

                default:
                    throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture,
                        "The unary operator '{0}' is not supported", u.NodeType));
            }

            return u;
        }

        protected override Expression VisitBinary(BinaryExpression b)
        {
            StringB.Append("(");
            Visit(b.Left);
            switch (b.NodeType)
            {
                case ExpressionType.AndAlso:

                case ExpressionType.And:
                    StringB.Append(" and ");
                    break;
                case ExpressionType.Or:
                    StringB.Append(" or ");
                    break;
                case ExpressionType.Add:
                    StringB.Append("+");
                    break;

                case ExpressionType.OrElse:
                    StringB.Append(" or ");
                    break;
                case ExpressionType.Equal:
                    if (b.Right.ToString() == "null")
                    {
                        StringB.Append(" is ");
                        break;
                    }

                    if (b.Right is UnaryExpression expression && expression.Operand.ToString() == "null")
                    {
                        StringB.Append(" is ");
                        break;
                    }
                    
                    StringB.Append(" = ");
                    break;
                case ExpressionType.NotEqual:

                    if (b.Right.ToString() == "null")
                    {
                        StringB.Append(" is not  ");
                        break;
                    }

                    if (b.Right is UnaryExpression unaryExpression && unaryExpression.Operand.ToString() == "null")
                    {
                        StringB.Append(" is not  ");
                        break;
                    }

                    StringB.Append(" <> ");
                    break;
                case ExpressionType.LessThan:
                    StringB.Append(" < ");
                    break;
                case ExpressionType.LessThanOrEqual:
                    StringB.Append(" <= ");
                    break;
                case ExpressionType.GreaterThan:
                    StringB.Append(" > ");
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    StringB.Append(" >= ");
                    break;
                case ExpressionType.Divide:
                    StringB.Append(" / ");
                    break;
                case ExpressionType.Modulo:
                    StringB.Append("%");
                    break;
                case ExpressionType.ExclusiveOr:
                    StringB.Append("^");
                    break;
                case ExpressionType.LeftShift:
                    StringB.Append("<<");
                    break;
                case ExpressionType.RightShift:
                    StringB.Append(">>");
                    break;
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    StringB.Append("-");
                    break;
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                    StringB.Append("*");
                    break;
                default:
                    throw new NotSupportedException(
                        string.Format(CultureInfo.CurrentCulture,
                            "The binary operator '{0}' is not supported", b.NodeType));
            }

            Visit(b.Right);
            StringB.Append(")");
            return b;
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            var q = c.Value as IQueryable;

            if (q != null) return c;
            if (c.Value == null)
                StringB.Append("null");
            else
                switch (Type.GetTypeCode(c.Value.GetType()))
                {
                    case TypeCode.Boolean:
                        if (_providerName == ProviderName.PostgreSql)
                            StringB.Append((bool)c.Value);
                        else
                            StringB.Append((bool)c.Value ? 1 : 0);
                        break;
                    case TypeCode.Decimal:
                        {
                            StringB.Append((decimal)c.Value);
                            break;
                        }
                    case TypeCode.Int64:
                        {
                            StringB.Append((long)c.Value);
                            break;
                        }
                    case TypeCode.Int32:
                        {
                            StringB.Append((int)c.Value);
                            break;
                        }
                    case TypeCode.Int16:
                        {
                            StringB.Append((short)c.Value);
                            break;
                        }
                    case TypeCode.UInt16:
                        {
                            StringB.Append((ushort)c.Value);
                            break;
                        }
                    case TypeCode.UInt32:
                        {
                            StringB.Append((uint)c.Value);
                            break;
                        }
                    case TypeCode.UInt64:
                        {
                            StringB.Append((ulong)c.Value);
                            break;
                        }
                    case TypeCode.Single:
                        {
                            StringB.Append((float)c.Value);
                            break;
                        }
                    case TypeCode.Double:
                        {
                            StringB.Append((double)c.Value);
                            break;
                        }
                    case TypeCode.String:

                        var p = ParamName;
                        //StringB.Append(p);
                        AddParameter(c.Value);
                        //Param.Add(p, c.Value);
                        break;
                    case TypeCode.Object:
                        {
                            if (c.Value is T && PingComposite(Evolution.Contains))
                            {
                                var o = (T)c.Value;
                                var propertyname = AttributesOfClass<T>.PkAttribute(_providerName).PropertyName;
                                var value = AttributesOfClass<T>.GetValueE(_providerName, propertyname, o);
                                var tablenane = AttributesOfClass<T>.TableName(_providerName);
                                var key = AttributesOfClass<T>.PkAttribute(_providerName).GetColumnName(_providerName);
                                StringB.Append(string.Format("({0}.{1} = '{2}')", tablenane, key, value));
                                break;
                            }

                            throw new NotSupportedException(
                                string.Format(CultureInfo.CurrentCulture,
                                    "The constant for '{0}' is not supported", c.Value));
                        }
                    default:
                        AddParameter(c.Value);
                        break;
                }

            return c;
        }

        private string _geoAsName = null;
        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            if (m.Expression != null
                && m.Expression.NodeType == ExpressionType.Parameter)
            {
                if (m.Expression.Type != typeof(T))
                {
                    if (UtilsCore.IsAnonymousType(m.Expression.Type)) return m;
                }
                else
                {
                    string nameColumn = GetColumnName(m.Member.Name, m.Expression.Type);
                    StringB.Append(nameColumn);
                    _geoAsName = GetColumnNameSimple(m.Member.Name, m.Expression.Type);
                    if (UtilsCore.IsGeo(m.Type))
                    {
                        var srid = GetSrid(m.Member.Name);
                        AddListOne(new OneComposite{Operand = Evolution.ListGeo,Body = nameColumn,Srid = srid});
                    }

                    if (UtilsCore.IsJson(m.Type))
                    {
                        AddListOne(new OneComposite { Operand = Evolution.ListJson, Body = nameColumn });
                    }
                }

                return m;
            }

            if (m.Expression != null
                && m.Expression.NodeType == ExpressionType.New)
            {
                StringB.Append(GetColumnName(m.Member.Name, m.Expression.Type));
                return m;
            }

            if (m.Expression != null && m.Expression.NodeType == ExpressionType.Constant)
            {
                VisitorMemberAccess(m);
                return m;
            }

            if (m.Expression != null && m.Expression.NodeType == ExpressionType.MemberAccess)
            {
                VisitorMemberAccess(m);
                return m;
            }


            if (m.Expression != null && m.NodeType == ExpressionType.MemberAccess)
            {
                VisitorMemberAccess(m);
                return m;
            }

            if (m.Expression == null && m.NodeType == ExpressionType.MemberAccess)
            {
                if (m.Type == typeof(int))
                {
                    var value = Expression.Lambda<Func<int>>(m).Compile()();
                    AddParameter(value);
                    return m;
                }

                if (m.Type == typeof(long))
                {
                    var value = Expression.Lambda<Func<long>>(m).Compile()();
                    AddParameter(value);
                    return m;
                }

                if (m.Member.ReflectedType == typeof(DateTime))
                {
                    var value = Expression.Lambda<Func<DateTime>>(m).Compile()();
                    AddParameter(value);
                    return m;
                }

                if (m.Type == typeof(Guid))
                {
                    var value = Expression.Lambda<Func<Guid>>(m).Compile()();
                    AddParameter(value);
                    return m;
                }

                var str = Expression.Lambda<Func<object>>(m).Compile().Invoke();
                AddParameter(str);
                return m;
            }

            throw new NotSupportedException(
                string.Format(CultureInfo.CurrentCulture, "The member '{0}' is not supported", m.Member.Name));
        }

        private void VisitorMemberAccess(MemberExpression m)
        {
            object value;
            if (m.Member.DeclaringType == typeof(string))
                switch (m.Member.Name)
                {
                    case "Length":

                        switch (_providerName)
                        {
                            case ProviderName.PostgreSql:
                                {
                                    StringB.Append("CHAR_LENGTH(");
                                    break;
                                }
                            case ProviderName.SqLite:
                                {
                                    StringB.Append("LENGTH(");
                                    break;
                                }
                            case ProviderName.MySql:
                                {
                                    StringB.Append("CHAR_LENGTH(");
                                    break;
                                }

                            case ProviderName.MsSql:
                                {
                                    StringB.Append("LEN(");
                                    break;
                                }
                        }

                        Visit(m.Expression);
                        StringB.Append(")");
                        return;
                }
            //if(m.Member.==)

            if (m.Member.MemberType == MemberTypes.Field)
            {
                // var st = UtilsCore.GetSerializeType(((FieldInfo)m.Member).FieldType);
                //
                // if (st == SerializeType.User)
                // {
                //     var o = Expression.Lambda<Func<object>>(m).Compile()();
                //     var v = ((IMapSerializable)o).Serialize();
                //     AddParameter(v);
                //     return;
                // }
            }

            if (_providerName == ProviderName.PostgreSql)
            {
                if (m.Member.DeclaringType == typeof(GeoObject))
                {
                    switch (m.Member.Name)
                    {
                        case "GeoData":
                            {
                                StringB.Append("ST_AsText(");
                                Visit(m.Expression);
                                StringB.Append(")");
                                break;
                            }
                    }
                    return;
                }
            }


            if (m.Member.ReflectedType == typeof(DateTime))
            {
                if (m.Member.DeclaringType == typeof(DateTimeOffset))
                    throw new Exception("m.Member.DeclaringType == typeof(DateTimeOffset)");

                switch (m.Member.Name)
                {
                    case "Day":
                        switch (_providerName)
                        {
                            case ProviderName.PostgreSql:
                                {
                                    StringB.Append("extract( day from ");
                                    Visit(m.Expression);
                                    StringB.Append(")");
                                    break;
                                }
                            case ProviderName.SqLite:
                                {
                                    StringB.Append("CAST(strftime('%d', ");
                                    Visit(m.Expression);
                                    StringB.Append(") as INTEGER)");
                                    break;
                                }
                            case ProviderName.MySql:
                                {
                                    StringB.Append("DAY(");
                                    Visit(m.Expression);
                                    StringB.Append(")");
                                    break;
                                }

                            case ProviderName.MsSql:
                                {
                                    StringB.Append("DATEPART(DAY,");
                                    Visit(m.Expression);
                                    StringB.Append(")");
                                    break;
                                }
                        }

                        return;
                    case "Month":
                        switch (_providerName)
                        {
                            case ProviderName.PostgreSql:
                                {
                                    StringB.Append("extract(MONTH from ");
                                    Visit(m.Expression);
                                    StringB.Append(")");
                                    break;
                                }
                            case ProviderName.SqLite:
                                {
                                    StringB.Append("CAST(strftime('%m', ");
                                    Visit(m.Expression);
                                    StringB.Append(") as INTEGER)");
                                    break;
                                }
                            case ProviderName.MySql:
                                {
                                    StringB.Append("MONTH(");
                                    Visit(m.Expression);
                                    StringB.Append(")");
                                    break;
                                }

                            case ProviderName.MsSql:
                                {
                                    StringB.Append("DATEPART(MONTH, ");
                                    Visit(m.Expression);
                                    StringB.Append(")");
                                    break;
                                }
                        }

                        return;
                    case "Year":
                        switch (_providerName)
                        {
                            case ProviderName.PostgreSql:
                                {
                                    StringB.Append("extract(YEAR from ");
                                    Visit(m.Expression);
                                    StringB.Append(")");
                                    break;
                                }
                            case ProviderName.SqLite:
                                {
                                    StringB.Append("CAST(strftime('%Y', ");
                                    Visit(m.Expression);
                                    StringB.Append(") as INTEGER)");
                                    break;
                                }
                            case ProviderName.MySql:
                                {
                                    StringB.Append("YEAR(");
                                    Visit(m.Expression);
                                    StringB.Append(")");
                                    break;
                                }

                            case ProviderName.MsSql:
                                {
                                    StringB.Append("DATEPART(YEAR,");
                                    Visit(m.Expression);
                                    StringB.Append(")");
                                    break;
                                }
                        }

                        return;
                    case "Hour":
                        switch (_providerName)
                        {
                            case ProviderName.PostgreSql:
                                {
                                    StringB.Append("extract(HOUR from ");
                                    Visit(m.Expression);
                                    StringB.Append(")");
                                    break;
                                }
                            case ProviderName.SqLite:
                                {
                                    StringB.Append("CAST(strftime('%H', ");
                                    Visit(m.Expression);
                                    StringB.Append(") as INTEGER)");
                                    break;
                                }
                            case ProviderName.MySql:
                                {
                                    StringB.Append("HOUR(");
                                    Visit(m.Expression);
                                    StringB.Append(")");
                                    break;
                                }

                            case ProviderName.MsSql:
                                {
                                    StringB.Append("DATEPART(HOUR,");
                                    Visit(m.Expression);
                                    StringB.Append(")");
                                    break;
                                }
                        }

                        return;
                    case "Minute":
                        switch (_providerName)
                        {
                            case ProviderName.PostgreSql:
                                {
                                    StringB.Append("extract(MINUTE from ");
                                    Visit(m.Expression);
                                    StringB.Append(")");
                                    break;
                                }
                            case ProviderName.SqLite:
                                {
                                    StringB.Append("CAST(strftime('%M', ");
                                    Visit(m.Expression);
                                    StringB.Append(") as INTEGER)");
                                    break;
                                }
                            case ProviderName.MySql:
                                {
                                    StringB.Append("MINUTE(");
                                    Visit(m.Expression);
                                    StringB.Append(")");
                                    break;
                                }

                            case ProviderName.MsSql:
                                {
                                    StringB.Append("DATEPART(MINUTE,");
                                    Visit(m.Expression);
                                    StringB.Append(")");
                                    break;
                                }
                        }

                        return;
                    case "Second":
                        switch (_providerName)
                        {
                            case ProviderName.PostgreSql:
                                {
                                    StringB.Append("extract(SECOND from ");
                                    Visit(m.Expression);
                                    StringB.Append(")");
                                    break;
                                }
                            case ProviderName.SqLite:
                                {
                                    StringB.Append("CAST(strftime('%S', ");
                                    Visit(m.Expression);
                                    StringB.Append(") as INTEGER)");
                                    break;
                                }
                            case ProviderName.MySql:
                                {
                                    StringB.Append("SECOND(");
                                    Visit(m.Expression);
                                    StringB.Append(")");
                                    break;
                                }

                            case ProviderName.MsSql:
                                {
                                    StringB.Append("DATEPART(SECOND,");
                                    Visit(m.Expression);
                                    StringB.Append(")");
                                    break;
                                }
                        }

                        return;
                    case "Millisecond":
                        switch (_providerName)
                        {
                            case ProviderName.PostgreSql:
                                {
                                    throw new Exception("not implemented");
                                }
                            case ProviderName.SqLite:
                                {
                                    throw new Exception("not implemented");
                                }
                            case ProviderName.MySql:
                                {
                                    throw new Exception("not implemented");
                                }

                            case ProviderName.MsSql:
                                {
                                    throw new Exception("not implemented");
                                }
                        }

                        break;


                    case "DayOfWeek":
                        switch (_providerName)
                        {
                            case ProviderName.PostgreSql:
                                {
                                    StringB.Append("extract( isodow from ");
                                    Visit(m.Expression);
                                    StringB.Append(")");
                                    break;
                                }
                            case ProviderName.SqLite:
                                {
                                    StringB.Append("CAST(strftime('%w', ");
                                    Visit(m.Expression);
                                    StringB.Append(") as INTEGER)");
                                    break;
                                }
                            case ProviderName.MySql:
                                {
                                    StringB.Append("(DAYOFWEEK(");
                                    Visit(m.Expression);
                                    StringB.Append(") - 1)");
                                    break;
                                }

                            case ProviderName.MsSql:
                                {
                                    StringB.Append("DATEPART(WEEKDAY,");
                                    Visit(m.Expression);
                                    StringB.Append(")");
                                    break;
                                }
                        }

                        return;
                    case "DayOfYear":
                        switch (_providerName)
                        {
                            case ProviderName.PostgreSql:
                                {
                                    StringB.Append("extract(doy from ");
                                    Visit(m.Expression);
                                    StringB.Append(")");
                                    break;
                                }
                            case ProviderName.SqLite:
                                {
                                    StringB.Append("CAST(strftime('%j', ");
                                    Visit(m.Expression);
                                    StringB.Append(") as INTEGER)");
                                    break;
                                }
                            case ProviderName.MySql:
                                {
                                    StringB.Append("(DAYOFYEAR(");
                                    Visit(m.Expression);
                                    StringB.Append(") - 1)");
                                    break;
                                }

                            case ProviderName.MsSql:
                                {
                                    StringB.Append("DATEPART(DAYOFYEAR,");
                                    Visit(m.Expression);
                                    StringB.Append(")");
                                    break;
                                }
                        }

                        return;
                }

                var str1 = Expression.Lambda<Func<DateTime>>(m.Expression).Compile()();
                var ss = str1.GetType().GetProperty(m.Member.Name);
                value = ss.GetValue(str1, null);
                AddParameter(value);
                return;
            }

            var strS = new JoinAlias().GetAlias(m.Expression);
            if (strS != null && strS.IndexOf("TransparentIdentifier", StringComparison.Ordinal) != -1)
            {
                StringB.Append(GetColumnName(m.Member.Name, m.Expression.Type));
                return;
            }

            var str = Expression.Lambda<Func<object>>(m.Expression).Compile()();
            value = null;
            if (m.Member.MemberType == MemberTypes.Field)
            {
                var fieldInfo = str.GetType().GetField(m.Member.Name,
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
                if (fieldInfo != null)
                {

                    value = fieldInfo.GetValue(str);
                    //    var ty = fieldInfo.FieldType;
                    // var st = UtilsCore.GetSerializeType(ty);
                    // switch (st)
                    // {
                    //     case SerializeType.None:
                    //         
                    //         break;
                    //
                    //     case SerializeType.User:
                    //         value = ((IMapSerializable)str).Serialize();
                    //         break;
                    //     default:
                    //         throw new ArgumentOutOfRangeException();
                    // }
                }
            }

            if (m.Member.MemberType == MemberTypes.Property)
            {
                var ass = str.GetType().GetProperty(m.Member.Name,
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
                value = ass.GetValue(str, null);
            }
            

            AddParameter(value);
        }

        private void AddParameter(object value)
        {
            if (_providerName == ProviderName.MsSql)
            {
                if (UtilsCore.IsGeo(value.GetType()))
                {
                    var p1 = ParamName;
                    StringB.Append(p1);
                    Param.Add(p1, ((IGeoShape)value).GeoText);

                } else if (UtilsCore.IsJson(value.GetType()))
                {
                    var p1 = ParamName;
                    StringB.Append(p1);
                    Param.Add(p1, JsonConvert.SerializeObject(value));
                }
                else if (PingComposite(Evolution.ElementAt) || PingComposite(Evolution.ElementAtOrDefault))
                {
                    StringB.Append(uint.Parse(value.ToString()));
                }
                else
                {
                    var p1 = ParamName;
                    StringB.Append(p1);
                    Param.Add(p1, value);
                }
            }
            else if(UtilsCore.IsJson(value.GetType())) //todo geo
            {
                var p1 = ParamName;
                
                StringB.Append(p1);
                Param.Add(p1, JsonConvert.SerializeObject(value));
            }
            
            else if (UtilsCore.IsGeo(value.GetType())) //todo geo
            {
                var p1 = ParamName;

                StringB.Append(p1);
                Param.Add(p1, ((IGeoShape)value).GeoText);
            }
            else
            {
                var p1 = ParamName;
                StringB.Append(p1);
                Param.Add(p1, value);
            }
        }

        private Expression VisitValue(Expression expr)
        {
            if (IsPredicate(expr))
            {
                StringB.Append("CASE WHEN (");
                Visit(expr);
                StringB.Append(") THEN 1 ELSE 0 END");
                return expr;
            }

            return Visit(expr);
        }

        private static bool IsPredicate(Expression expr)
        {
            switch (expr.NodeType)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    return IsBoolean(expr.Type);
                case ExpressionType.Not:
                    return IsBoolean(expr.Type);
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                    return true;
                case ExpressionType.Call:
                    return IsBoolean(expr.Type);
                default:
                    return false;
            }
        }

        private static bool IsBoolean(Type type)
        {
            return type == typeof(bool) || type == typeof(bool?);
        }

        private static string GetOperator(string methodName)
        {
            switch (methodName)
            {
                case "Add": return "+";
                case "Subtract": return "-";
                case "Multiply": return "*";
                case "Divide": return "/";
                case "Negate": return "-";
                case "Remainder": return "%";
                default: return null;
            }
        }

        protected override ReadOnlyCollection<Expression> VisitExpressionList(ReadOnlyCollection<Expression> original)
        {
            List<Expression> list = null;
            for (int i = 0, n = original.Count; i < n; i++)
            {
                StringB.Append(" ,");

                var p = Visit(original[i]);
               
                if (list != null)
                {
                    list.Add(p);
                }
                else if (p != original[i])
                {
                    list = new List<Expression>(n);
                    for (var j = 0; j < i; j++) list.Add(original[j]);
                    list.Add(p);
                }

            }

            if (list != null) return list.AsReadOnly();
            return original;
        }

        protected override NewExpression VisitNew(NewExpression nex)
        {
            if (nex.Type == typeof(Guid))
            {
                if (_providerName == ProviderName.PostgreSql || _providerName == ProviderName.MySql)
                {
                    var str = Expression.Lambda<Func<Guid>>(nex).Compile()();
                    var p = ParamName;
                    //StringB.Append(p);
                    AddParameter(str);
                    //Param.Add(p, str);
                    return nex;
                }

                foreach (var nexArgument in nex.Arguments) Visit(nexArgument);
                return nex;
            }

            if (nex.Type == typeof(DateTime))
            {
                var str = Expression.Lambda<Func<DateTime>>(nex).Compile()();
                var p = ParamName;
                //StringB.Append(p);
                AddParameter(str);
                //Param.Add(p, str);
                return nex;
            }

            // var st = UtilsCore.GetSerializeType(nex.Type);
            // switch (st)
            // {
            //     case SerializeType.User:
            //     {
            //         var str = Expression.Lambda<Func<object>>(nex).Compile()();
            //         var p = ParamName;
            //         StringB.Append(p);
            //         var value = ((IMapSerializable)str).Serialize();
            //         Param.Add(p, value);
            //         return nex;
            //     }
            // }


            IEnumerable<Expression> args = VisitExpressionList(nex.Arguments);
            if (args != nex.Arguments)
            {
                if (nex.Members != null)
                {
                    AddListOne(new OneComposite
                    {
                        Operand = Evolution.SelectNew,
                        NewConstructor = Expression.New(nex.Constructor, args, nex.Members)
                    });
                    return Expression.New(nex.Constructor, args, nex.Members);
                }

                AddListOne(new OneComposite { Operand = Evolution.SelectNew, NewConstructor = nex });
                return Expression.New(nex.Constructor, args);
            }

            //todo ion100
            AddListOne(new OneComposite { Operand = Evolution.SelectNew, NewConstructor = nex });
            return nex;
        }

        private Expression BindSelectMany(Expression exp, Expression source, LambdaExpression collectionSelector,
            LambdaExpression resultSelector)
        {
            throw new Exception("not implemented");
        }

        protected override Expression VisitParameter(ParameterExpression m)
        {
            if (m.Type == typeof(int) && _currentMethod == "Select")
            {
                switch (_providerName)
                {
                    case ProviderName.MsSql:
                        StringB.Append(" CAST((ROW_NUMBER() OVER(ORDER BY (Select 0))) AS int) ");
                        break;
                    case ProviderName.MySql:
                        StringB.Append(" (CAST((row_number() OVER ()) AS UNSIGNED)) ");
                        break;
                    case ProviderName.PostgreSql:
                        StringB.Append(" (row_number() OVER ())  ");
                        break;
                    case ProviderName.SqLite:
                        StringB.Append(" (CAST((row_number() OVER ()) AS UNSIGNED)) ");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }


            }

            return m;
        }
    }

    internal class MyClass
    {
        public int Age { get; set; }
        public string Name { get; set; }
    }
}