namespace ORM_1_21_
{
    /// <summary>
    /// Operations control for working with the database
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IActionDal<in T> where T : class
    {
        /// <summary>
        /// Call before insert
        /// </summary>
        void BeforeInsert(T item);

        /// <summary>
        /// Call after insert
        /// </summary>
        void AfterInsert(T item);

        /// <summary>
        /// Call before update
        /// </summary>
        void BeforeUpdate(T item);

        /// <summary>
        /// Call after update
        /// </summary>
        void AfterUpdate(T item);

        /// <summary>
        /// Call before delete
        /// </summary>
        void BeforeDelete(T item);

        /// <summary>
        /// Call after delete
        /// </summary>
        void AfterDelete(T item);
    }
}