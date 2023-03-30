using ORM_1_21_.Linq;
using ORM_1_21_.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ORM_1_21_.Extensions
{
    public static partial class Helper
    {
        #region MethodInfo




        private static readonly MethodInfo _first = GetMethod("First", (Type T) => new Type[1] { typeof(IQueryable<>).MakeGenericType(T) });

        private static readonly MethodInfo _first_Predicate = GetMethod("First", (Type T) => new Type[2]
        {
            typeof(IQueryable<>).MakeGenericType(T),
            typeof(Expression<>).MakeGenericType(typeof(Func<, >).MakeGenericType(T, typeof(bool)))
        });

        private static readonly MethodInfo _firstOrDefault = GetMethod("FirstOrDefault", (Type T) => new Type[1] { typeof(IQueryable<>).MakeGenericType(T) });

        private static readonly MethodInfo _firstOrDefault_Predicate = GetMethod("FirstOrDefault", (Type T) => new Type[2]
        {
            typeof(IQueryable<>).MakeGenericType(T),
            typeof(Expression<>).MakeGenericType(typeof(Func<, >).MakeGenericType(T, typeof(bool)))
        });

        private static readonly MethodInfo _last = GetMethod("Last", (Type T) => new Type[1] { typeof(IQueryable<>).MakeGenericType(T) });

        private static readonly MethodInfo _last_Predicate = GetMethod("Last", (Type T) => new Type[2]
        {
            typeof(IQueryable<>).MakeGenericType(T),
            typeof(Expression<>).MakeGenericType(typeof(Func<, >).MakeGenericType(T, typeof(bool)))
        });

        private static readonly MethodInfo _lastOrDefault = GetMethod("LastOrDefault", (Type T) => new Type[1] { typeof(IQueryable<>).MakeGenericType(T) });

        private static readonly MethodInfo _lastOrDefault_Predicate = GetMethod("LastOrDefault", (Type T) => new Type[2]
        {
            typeof(IQueryable<>).MakeGenericType(T),
            typeof(Expression<>).MakeGenericType(typeof(Func<, >).MakeGenericType(T, typeof(bool)))
        });

        private static readonly MethodInfo _single = GetMethod("Single", (Type T) => new Type[1] { typeof(IQueryable<>).MakeGenericType(T) });

        private static readonly MethodInfo _single_Predicate = GetMethod("Single", (Type T) => new Type[2]
        {
            typeof(IQueryable<>).MakeGenericType(T),
            typeof(Expression<>).MakeGenericType(typeof(Func<, >).MakeGenericType(T, typeof(bool)))
        });

        private static readonly MethodInfo _singleOrDefault = GetMethod("SingleOrDefault", (Type T) => new Type[1] { typeof(IQueryable<>).MakeGenericType(T) });

        private static readonly MethodInfo _singleOrDefault_Predicate = GetMethod("SingleOrDefault", (Type T) => new Type[2]
        {
            typeof(IQueryable<>).MakeGenericType(T),
            typeof(Expression<>).MakeGenericType(typeof(Func<, >).MakeGenericType(T, typeof(bool)))
        });

        private static readonly MethodInfo _contains = GetMethod("Contains", (Type T) => new Type[2]
        {
            typeof(IQueryable<>).MakeGenericType(T),
            T
        });

        private static readonly MethodInfo _any = GetMethod("Any", (Type T) => new Type[1] { typeof(IQueryable<>).MakeGenericType(T) });

        private static readonly MethodInfo _any_Predicate = GetMethod("Any", (Type T) => new Type[2]
        {
            typeof(IQueryable<>).MakeGenericType(T),
            typeof(Expression<>).MakeGenericType(typeof(Func<, >).MakeGenericType(T, typeof(bool)))
        });

        private static readonly MethodInfo _all_Predicate = GetMethod("All", (Type T) => new Type[2]
        {
            typeof(IQueryable<>).MakeGenericType(T),
            typeof(Expression<>).MakeGenericType(typeof(Func<, >).MakeGenericType(T, typeof(bool)))
        });

        private static readonly MethodInfo _count = GetMethod("Count", (Type T) => new Type[1] { typeof(IQueryable<>).MakeGenericType(T) });

        private static readonly MethodInfo _count_Predicate = GetMethod("Count", (Type T) => new Type[2]
        {
            typeof(IQueryable<>).MakeGenericType(T),
            typeof(Expression<>).MakeGenericType(typeof(Func<, >).MakeGenericType(T, typeof(bool)))
        });

        private static readonly MethodInfo _longCount = GetMethod("LongCount", (Type T) => new Type[1] { typeof(IQueryable<>).MakeGenericType(T) });

        private static readonly MethodInfo _longCount_Predicate = GetMethod("LongCount", (Type T) => new Type[2]
        {
            typeof(IQueryable<>).MakeGenericType(T),
            typeof(Expression<>).MakeGenericType(typeof(Func<, >).MakeGenericType(T, typeof(bool)))
        });

        private static readonly MethodInfo _min = GetMethod("Min", (Type T) => new Type[1] { typeof(IQueryable<>).MakeGenericType(T) });

        private static readonly MethodInfo _min_Selector = GetMethod("Min", (Type T, Type U) => new Type[2]
        {
            typeof(IQueryable<>).MakeGenericType(T),
            typeof(Expression<>).MakeGenericType(typeof(Func<, >).MakeGenericType(T, U))
        });

        private static readonly MethodInfo _max = GetMethod("Max", (Type T) => new Type[1] { typeof(IQueryable<>).MakeGenericType(T) });

        private static readonly MethodInfo _max_Selector = GetMethod("Max", (Type T, Type U) => new Type[2]
        {
            typeof(IQueryable<>).MakeGenericType(T),
            typeof(Expression<>).MakeGenericType(typeof(Func<, >).MakeGenericType(T, U))
        });


        private static readonly MethodInfo _sum_Int_Selector = GetMethod("Sum", (Type T) => new Type[2]
        {
            typeof(IQueryable<>).MakeGenericType(T),
            typeof(Expression<>).MakeGenericType(typeof(Func<, >).MakeGenericType(T, typeof(int)))
        });

        private static readonly MethodInfo _sum_IntNullable_Selector = GetMethod("Sum", (Type T) => new Type[2]
        {
            typeof(IQueryable<>).MakeGenericType(T),
            typeof(Expression<>).MakeGenericType(typeof(Func<, >).MakeGenericType(T, typeof(int?)))
        });

        private static readonly MethodInfo _sum_Long_Selector = GetMethod("Sum", (Type T) => new Type[2]
        {
            typeof(IQueryable<>).MakeGenericType(T),
            typeof(Expression<>).MakeGenericType(typeof(Func<, >).MakeGenericType(T, typeof(long)))
        });

        private static readonly MethodInfo _sum_LongNullable_Selector = GetMethod("Sum", (Type T) => new Type[2]
        {
            typeof(IQueryable<>).MakeGenericType(T),
            typeof(Expression<>).MakeGenericType(typeof(Func<, >).MakeGenericType(T, typeof(long?)))
        });

        private static readonly MethodInfo _sum_Float_Selector = GetMethod("Sum", (Type T) => new Type[2]
        {
            typeof(IQueryable<>).MakeGenericType(T),
            typeof(Expression<>).MakeGenericType(typeof(Func<, >).MakeGenericType(T, typeof(float)))
        });

        private static readonly MethodInfo _sum_FloatNullable_Selector = GetMethod("Sum", (Type T) => new Type[2]
        {
            typeof(IQueryable<>).MakeGenericType(T),
            typeof(Expression<>).MakeGenericType(typeof(Func<, >).MakeGenericType(T, typeof(float?)))
        });

        private static readonly MethodInfo _sum_Double_Selector = GetMethod("Sum", (Type T) => new Type[2]
        {
            typeof(IQueryable<>).MakeGenericType(T),
            typeof(Expression<>).MakeGenericType(typeof(Func<, >).MakeGenericType(T, typeof(double)))
        });

        private static readonly MethodInfo _sum_DoubleNullable_Selector = GetMethod("Sum", (Type T) => new Type[2]
        {
            typeof(IQueryable<>).MakeGenericType(T),
            typeof(Expression<>).MakeGenericType(typeof(Func<, >).MakeGenericType(T, typeof(double?)))
        });

        private static readonly MethodInfo _sum_Decimal_Selector = GetMethod("Sum", (Type T) => new Type[2]
        {
            typeof(IQueryable<>).MakeGenericType(T),
            typeof(Expression<>).MakeGenericType(typeof(Func<, >).MakeGenericType(T, typeof(decimal)))
        });

        private static readonly MethodInfo _sum_DecimalNullable_Selector = GetMethod("Sum", (Type T) => new Type[2]
        {
            typeof(IQueryable<>).MakeGenericType(T),
            typeof(Expression<>).MakeGenericType(typeof(Func<, >).MakeGenericType(T, typeof(decimal?)))
        });

        

        private static readonly MethodInfo _average_Int_Selector = GetMethod("Average", (Type T) => new Type[2]
        {
            typeof(IQueryable<>).MakeGenericType(T),
            typeof(Expression<>).MakeGenericType(typeof(Func<, >).MakeGenericType(T, typeof(int)))
        });

        private static readonly MethodInfo _average_IntNullable_Selector = GetMethod("Average", (Type T) => new Type[2]
        {
            typeof(IQueryable<>).MakeGenericType(T),
            typeof(Expression<>).MakeGenericType(typeof(Func<, >).MakeGenericType(T, typeof(int?)))
        });

        private static readonly MethodInfo _average_Long_Selector = GetMethod("Average", (Type T) => new Type[2]
        {
            typeof(IQueryable<>).MakeGenericType(T),
            typeof(Expression<>).MakeGenericType(typeof(Func<, >).MakeGenericType(T, typeof(long)))
        });

        private static readonly MethodInfo _average_LongNullable_Selector = GetMethod("Average", (Type T) => new Type[2]
        {
            typeof(IQueryable<>).MakeGenericType(T),
            typeof(Expression<>).MakeGenericType(typeof(Func<, >).MakeGenericType(T, typeof(long?)))
        });

        private static readonly MethodInfo _average_Float_Selector = GetMethod("Average", (Type T) => new Type[2]
        {
            typeof(IQueryable<>).MakeGenericType(T),
            typeof(Expression<>).MakeGenericType(typeof(Func<, >).MakeGenericType(T, typeof(float)))
        });

        private static readonly MethodInfo _average_FloatNullable_Selector = GetMethod("Average", (Type T) => new Type[2]
        {
            typeof(IQueryable<>).MakeGenericType(T),
            typeof(Expression<>).MakeGenericType(typeof(Func<, >).MakeGenericType(T, typeof(float?)))
        });

        private static readonly MethodInfo _average_Double_Selector = GetMethod("Average", (Type T) => new Type[2]
        {
            typeof(IQueryable<>).MakeGenericType(T),
            typeof(Expression<>).MakeGenericType(typeof(Func<, >).MakeGenericType(T, typeof(double)))
        });

        private static readonly MethodInfo _average_DoubleNullable_Selector = GetMethod("Average", (Type T) => new Type[2]
        {
            typeof(IQueryable<>).MakeGenericType(T),
            typeof(Expression<>).MakeGenericType(typeof(Func<, >).MakeGenericType(T, typeof(double?)))
        });

        private static readonly MethodInfo _average_Decimal_Selector = GetMethod("Average", (Type T) => new Type[2]
        {
            typeof(IQueryable<>).MakeGenericType(T),
            typeof(Expression<>).MakeGenericType(typeof(Func<, >).MakeGenericType(T, typeof(decimal)))
        });

        private static readonly MethodInfo _average_DecimalNullable_Selector = GetMethod("Average", (Type T) => new Type[2]
        {
            typeof(IQueryable<>).MakeGenericType(T),
            typeof(Expression<>).MakeGenericType(typeof(Func<, >).MakeGenericType(T, typeof(decimal?)))
        });

        private static readonly MethodInfo _skip = GetMethod("Skip", (Type T) => new Type[2]
        {
            typeof(IQueryable<>).MakeGenericType(T),
            typeof(int)
        });

        private static readonly MethodInfo _take = GetMethod("Take", (Type T) => new Type[2]
        {
            typeof(IQueryable<>).MakeGenericType(T),
            typeof(int)
        });
        private static MethodInfo GetMethod(string methodName, Func<Type[]> getParameterTypes)
        {
            return GetMethod(methodName, getParameterTypes, 0);
        }

        private static MethodInfo GetMethod(string methodName, Func<Type, Type, Type[]> getParameterTypes)
        {
            return GetMethod(methodName, getParameterTypes, 2);
        }

        private static MethodInfo GetMethod(string methodName, Func<Type, Type[]> getParameterTypes)
        {
            return GetMethod(methodName, getParameterTypes, 1);
        }


        private static MethodInfo GetMethod(string methodName, Delegate getParameterTypesDelegate, int genericArgumentsCount)
        {
            foreach (MethodInfo declaredMethod in typeof(Queryable).GetDeclaredMethods(methodName))
            {
                Type[] genericArguments = declaredMethod.GetGenericArguments();
                if (genericArguments.Length == genericArgumentsCount)
                {
                    object[] args = genericArguments;
                    if (Matches(declaredMethod, (Type[])getParameterTypesDelegate.DynamicInvoke(args)))
                    {
                        return declaredMethod;
                    }
                }
            }

            return null;
        }
        internal static IEnumerable<MethodInfo> GetDeclaredMethods(this Type type, string name)
        {
            return type.GetTypeInfo().GetDeclaredMethods(name);
        }
        private static bool Matches(MethodInfo methodInfo, Type[] parameterTypes)
        {
            return (from p in methodInfo.GetParameters()
                    select p.ParameterType).SequenceEqual(parameterTypes);
        }
        #endregion



        /// <summary>
        /// Asynchronously returns the first element of a sequence that
        /// satisfies a specified condition.
        /// </summary>
        /// <param name="source">An System.Linq.IQueryable`1 to return the first element of.</param>
        /// <param name="cancellationToken">A System.Threading.CancellationToken to observe while waiting for the task to</param>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <returns>A task that represents the asynchronous operation. The task result contains the
        ///     first element in source that passes the test in predicate.</returns>
        public static Task<TSource> FirstAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, "source");
            var ex = Expression.Call(null,
                _first.MakeGenericMethod(typeof(TSource)), source.Expression);
            return ((QueryProvider)source.Provider).ExecuteExtensionAsync<TSource>(ex, null, cancellationToken);
        }

        /// <summary>
        /// Asynchronously returns the first element of a sequence that
        /// satisfies a specified condition.
        /// </summary>
        /// <param name="source">An System.Linq.IQueryable`1 to return the first element of.</param>
        /// <param name="predicate">A function to test each element for a condition</param>
        /// <param name="cancellationToken">A System.Threading.CancellationToken to observe while waiting for the task to</param>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <returns>A task that represents the asynchronous operation. The task result contains the
        ///     first element in source that passes the test in predicate.</returns>
        public static Task<TSource> FirstAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, "source");
            Check.NotNull(predicate, "predicate");
            var ex = Expression.Call(null, _first_Predicate.MakeGenericMethod(typeof(TSource)),
                source.Expression, predicate);
            return ((QueryProvider)source.Provider).ExecuteExtensionAsync<TSource>(ex, null, cancellationToken);
        }

        /// <summary>
        ///  Asynchronously returns the first element of a sequence, or a default value if
        ///     the sequence contains no elements.
        /// </summary>
        /// <param name="source">An System.Linq.IQueryable`1 to return the first element of.</param>
        /// <param name="cancellationToken"> A System.Threading.CancellationToken to observe while waiting for the task to
        ///     complete.</param>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <returns>A task that represents the asynchronous operation. The task result contains default
        ///     ( TSource ) if source is empty; otherwise, the first element in source.</returns>
        public static Task<TSource> FirstOrDefaultAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default)
        {
          
            Check.NotNull(source, "source");
            var ex = Expression.Call(null,
                _firstOrDefault.MakeGenericMethod(typeof(TSource)), source.Expression);
            return ((QueryProvider)source.Provider).ExecuteExtensionAsync<TSource>(ex, null, cancellationToken);
        }

        /// <summary>
        ///  Asynchronously returns the first element of a sequence, or a default value if
        ///     the sequence contains no elements.
        /// </summary>
        /// <param name="source">An System.Linq.IQueryable`1 to return the first element of.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="cancellationToken"> A System.Threading.CancellationToken to observe while waiting for the task to
        ///     complete.</param>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <returns>A task that represents the asynchronous operation. The task result contains default
        ///     ( TSource ) if source is empty; otherwise, the first element in source.</returns>
        public static Task<TSource> FirstOrDefaultAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, "source");
            Check.NotNull(predicate, "predicate");
            var ex = Expression.Call(null, _firstOrDefault_Predicate.MakeGenericMethod(typeof(TSource)),
                source.Expression, predicate);
            return ((QueryProvider)source.Provider).ExecuteExtensionAsync<TSource>(ex, null, cancellationToken);

        }


        /// <summary>
        /// Asynchronously returns the last element of a sequence that
        /// satisfies a specified condition.
        /// </summary>
        /// <param name="source">An System.Linq.IQueryable`1 to return the last element of.</param>
        /// <param name="cancellationToken">A System.Threading.CancellationToken to observe while waiting for the task to</param>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <returns>A task that represents the asynchronous operation. The task result contains the
        ///     last element in source that passes the test in predicate.</returns>
        public static Task<TSource> LastAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, "source");
            var ex = Expression.Call(null,
                _last.MakeGenericMethod(typeof(TSource)), source.Expression);
            return ((QueryProvider)source.Provider).ExecuteExtensionAsync<TSource>(ex, null, cancellationToken);
        }


        /// <summary>
        /// Asynchronously returns the last element of a sequence that
        /// satisfies a specified condition.
        /// </summary>
        /// <param name="source">An System.Linq.IQueryable`1 to return the last element of.</param>
        /// <param name="predicate">A function to test each element for a condition</param>
        /// <param name="cancellationToken">A System.Threading.CancellationToken to observe while waiting for the task to</param>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <returns>A task that represents the asynchronous operation. The task result contains the
        ///     last element in source that passes the test in predicate.</returns>
        public static Task<TSource> LastAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, "source");
            Check.NotNull(predicate, "predicate");
            var ex = Expression.Call(null, _last_Predicate.MakeGenericMethod(typeof(TSource)),
                source.Expression, predicate);
            return ((QueryProvider)source.Provider).ExecuteExtensionAsync<TSource>(ex, null, cancellationToken);

        }




        /// <summary>
        /// Asynchronously returns the last element of a sequence, or a default value if
        ///     the sequence contains no elements.
        /// </summary>
        /// <param name="source">An System.Linq.IQueryable`1 to return the last element of.</param>
        /// <param name="cancellationToken">A System.Threading.CancellationToken to observe while waiting for the task to
        ///     complete.</param>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <returns>A task that represents the asynchronous operation. The task result contains default
        ///     ( TSource ) if source is empty; otherwise, the last element in source.</returns>
        public static Task<TSource> LastOrDefaultAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, "source");
            var ex = Expression.Call(null,
                _lastOrDefault.MakeGenericMethod(typeof(TSource)), source.Expression);
            return ((QueryProvider)source.Provider).ExecuteExtensionAsync<TSource>(ex, null, cancellationToken);
        }


        /// <summary>
        /// Asynchronously returns the last element of a sequence, or a default value if
        ///     the sequence contains no elements.
        /// </summary>
        /// <param name="source">An System.Linq.IQueryable`1 to return the last element of.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="cancellationToken">A System.Threading.CancellationToken to observe while waiting for the task to
        ///     complete.</param>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <returns>A task that represents the asynchronous operation. The task result contains default
        ///     ( TSource ) if source is empty; otherwise, the last element in source.</returns>
        public static Task<TSource> LastOrDefaultAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, "source");
            Check.NotNull(predicate, "predicate");
            var ex = Expression.Call(null, _lastOrDefault_Predicate.MakeGenericMethod(typeof(TSource)),
                source.Expression, predicate);
            return ((QueryProvider)source.Provider).ExecuteExtensionAsync<TSource>(ex, null, cancellationToken);

        }


        /// <summary>
        ///  Asynchronously returns the only element of a sequence that satisfies a specified
        ///    condition, and throws an exception if more than one such element exists.
        /// </summary>
        /// <param name="source">An System.Linq.IQueryable`1 to return the single element of.</param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TSource"> A System.Threading.CancellationToken to observe while waiting for the task to
        ///     complete.</typeparam>
        /// <returns>A task that represents the asynchronous operation. The task result contains the
        ///    single element of the input sequence that satisfies the condition in predicate.</returns>
        public static Task<TSource> SingleAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, "source");
            var ex = Expression.Call(null,
                _single.MakeGenericMethod(typeof(TSource)), source.Expression);
            return ((QueryProvider)source.Provider).ExecuteExtensionAsync<TSource>(ex, null, cancellationToken);
        }


        /// <summary>
        ///  Asynchronously returns the only element of a sequence that satisfies a specified
        ///    condition, and throws an exception if more than one such element exists.
        /// </summary>
        /// <param name="source">An System.Linq.IQueryable`1 to return the single element of.</param>
        /// <param name="predicate">A function to test an element for a condition.</param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TSource"> A System.Threading.CancellationToken to observe while waiting for the task to
        ///     complete.</typeparam>
        /// <returns>A task that represents the asynchronous operation. The task result contains the
        ///    single element of the input sequence that satisfies the condition in predicate.</returns>
        public static Task<TSource> SingleAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, "source");
            Check.NotNull(predicate, "predicate");
            var ex = Expression.Call(null, _single_Predicate.MakeGenericMethod(typeof(TSource)),
                source.Expression, predicate);
            return ((QueryProvider)source.Provider).ExecuteExtensionAsync<TSource>(ex, null, cancellationToken);

        }

        /// <summary>
        /// Asynchronously returns the only element of a sequence that satisfies a specified
        ///     condition or a default value if no such element exists; this method throws an
        ///     exception if more than one element satisfies the condition.
        /// </summary>
        /// <param name="source">An System.Linq.IQueryable`1 to return the single element of.</param>
        /// <param name="cancellationToken">A System.Threading.CancellationToken to observe while waiting for the task to
        ///    complete.</param>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <returns>A task that represents the asynchronous operation. The task result contains the
        ///     single element of the input sequence that satisfies the condition in predicate,
        ///     or default ( TSource ) if no such element is found.</returns>
        public static Task<TSource> SingleOrDefaultAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, "source");
            var ex = Expression.Call(null,
                _singleOrDefault.MakeGenericMethod(typeof(TSource)), source.Expression);
            return ((QueryProvider)source.Provider).ExecuteExtensionAsync<TSource>(ex, null, cancellationToken);
        }


        /// <summary>
        /// Asynchronously returns the only element of a sequence that satisfies a specified
        ///     condition or a default value if no such element exists; this method throws an
        ///     exception if more than one element satisfies the condition.
        /// </summary>
        /// <param name="source">An System.Linq.IQueryable`1 to return the single element of.</param>
        /// <param name="predicate">A function to test an element for a condition.</param>
        /// <param name="cancellationToken">A System.Threading.CancellationToken to observe while waiting for the task to
        ///    complete.</param>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <returns>A task that represents the asynchronous operation. The task result contains the
        ///     single element of the input sequence that satisfies the condition in predicate,
        ///     or default ( TSource ) if no such element is found.</returns>
        public static Task<TSource> SingleOrDefaultAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, "source");
            Check.NotNull(predicate, "predicate");
            var ex = Expression.Call(null, _singleOrDefault_Predicate.MakeGenericMethod(typeof(TSource)),
                source.Expression, predicate);
            return ((QueryProvider)source.Provider).ExecuteExtensionAsync<TSource>(ex, null, cancellationToken);

        }



        /// <summary>
        ///  Asynchronously determines whether any element of a sequence satisfies a condition.
        /// </summary>
        /// <param name="source">An System.Linq.IQueryable`1 whose elements to test for a condition.</param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <returns>A task that represents the asynchronous operation. The task result contains true
        ///     if any elements in the source sequence pass the test in the specified predicate;
        ///     otherwise, false.</returns>
        public static Task<bool> AnyAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, "source");
            var ex = Expression.Call(null,
                _any.MakeGenericMethod(typeof(TSource)), source.Expression);
            return ((QueryProvider)source.Provider).ExecuteExtensionAsync<bool>(ex, null, cancellationToken);
        }


        /// <summary>
        ///  Asynchronously determines whether any element of a sequence satisfies a condition.
        /// </summary>
        /// <param name="source">An System.Linq.IQueryable`1 whose elements to test for a condition.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <returns>A task that represents the asynchronous operation. The task result contains true
        ///     if any elements in the source sequence pass the test in the specified predicate;
        ///     otherwise, false.</returns>
        public static Task<bool> AnyAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, "source");
            Check.NotNull(predicate, "predicate");
            var ex = Expression.Call(null, _any_Predicate.MakeGenericMethod(typeof(TSource)),
                source.Expression, predicate);
            return ((QueryProvider)source.Provider).ExecuteExtensionAsync<bool>(ex, null, cancellationToken);

        }

        /// <summary>
        ///  Asynchronously determines whether all the elements of a sequence satisfy a condition.
        /// </summary>
        /// <param name="source">An System.Linq.IQueryable`1 whose elements to test for a condition.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="cancellationToken">A System.Threading.CancellationToken to observe while waiting for the task to
        ///    complete.</param>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <returns> A task that represents the asynchronous operation. The task result contains true
        /// if every element of the source sequence passes the test in the specified predicate;
        /// otherwise, false.</returns>
        public static Task<bool> AllAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, "source");
            Check.NotNull(predicate, "predicate");
            var ex = Expression.Call(null, _all_Predicate.MakeGenericMethod(typeof(TSource)),
                source.Expression, predicate);
            return ((QueryProvider)source.Provider).ExecuteExtensionAsync<bool>(ex, null, cancellationToken);

        }



        /// <summary>
        ///  Asynchronously returns the number of elements in a sequence.
        /// </summary>
        /// <param name="source">An System.Linq.IQueryable`1 that contains the elements to be counted</param>
        /// <param name="cancellationToken">A System.Threading.CancellationToken to observe while waiting for the task to
        ///    complete.</param>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <returns> A task that represents the asynchronous operation. The task result contains the
        ///    number of elements in the input sequence.</returns>
        public static Task<int> CountAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, "source");
            var ex = Expression.Call(null,
                _count.MakeGenericMethod(typeof(TSource)), source.Expression);
            return ((QueryProvider)source.Provider).ExecuteExtensionAsync<int>(ex, null, cancellationToken);
        }

        /// <summary>
        ///  Asynchronously returns the number of elements in a sequence.
        /// </summary>
        /// <param name="source">An System.Linq.IQueryable`1 that contains the elements to be counted</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="cancellationToken">A System.Threading.CancellationToken to observe while waiting for the task to
        ///    complete.</param>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <returns> A task that represents the asynchronous operation. The task result contains the
        ///    number of elements in the input sequence.</returns>
        public static Task<int> CountAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, "source");
            Check.NotNull(predicate, "predicate");
            var ex = Expression.Call(null, _count_Predicate.MakeGenericMethod(typeof(TSource)),
                source.Expression, predicate);
            return ((QueryProvider)source.Provider).ExecuteExtensionAsync<int>(ex, null, cancellationToken);

        }




        /// <summary>
        ///  Asynchronously returns an System.Int64 that represents the number of elements
        ///    in a sequence that satisfy a condition.
        /// </summary>
        /// <param name="source">An System.Linq.IQueryable`1 that contains the elements to be counted.</param>
        /// <param name="cancellationToken">A System.Threading.CancellationToken to observe while waiting for the task to
        ///    complete.</param>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <returns>A task that represents the asynchronous operation. The task result contains the
        ///    number of elements in the sequence that satisfy the condition in the predicate
        ///    function.</returns>
        public static Task<long> LongCountAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, "source");
            var ex = Expression.Call(null,
                _longCount.MakeGenericMethod(typeof(TSource)), source.Expression);
            return ((QueryProvider)source.Provider).ExecuteExtensionAsync<long>(ex, null, cancellationToken);
        }

        /// <summary>
        ///  Asynchronously returns an System.Int64 that represents the number of elements
        ///    in a sequence that satisfy a condition.
        /// </summary>
        /// <param name="source">An System.Linq.IQueryable`1 that contains the elements to be counted.</param>
        /// <param name="predicate"> A function to test each element for a condition.</param>
        /// <param name="cancellationToken">A System.Threading.CancellationToken to observe while waiting for the task to
        ///    complete.</param>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <returns>A task that represents the asynchronous operation. The task result contains the
        ///    number of elements in the sequence that satisfy the condition in the predicate
        ///    function.</returns>
        public static Task<long> LongCountAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, "source");
            Check.NotNull(predicate, "predicate");
            var ex = Expression.Call(null, _longCount_Predicate.MakeGenericMethod(typeof(TSource)),
                source.Expression, predicate);
            return ((QueryProvider)source.Provider).ExecuteExtensionAsync<long>(ex, null, cancellationToken);

        }



        /// <summary>
        /// Asynchronously invokes a projection function on each element of a sequence and
        ///    returns the minimum resulting value.
        /// </summary>
        public static Task<TResult> MinAsync<TSource, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, TResult>> selector, CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, "source");
            Check.NotNull(selector, "selector");
            var ex = Expression.Call(null, _min_Selector.MakeGenericMethod(typeof(TSource), typeof(TResult)), new Expression[2]
            {
                source.Expression,
                Expression.Quote(selector)
            });
            //IDbAsyncQueryProvider dbAsyncQueryProvider = source.Provider as IDbAsyncQueryProvider;
            return ((QueryProvider)source.Provider).ExecuteExtensionAsync<TResult>(ex, null, cancellationToken);
        }




        /// <summary>
        /// Asynchronously invokes a projection function on each element of a sequence and
        ///    returns the maximum resulting value.
        /// </summary>
        public static Task<TResult> MaxAsync<TSource, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, TResult>> selector, CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, "source");
            Check.NotNull(selector, "selector");
            var ex = Expression.Call(null, _max_Selector.MakeGenericMethod(typeof(TSource), typeof(TResult)), new Expression[2]
            {
                source.Expression,
                Expression.Quote(selector)
            });
            //IDbAsyncQueryProvider dbAsyncQueryProvider = source.Provider as IDbAsyncQueryProvider;
            return ((QueryProvider)source.Provider).ExecuteExtensionAsync<TResult>(ex, null, cancellationToken);
        }



        /// <summary>
        /// Asynchronously computes the sum of a sequence of Int32 values.
        /// </summary>
        public static Task<int> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, int>> selector, CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, "source");
            Check.NotNull(selector, "selector");
            var ex = Expression.Call(null, _sum_Int_Selector.MakeGenericMethod(typeof(TSource)), new Expression[2]
            {
                source.Expression,
                Expression.Quote(selector)
            });
            return ((QueryProvider)source.Provider).ExecuteExtensionAsync<int>(ex, null, cancellationToken);
        }


        /// <summary>
        /// Asynchronously computes the sum of a sequence of Int32? values.
        /// </summary>
        public static Task<int?> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, int?>> selector, CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, "source");
            Check.NotNull(selector, "selector");
            var ex = Expression.Call(null, _sum_IntNullable_Selector.MakeGenericMethod(typeof(TSource)), new Expression[2]
            {
                source.Expression,
                Expression.Quote(selector)
            });
            return ((QueryProvider)source.Provider).ExecuteExtensionAsync<int?>(ex, null, cancellationToken);
        }


        /// <summary>
        /// Asynchronously computes the sum of a sequence of Int64 values.
        /// </summary>
        public static Task<long> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, long>> selector, CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, "source");
            Check.NotNull(selector, "selector");
            var ex = Expression.Call(null, _sum_Long_Selector.MakeGenericMethod(typeof(TSource)), new Expression[2]
            {
                source.Expression,
                Expression.Quote(selector)
            });
            return ((QueryProvider)source.Provider).ExecuteExtensionAsync<long>(ex, null, cancellationToken);
        }


        /// <summary>
        /// Asynchronously computes the sum of a sequence of Int64? values.
        /// </summary>
      
        public static Task<long?> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, long?>> selector, CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, "source");
            Check.NotNull(selector, "selector");
            var ex = Expression.Call(null, _sum_LongNullable_Selector.MakeGenericMethod(typeof(TSource)), new Expression[2]
            {
                source.Expression,
                Expression.Quote(selector)
            });
            return ((QueryProvider)source.Provider).ExecuteExtensionAsync<long?>(ex,null, cancellationToken);
        }

        /// <summary>
        /// Asynchronously computes the sum of a sequence of Decimal values.
        /// </summary>
        public static Task<decimal> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, decimal>> selector, CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, "source");
            Check.NotNull(selector, "selector");
            var ex = Expression.Call(null, _sum_Decimal_Selector.MakeGenericMethod(typeof(TSource)), new Expression[2]
            {
                source.Expression,
                Expression.Quote(selector)
            });
            return ((QueryProvider)source.Provider).ExecuteExtensionAsync<decimal>(ex, null, cancellationToken);
        }

        /// <summary>
        /// Asynchronously computes the sum of a sequence of Decimal? values.
        /// </summary>
        public static Task<decimal?> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, decimal?>> selector, CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, "source");
            Check.NotNull(selector, "selector");
            var ex = Expression.Call(null, _sum_DecimalNullable_Selector.MakeGenericMethod(typeof(TSource)), new Expression[2]
            {
                source.Expression,
                Expression.Quote(selector)
            });
            return ((QueryProvider)source.Provider).ExecuteExtensionAsync<decimal?>(ex,null, cancellationToken);
        }


        /// <summary>
        /// Asynchronously computes the sum of a sequence of Float values.
        /// </summary>
        public static Task<float> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, float>> selector, CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, "source");
            Check.NotNull(selector, "selector");
            var ex = Expression.Call(null, _sum_Float_Selector.MakeGenericMethod(typeof(TSource)), new Expression[2]
            {
                source.Expression,
                Expression.Quote(selector)
            });
            return ((QueryProvider)source.Provider).ExecuteExtensionAsync<float>(ex, null, cancellationToken);
        }

        /// <summary>
        /// Asynchronously computes the sum of a sequence of Float? values.
        /// </summary>
        public static Task<float?> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, float?>> selector, CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, "source");
            Check.NotNull(selector, "selector");
            var ex = Expression.Call(null, _sum_FloatNullable_Selector.MakeGenericMethod(typeof(TSource)), new Expression[2]
            {
                source.Expression,
                Expression.Quote(selector)
            });
            return ((QueryProvider)source.Provider).ExecuteExtensionAsync<float?>(ex, null, cancellationToken);
        }

        /// <summary>
        /// Asynchronously computes the sum of a sequence of Double values.
        /// </summary>
        public static Task<double> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, double>> selector, CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, "source");
            Check.NotNull(selector, "selector");
            var ex = Expression.Call(null, _sum_Double_Selector.MakeGenericMethod(typeof(TSource)), new Expression[2]
            {
                source.Expression,
                Expression.Quote(selector)
            });
            return ((QueryProvider)source.Provider).ExecuteExtensionAsync<double>(ex, null, cancellationToken);
        }


        /// <summary>
        /// Asynchronously computes the sum of a sequence of Double? values.
        /// </summary>
        public static Task<double?> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, double?>> selector, CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, "source");
            Check.NotNull(selector, "selector");
            var ex = Expression.Call(null, _sum_DoubleNullable_Selector.MakeGenericMethod(typeof(TSource)), new Expression[2]
            {
                source.Expression,
                Expression.Quote(selector)
            });
            return ((QueryProvider)source.Provider).ExecuteExtensionAsync<double?>(ex, null, cancellationToken);
        }


        /// <summary>
        /// Asynchronously computes the average of a sequence of Int32 values.
        /// </summary>
        public static Task<int> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, int>> selector, CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, "source");
            Check.NotNull(selector, "selector");
            //cancellationToken.ThrowIfCancellationRequested();
            var ex = Expression.Call(null, _average_Int_Selector.MakeGenericMethod(typeof(TSource)), new Expression[2]
            {
                source.Expression,
                Expression.Quote(selector)
            });
            return ((QueryProvider)source.Provider).ExecuteExtensionAsync<int>(ex, null, cancellationToken);

        }
        /// <summary>
        /// Asynchronously computes the average of a sequence of Int32? values.
        /// </summary>
        public static Task<int?> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, int?>> selector, CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, "source");
            Check.NotNull(selector, "selector");
            //cancellationToken.ThrowIfCancellationRequested();
            var ex = Expression.Call(null, _average_IntNullable_Selector.MakeGenericMethod(typeof(TSource)), new Expression[2]
            {
                source.Expression,
                Expression.Quote(selector)
            });
            return ((QueryProvider)source.Provider).ExecuteExtensionAsync<int?>(ex, null, cancellationToken);

        }
        /// <summary>
        /// Asynchronously computes the average of a sequence of Double values.
        /// </summary>
        public static Task<double> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, double>> selector, CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, "source");
            Check.NotNull(selector, "selector");
            //cancellationToken.ThrowIfCancellationRequested();
            var ex = Expression.Call(null, _average_Double_Selector.MakeGenericMethod(typeof(TSource)), new Expression[2]
            {
                source.Expression,
                Expression.Quote(selector)
            });
            return ((QueryProvider)source.Provider).ExecuteExtensionAsync<double>(ex, null, cancellationToken);

        }

        /// <summary>
        /// Asynchronously computes the average of a sequence of Double? values.
        /// </summary>
        public static Task<double?> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, double?>> selector, CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, "source");
            Check.NotNull(selector, "selector");
            //cancellationToken.ThrowIfCancellationRequested();
            var ex = Expression.Call(null, _average_DoubleNullable_Selector.MakeGenericMethod(typeof(TSource)), new Expression[2]
            {
                source.Expression,
                Expression.Quote(selector)
            });
            return ((QueryProvider)source.Provider).ExecuteExtensionAsync<double?>(ex, null, cancellationToken);

        }

        /// <summary>
        /// Asynchronously computes the average of a sequence of Decimal values.
        /// </summary>
        public static Task<decimal> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, decimal>> selector, CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, "source");
            Check.NotNull(selector, "selector");
            //cancellationToken.ThrowIfCancellationRequested();
            var ex = Expression.Call(null, _average_Decimal_Selector.MakeGenericMethod(typeof(TSource)), new Expression[2]
            {
                source.Expression,
                Expression.Quote(selector)
            });
            return ((QueryProvider)source.Provider).ExecuteExtensionAsync<decimal>(ex, null, cancellationToken);

        }
        /// <summary>
        /// Asynchronously computes the average of a sequence of Decimal? values.
        /// </summary>
        public static Task<decimal?> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, decimal?>> selector, CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, "source");
            Check.NotNull(selector, "selector");
            //cancellationToken.ThrowIfCancellationRequested();
            var ex = Expression.Call(null, _average_DecimalNullable_Selector.MakeGenericMethod(typeof(TSource)), new Expression[2]
            {
                source.Expression,
                Expression.Quote(selector)
            });
            return ((QueryProvider)source.Provider).ExecuteExtensionAsync<decimal?>(ex, null, cancellationToken);

        }

        /// <summary>
        /// Asynchronously computes the average of a sequence of Float values.
        /// </summary>
        public static Task<float> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, float>> selector, CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, "source");
            Check.NotNull(selector, "selector");
            //cancellationToken.ThrowIfCancellationRequested();
            var ex = Expression.Call(null, _average_Float_Selector.MakeGenericMethod(typeof(TSource)), new Expression[2]
            {
                source.Expression,
                Expression.Quote(selector)
            });
            return ((QueryProvider)source.Provider).ExecuteExtensionAsync<float>(ex, null, cancellationToken);

        }

        /// <summary>
        /// Asynchronously computes the average of a sequence of Float? values.
        /// </summary>
        public static Task<float?> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, float?>> selector, CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, "source");
            Check.NotNull(selector, "selector");
            //cancellationToken.ThrowIfCancellationRequested();
            var ex = Expression.Call(null, _average_FloatNullable_Selector.MakeGenericMethod(typeof(TSource)), new Expression[2]
            {
                source.Expression,
                Expression.Quote(selector)
            });
            return ((QueryProvider)source.Provider).ExecuteExtensionAsync<float?>(ex, null, cancellationToken);

        }

        /// <summary>
        /// Asynchronously computes the average of a sequence of Int64 values.
        /// </summary>
        public static Task<long> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, long>> selector, CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, "source");
            Check.NotNull(selector, "selector");
            //cancellationToken.ThrowIfCancellationRequested();
            var ex = Expression.Call(null, _average_Long_Selector.MakeGenericMethod(typeof(TSource)), new Expression[2]
            {
                source.Expression,
                Expression.Quote(selector)
            });
            return ((QueryProvider)source.Provider).ExecuteExtensionAsync<long>(ex, null, cancellationToken);

        }

        /// <summary>
        /// Asynchronously computes the average of a sequence of Int64? values.
        /// </summary>
        public static Task<long?> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, long?>> selector, CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, "source");
            Check.NotNull(selector, "selector");
            //cancellationToken.ThrowIfCancellationRequested();
            var ex = Expression.Call(null, _average_Long_Selector.MakeGenericMethod(typeof(TSource)), new Expression[2]
            {
                source.Expression,
                Expression.Quote(selector)
            });
            return ((QueryProvider)source.Provider).ExecuteExtensionAsync<long?>(ex, null, cancellationToken);

        }



























        private static MethodInfo GetMethodInfo<T1, T2, T3>(Func<T1, T2, T3> f, T1 unused1, T2 unused2)
        {
            return f.Method;
        }
        private static MethodInfo GetMethodInfo<T1, T2>(Func<T1, T2> f, T1 unused1)
        {
            return f.Method;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static Task<IDataReader> ExecuteReaderAsync(this IDbCommand  command)
        {
            var tk = new TaskCompletionSource<IDataReader>(TaskCreationOptions.RunContinuationsAsynchronously);
            tk.SetResult(command.ExecuteReader());
            return tk.Task;
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static Task<object> ExecuteScalarAsync(this IDbCommand command)
        {
            var tk = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
            tk.SetResult(command.ExecuteScalar());
            return tk.Task;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static Task<int> ExecuteNonQueryAsync(this IDbCommand command)
        {
            var tk = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
            tk.SetResult(command.ExecuteNonQuery());
            return tk.Task;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static async Task OpenAsync(this IDbConnection connection)
        {
            await Task.Run(connection.Open).ConfigureAwait(false);

        }
        /// <summary>
        /// This method begins a database connect transaction asynchronously
        /// </summary>
        public static  Task<IDbTransaction> BeginTransactionAsync(this IDbConnection connection)
        {
            var tk = new TaskCompletionSource<IDbTransaction>(TaskCreationOptions.RunContinuationsAsynchronously);
            tk.SetResult(connection.BeginTransaction());
            return tk.Task;
        }

        /// <summary>
        /// This method begins a database connect transaction asynchronously
        /// </summary>
        public static Task<IDbTransaction> BeginTransactionAsync(this IDbConnection connection, IsolationLevel il)
        {
            var tk = new TaskCompletionSource<IDbTransaction>(TaskCreationOptions.RunContinuationsAsynchronously);
            tk.SetResult(connection.BeginTransaction(il));
            return tk.Task;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="transaction"></param>
        public static async Task CommitAsync(this IDbTransaction transaction)
        {
            await Task.Run(transaction.Commit).ConfigureAwait(false);

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="transaction"></param>
        public static async Task RollbackAsync(this IDbTransaction transaction)
        {
            await Task.Run(transaction.Rollback).ConfigureAwait(false);

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        public static async Task CloseAsync(this IDbConnection connection)
        {
            await Task.Run(connection.Close).ConfigureAwait(false);

        }




        /// <summary>
        /// Performs application-defined tasks that remove,
        /// release, or reset unmanaged resources asynchronously
        /// </summary>
        /// <param name="disposable"></param>
        public static async Task DisposeAsync(this IDisposable disposable)
        {
            if (disposable is Sessione session)
            {
                await session.DisposeAsync();
            }
            else
            {
                await Task.Run(disposable.Dispose).ConfigureAwait(false);
            }
        }
        /// <summary>
        /// Reverts all changes made to the database in the current transaction asynchronously.
        /// </summary>
        public static async Task RollbackAsync(this ITransaction transaction)
        {
            if (transaction is Transactionale trans)
            {
                await trans.RollbackAsync();
            }
            else
            {
                await Task.Run(transaction.Rollback).ConfigureAwait(false);
            }
        }
        /// <summary>
        /// Asynchronously commits all changes made to the database in the current transaction.
        /// </summary>
        /// <param name="transaction"></param>
        public static async Task CommitAsync(this ITransaction transaction)
        {
            if (transaction is Transactionale trans)
            {
                await trans.CommitAsync();
            }
            else
            {
                await Task.Run(transaction.Commit).ConfigureAwait(false);
            }
        }


        /// <summary>
        /// Creates a Dictionary&lt;TKey,TValue&gt; from an IQueryable&lt;TSource&gt;
        /// according to specified key selector and element selector functions asynchronously
        /// </summary>
        public static async Task<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(
            this IQueryable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector,CancellationToken  cancellationToken=default)
        {
            var d = new Dictionary<TKey, TElement>();
            await source.ForEachAsync(element =>
            {
                d.Add(keySelector(element), elementSelector(element));
            }, cancellationToken: cancellationToken);
            return d;
        }
        /// <summary>
        /// Creates a Dictionary&lt;TKey,TValue&gt; from an IQueryable&lt;TSource&gt;
        /// according to specified key selector  asynchronously
        /// </summary>
        public static async Task<Dictionary<TKey, TSource>> ToDictionaryAsync<TSource, TKey>(
            this IQueryable<TSource> source,
            Func<TSource, TKey> keySelector,CancellationToken cancellationToken = default)
        {
            var d = new Dictionary<TKey, TSource>();
            await source.ForEachAsync(element =>
            {
                d.Add(keySelector(element), element);
            }, cancellationToken: cancellationToken);
            return d;
        }


        /// <summary>
        /// Creates a Dictionary&lt;TKey,TValue&gt; from an IQueryable&lt;TSource&gt;
        /// according to specified key selector and element selector functions asynchronously
        /// </summary>
        public static async Task<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(
            this IQueryable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector,IEqualityComparer<TKey> comparer, CancellationToken cancellationToken = default)
        {
            var d = new Dictionary<TKey, TElement>(comparer);
            await source.ForEachAsync(element =>
            {
                d.Add(keySelector(element), elementSelector(element));
            }, cancellationToken: cancellationToken);
            return d;
        }

      

        /// <summary>
        /// Asynchronously enumerates a query and uses a key selector and an element selector to construct a Lookup&lt;TKey,TElement&gt;﻿
        /// </summary>
        /// <param name="query"></param>
        /// <param name="keySelector">A function that acquires each persistent object’s key.</param>
        /// <param name="elementSelector">A function that returns a result value for each persistent object.</param>
        /// <param name="cancellationToken">token cancelling the action</param>
        public static async Task<ILookup<TKey, TElement>> ToLookupAsync<TSource, TKey, TElement>(
            this IQueryable<TSource> query,
            Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector,
            CancellationToken cancellationToken = default
        )
        {
            var tk = new TaskCompletionSource<ILookup<TKey, TElement>>(TaskCreationOptions.RunContinuationsAsynchronously);
            var s = await query.ToListAsync(cancellationToken);
            var res = s.ToLookup(keySelector, elementSelector);
            tk.SetResult(res);
            return await tk.Task;
        }

        /// <summary>
        /// Asynchronously enumerates a query and uses a key selector and an element selector to construct a Lookup&lt;TKey,TElement&gt;﻿
        /// </summary>
        /// <param name="query"></param>
        /// <param name="keySelector">A function that acquires each persistent object’s key.</param>
        /// <param name="cancellationToken">token cancelling the action</param>
        public static async Task<ILookup<TKey, TSource>> ToLookupAsync<TSource, TKey>(
            this IQueryable<TSource> query,
            Func<TSource, TKey> keySelector,
            CancellationToken cancellationToken = default
        )
        {
            var tk = new TaskCompletionSource<ILookup<TKey, TSource>>(TaskCreationOptions.RunContinuationsAsynchronously);
            var s = await query.ToListAsync(cancellationToken);
            var res = s.ToLookup(keySelector);
            tk.SetResult(res);
            return await tk.Task;
        }










    }
}
