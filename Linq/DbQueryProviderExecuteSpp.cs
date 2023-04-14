using ORM_1_21_.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;

namespace ORM_1_21_.Linq
{
    internal partial class DbQueryProvider<T>
    {
        public override object ExecuteSpp<TS>(Expression expression)
        {
            IDataReader dataReader = null;
            var services = (IServiceSessions)Sessione;
            var com = services.CommandForLinq;
            try
            {
                var sb = new StringBuilder();

                com.CommandType = CommandType.StoredProcedure;
                var re = TranslateE(expression);
                com.CommandText = re.Sql;
                if (_providerName == ProviderName.MsSql)
                {
                    var mat = new Regex(@"TOP\s@p\d").Matches(com.CommandText);
                    foreach (var variable in mat)
                    {
                        var st = variable.ToString().Split(' ')[1];
                        var val = _param.FirstOrDefault(a => a.Key == st).Value;
                        com.CommandText = com.CommandText.Replace(variable.ToString(),
                            string.Format("{1} ({0})", val, StringConst.Top));
                        _param.Remove(st);
                    }
                }

                foreach (var p in _paramFreeStoredPr)
                {
                    sb.Append(string.Format(CultureInfo.CurrentCulture, "{0}-{1},", p.Name, p.Value));
                    IDataParameter pr = com.CreateParameter();
                    pr.Direction = p.Direction;
                    pr.ParameterName = p.Name;
                    pr.Value = p.Value;
                    com.Parameters.Add(pr);
                }

                _sessione.OpenConnectAndTransaction(com);
                dataReader = com.ExecuteReader();
                if (UtilsCore.IsValid<TS>())
                {
                    var lResult = AttributesOfClass<TS>.GetEnumerableObjects(dataReader, _providerName);
                    foreach (var par in com.Parameters)
                        if (((IDataParameter)par).Direction == ParameterDirection.InputOutput ||
                            ((IDataParameter)par).Direction == ParameterDirection.Output ||
                            ((IDataParameter)par).Direction == ParameterDirection.ReturnValue)
                            _parOut.Add(((IDataParameter)par).ParameterName, ((IDataParameter)par).Value);
                    return lResult;
                }
                else
                {
                    #region

                    var count = dataReader.FieldCount;
                    var list = new List<Type>();
                    for (var i = 0; i < count; i++) list.Add(dataReader.GetFieldType(i));
                    var ci = typeof(TS).GetConstructor(list.ToArray());
                    var resDis = new List<TS>();
                    while (dataReader.Read())
                        if (ci != null)
                        {
                            var par = new List<object>();
                            for (var i = 0; i < count; i++)
                                par.Add(dataReader[i] == DBNull.Value ? null : dataReader[i]);
                            var e = ci.Invoke(par.ToArray());
                            resDis.Add((TS)e);
                        }
                        else
                        {
                            if (count == 1)
                            {
                                resDis.Add((TS)UtilsCore.Convertor<TS>(dataReader[0]));
                            }
                            else
                            {
                                dynamic employee = new ExpandoObject();
                                for (var i = 0; i < count; i++)
                                    ((IDictionary<string, object>)employee).Add(dataReader.GetName(i),
                                        dataReader[i] == DBNull.Value ? null : dataReader[i]);
                                resDis.Add((TS)employee);
                            }
                        }

                    dataReader.NextResult();
                    foreach (var par in com.Parameters)
                        if (((IDataParameter)par).Direction == ParameterDirection.InputOutput ||
                            ((IDataParameter)par).Direction == ParameterDirection.Output ||
                            ((IDataParameter)par).Direction == ParameterDirection.ReturnValue)
                            _parOut.Add(((IDataParameter)par).ParameterName, ((IDataParameter)par).Value);

                    return resDis;

                    #endregion
                }
            }
            catch (Exception ex)
            {
                _sessione.Transactionale.isError = true;
                MySqlLogger.Error(com.CommandText, ex);
                throw new Exception(ex.Message + Environment.NewLine + com.CommandText, ex);

            }
            finally
            {
                _sessione.ComDisposable(com);
                if (dataReader != null)
                {
                    dataReader.Close();
                    dataReader.Dispose();
                }
            }
        }
    }
}
