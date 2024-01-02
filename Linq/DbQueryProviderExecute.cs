using ORM_1_21_.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;

namespace ORM_1_21_.Linq
{
    internal partial class DbQueryProvider<T>
    {
        public override object Execute<TS>(Expression expression)
        {
           
            var services = (IServiceSessions)Sessione;
            var re = TranslateE(expression);
            string sql = re.Sql;
            List<PostExpression> postExpressions = re.ListPostExpression;
            if (postExpressions.Count > 0)
            {

            }
            var paramJon = re.Param;


            List<OneComposite> listCore = re.Composites;
            listCore.AddRange(ListOuterOneComposites);

          

            var com = services.CommandForLinq;

            var to = GetTimeout();
            if (to >= 0)
            {
                com.CommandTimeout = to;
            }


            if (_isStoredPr)
                com.CommandType = CommandType.StoredProcedure;

            com.CommandText = sql;
            var sb = new StringBuilder();
            if (_providerName == ProviderName.MsSql)
            {
                var mat = new Regex(@"TOP\s@p\d").Matches(com.CommandText);
                foreach (var variable in mat)
                {
                    var st = variable.ToString().Split(' ')[1];
                    var val = paramJon.FirstOrDefault(a => a.Key == st).Value;
                    com.CommandText = com.CommandText.Replace(variable.ToString(),
                        string.Format("{1} ({0})", val, StringConst.Top));
                    paramJon.Remove(st);
                }
            }

            com.Parameters.Clear();


            foreach (var p in paramJon)
            {
                sb.Append(string.Format(CultureInfo.CurrentCulture, "{0}-{1},", p.Key, p.Value));
                IDataParameter pr = com.CreateParameter();
                pr.ParameterName = p.Key;
                pr.Value = p.Value;
                com.Parameters.Add(pr);
            }

            if (_paramFree.Any())
            {
                UtilsCore.AddParam(com, _providerName, _paramFree.ToArray());
            }


            foreach (var p in _paramFreeStoredPr)
            {
                IDataParameter pr = com.CreateParameter();
                pr.Direction = p.Direction;
                pr.ParameterName = p.Name;
                pr.Value = p.Value;
                com.Parameters.Add(pr);
            }


            IDataReader dataReader = null;
            try
            {
                _session.OpenConnectAndTransaction(com);

                if (PingCompositeE(Evolution.All, listCore))
                {
                    dataReader = com.ExecuteReader();
                   
                        while (dataReader.Read())
                        {
                            var v1 = dataReader.GetInt32(0);
                            var v2 = dataReader.GetInt32(1);
                            var res = v1 == v2;
                            return res;
                        }
                }

                if (PingCompositeE(Evolution.LongCount, listCore))
                {
                    var ee = com.ExecuteScalar();
                    var res = Convert.ToInt64(ee, CultureInfo.CurrentCulture);
                   
                    return res;
                }

                if (PingCompositeE(Evolution.Count, listCore))
                {
                    var ee = com.ExecuteScalar();
                    var res = Convert.ToInt32(ee, CultureInfo.CurrentCulture);
                
                    return res;
                }


                if (PingCompositeE(Evolution.Delete, listCore))
                {
                    var ee = com.ExecuteNonQuery();
                    return ee;
                }

                if (PingCompositeE(Evolution.Update, listCore))
                {
                    var ee = com.ExecuteNonQuery();
                    return ee;
                }

                if (PingCompositeE(Evolution.Any, listCore))
                {
                    var ee = com.ExecuteScalar();
                    var res = Convert.ToInt32(ee, CultureInfo.CurrentCulture) != 0;
                    return res;
                }

                #region dataReader

                if (PingCompositeE(Evolution.TableCreate, listCore))
                {
                    if (UtilsCore.IsValid<T>() == false)
                    {
                        throw new Exception(
                            $"I can't create a table from type {typeof(T)}, it doesn't have attribute: MapTableAttribute");
                    }

                    com.CommandText = listCore.First(a => a.Operand == Evolution.TableCreate).Body;
                    int res = com.ExecuteNonQuery();
                    return res;
                }

                if (PingCompositeE(Evolution.DropTable, listCore))
                {
                    com.CommandText = listCore.First(a => a.Operand == Evolution.DropTable).Body;
                    int res = com.ExecuteNonQuery();
                    return res;
                }

                if (PingCompositeE(Evolution.DataTable, listCore))
                {
                    com.CommandText = listCore.First(a => a.Operand == Evolution.DataTable).Body;
                    var table = new DataTable();
                    dataReader = com.ExecuteReader();
                    table.BeginLoadData();
                    table.Load(dataReader);
                    table.EndLoadData();
                    return table;
                }

                if (PingCompositeE(Evolution.ExecuteNonQuery, listCore))
                {
                    com.CommandText = listCore.First(a => a.Operand == Evolution.ExecuteNonQuery).Body;
                    int res = com.ExecuteNonQuery();
                    return res;
                }

                if (PingCompositeE(Evolution.TruncateTable, listCore))
                {
                    com.CommandText = listCore.First(a => a.Operand == Evolution.TruncateTable).Body;
                    int res = com.ExecuteNonQuery();
                    return res;
                }

                if (PingCompositeE(Evolution.ExecuteScalar, listCore))
                {

                    com.CommandText = listCore.First(a => a.Operand == Evolution.ExecuteScalar).Body;
                    object res = com.ExecuteScalar();
                    return res;
                }

                if (PingCompositeE(Evolution.TableExists, listCore))
                {
                    com.CommandText = listCore.First(a => a.Operand == Evolution.TableExists).Body;
                    if (_providerName == ProviderName.PostgreSql)
                    {
                        var r = (long)com.ExecuteScalar();
                        return r != 0;
                    }
                    if (_providerName == ProviderName.MsSql)
                    {
                        var r = com.ExecuteScalar();
                        return !(r is DBNull);
                    }
                    else
                    {
                        try
                        {
                            com.ExecuteNonQuery();
                            return true;
                        }
                        catch (Exception)
                        {
                            return false;
                        }
                    }
                }

                if (PingCompositeE(Evolution.FreeSql, listCore) &&
                    UtilsCore.IsValid<TS>() == false)
                {
                    dataReader = com.ExecuteReader();
                    var count = dataReader.FieldCount;
                    var list = new List<Type>();
                    var resDis = new List<TS>();
                    if (UtilsCore.IsAnonymousType(typeof(TS)))
                    {
                        var ci = typeof(TS).GetConstructors().First();
                        foreach (var parameterInfo in ci.GetParameters())
                        {
                            list.Add(parameterInfo.ParameterType);
                        }
                        if (count != list.Count) throw new Exception("The number of anonymous type parameters does not match the number of sql query fields");
                        while (dataReader.Read())
                        {
                            var par = new List<object>();
                            for (var i = 0; i < count; i++)
                            {
                                var val = Pizdaticus.MethodFreeIndex(_providerName, list[i], dataReader, i);
                                par.Add(val);
                            }
                            var e = ci.Invoke(par.ToArray());
                            resDis.Add((TS)e);
                        }
                    }
                    else if (UtilsCore.IsReceiverFreeSql<TS>())
                    {
                        var c = typeof(TS).GetConstructors();
                        if (c.Length == 0)
                        {
                            throw new Exception(
                                $"The type {typeof(TS)} is marked with an attribute but does not have a constructor");
                        }

                        if (c.Length > 1)
                        {
                            throw new Exception(
                                $"The type {typeof(TS)} is marked with an attribute. Must have only one constructor");
                        }

                        var ci = c[0];

                        if (ci.GetParameters().Length != count)
                        {
                            throw new Exception(
                                $"The number of parameters of the constructor method:{ci.GetParameters().Length}  is not equal to the number of" +
                                $" fields retrieved from the database: {list.Count}, check sql the query");
                        }

                        while (dataReader.Read())
                        {
                            var par = new List<object>();
                            for (var i = 0; i < count; i++)
                            {
                                var ss = dataReader.GetName(i);
                                var sss = dataReader.GetValue(i).GetType();
                                var val = Pizdaticus.MethodFreeIndex(_providerName, ci.GetParameters()[i].ParameterType,
                                    dataReader, i);
                                par.Add(val);
                            }

                            var e = ci.Invoke(par.ToArray());
                            resDis.Add((TS)e);
                        }

                    }

                    else if (count == 1 && typeof(TS).IsValueType || typeof(TS) == typeof(string) ||
                             typeof(TS).GetInterface("IEnumerable") != null || typeof(TS).IsGenericTypeDefinition)
                    {
                        while (dataReader.Read())
                        {
                            resDis.Add((TS)(Pizdaticus.MethodFreeIndex(_providerName, typeof(TS), dataReader, 0)));
                        }
                    }
                    else
                    {
                        if (typeof(TS) != typeof(object) && typeof(TS).IsClass)
                        {

                            var isLegalese = AttributesOfClass<TS>.IsUsageActivator(_providerName);


                            while (dataReader.Read())
                            {
                                object employee;
                                if (isLegalese)
                                {
                                    employee = Activator.CreateInstance<TS>();
                                }
                                else
                                {
                                    employee = (TS)FormatterServices.GetSafeUninitializedObject(typeof(TS));
                                }
                                for (var i = 0; i < dataReader.FieldCount; i++)
                                    AttributesOfClass<TS>.SetValueFreeSqlE(_providerName, dataReader.GetName(i),
                                        (TS)employee,
                                        dataReader[i] == DBNull.Value ? null : dataReader[i]);
                                resDis.Add((TS)employee);
                            }
                        }
                        else
                        {
                            while (dataReader.Read())
                            {
                                dynamic employee = new ExpandoObject();
                                for (var i = 0; i < count; i++)
                                    ((IDictionary<string, object>)employee).Add(dataReader.GetName(i),
                                        dataReader[i] == DBNull.Value ? null : dataReader[i]);
                                resDis.Add((TS)employee);
                            }
                        }
                    }

                    foreach (var par in com.Parameters)
                        if (((IDataParameter)par).Direction == ParameterDirection.InputOutput ||
                            ((IDataParameter)par).Direction == ParameterDirection.Output ||
                            ((IDataParameter)par).Direction == ParameterDirection.ReturnValue)
                            _parOut.Add(((IDataParameter)par).ParameterName, ((IDataParameter)par).Value);
                    return resDis;
                }

                if (listCore.Any(a => a.Operand == Evolution.Select && a.IsAggregate))
                {
                    dataReader = com.ExecuteReader();
                    object rObj = null;
                    while (dataReader.Read())
                    {
                        rObj = UtilsCore.Convertor(dataReader.GetValue(0), typeof(TS));
                        break;
                    }
                    var res = rObj;
                    return res;
                }

                if (PingCompositeE(Evolution.Select, listCore) &&
                    !PingCompositeE(Evolution.SelectNew, listCore))
                {
                    var type = typeof(TS);
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    {
                        var ttType = typeof(TS).GenericTypeArguments[0];
                        var lees = new List<object>();
                        dataReader = com.ExecuteReader();
                        while (dataReader.Read())
                        {
                            lees.Add(Pizdaticus.MethodFreeIndex(_providerName, ttType, dataReader, 0));

                        }

                        var listNativeInvoke = DbHelp.CastList(lees);
                        var devastatingly1 = Pizdaticus.SingleData(listCore, lees, out var active1);
                        var res = !active1 ? listNativeInvoke : devastatingly1;
                        return res;

                    }
                    else
                    {
                        var lees = new List<TS>();
                        dataReader = com.ExecuteReader();
                        while (dataReader.Read())
                        {
                            lees.Add((TS)Pizdaticus.MethodFreeIndex(_providerName, typeof(TS), dataReader, 0));
                        }
                        var devastatingly1 = Pizdaticus.SingleData(listCore, lees, out var active1);
                        var res = !active1 ? (object)lees : devastatingly1;
                        return res;
                    }
                }

                if (PingCompositeE(Evolution.ElementAt, listCore))
                {
                    dataReader = com.ExecuteReader();
                    var r = AttributesOfClass<T>.GetEnumerableObjects(dataReader, _providerName);
                    var enumerable = r as T[] ?? r.ToArray();
                    if (enumerable.Any())
                    {
                        var res = enumerable.First();
                        return res;
                    }
                    throw new Exception("Element not in selection.");
                }

                if (PingCompositeE(Evolution.ElementAtOrDefault, listCore))
                {
                    dataReader = com.ExecuteReader();
                    var r = AttributesOfClass<T>.GetEnumerableObjects(dataReader, _providerName);
                    var enumerable = r as T[] ?? r.ToArray();

                    if (enumerable.Any())
                    {
                        return enumerable.First();
                    }
                    return enumerable.FirstOrDefault();
                }

                if (PingCompositeE(Evolution.DistinctCore, listCore))
                {
                    var sas = ListCastExpression.Single(a => a.TypeEvolution == Evolution.DistinctCore);
                    IList resT = sas.ListDistinct;
                    dataReader = com.ExecuteReader();
                    if (PingCompositeE(Evolution.SelectNew, listCore))
                    {
                        var ss = listCore.Single(a => a.Operand == Evolution.SelectNew).NewConstructor;
                        Pizdaticus.GetListAnonymousObjDistinct(dataReader, ss, resT, _providerName);
                        return resT;

                    }
                    else
                    {
                        var resDis = resT;
                        while (dataReader.Read())
                        {
                            var val = Pizdaticus.MethodFreeIndex(_providerName, sas.TypeReturn, dataReader, 0);
                            resDis.Add(val);
                        }
                        return resDis;
                    }
                }

                if (PingCompositeE(Evolution.SelectNew, listCore))
                {
                    //todo ion100
                    var ss = listCore.Single(a => a.Operand == Evolution.SelectNew).NewConstructor;
                    com.CommandText = com.CommandText.Replace(",?p", "?p");
                    dataReader = com.ExecuteReader();
                    var type = typeof(TS);
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    {
                        var ttType = typeof(TS).GenericTypeArguments[0];
                        if (UtilsCore.IsAnonymousType(ttType))
                        {
                            var lRes = Pizdaticus.GetListAnonymousObj<object>(dataReader, ss, _providerName);
                            var listNativeInvoke = DbHelp.CastList(lRes);

                            var dataSing1 = Pizdaticus.SingleData(listCore, lRes, out var isaActive1);

                            var res = !isaActive1 ? listNativeInvoke : dataSing1;
                            return res;
                        }
                        else
                        {
                            throw new Exception($"Method Select for IQueryable is not implemented, use method SelectCore or ...toList().Select()");

                        }
                    }
                    else
                    {
                        if (UtilsCore.IsAnonymousType(typeof(TS)))
                        {
                            var lRes = Pizdaticus.GetListAnonymousObj<TS>(dataReader, ss, _providerName);
                            var dataSing1 = Pizdaticus.SingleData(listCore, lRes, out var isaActive1);

                            var res = !isaActive1 ? (object)lRes : dataSing1;
                            return res;
                        }
                        else
                        {
                            throw new Exception($"Method Select for IQueryable is not implemented, use method SelectCore or ...toList().Select()");

                        }
                    }

                }

                #endregion

                dataReader = com.ExecuteReader();
               
                IEnumerable<T> res1 = AttributesOfClass<T>.GetEnumerableObjects(dataReader, _providerName, listCore.Any(a => a.Operand == Evolution.FreeSql));
               
                var dataSingle = Pizdaticus.SingleData(listCore, res1, out var isActive);
                var res2 = !isActive ? (object)res1 : dataSingle;
                return res2;
            }
            catch (Exception ex)
            {
                _session.Transactionale.isError = true;
                MySqlLogger.Error(UtilsCore.GetStringSql(com),ex);
                throw new Exception(ex.Message + Environment.NewLine + com.CommandText, ex);
            }

            finally
            {
                if (dataReader != null)
                {
                    dataReader.Dispose();
                }
                _session.ComDisposable(com);

            }
        }
    }
}
