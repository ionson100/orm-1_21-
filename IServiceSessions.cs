using System.Data;


namespace ORM_1_21_
{
    interface IServiceSessions
    {
        IDbCommand CommandForLinq { get; }
        ProviderName CurrentProviderName { get; }
 
        object Locker { get; }

    }
}
