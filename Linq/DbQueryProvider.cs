using ORM_1_21_.Linq.MsSql;
using ORM_1_21_.Linq.MySql;
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
using System.Threading;
using System.Threading.Tasks;

namespace ORM_1_21_.Linq
{
    internal partial class DbQueryProvider<T> : QueryProvider, ISqlComposite
    {
        private readonly List<object> _paramFree = new List<object>();
        private readonly List<ParameterStoredPr> _paramFreeStoredPr = new List<ParameterStoredPr>();
        private readonly Dictionary<string, object> _parOut = new Dictionary<string, object>();
        private readonly Sessione _sessione;
        private IDbCommand _com;
        private readonly ProviderName _providerName;

        private bool _isStoredPr;
        private Dictionary<string, object> _param;

        public Type GetSourceType()
        {
            return typeof(T);
        }

        public DbQueryProvider<T> GetDbQueryProvider()
        {
            return this;
        }
        public DbQueryProvider(Sessione ses)
        {
            _sessione = ses;
            Sessione = ses;
            _providerName = ses.MyProviderName;
        }

        public ISession Sessione { get; }
        public IDbTransaction Transaction { get; set; }
        public List<ContainerCastExpression> ListCastExpression { get; set; } = new List<ContainerCastExpression>();

        private static bool PingCompositeE(Evolution eval, List<OneComposite> list)
        {
            return list.Any(a => a.Operand == eval);
        }

        public override string GetQueryText(Expression expression)
        {
            return TranslateString(expression);
        }

        private CacheState CacheState
        {
            get
            {
                if (ListCastExpression.Any(t => t.TypeEvolution == Evolution.CacheUsage))
                {
                    return CacheState.CacheUsage;
                }
                if (ListCastExpression.Any(t => t.TypeEvolution == Evolution.CacheOver))
                {
                    return CacheState.CacheOver;
                }
                if (ListCastExpression.Any(t => t.TypeEvolution == Evolution.CacheKey))
                {
                    return CacheState.CacheKey;
                }

                return CacheState.NoCache;
            }
        }

        private async Task<List<TResult>> ActionCoreGroupBy<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            List<TResult> resList = new List<TResult>();
            var res = await ExecuteAsync<TResult>(expression, null, cancellationToken);
            foreach (var o in (IEnumerable<object>)res)
            {
                resList.Add((TResult)o);
            }
            return resList;
        }

        public override object Execute(Expression expression)
        {
            return null;
        }
        public override object ExecuteSpp<TS>(Expression expression)
        {
            IDataReader dataReader = null;
            try
            {
                var sb = new StringBuilder();
                var services = (IServiceSessions)Sessione;
                _com = services.CommandForLinq;
                _com.CommandType = CommandType.StoredProcedure;
                var re = TranslateE(expression);
                _com.CommandText = re.Sql;
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
                _sessione.Transactionale.isError = true;
                MySqlLogger.Error(_com.CommandText, ex);
                throw new Exception(ex.Message + Environment.NewLine + _com.CommandText, ex);

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


        private int GetTimeout()
        {
            foreach (var t in ListCastExpression)
            {
                if (t.TypeEvolution == Evolution.Timeout && t.Timeout >= 0) { return t.Timeout; }
            }
            return 30;
        }

        public List<object> GetParamFree()
        {
            return _paramFree;
        }

        public List<OneComposite> ListOuterOneComposites { get; set; } = new List<OneComposite>();

        public TS ExecuteExtension<TS>(Expression expression, params object[] param)
        {
            if (param != null && param.Length > 0)
            {
                _paramFree.AddRange(param);
            }
            var res = Execute<TS>(expression);
            return (TS)res;
        }

        public override async Task<TS> ExecuteExtensionAsync<TS>(Expression expression, object[] param, CancellationToken cancellationToken)
        {
            var tk = new TaskCompletionSource<TS>(TaskCreationOptions.RunContinuationsAsynchronously);
            if (param != null && param.Length > 0)
            {
                _paramFree.AddRange(param);
            }
            var r = await ExecuteAsync<TS>(expression, param, cancellationToken);
            tk.SetResult((TS)r);
            return await tk.Task;
        }


        public override async Task<List<TS>> ExecuteToListAsync<TS>(Expression expression, CancellationToken cancellationToken)
        {

            var tk = new TaskCompletionSource<List<TS>>(TaskCreationOptions.RunContinuationsAsynchronously);


            object rr = await ExecuteAsync<TS>(expression, null, cancellationToken);
            tk.SetResult((List<TS>)rr);
            return await tk.Task;

        }

        public override async Task<TS[]> ExecuteToArray<TS>(Expression expression, CancellationToken cancellationToken)
        {

            object rr = await ExecuteAsync<TS>(expression, null, cancellationToken);
             return ((IEnumerable<TS>)rr).ToArray();
          

        }

        private MyTuple TranslateE(Expression expression)
        {
            ITranslate sq = new QueryTranslator<T>(_providerName);
            _param = sq.Param;
            ListCastExpression.ForEach(a => sq.Translate(a.CustomExpression, a.TypeEvolution, a.ParamList));
            string res = sq.Translate(expression, out _);
            _param = sq.Param;
            var sdd = sq.GetListOne();
            Thread.MemoryBarrier();
            var ss = sq.GetListPostExpression();
            return new MyTuple { Sql = res, Composites = sdd, Param = sq.Param ,ListPostExpression=sq.GetListPostExpression() };
        }

        private string TranslateString(Expression expression)
        {
            ITranslate sq = new QueryTranslator<T>(_providerName);
            _param = sq.Param;
            ListCastExpression.ForEach(a => sq.Translate(a.CustomExpression, a.TypeEvolution, a.ParamList));
            string res = sq.Translate(expression, out _);
            return res;
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
            var res = (IEnumerable<TS>)ExecuteSpp<TS>(callExpr);

            foreach (var re in _parOut)
            {
                if (par == null) continue;
                var p = par.FirstOrDefault(a => a.Name == re.Key);
                if (p != null) p.Value = re.Value;
            }
            return res;
        }
    }

    internal class MyTuple
    {
        public string Sql { get; set; }
        public List<OneComposite> Composites { get; set; } = new List<OneComposite>();
        public Dictionary<string, object> Param { get; set; }
        public List<PostExpression> ListPostExpression { get; set; }
    }
}