namespace ORM_1_21_.Transaction
{
    /// <summary>
    /// Транзакция
    /// </summary>
    public interface ITransaction
    {
        /// <summary>
        /// Завершить транзакцию
        /// </summary>
        void Commit();
        /// <summary>
        /// Откатить транзакцию
        /// </summary>
        void Rollback();
    }
}
