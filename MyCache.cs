//using System.Collections.Concurrent;
//
//namespace ORM_1_21_
//{
//    static class MyCache<TS>
//    {
//        internal static readonly ConcurrentDictionary<int, object> Dic = new ConcurrentDictionary<int, object>();
//
//        public static void Push(int key, object value)
//        {
//            Dic.GetOrAdd(key, value);
//        }
//
//        public static object GetValue(int key)
//        {
//            if (Dic.ContainsKey(key) == false) return null;
//            return Dic[key];
//        }
//
//        public static void Clear()
//        {
//            Dic.Clear();
//        }
//
//        public static int DeleteKey(int key)
//        {
//            if (Dic.TryRemove(key, out _))
//            {
//
//                return 1;
//            }
//            return 0;
//        }
//    }
//}