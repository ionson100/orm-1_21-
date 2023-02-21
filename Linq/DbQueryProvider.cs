using ORM_1_21_.Linq.MsSql;
using ORM_1_21_.Linq.MySql;
using ORM_1_21_.Transaction;
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
    internal class DbQueryProvider<T> :  QueryProvider, ISqlComposite
    {
        private readonly Dictionary<string, object> _paramFree = new Dictionary<string, object>();
        private readonly List<ParameterStoredPr> _paramFreeStoredPr = new List<ParameterStoredPr>();


        private readonly Dictionary<string, object> _parOut = new Dictionary<string, object>();
        private readonly Sessione _sessione;
        private IDbCommand _com;
        private ProviderName _providerName;


        private bool _isStoredPr;
        private List<OneComprosite> _listOne;
        private Dictionary<string, object> _param;
        

        public DbQueryProvider(Sessione ses)
        {
            _sessione = ses;
            Sessione = ses;
            _providerName = ses.MyProviderName;
        }

        public ISession Sessione { get; }

        public List<ContainerCastExpression> ListCastExpression { get; set; } = new List<ContainerCastExpression>();

        public Transactionale Transactionale
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public IDbTransaction Transaction
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        private bool PingComposite(Evolution eval)
        {
            return _listOne.Any(a => a.Operand == eval);
        }


        public override string GetQueryText(Expression expression)
        {
            return TranslateString(expression, out _);
        }

        public override object Execute(Expression expression)
        {
            return null;
        }

        public object Execute<TS>(Expression expression, List<OneComprosite> comprosites,
            Dictionary<string, object> dictionary)
        {
            _param = dictionary;
            _listOne = comprosites;
            return Execute<TS>(expression);
        }

        public object ExecuteParam<TS>(Expression expression, params Parameter[] par)
        {
            if (par != null) par.ToList().ForEach(a => _paramFree.Add(a.Name, a.Value));
            return Execute<TS>(expression);
        }

        public override object ExecuteSPP<TS>(Expression expression)
        {
            var sb = new StringBuilder();
            IDataReader dataReader = null;
            var services = (IServiceSessions)Sessione;
            _com = services.CommandForLinq;
            _com.CommandType = CommandType.StoredProcedure;
            _com.CommandText = Translate(expression, out _);
            if (_providerName == ProviderName.MsSql)
            {
                var mat = new Regex(@"TOP\s@p\d").Matches(_com.CommandText);
                foreach (var variable in mat)
                {
                    var st = variable.ToString().Split(' ')[1];
                    var val = _param.FirstOrDefault(a => a.Key == st).Value;
                    _com.CommandText = _com.CommandText.Replace(variable.ToString(),
                        string.Format("{1} ({0})", val, StringConst.Top));
                    _param.Remove(st);
                }
            }

            foreach (var p in _paramFreeStoredPr)
            {
                sb.Append(string.Format(CultureInfo.CurrentCulture, "{0}-{1},", p.Name, p.Value));
                IDataParameter pr = _com.CreateParameter();
                pr.Direction = p.Direction;
                pr.ParameterName = p.Name;
                pr.Value = p.Value;
                _com.Parameters.Add(pr);
            }

            try
            {
                _sessione.OpenConnectAndTransaction(_com);
                dataReader = _com.ExecuteReader();
                if (AttributesOfClass<TS>.IsValid)
                {
                    var lResult = AttributesOfClass<TS>.GetEnumerableObjects(dataReader, _providerName);
                    foreach (var par in _com.Parameters)
                        if (((IDataParameter)par).Direction == ParameterDirection.InputOutput ||
                            ((IDataParameter)par).Direction == ParameterDirection.Output ||
                            ((IDataParameter)par).Direction == ParameterDirection.ReturnValue)
                            _parOut.Add(((IDataParameter)par).ParameterName, ((IDataParameter)par).Value);
                    return lResult;
                }
                else
                {
                    #region

                    var countcol = dataReader.FieldCount;
                    var list = new List<Type>();
                    for (var i = 0; i < countcol; i++) list.Add(dataReader.GetFieldType(i));
                    var ci = typeof(TS).GetConstructor(list.ToArray());
                    var resDis = new List<TS>();
                    while (dataReader.Read())
                        if (ci != null)
                        {
                            var par = new List<object>();
                            for (var i = 0; i < countcol; i++)
                                par.Add(dataReader[i] == DBNull.Value ? null : dataReader[i]);
                            var e = ci.Invoke(par.ToArray());
                            resDis.Add((TS)e);
                        }
                        else
                        {
                            if (countcol == 1)
                            {
                                resDis.Add((TS)UtilsCore.Convertor<TS>(dataReader[0]));
                            }
                            else
                            {
                                dynamic employee = new ExpandoObject();
                                for (var i = 0; i < countcol; i++)
                                    ((IDictionary<string, object>)employee).Add(dataReader.GetName(i),
                                        dataReader[i] == DBNull.Value ? null : dataReader[i]);
                                resDis.Add((TS)employee);
                            }
                        }

                    dataReader.NextResult();
                    foreach (var par in _com.Parameters)
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
                MySqlLogger.Error(_com.CommandText, ex);
                throw;

            }
            finally
            {
                _sessione.ComDisposable(_com);
                if (dataReader != null)
                {
                    dataReader.Close();
                    dataReader.Dispose();
                }
            }
        }

        

        public object ExecuteMonster<TS>(IDataReader dataReader)
        {

            var count = dataReader.FieldCount;
            var list = new List<Type>();
            for (var i = 0; i < count; i++) list.Add(dataReader.GetFieldType(i));
            var ci = typeof(TS).GetConstructor(list.ToArray());
            var resDis = new List<TS>();
            if (ci != null)
            {
                while (dataReader.Read())
                {
                    var par = new List<object>();
                    for (var i = 0; i < count; i++)
                        par.Add(dataReader[i] == DBNull.Value ? null : dataReader[i]);
                    var e = ci.Invoke(par.ToArray());
                    resDis.Add((TS)e);
                }
            }
            else if (count == 1 && typeof(TS).IsValueType || typeof(TS) == typeof(string) ||
                     typeof(TS).GetInterface("IEnumerable") != null || typeof(TS).IsGenericTypeDefinition)
            {
                while (dataReader.Read())
                    resDis.Add((TS)(dataReader[0] == DBNull.Value ? null : dataReader[0]));
            }
            else
            {
                if (typeof(TS) != typeof(object) && typeof(TS).IsClass)
                {
                    object employee;
                    var isLegasy = AttributesOfClass<TS>.IsUssageActivator(_providerName);
                    if (isLegasy)
                    {
                        employee = Activator.CreateInstance<TS>();
                    }
                    else
                    {
                        employee = (TS)FormatterServices.GetSafeUninitializedObject(typeof(TS));
                    }
                    while (dataReader.Read())
                    {
                        for (var i = 0; i < dataReader.FieldCount; i++)
                            AttributesOfClass<TS>.SetValueFreeSqlE(_providerName, dataReader.GetName(i), (TS)employee,
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

            return resDis;
        }


        private int? GetTimeout()
        {
            foreach (var t in ListCastExpression)
            {
                if (t.TypeRevalytion == Evolution.Timeout && t.Timeout >= 0) { return t.Timeout; }
            }
            return null;
        }

        public override object Execute<TS>(Expression expression)
        {
            var services = (IServiceSessions)Sessione;
            _com = services.CommandForLinq;
            if (GetTimeout() >= 0)
            {
                _com.CommandTimeout = GetTimeout().Value;
            }


            if (_isStoredPr)
                _com.CommandType = CommandType.StoredProcedure;

            _com.CommandText = Translate(expression, out _).Replace("FROM", " FROM ");
            var sb = new StringBuilder();
            if (_providerName == ProviderName.MsSql)
            {
                var mat = new Regex(@"TOP\s@p\d").Matches(_com.CommandText);
                foreach (var variable in mat)
                {
                    var st = variable.ToString().Split(' ')[1];
                    var val = _param.FirstOrDefault(a => a.Key == st).Value;
                    _com.CommandText = _com.CommandText.Replace(variable.ToString(),
                        string.Format("{1} ({0})", val, StringConst.Top));
                    _param.Remove(st);
                }
            }

            _com.Parameters.Clear();
           

            foreach (var p in _param)
            {
                sb.Append(string.Format(CultureInfo.CurrentCulture, "{0}-{1},", p.Key, p.Value));
                IDataParameter pr = _com.CreateParameter();
                pr.ParameterName = p.Key;
                pr.Value = p.Value;
                _com.Parameters.Add(pr);
            }

            foreach (var p in _paramFree)
            {
                IDataParameter pr = _com.CreateParameter();
                pr.ParameterName = p.Key;
                pr.Value = p.Value;
                _com.Parameters.Add(pr);
            }

            foreach (var p in _paramFreeStoredPr)
            {
                IDataParameter pr = _com.CreateParameter();
                pr.Direction = p.Direction;
                pr.ParameterName = p.Name;
                pr.Value = p.Value;
                _com.Parameters.Add(pr);
            }


            IDataReader dataReader = null;
            try
            {
                _sessione.OpenConnectAndTransaction(_com);
                if (PingComposite(Evolution.All))
                {
                    var ee = _com.ExecuteScalar();
                    var res = ee.ToString() != "0";
                    return res;
                }

                if (PingComposite(Evolution.Count))
                {
                    var ee = _com.ExecuteScalar();
                    var res = Convert.ToInt32(ee, CultureInfo.CurrentCulture);
                    return res;
                }

                if (PingComposite(Evolution.Delete))
                {
                    var ee = _com.ExecuteNonQuery();
                    return ee;
                }

                if (PingComposite(Evolution.Update))
                {
                    var ee = _com.ExecuteNonQuery();
                    return ee;
                }

                if (PingComposite(Evolution.Any))
                {
                    var ee = _com.ExecuteScalar();
                    var res = Convert.ToInt32(ee, CultureInfo.CurrentCulture) != 0;
                    return res;
                }


                #region  dataReader

                if (PingComposite(Evolution.FreeSql) &&
                    AttributesOfClass<TS>.IsValid == false)
                {
                    dataReader = _com.ExecuteReader();
                    var count = dataReader.FieldCount;
                    var list = new List<Type>();
                    for (var i = 0; i < count; i++) list.Add(dataReader.GetFieldType(i));
                    var ci = typeof(TS).GetConstructor(list.ToArray());
                    var resDis = new List<TS>();
                    if (ci != null)
                    {
                        while (dataReader.Read())
                        {
                            var par = new List<object>();
                            for (var i = 0; i < count; i++)
                                par.Add(dataReader[i] == DBNull.Value ? null : dataReader[i]);
                            var e = ci.Invoke(par.ToArray());
                            resDis.Add((TS)e);
                        }
                    }
                    else if (count == 1 && typeof(TS).IsValueType || typeof(TS) == typeof(string) ||
                             typeof(TS).GetInterface("IEnumerable") != null || typeof(TS).IsGenericTypeDefinition)
                    {
                        while (dataReader.Read())
                            resDis.Add((TS)(dataReader[0] == DBNull.Value ? null : dataReader[0]));
                    }
                    else
                    {
                        if (typeof(TS) != typeof(object) && typeof(TS).IsClass)
                        {
                            object employee;
                            var isLegasy = AttributesOfClass<TS>.IsUssageActivator(_providerName);
                            if (isLegasy)
                            {
                                employee = Activator.CreateInstance<TS>();
                            }
                            else
                            {
                                employee = (TS)FormatterServices.GetSafeUninitializedObject(typeof(TS));
                            }
                            while (dataReader.Read())
                            {
                                for (var i = 0; i < dataReader.FieldCount; i++)
                                    AttributesOfClass<TS>.SetValueFreeSqlE(_providerName, dataReader.GetName(i), (TS)employee,
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

                    dataReader.Dispose();
                    foreach (var par in _com.Parameters)
                        if (((IDataParameter)par).Direction == ParameterDirection.InputOutput ||
                            ((IDataParameter)par).Direction == ParameterDirection.Output ||
                            ((IDataParameter)par).Direction == ParameterDirection.ReturnValue)
                            _parOut.Add(((IDataParameter)par).ParameterName, ((IDataParameter)par).Value);
                    return resDis;
                }

                if (_listOne.Any(a => a.Operand == Evolution.Select && a.IsAgregate))
                {
                    dataReader = _com.ExecuteReader();
                    object rObj = null;
                    while (dataReader.Read())
                    {
                        rObj = dataReader[0];
                        break;
                    }
                    

                    dataReader.Dispose();
                    var res = UtilsCore.Convertor<TS>(rObj);
                    return res;
                }

                if (PingComposite(Evolution.Join))
                {
                    dataReader = _com.ExecuteReader();
                    var lres = new List<TS>();
                    var ss = _listOne.Single(a => a.Operand == Evolution.Join).NewConctructor;
                    if (ss == null)
                        while (dataReader.Read())
                            lres.Add((TS)dataReader[0]);
                    else
                        lres = Pizdaticus.GetListAnonymousObj<TS>(dataReader, ss);
                    return lres;
                }


                if (PingComposite(Evolution.Select) &&
                    !PingComposite(Evolution.SelectNew))
                {
                    var lres = new List<TS>();
                    dataReader = _com.ExecuteReader();
                    while (dataReader.Read()) lres.Add((TS)UtilsCore.Convertor<TS>(dataReader[0]));
                    dataReader.Dispose();
                    bool isactive1;
                    var datasingl1 = Pizdaticus.SingleData(_listOne, lres, out isactive1);
                    return !isactive1 ? (object)lres : datasingl1;
                }

                if (PingComposite(Evolution.ElementAt))
                {
                    dataReader = _com.ExecuteReader();
                    var r = AttributesOfClass<T>.GetEnumerableObjects(dataReader, _providerName);
                    var enumerable = r as T[] ?? r.ToArray();
                    if (enumerable.Any()) return enumerable.First();
                    throw new Exception("Элемент отсутствует в выбоке.");
                }
                if (PingComposite(Evolution.ElementAtOrDefault))
                {
                    dataReader = _com.ExecuteReader();
                    var r = AttributesOfClass<T>.GetEnumerableObjects(dataReader, _providerName);
                    var enumerable = r as T[] ?? r.ToArray();
                    if (enumerable.Any()) return enumerable.First();
                    return null;
                }

                if (PingComposite(Evolution.DistinctCustom))
                {
                    var tt = typeof(TS);
                    var tt3 = typeof(T);
                    Type retType = this.ListCastExpression.Single(a => a.TypeRevalytion == Evolution.DistinctCustom).TypeRetyrn;
                    IList resT = this.ListCastExpression.Single(a => a.TypeRevalytion == Evolution.DistinctCustom).ListDistict;
                    dataReader = _com.ExecuteReader();
                    if (PingComposite(Evolution.SelectNew))
                    {
                        var ss = _listOne.Single(a => a.Operand == Evolution.SelectNew).NewConctructor;
                        Pizdaticus.GetListAnonymusObjDistinct(dataReader, ss, resT);
                        return resT;

                    }
                    else
                    {
                        var resDis = resT;
                        while (dataReader.Read())
                        {
                            resDis.Add(dataReader[0]);
                        }

                        dataReader.Dispose();
                        return resDis;
                    }

                }

                if (PingComposite(Evolution.SelectNew))
                {
                    //todo ion100
                    var ss = _listOne.Single(a => a.Operand == Evolution.SelectNew).NewConctructor;
                    _com.CommandText = _com.CommandText.Replace(",?p", "?p");
                    dataReader = _com.ExecuteReader();
                    if (UtilsCore.IsAnonymousType(typeof(TS)))
                    {
                        var lRes = Pizdaticus.GetListAnonymousObj<TS>(dataReader, ss);
                        bool isaActive1;
                        var dataSingl1 = Pizdaticus.SingleData(_listOne, lRes, out isaActive1);
                        var res = !isaActive1 ? (object)lRes : dataSingl1;
                        return res;
                    }
                    else
                    {
                        if (_listOne.Any(a => a.Operand == Evolution.GroupBy && a.ExpressionDelegate != null))
                        {
                            var lRes = Pizdaticus.GetListAnonymousObj<object>(dataReader, ss);
                            bool isActive1;
                            var dataSingl1 = Pizdaticus.SingleData(_listOne, lRes, out isActive1);
                            var res = !isActive1 ? lRes : dataSingl1;
                            return res;
                        }

                        throw new Exception("Не реализовано");
                    }
                }

                #endregion

                ////////////////////////////////////////////////////////////////////////////////////////////////
                dataReader = _com.ExecuteReader();
                if (_listOne.Any(a => a.Operand == Evolution.GroupBy && a.ExpressionDelegate != null))
                {
                    var lResult = AttributesOfClass<TS>.GetEnumerableObjectsGroupBy<T>(dataReader,
                        _listOne.First(a => a.Operand == Evolution.GroupBy).ExpressionDelegate, _providerName);
                    return lResult;
                }

                var resd = AttributesOfClass<T>.GetEnumerableObjects(dataReader, _providerName);
                bool isActive;
                var dataSingl = Pizdaticus.SingleData(_listOne, resd, out isActive);
                var ress2 = !isActive ? (object)resd : dataSingl;
                return ress2;
            }

            catch (Exception ex)
            {
                MySqlLogger.Error(UtilsCore.GetStringSql(_com), ex);
                throw;
            }

            finally
            {
                _sessione.ComDisposable(_com);
                if (dataReader != null)
                {
                    dataReader.Close();
                    dataReader.Dispose();
                }
            }
        }

        private string Translate(Expression expression, out Evolution ev1)
        {
            ITranslate sq;
            switch (_providerName)
            {
                case ProviderName.MySql:
                    sq = new QueryTranslator<T>(_providerName);
                    break;
                case ProviderName.MsSql:
                    sq = new QueryTranslatorMsSql<T>(_providerName);
                    break;
                case ProviderName.Postgresql:
                    sq = new QueryTranslator<T>(_providerName);
                    break;
                case ProviderName.Sqlite:
                    sq = new QueryTranslator<T>(_providerName);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }


            _listOne = sq.ListOne;
            _param = sq.Param;
            ListCastExpression.ForEach(a => sq.Translate(a.CastomExpression, a.TypeRevalytion, a.ParamList));
            string res = sq.Translate(expression, out ev1);
           
            return res;

        }

        private string TranslateString(Expression expression, out Evolution ev1)
        {
            ITranslate sq;
            switch (_providerName)
            {
                case ProviderName.MySql:
                    sq = new QueryTranslator<T>(_providerName);
                    break;
                case ProviderName.MsSql:
                    sq = new QueryTranslatorMsSql<T>(_providerName);
                    break;
                case ProviderName.Postgresql:
                    sq = new QueryTranslator<T>(_providerName);
                    break;
                case ProviderName.Sqlite:
                    sq = new QueryTranslator<T>(_providerName);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _listOne = sq.ListOne;
            _param = sq.Param;
            ListCastExpression.ForEach(a => sq.Translate(a.CastomExpression, a.TypeRevalytion, a.ParamList));
            string res = sq.Translate(expression, out ev1);
           

            return res;
        }

        private string TranslateString(Expression expression, List<OneComprosite> comprosites,
            IDictionary<string, object> dictionary, string parstr)
        {
            ITranslate sq;
            switch (_providerName)
            {
                case ProviderName.MySql:
                    sq = new QueryTranslator<T>(_providerName);
                    break;
                case ProviderName.MsSql:
                    sq = new QueryTranslatorMsSql<T>(_providerName);
                    break;
                case ProviderName.Postgresql:
                    sq = new QueryTranslator<T>(_providerName);
                    break;
                case ProviderName.Sqlite:
                    sq = new QueryTranslator<T>(_providerName);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }


            _listOne = sq.ListOne;
            _param = sq.Param;
            ListCastExpression.ForEach(a => sq.Translate(a.CastomExpression, a.TypeRevalytion, a.ParamList));

            var eee = sq.Translate(expression, out _, parstr);
            comprosites.AddRange(_listOne);
            if (_param != null && _param.Any())
                foreach (var d in _param)
                    dictionary.Add(d.Key, d.Value);
            return eee;
        }

        internal override string GetQueryTextForJoin(Expression expression, List<OneComprosite> comprosite,
            Dictionary<string, object> dictionary, string parStr)
        {
            return TranslateString(expression, comprosite, dictionary, parStr);
        }

        public IEnumerable<TS> ExecuteCall<TS>(Expression callExpr)
        {
            _isStoredPr = true;
            return (IEnumerable<TS>)Execute<TS>(callExpr);
        }

        public IEnumerable<TS> ExecuteCallParam<TS>(Expression callExpr, params ParameterStoredPr[] par)
        {
            _isStoredPr = true;
            if (par != null) _paramFreeStoredPr.AddRange(par);
            var res = (IEnumerable<TS>)ExecuteSPP<TS>(callExpr);

            foreach (var re in _parOut)
            {
                if (par == null) continue;
                var p = par.FirstOrDefault(a => a.Name == re.Key);
                if (p != null) p.Value = re.Value;
            }

            return res;
        }

        public Type GetTypeGetTypeGeneric()
        {
            return typeof(T);
        }
    }
}