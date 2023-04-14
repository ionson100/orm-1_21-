using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace ORM_1_21_
{
   
    internal class Transactionale : ITransaction
    {
        IDbConnection _connection;
        List<IDisposable> _listDispose = new List<IDisposable>();
        public bool isError;

        public List<IDisposable> ListDispose
        {
            get { return _listDispose; }
            set { _listDispose = value; }
        }

        public IDbConnection Connection
        {
            set { _connection = value; }
        }

        public Transactionale()
        {
            
        }
        
        public IsolationLevel? IsolationLevel { get; set; }

        public IDbTransaction Transaction { get; set; }

        //public bool IsOccupied { get; set; }
        public StateTransaction MyStateTransaction { get; set; }

        #region ITransaction Members

        public void Commit()
        {
            NewMethodExceptionTransaction(StateTransaction.Commit);
            MyStateTransaction = StateTransaction.Commit;
            Transaction?.Commit();
            InnerTransaction();
            //isError = false;
        }
        public async Task CommitAsync()
        {
            NewMethodExceptionTransaction(StateTransaction.Commit);

            MyStateTransaction = StateTransaction.Commit;
            if (Transaction != null)
            {
                await Transaction.CommitAsync();

            }
            await InnerTransactionAsync();
            //isError = false;
        }

        public void Rollback()
        {
            NewMethodExceptionTransaction(StateTransaction.Rollback);
            MyStateTransaction = StateTransaction.Rollback;
            Transaction?.Rollback();
            InnerTransaction();
            //isError = false;
        }

        private void NewMethodExceptionTransaction(StateTransaction currenTransaction)
        {
            if (MyStateTransaction != StateTransaction.Begin)
            {
                throw new Exception(
                    $"The transaction state is {MyStateTransaction}, which means that the transaction is used. " +
                    $"Possible you try more one times change transactions");
            }
        }

        public async Task RollbackAsync()
        {
            NewMethodExceptionTransaction(StateTransaction.Rollback);
            MyStateTransaction = StateTransaction.Rollback;
            if (Transaction != null)
            {
               await Transaction.RollbackAsync();
            }
            await InnerTransactionAsync();
            //isError = false;
        }

        #endregion ITransaction Members

        private void InnerTransaction()
        {
            if (_connection?.State == ConnectionState.Open)
            {
                _connection.Close();
            }
            _listDispose.ForEach(a => a.Dispose());
            _listDispose.Clear();
        }
        private async Task InnerTransactionAsync()
        {
            if (_connection!=null&&_connection?.State == ConnectionState.Open)
            {
                await _connection.CloseAsync();
            }

            foreach (IDisposable disposable in _listDispose)
            {
                 await disposable.DisposeAsync();
            }
            
            _listDispose.Clear();
        }

        public void Dispose()
        {
            if (MyStateTransaction == StateTransaction.Begin)
            {
                if (isError)
                {
                    Rollback();
                }
                else
                {
                    Commit();
                }
            }
            Transaction?.Dispose();
            isError = false;
        }
    }
    internal enum StateTransaction
    {
        None,Begin,Commit,Rollback
    }
}