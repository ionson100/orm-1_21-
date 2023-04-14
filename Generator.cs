namespace ORM_1_21_
{
    /// <summary>
    /// Table primary key generator type
    /// </summary>
    public enum Generator
    {
        /// <summary>
        /// Database assignable as auto increment field
        /// </summary>
        Native = 0,
        /// <summary>
        /// User assignable
        /// </summary>
        Assigned,

        /// <summary>
        /// Database assignable as auto generate,importance of the key does not return on client for insert command
        /// </summary>
        NativeNotReturningId,
    }
}