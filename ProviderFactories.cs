using System;
using System.Data;
using System.Data.Common;


namespace ORM_1_21_
{
    internal static class ProviderFactories
    {
        private static object _l = new object();
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

        private static object o = new object();
        public static IDbCommand GetCommand(IOtherDataBaseFactory factory, bool isDispose)
        {
            if (isDispose)
            {
                throw new Exception("The session has been destroyed, it is impossible to work with the session! (Dispose)");
            }

            if (factory != null)
            {
                lock (o)
                {
                    return factory.GetDbProviderFactories().CreateCommand();
                }

            }
            return GetFactory().CreateCommand();
        }

        public static IDbDataAdapter GetDataAdapter(IOtherDataBaseFactory factory)
        {
            return GetFactory().CreateDataAdapter();

        }


    }
}