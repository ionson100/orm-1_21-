using System;

namespace ORM_1_21_
{
    /// <summary>
    /// Transaction in the context of the session
    /// </summary>
    public interface ITransaction:IDisposable
    {
        /// <summary>
        /// Commit transaction
        /// </summary>
        void Commit();
        /// <summary>
        /// Rollback transaction
        /// </summary>
        void Rollback();
    }
}
