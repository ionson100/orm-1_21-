using ORM_1_21_.Linq;
using ORM_1_21_.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace ORM_1_21_
{
    internal static class Pizdaticus
    {
        public static IEnumerable<TObj> GetRiderToList<TObj>(IDataReader reader, ProviderName providerName, bool isFree)
        {
            var isLegalese = AttributesOfClass<TObj>.IsUsageActivator(providerName);
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
                        reader.SpotRider(providerName,d);
                    }
                    else
                    {
                        reader.SpotRiderFree(providerName,d);
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
                        d[i] = val;
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

            if (type == typeof(int))
            {
                var ss = reader.GetValue(index).GetType();
                return reader.GetInt32(index);
            }
            if (type == typeof(string)) return reader.GetString(index);
            if (type == typeof(bool)) return reader.GetBoolean(index);
            if (type == typeof(DateTime)) return reader.GetDateTime(index);
            if (type == typeof(decimal)) return reader.GetDecimal(index);
            if (type == typeof(double)) return reader.GetDouble(index);
            if (type == typeof(short)) return reader.GetInt16(index);
            if (type == typeof(long)) return reader.GetInt64(index);
            if (type == typeof(uint)) return Convert.ToUInt32(reader.GetValue(index));
            if (type == typeof(ulong)) return Convert.ToUInt64(reader.GetValue(index));
            if (type == typeof(ushort)) return Convert.ToUInt16(reader.GetValue(index));
            if (type == typeof(byte)) return reader.GetByte(index);
            if (type == typeof(char))
            {
                if (providerName == ProviderName.MsSql)
                {
                    return Convert.ToChar(reader.GetValue(index));
                }
                return reader.GetChar(index);
            }

            if (type == typeof(float)) return reader.GetFloat(index);
            if (type == typeof(Guid)) return reader.GetGuid(index);
            if (type.BaseType == typeof(Enum))
            {
                var o = reader.GetValue(index);
                return Enum.Parse(type, Convert.ToInt32(o).ToString());
            }
            var res = reader.GetValue(index);
            return res;
        }
    }
}