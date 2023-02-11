namespace ORM_1_21_
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
