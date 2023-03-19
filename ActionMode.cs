using System.Diagnostics.Tracing;

namespace ORM_1_21_
{
    /// <summary>
    /// Type of modification in the database
    /// </summary>
    internal enum ActionMode
    {
        /// <summary>
        /// None
        /// </summary>
        None = 0,
        /// <summary>
        /// Insert
        /// </summary>
        Insert = 1,
        /// <summary>
        /// Update
        /// </summary>
        Update = 2,
        /// <summary>
        /// Delete
        /// </summary>
        Delete = 3
    }

    /// <summary>
    /// Modification type
    /// </summary>
    public enum CommandMode
    {
        None,
        /// <summary>
        /// Before Update
        /// </summary>
        BeforeUpdate,
        /// <summary>
        /// Before Insert
        /// </summary>
        BeforeInsert,
        /// <summary>
        /// Before Delete
        /// </summary>
        BeforeDelete,
        /// <summary>
        /// After Update
        /// </summary>
        AfterUpdate,
        /// <summary>
        /// After Insert
        /// </summary>
        AfterInsert,
        /// <summary>
        /// After Delete
        /// </summary>
        AfterDelete,

    }
}