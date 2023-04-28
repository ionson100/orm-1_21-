using ORM_1_21_.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ORM_1_21_.Linq
{
    interface ICloneSession
    {
        Session CloneSession();
    }
    internal partial class DbQueryProvider<T> : QueryProvider, ISqlComposite, ICloneSession
    {
        private readonly List<object> _paramFree = new List<object>();
        private readonly List<ParameterStoredPr> _paramFreeStoredPr = new List<ParameterStoredPr>();
        private readonly Dictionary<string, object> _parOut = new Dictionary<string, object>();
        private readonly Session _session;
        //private IDbCommand _com;
        private readonly ProviderName _providerName;
        public IDbTransaction Transaction { get; set; }
        public List<ContainerCastExpression> ListCastExpression { get; set; } = new List<ContainerCastExpression>();
        private bool _isStoredPr;
        private Dictionary<string, object> _param;
        public ISession Sessione { get; }
        public Type GetSourceType()
        {
            return typeof(T);
        }

        public Session GeSessione()
        {
            return _session;
        }

        public DbQueryProvider<T> GetDbQueryProvider()
        {
            return this;
        }
        public DbQueryProvider(Session ses)
        {
            _session = ses;
            Sessione = ses;
            _providerName = ses.MyProviderName;
        }







        private static bool PingCompositeE(Evolution eval, List<OneComposite> list)
        {
            return list.Any(a => a.Operand == eval);
        }

        public override string GetQueryText(Expression expression)
        {
            return TranslateString(expression);
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

            if (param != null && param.Length > 0)
            {
                _paramFree.AddRange(param);
            }
            var r = await ExecuteAsync<TS>(expression, param, cancellationToken);
            return (TS)r;

        }


        public override async Task<List<TS>> ExecuteToListAsync<TS>(Expression expression, CancellationToken cancellationToken)
        {
            object rr = await ExecuteAsync<TS>(expression, null, cancellationToken);
            return (List<TS>)rr;
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
            return new MyTuple { Sql = res, Composites = sdd, Param = sq.Param, ListPostExpression = sq.GetListPostExpression() };
        }

        private string TranslateString(Expression expression)
        {
            ITranslate sq = new QueryTranslator<T>(_providerName);
            _param = sq.Param;
            ListCastExpression.ForEach(a => sq.Translate(a.CustomExpression, a.TypeEvolution, a.ParamList));
            string res = sq.Translate(expression, out _);
            return res;
        }

        public object ExecuteCall<TS>(Expression callExpr)
        {
            _isStoredPr = true;
            return Execute<TS>(callExpr);
        }
        public async Task<object> ExecuteCallAsync<TS>(Expression callExpr,CancellationToken cancellationToken)
        {
            _isStoredPr = true;

            return await  ExecuteAsync<TS> (callExpr,null,cancellationToken);
        }

        public object ExecuteCallParam<TS>(Expression callExpr, params ParameterStoredPr[] par)
        {
            _isStoredPr = true;
            if (par != null) _paramFreeStoredPr.AddRange(par);
            var res = ExecuteSpp<TS>(callExpr);

            foreach (var re in _parOut)
            {
                if (par == null) continue;
                var p = par.FirstOrDefault(a => a.Name == re.Key);
                if (p != null) p.Value = re.Value;
            }
            return res;
        }
        public async Task<object> ExecuteCallParamAsync<TS>(Expression callExpr,  ParameterStoredPr[] par,CancellationToken cancellationToken)
        {
            _isStoredPr = true;
            if (par != null) _paramFreeStoredPr.AddRange(par);
            var res = await ExecuteSppAsync<TS>(callExpr,cancellationToken);

            foreach (var re in _parOut)
            {
                if (par == null) continue;
                var p = par.FirstOrDefault(a => a.Name == re.Key);
                if (p != null) p.Value = re.Value;
            }

            return res;
        }

        public Session CloneSession()
        {
            return (Session)_session.GetCloneSession();
        }




    }

    class DbHelp
    {
        public static object CastList(List<object> list)
        {
            MethodInfo method = typeof(DbHelp).GetMethod("CloneListAs");
            MethodInfo genericMethod = method.MakeGenericMethod(list[0].GetType());

            return genericMethod.Invoke(null, new object[] { list });
        }
        public static List<TC> CloneListAs<TC>(IList<object> source)
        {
            return source.Cast<TC>().ToList();
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