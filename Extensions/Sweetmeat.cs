using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ORM_1_21_.Linq;

namespace ORM_1_21_.Extensions
{
    class Sweetmeat<TFirst,TSecond>
    {
        private readonly IQueryable<TFirst> _first;
        private readonly IQueryable<TSecond> _second;
        private readonly CancellationToken _cancellationToken;
        public IEnumerable<TFirst> First { get; private set; }
        public IEnumerable<TSecond> Seconds { get; private set; }


        public Sweetmeat(IQueryable<TFirst> first, IQueryable<TSecond> second,CancellationToken cancellationToken=default)
        {
            _first = first;
            _second = second;
            _cancellationToken = cancellationToken;
        }


        public void Wait()
        {
            var pTOuter = (QueryProvider)_first.Provider;
            var pTInner = new DbQueryProvider<TSecond>((Sessione)((ISqlComposite)_second.Provider).SessioneInner.SessionCloneForTask());
            var t1 = pTOuter.ExecuteExtensionAsync<IEnumerable<TFirst>>(_first.Expression, null, CancellationToken.None);
            var t2 = pTInner.ExecuteExtensionAsync<IEnumerable<TSecond>>(_second.Expression, null, CancellationToken.None);
            Task.WaitAll(t1, t2);
            First = t1.Result;
            Seconds = t2.Result;
        }
        public async Task WaitAsync()
        {
            var pTOuter = (QueryProvider)_first.Provider;
            var pTInner = new DbQueryProvider<TSecond>((Sessione)((ISqlComposite)_second.Provider).SessioneInner.SessionCloneForTask());
            var t1 = pTOuter.ExecuteExtensionAsync<IEnumerable<TFirst>>(_first.Expression, null, _cancellationToken);
            var t2 = pTInner.ExecuteExtensionAsync<IEnumerable<TSecond>>(_second.Expression, null, _cancellationToken);
            await Task.WhenAll(t1, t2).ConfigureAwait(false);
            First = t1.Result;
            Seconds = t2.Result;
        }

    }
}