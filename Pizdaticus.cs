using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Text;
using ORM_1_21_.Linq;
using ORM_1_21_.Utils;

namespace ORM_1_21_
{
    internal static class Pizdaticus
    {
        public static IEnumerable<TObj> GetRiderToList<TObj>(IDataReader reader, ProviderName providerName, bool isFree)
        {
            var isLegalese = AttributesOfClass<TObj>.IsUsageActivator(providerName);
            var res = new List<TObj>();
            var list = AttributesOfClass<TObj>.ListBaseAttrE(providerName);
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
                        var index = 0;
                        list.ForEach(s =>
                        {
                            try
                            {
                                var resCore = MethodFreeIndex(providerName, s.PropertyType, reader, index);
                                AttributesOfClass<TObj>.SetValueE(providerName, s.PropertyName, d, resCore);
                                index++;
                            }
                            catch (Exception e)
                            {
                                throw new Exception($"{e.Message} {typeof(TObj)} {s.PropertyName} {s.PropertyType}", e);
                            }
                        });
                    }
                    else
                    {
                        try
                        {
                            for (var i = 0; i < reader.FieldCount; i++)
                            {
                                var n = reader.GetName(i);
                                var s = list.First(a => a.ColumnNameAlias == n || a.GetColumnNameRaw() == n);
                                var resCore = MethodFreeIndex(providerName, s.PropertyType, reader, i);
                                AttributesOfClass<TObj>.SetValueE(providerName, s.PropertyName, d, resCore);
                            }
                        }
                        catch (Exception e)
                        {
                            throw new Exception($"reader engine: {typeof(TObj)} {e.Message}",e);
                        }
                    }

                    //UtilsCore.SetPersistent(d);
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
                        var val = MethodFreeIndex(providerName, t, reader,i);
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
                        var val = MethodFreeIndex(providerName, t, reader,i);
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
            Check.NotNull(listOne, "listOne");
            Check.NotNull(source, "source");
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


        public static object MethodFree(ProviderName providerName, Type type, object e)
        {
            if (type == typeof(string)) return e.ToString();
            if (type == typeof(int)) return e == DBNull.Value ? 0 : Convert.ToInt32(e);


            if (type == typeof(bool))
            {
                if (e == DBNull.Value) return false;

                return Convert.ToBoolean(e);
            }

            if (type == typeof(bool?))
            {
                if (e == DBNull.Value) return null;

                return Convert.ToBoolean(e);
            }

            if (type == typeof(DateTime)) return providerName == ProviderName.SqLite ? DateTime.Parse(e.ToString()) : e;
            if (type == typeof(DateTime?))
            {
                if (providerName == ProviderName.SqLite)
                {
                    DateTime? dateTime = null;
                    if (e != DBNull.Value) dateTime = DateTime.Parse(e.ToString());

                    return dateTime;
                }

                return e;
            }

            if (type.BaseType == typeof(Enum))
            {
                if (e == DBNull.Value) return -1;

                return Enum.Parse(type, Convert.ToInt32(e).ToString());
            }

            if (type == typeof(Guid))
            {
                if (e == DBNull.Value) return Guid.Empty;

                if (e is Guid) return e;

                var guid = Guid.Empty;
                if (e is byte[] bytes)
                    return bytes.Length == 16 ? new Guid(bytes) : new Guid(Encoding.ASCII.GetString(bytes));

                return e is string ? new Guid(e.ToString()) : guid;
            }

            if (type == typeof(Guid?)) return e == DBNull.Value ? (object)null : new Guid(e.ToString());
            if (type == typeof(float)) return e == DBNull.Value ? 0f : Convert.ToSingle(e);
            if (type == typeof(float?)) return e == DBNull.Value ? (object)null : Convert.ToSingle(e);
            if (type == typeof(double)) return e == DBNull.Value ? 0 : Convert.ToDouble(e);
            if (type == typeof(double?)) return e == DBNull.Value ? (object)null : Convert.ToDouble(e);
            if (type == typeof(decimal)) return e == DBNull.Value ? 0 : Convert.ToDecimal(e);
            if (type == typeof(decimal?)) return e == DBNull.Value ? (object)null : Convert.ToDecimal(e);

