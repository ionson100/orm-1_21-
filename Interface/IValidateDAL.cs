namespace ORM_1_21_
{

    /// <summary>
    /// Tracking the modification  of an object in the database
    /// </summary>
    /// <typeparam name="T">Type object</typeparam>
    public interface IMapAction<in T> where T : class
    {
        /// <summary>
        /// Modification event
        /// </summary>
        /// <param name="item">Modification object</param>
        /// <param name="mode">Modification type</param>
        void ActionCommand(T item, CommandMode mode);
    }


}