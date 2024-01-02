using ORM_1_21_.geo;
using ORM_1_21_.Linq;
using ORM_1_21_.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace ORM_1_21_
{
    internal static class Pizdaticus
    {
        static readonly Dictionary<Type, Func<IDataReader, ProviderName, int, object>> TypeStorage = new Dictionary<Type, Func<IDataReader, ProviderName, int, object>> {
            { typeof(int), (a,p,index) => a.GetInt32(index) },
            { typeof(string), (a,p,index) => a.GetString(index)},
            { typeof(bool), (a,p, index) => a.GetBoolean(index)},
            { typeof(DateTime), (a,p, index) => a.GetDateTime(index)},
            { typeof(decimal), (a,p, index) => a.GetDecimal(index)},
            { typeof(double), (a,p, index) => a.GetDouble(index)},
            { typeof(short), (a,p, index) => a.GetInt16(index)},
            { typeof(long), (a,p, index) => a.GetInt64(index)},
            { typeof(uint), (a,p, index) => Convert.ToUInt32(a.GetValue(index))},
            { typeof(ulong), (a,p, index) => Convert.ToUInt64(a.GetValue(index))},
            { typeof(ushort), (a,p, index) => Convert.ToUInt16(a.GetValue(index))},
            { typeof(byte), (a,p, index) => a.GetByte(index)},
            { typeof(float), (a,p, index) => a.GetFloat(index)},
            { typeof(Guid), (a,p, index) => a.GetGuid(index)},
            { typeof(byte[]), (a,p, index) => a.GetValue(index)},
            { typeof(object), (a,p, index) => a.GetValue(index)},
            {typeof(Char), (a, p, index) =>
            {
                if (p == ProviderName.MsSql)
                {
                    return Convert.ToChar(a.GetValue(index));
                }
                return a.GetChar(index);
            }}


        };
        public static IEnumerable<TObj> GetRiderToList<TObj>(IDataReader reader, ProviderName providerName, bool isFree)
        {
            var isLegalese = AttributesOfClass<TObj>.IsUsageActivator(providerName);
            bool isPersistent = AttributesOfClass<TObj>.IsUsagePersistent.Value;
            var res = new List<TObj>();
            try
            {
                while (reader.Read())
                {
                    TObj d;
                    if (isLegalese)
                        d = Activator.CreateInstance<TObj>();
                    else
                        d = (TObj)FormatterServices.GetSafeUninitializedObject(typeof(TObj));

                    if (isFree == false)
                    {
                        reader.SpotRider(providerName, d);
                    }
                    else
                    {
                        reader.SpotRiderFree(providerName, d);
                    }

                    if (isPersistent)
                    {
                        UtilsCore.SetPersistent(d);
                    }
                    res.Add(d);
                }

                return res;
            }
            finally
            {
                reader.Dispose();
            }
        }


        public static List<T> GetListAnonymousObj<T>(IDataReader reader, object ss, ProviderName providerName)
        {
            try
            {
                var ctor = ((NewExpression)ss).Constructor;
                var lRes = new List<T>();
                var d = new object[reader.FieldCount];
                while (reader.Read())
                {
                    for (var i = 0; i < reader.FieldCount; i++)
                    {
                        var t = ((NewExpression)ss).Arguments[i].Type;
                        var val = MethodFreeIndex(providerName, t, reader, i);
                        if (UtilsCore.IsGeo(t))
                        {
                            if (val == null)
                            {
                                d[i] = null;
                            }else if (val is string)
                            {
                                var o = new GeoObject();
                                ((IGeoShape)o).GeoData = val.ToString();
                                d[i] = o;
                            }
                            else
                            {
                                d[i] = val;
                            }

                            
                        }
                        else
                        {
                            d[i] = val;
                        }

                    }

                    var ee = ctor.Invoke(d);
                    lRes.Add((T)ee);
                }

                return lRes;
            }
            finally
            {
                reader.Dispose();
            }
        }

        public static void GetListAnonymousObjDistinct(IDataReader reader, object ss, IList list,
            ProviderName providerName)
        {
            try
            {
                var ctor = ((NewExpression)ss).Constructor;
                var d = new object[reader.FieldCount];
                while (reader.Read())
                {
                    for (var i = 0; i < reader.FieldCount; i++)
                    {
                        var t = ((NewExpression)ss).Arguments[i].Type;
                        var val = MethodFreeIndex(providerName, t, reader, i);
                        d[i] = val;
                    }

                    var ee = ctor.Invoke(d);
                    list.Add(ee);
                }
            }
            finally
            {
                reader.Dispose();
            }
        }


        public static T SingleData<T>(IEnumerable<OneComposite> listOne, IEnumerable<T> source, out bool isActive)
        {
            if (listOne == null) throw new ArgumentException("listOne == null ");
            var oneComposite = listOne as OneComposite[] ?? listOne.ToArray();
            var result = source as T[] ?? source.ToArray();
            var enumerable = source as T[] ?? result.ToArray();
            if (oneComposite.Any(a => a.Operand == Evolution.SingleOrDefault && a.IsAggregate))
            {
                isActive = true;
                if (enumerable.Length == 1) return enumerable.First();

                if (enumerable.Length == 0) return default;

                if (enumerable.Length > 1) throw new Exception("Sequence contains more than one element");
            }

            if (oneComposite.Any(a => a.Operand == Evolution.Single && a.IsAggregate))
            {
                isActive = true;
                if (result.Count() == 1) return enumerable.First();

                throw new Exception("Sequence contains more than one element or is empty count -" +
                                    enumerable.Count());
            }

            if (oneComposite.Any(a => a.Operand == Evolution.First))
            {
                isActive = true;
                return result.First();
            }

            if (oneComposite.Any(a => a.Operand == Evolution.FirstOrDefault))
            {
                isActive = true;
                return !result.Any() ? default : enumerable.First();
            }

            if (oneComposite.Any(a => a.Operand == Evolution.LastOrDefault))
            {
                isActive = true;
                return !result.Any() ? default : enumerable.First();
            }

            if (oneComposite.Any(a => a.Operand == Evolution.Last))
            {
                isActive = true;
                return result.Last();
            }

            if (oneComposite.Any(a => a.Operand == Evolution.ElementAt))
            {
                isActive = true;
                return result.First();
            }

            if (oneComposite.Any(a => a.Operand == Evolution.ElementAtOrDefault))
            {
                isActive = true;
                return result.Any() ? enumerable.First() : default;
            }

            isActive = false;
            return default;
        }

        public static object MethodFreeIndex(ProviderName providerName, Type type, IDataReader reader, int index)
        {
            if (reader.IsDBNull(index)) return null;
            type = UtilsCore.GetCoreType(type);

            if (TypeStorage.ContainsKey(type))
            {
                return TypeStorage[type].Invoke(reader, providerName, index);
            }



            if (type.BaseType == typeof(Enum))
            {
                var o = reader.GetValue(index);
                return Enum.Parse(type, Convert.ToInt32(o).ToString());
            }
            var res = reader.GetValue(index);

            if (UtilsCore.IsGeo(type))
            {
                var o = new GeoObject(res.ToString());
                return o;
            }

            if (UtilsCore.IsJson(type))
            {
                return JsonConvert.DeserializeObject(res.ToString(), type);
            }

            
            return res;
        }
    }
}