using System;

namespace ORM_1_21_.Extensions
{
    internal class IdentityFunction<T>
    {
        public static Func<T, T> Instance => x => x;
    }
}