using System;
using System.Collections.Generic;
using System.Data;

namespace ORM_1_21_.Transaction
{
   
    internal class Transactionale : ITransaction
    {
        IDbConnection _connection;
        List<IDisposable> _listDispose = new List<IDisposable>();

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
            MyStateTransaction = StateTransaction.Commit;
            Transaction?.Commit();
            //Transaction = null;
            InnerTransaction();
        }

        public void Rollback()
        {
            MyStateTransaction = StateTransaction.Rollback;
            Transaction?.Rollback();
            _connection.Close();
            InnerTransaction();
        }

        #endregion ITransaction Members

        private void InnerTransaction()
        {
            //IsOccupied = false;
            if (_connection?.State == ConnectionState.Open)
            {
                _connection.Close();
            }
            _listDispose.ForEach(a => a.Dispose());
            _listDispose.Clear();
        }
    }
    internal enum StateTransaction
    {
        None,Begin,Commit,Rollback
    }
}