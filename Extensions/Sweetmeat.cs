using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
            //var pFirst = (QueryProvider)_first.Provider;
            //var pSecond = new DbQueryProvider<TSecond>(((ICloneSession)_second.Provider).CloneSession());
            //var t1 = pFirst.ExecuteExtensionAsync<IEnumerable<TFirst>>(_first.Expression, null, CancellationToken.None);
            //var t2 = pSecond.ExecuteExtensionAsync<IEnumerable<TSecond>>(_second.Expression, null, CancellationToken.None);
            //Task.WaitAll(t1, t2);
            First = _first.Provider.Execute<IEnumerable<TFirst>>(_first.Expression);
            Seconds = _second.Provider.Execute<IEnumerable<TSecond>>(_second.Expression);
        }
        public async Task WaitAsync()
        {
            //var pFirst = (QueryProvider)_first.Provider;
            //var pSecond = new DbQueryProvider<TSecond>(((ICloneSession)_second.Provider).CloneSession());
            //var t1 = pFirst.ExecuteExtensionAsync<IEnumerable<TFirst>>(_first.Expression, null, _cancellationToken);
            //var t2 = pSecond.ExecuteExtensionAsync<IEnumerable<TSecond>>(_second.Expression, null, _cancellationToken);
            //await Task.WhenAll(t1, t2).ConfigureAwait(false);
            First = await _first.Provider.ExecuteAsync<IEnumerable<TFirst>>(_first.Expression,_cancellationToken);
            Seconds = await _second.Provider.ExecuteAsync<IEnumerable<TSecond>>(_second.Expression,_cancellationToken);
        }

    }
}