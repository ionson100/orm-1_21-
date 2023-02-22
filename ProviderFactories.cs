using System;
using System.Data;
using System.Data.Common;


namespace ORM_1_21_
{
    internal static class ProviderFactories
    {

        static DbProviderFactory GetFactory()
        {

            return Configure.CurFactory;
           
        }
        public static IDbConnection GetConnect(IOtherDataBaseFactory factory)
        {
            if (factory != null)
            {
                return factory.GetDbProviderFactories().CreateConnection();
            }
            return GetFactory().CreateConnection();

        }

        public static IDbCommand GetCommand(IOtherDataBaseFactory factory, bool isDispose)
        {
            if (isDispose == true)
            {
                throw new Exception("The session has been destroyed, it is impossible to work with the session! (Dispose)");
            }
            if (factory != null)
            {
                return factory.GetDbProviderFactories().CreateCommand();
            }
            return GetFactory().CreateCommand();
        }

        public static IDbDataAdapter GetDataAdapter(IOtherDataBaseFactory factory)
        {
            return GetFactory().CreateDataAdapter();

        }


    }
}