using System;
using System.Collections.Generic;
using System.Linq;

namespace ORM_1_21_.Linq
{
    internal static class TypeSystem
    {
        internal static Type GetElementType(Type seqType)
        {
            var iEnum = FindIEnumerable(seqType);
            return iEnum == null ? seqType : iEnum.GetGenericArguments()[0];
        }

        private static Type FindIEnumerable(Type seqType)
        {
            if (seqType == null || seqType == typeof(string))
                return null;
            if (seqType.IsArray)
                return typeof(IEnumerable<>).MakeGenericType(seqType.GetElementType());
            if (seqType.IsGenericType)
            {
                foreach (var type in seqType.GetGenericArguments().Select(arg => typeof(IEnumerable<>).MakeGenericType(arg)).Where(im => im.IsAssignableFrom(seqType)))
                {
                    return type;
                }
            }
            var iFaces = seqType.GetInterfaces();
            if (iFaces.Length > 0)
            {
                foreach (var type in iFaces.Select(FindIEnumerable).Where(im => im != null))
                {
                    return type;
                }
            }
            if (seqType.BaseType != null && seqType.BaseType != typeof(object))
            {
                return FindIEnumerable(seqType.BaseType);
            }
            return null;
        }
    }
}
