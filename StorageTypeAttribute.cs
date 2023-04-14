using System;
using System.Collections.Concurrent;

namespace ORM_1_21_
{
    internal class StorageTypeAttribute
    {
        public static ConcurrentDictionary<Type, IProxy> DictionaryAttribute = new ConcurrentDictionary<Type, IProxy>();
    }
}