using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ORM_1_21_
{
    static class StorageOtherBaseFactory<T>
    {
        private static readonly Type Type;
         static StorageOtherBaseFactory()
         {
             Type = typeof(T);
         }
         private static readonly Lazy<IOtherDataBaseFactory> DbProxyLazy = 
             new Lazy<IOtherDataBaseFactory>(() => (IOtherDataBaseFactory)Activator.CreateInstance(Type));
         private static readonly Lazy<DbProviderFactory> DbProviderFactory = 
             new Lazy<DbProviderFactory>(() => DbProxyLazy.Value.GetDbProviderFactories(),LazyThreadSafetyMode.PublicationOnly);


       internal static IOtherDataBaseFactory GetDataBaseFactory()
       {
           return new MyTempOtherFactory();
       }
      
       class MyTempOtherFactory : IOtherDataBaseFactory
       {
           public ProviderName GetProviderName()
           {
               return DbProxyLazy.Value.GetProviderName();
           }
      
           public DbProviderFactory GetDbProviderFactories()
           {
               return DbProviderFactory.Value;
           }
      
           public string GetConnectionString()
           {
               return DbProxyLazy.Value.GetConnectionString();
           }
       }
    }

   
}