            if (type == typeof(int?)) return e == DBNull.Value ? (object)null : Convert.ToInt32(e);
            if (type == typeof(uint)) return e == DBNull.Value ? 0 : Convert.ToUInt32(e);
            if (type == typeof(uint?)) return e == DBNull.Value ? (object)null : Convert.ToUInt32(e);
            if (type == typeof(ushort)) return e == DBNull.Value ? 0 : Convert.ToUInt16(e);
            if (type == typeof(ushort?)) return e == DBNull.Value ? (object)null : Convert.ToUInt16(e);
            if (type == typeof(ulong)) return e == DBNull.Value ? 0 : Convert.ToUInt64(e);
            if (type == typeof(ulong?)) return e == DBNull.Value ? (object)null : Convert.ToUInt64(e);
            if (type == typeof(byte)) return e == DBNull.Value ? 0 : Convert.ToByte(e);
            if (type == typeof(byte?)) return e == DBNull.Value ? (object)null : Convert.ToByte(e);
            if (type == typeof(sbyte)) return e == DBNull.Value ? 0 : Convert.ToSByte(e);
            if (type == typeof(sbyte?)) return e == DBNull.Value ? (object)null : Convert.ToSByte(e);
            if (type == typeof(char?)) return e == DBNull.Value ? (object)null : Convert.ToChar(e);
            if (type == typeof(short)) return e == DBNull.Value ? 0 : Convert.ToInt16(e);
            if (type == typeof(short?)) return e == DBNull.Value ? (object)null : Convert.ToInt16(e);
            if (type == typeof(long)) return e == DBNull.Value ? 0 : Convert.ToInt64(e);
            if (type == typeof(long?)) return e == DBNull.Value ? (object)null : Convert.ToInt64(e);
            if (type == typeof(char)) return e == DBNull.Value ? '\0' : Convert.ToChar(e);
            return e == DBNull.Value ? null : e;
        }

        public static object MethodFreeIndex(ProviderName providerName, Type type, IDataReader reader, int index)
        {
            if (reader.IsDBNull(index)) return null;

            if (providerName == ProviderName.SqLite)
            {
                if (type == typeof(Guid))
                {
                    var e = reader.GetValue(index);
                    if (e == DBNull.Value) return Guid.Empty;

                    if (e is Guid) return e;

                    var guid = Guid.Empty;
                    if (e is byte[] bytes)
                        return bytes.Length == 16 ? new Guid(bytes) : new Guid(Encoding.ASCII.GetString(bytes));

                    return e is string ? new Guid(e.ToString()) : guid;
                }

                if (type == typeof(DateTime)) return DateTime.Parse(reader.GetValue(index).ToString());
                if (type == typeof(DateTime?))
                {
                    var e = reader.GetValue(index);
                    DateTime? dateTime = null;
                    if (e != DBNull.Value) dateTime = DateTime.Parse(e.ToString());

                    return dateTime;
                }


                if (type == typeof(long) || type == typeof(long?)) return Convert.ToInt64(reader.GetValue(index));


                if (type == typeof(uint) || type == typeof(uint?)) return Convert.ToUInt32(reader.GetValue(index));

                if (type == typeof(ulong) || type == typeof(ulong?)) return Convert.ToUInt64(reader.GetValue(index));

                if (type == typeof(short) || type == typeof(short?)) return Convert.ToInt16(reader.GetValue(index));

                if (type == typeof(ushort) || type == typeof(ushort?)) return Convert.ToUInt16(reader.GetValue(index));

                if (type == typeof(decimal) || type == typeof(decimal?))
                    return Convert.ToDecimal(reader.GetValue(index));

                if (type == typeof(float) || type == typeof(float?)) return Convert.ToSingle(reader.GetValue(index));

                if (type == typeof(char) || type == typeof(char?)) return Convert.ToChar(reader.GetValue(index));

                if (type == typeof(bool) || type == typeof(bool?)) return Convert.ToBoolean(reader.GetValue(index));

                if (type == typeof(byte) || type == typeof(byte?)) return Convert.ToByte(reader.GetValue(index));
            }

            if (type == typeof(uint) || type == typeof(uint?)) return Convert.ToUInt32(reader.GetValue(index));
            if (type == typeof(ulong) || type == typeof(ulong?)) return Convert.ToUInt64(reader.GetValue(index));
            if (type == typeof(ushort) || type == typeof(ushort?)) return Convert.ToUInt16(reader.GetValue(index));
            if (type.BaseType == typeof(Enum))
            {
                var o = reader.GetValue(index);

                if (o == DBNull.Value) return -1;

                return Enum.Parse(type, Convert.ToInt32(o).ToString());
            }
            if (type == typeof(string)) return reader.GetString(index);
            if (type == typeof(bool)) return reader.GetBoolean(index);
            if (type == typeof(byte)) return reader.GetByte(index);
            if (type == typeof(char)) return Convert.ToChar(reader.GetValue(index));
            if (type == typeof(DateTime)) return reader.GetDateTime(index);
            if (type == typeof(decimal)) return reader.GetDecimal(index);
            if (type == typeof(double)) return reader.GetDouble(index);
            if (type == typeof(short)) return reader.GetInt16(index);
            if (type == typeof(int)) return reader.GetInt32(index);
            if (type == typeof(long)) return reader.GetInt64(index);
            if (type == typeof(float)) return Convert.ToSingle(reader.GetValue(index));
            if (type == typeof(float)) return reader.GetFloat(index);
            if (type == typeof(Guid)) return reader.GetGuid(index);


            var res = reader.GetValue(index);
            return res;
        }
    }
}