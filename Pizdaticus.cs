using ORM_1_21_.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace ORM_1_21_
{
    internal static class Pizdaticus
    {
        public static IEnumerable<TObj> GetRiderToList<TObj>(IDataReader reader,ProviderName providerName)
        {
            bool? fied = null;
            var res = new List<TObj>();
            while (reader.Read())
            {
                var d = (TObj)FormatterServices.GetSafeUninitializedObject(typeof(TObj));
                foreach (var s in AttributesOfClass<TObj>.ListBaseAttrE(providerName))
                {
                    try
                    {
                        if (fied == null)
                            fied = Utils.ColumnExists(reader, s.ColumnNameAlias);

                        object e;
                        if (fied == true)
                            e = reader[s.ColumnNameAlias];
                        else
                            e = reader[Utils.ClearTrim(s.GetColumnName(providerName))];

                        var pr = AttributesOfClass<TObj>.PropertyInfoList.Value[s.PropertyName];


                        if (Utils.IsJsonType(pr.PropertyType))
                        {
                            var o = Utils.JsonToObject(e.ToString(), pr.PropertyType);
                            AttributesOfClass<TObj>.SetValueE(providerName, pr.Name, d,
                                e == DBNull.Value ? null : o);
                        }

                        else if (pr.PropertyType == typeof(Image))
                        {
                            AttributesOfClass<TObj>.SetValueE(providerName, pr.Name, d,
                                e == DBNull.Value ? null : Utils.ImageFromByte((byte[])e));
                        }
                        else if (pr.PropertyType == typeof(bool))
                        {
                            if (e == DBNull.Value)
                            {
                                AttributesOfClass<TObj>.SetValueE(providerName, pr.Name, d, null);
                            }
                            else
                            {
                                var b = Convert.ToBoolean(e);
                                AttributesOfClass<TObj>.SetValueE(providerName, pr.Name, d, b);
                            }
                        }
                        else if (pr.PropertyType == typeof(DateTime))
                        {
                            if (providerName == ProviderName.Sqlite)
                            {
                                DateTime dateTime = DateTime.Parse(e.ToString());
                                AttributesOfClass<TObj>.SetValueE(providerName, pr.Name, d, dateTime);
                            }
                            else
                            {
                                AttributesOfClass<TObj>.SetValueE(providerName, pr.Name, d, e);
                            }
                        }
                        else if (pr.PropertyType == typeof(DateTime?))
                        {
                            if (providerName == ProviderName.Sqlite)
                            {
                                DateTime? dateTime = null;
                                if (e != DBNull.Value)
                                {
                                    dateTime = DateTime.Parse(e.ToString());
                                }

                                AttributesOfClass<TObj>.SetValueE(providerName, pr.Name, d, dateTime);
                            }
                            else
                            {
                                AttributesOfClass<TObj>.SetValueE(providerName, pr.Name, d, e == DBNull.Value ? null : e);
                            }
                        }
                        else if (pr.PropertyType.BaseType == typeof(Enum))
                        {
                            var eres = Enum.Parse(pr.PropertyType, e.ToString());
                            AttributesOfClass<TObj>.SetValueE(providerName, pr.Name, d, e == DBNull.Value ? null : eres);
                        }
                        else if (pr.PropertyType == typeof(Guid))
                        {
                            var eres = new Guid(e.ToString());
                            AttributesOfClass<TObj>.SetValueE(providerName, pr.Name, d, e == DBNull.Value ? Guid.Empty : eres);
                        }
                        else if (pr.PropertyType == typeof(Guid?))
                        {
                            if (e == DBNull.Value)
                            {
                                AttributesOfClass<TObj>.SetValueE(providerName, pr.Name, d, null);
                            }
                            else
                            {
                                var resUuid = new Guid(e.ToString());
                                AttributesOfClass<TObj>.SetValueE(providerName, pr.Name, d, resUuid);
                            }
                        }
                        else if (pr.PropertyType == typeof(float))
                        {
                            if (e == DBNull.Value)
                                AttributesOfClass<TObj>.SetValueE(providerName, pr.Name, d, 0f);
                            else
                                AttributesOfClass<TObj>.SetValueE(providerName, pr.Name, d, Convert.ToSingle(e));
                        }
                        else if (pr.PropertyType == typeof(float?))
                        {
                            if (e == DBNull.Value)
                                AttributesOfClass<TObj>.SetValueE(providerName, pr.Name, d, null);
                            else
                                AttributesOfClass<TObj>.SetValueE(providerName, pr.Name, d, Convert.ToSingle(e));
                        }
                        else if (pr.PropertyType == typeof(double))
                        {
                            if (e == DBNull.Value)
                                AttributesOfClass<TObj>.SetValueE(providerName, pr.Name, d, 0);
                            else
                                AttributesOfClass<TObj>.SetValueE(providerName, pr.Name, d, Convert.ToDouble(e));
                        }
                        else if (pr.PropertyType == typeof(double?))
                        {
                            if (e == DBNull.Value)
                                AttributesOfClass<TObj>.SetValueE(providerName, pr.Name, d, null);
                            else
                                AttributesOfClass<TObj>.SetValueE(providerName, pr.Name, d, Convert.ToDouble(e));
                        }
                        else if (pr.PropertyType == typeof(decimal))
                        {
                            if (e == DBNull.Value)
                                AttributesOfClass<TObj>.SetValueE(providerName, pr.Name, d, 0);
                            else
                                AttributesOfClass<TObj>.SetValueE(providerName, pr.Name, d, Convert.ToDecimal(e));
                        }
                        else if (pr.PropertyType == typeof(decimal?))
                        {
                            if (e == DBNull.Value)
                                AttributesOfClass<TObj>.SetValueE(providerName, pr.Name, d, null);
                            else
                                AttributesOfClass<TObj>.SetValueE(providerName, pr.Name, d, Convert.ToDecimal(e));
                        }
                        else if (pr.PropertyType == typeof(int))
                        {
                            if (e == DBNull.Value)
                                AttributesOfClass<TObj>.SetValueE(providerName, pr.Name, d, 0);
                            else
                                AttributesOfClass<TObj>.SetValueE(providerName, pr.Name, d, Convert.ToInt32(e));
                        }
                        else if (pr.PropertyType == typeof(int?))
                        {
                            if (e == DBNull.Value)
                                AttributesOfClass<TObj>.SetValueE(providerName, pr.Name, d, null);
                            else
                                AttributesOfClass<TObj>.SetValueE(providerName, pr.Name, d, Convert.ToInt32(e));
                        }
                        else if (pr.PropertyType == typeof(uint))
                        {
                            if (e == DBNull.Value)
                                AttributesOfClass<TObj>.SetValueE(providerName, pr.Name, d, 0);
                            else
                                AttributesOfClass<TObj>.SetValueE(providerName, pr.Name, d, Convert.ToUInt32(e));
                        }
                        else if (pr.PropertyType == typeof(uint?))
                        {
                            if (e == DBNull.Value)
                                AttributesOfClass<TObj>.SetValueE(providerName, pr.Name, d, null);
                            else
                                AttributesOfClass<TObj>.SetValueE(providerName, pr.Name, d, Convert.ToUInt32(e));
                        }
                        else if (pr.PropertyType == typeof(ushort))
                        {
                            if (e == DBNull.Value)
                                AttributesOfClass<TObj>.SetValueE(providerName, pr.Name, d, 0);
                            else
                                AttributesOfClass<TObj>.SetValueE(providerName, pr.Name, d, Convert.ToUInt16(e));
                        }
                        else if (pr.PropertyType == typeof(ushort?))
                        {
                            if (e == DBNull.Value)
                                AttributesOfClass<TObj>.SetValueE(providerName, pr.Name, d, null);
                            else
                                AttributesOfClass<TObj>.SetValueE(providerName, pr.Name, d, Convert.ToUInt16(e));
                        }
                        else if (pr.PropertyType == typeof(ulong))
                        {
                            if (e == DBNull.Value)
                                AttributesOfClass<TObj>.SetValueE(providerName, pr.Name, d, 0);
                            else
                                AttributesOfClass<TObj>.SetValueE(providerName, pr.Name, d, Convert.ToUInt64(e));
                        }
                        else if (pr.PropertyType == typeof(ulong?))
                        {
                            if (e == DBNull.Value)
                                AttributesOfClass<TObj>.SetValueE(providerName, pr.Name, d, null);
                            else
                                AttributesOfClass<TObj>.SetValueE(providerName, pr.Name, d, Convert.ToUInt64(e));
                        }
                        else if (pr.PropertyType == typeof(byte))
                        {
                            if (e == DBNull.Value)
                                AttributesOfClass<TObj>.SetValueE(providerName, pr.Name, d, 0);
                            else
                                AttributesOfClass<TObj>.SetValueE(providerName, pr.Name, d, Convert.ToByte(e));
                        }
                        else if (pr.PropertyType == typeof(byte?))
                        {
                            if (e == DBNull.Value)
                                AttributesOfClass<TObj>.SetValueE(providerName, pr.Name, d, null);
                            else
                                AttributesOfClass<TObj>.SetValueE(providerName, pr.Name, d, Convert.ToByte(e));
                        }
                        else if (pr.PropertyType == typeof(sbyte))
                        {
                            if (e == DBNull.Value)
                                AttributesOfClass<TObj>.SetValueE(providerName, pr.Name, d, 0);
                            else
                                AttributesOfClass<TObj>.SetValueE(providerName, pr.Name, d, Convert.ToSByte(e));
                        }
                        else if (pr.PropertyType == typeof(sbyte?))
                        {
                            if (e == DBNull.Value)
                                AttributesOfClass<TObj>.SetValueE(providerName, pr.Name, d, null);
                            else
                                AttributesOfClass<TObj>.SetValueE(providerName, pr.Name, d, Convert.ToSByte(e));
                        }
                        else
                        {
                            var tt = e.GetType();
                            var tte = pr.PropertyType;
                            try
                            {
                                AttributesOfClass<TObj>.SetValueE(providerName, pr.Name, d, e == DBNull.Value ? null : e);
                            }
                            catch (Exception exception)
                            {
                                Console.WriteLine(exception);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Configure.SendError("create item " + typeof(TObj), e);
                    }

                }

                Utils.SetPersisten(d);
                res.Add(d);
            }

            reader.Dispose();
            return res;
        }


        public static List<T> GetListAnonymusObj<T>(IDataReader reader, object ss)
        {
            try
            {
                var ctor = ((NewExpression)ss).Constructor;
                var lres = new List<T>();
                var d = new object[reader.FieldCount]; //////////////////////////////
                while (reader.Read())
                {
                    for (var i = 0; i < reader.FieldCount; i++)
                    {
                        var o = reader[i];
                        var t = ((NewExpression)ss).Arguments[i].Type;
                        var res = Utils.Convertor(t, o);
                        d[i] = res;
                    }
                    var ee = ctor.Invoke(d);
                    lres.Add((T)ee);
                }
                return lres;
            }
            finally
            {
                reader.Dispose();
            }

        }

        public static void GetListAnonymusObjDistinct(IDataReader reader, object ss, IList list)
        {
            try
            {
                var ctor = ((NewExpression)ss).Constructor;
                var d = new object[reader.FieldCount];
                while (reader.Read())
                {
                    for (var i = 0; i < reader.FieldCount; i++)
                    {
                        var o = reader[i];
                        var t = ((NewExpression)ss).Arguments[i].Type;
                        var res = Utils.Convertor(t, o);
                        d[i] = res;
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

        public static Delegate GetDelegateForGroupBy(LambdaExpression lambda)
        {
            return lambda.Compile();
        }

        public static T SingleData<T>(IEnumerable<OneComprosite> listOne, IEnumerable<T> lResul, out bool isActive)
        {
            if (listOne == null) throw new ArgumentException("listOne == null ");
            if (listOne.Any(a => a.Operand == Evolution.SingleOrDefault && a.IsAgregate))
            {
                if (!lResul.Any())
                {
                    isActive = true;
                    return default(T);
                }

                if (lResul.Count() == 1)
                {
                    isActive = true;
                    return lResul.First();
                }

                throw new Exception("Последовательност содержит больше чем одни элемент -" + lResul.Count());
            }

            if (listOne.Any(a => a.Operand == Evolution.Single && a.IsAgregate))
            {
                if (lResul.Count() == 1)
                {
                    isActive = true;
                    return lResul.First();
                }

                throw new Exception("Последовательност содержит больше чем одни элемент или пустая count -" +
                                    lResul.Count());
            }

            if (listOne.Any(a => a.Operand == Evolution.First))
            {
                isActive = true;
                return lResul.First();
            }

            if (listOne.Any(a => a.Operand == Evolution.FirstOrDefault))
            {
                isActive = true;
                return !lResul.Any() ? default(T) : lResul.First();
            }

            if (listOne.Any(a => a.Operand == Evolution.LastOrDefault))
            {
                isActive = true;
                return !lResul.Any() ? default(T) : lResul.First();
            }

            if (listOne.Any(a => a.Operand == Evolution.Last))
            {
                isActive = true;
                return lResul.Last();
            }

            if (listOne.Any(a => a.Operand == Evolution.ElementAt))
            {
                isActive = true;
                return lResul.First();
            }

            if (listOne.Any(a => a.Operand == Evolution.ElementAtOrDefault))
            {
                isActive = true;
                return lResul.Any() ? lResul.First() : default(T);
            }

            isActive = false;
            return default(T);
        }

        public static object Carding(Type type)
        {
            if (type == typeof(uint)) return (uint)1;
            if (type == typeof(ulong)) return (ulong)1;
            if (type == typeof(ushort)) return (ushort)1;
            if (type == typeof(bool)) return true;
            if (type == typeof(byte)) return (byte)1;

            if (type == typeof(char)) return (char)1;
            if (type == typeof(DateTime)) return DateTime.Now;
            if (type == typeof(decimal)) return (decimal)1;
            if (type == typeof(double)) return (double)1;
            if (type == typeof(short)) return (short)1;
            if (type == typeof(int)) return 1;
            if (type == typeof(long)) return (long)1;
            if (type == typeof(sbyte)) return (sbyte)1;
            if (type == typeof(float)) return (float)1;
            if (type == typeof(string)) return string.Empty;
            ////
            if (type == typeof(uint?)) return (uint?)1;
            if (type == typeof(ulong?)) return (ulong?)1;
            if (type == typeof(ushort?)) return (ushort?)2;
            if (type == typeof(bool?)) return true;
            if (type == typeof(byte?)) return (byte?)1;
            if (type == typeof(char?)) return (char?)1;
            if (type == typeof(DateTime?)) return null;
            if (type == typeof(decimal?)) return (decimal?)1;
            if (type == typeof(double?)) return (double?)1;
            if (type == typeof(short?)) return (short?)1;
            if (type == typeof(int?)) return 1;

            if (type == typeof(long?)) return (long?)1;
            if (type == typeof(sbyte?)) return (sbyte?)1;
            if (type == typeof(float?)) return (float?)1;
            return null;
        }

        public static IEnumerable<TObj> GetRiderToList2<TObj>(IDataReader reader,ProviderName providerName)
        {
            bool? fied = null;
            var res = new List<TObj>();
            using (var read = reader)
            {
                while (read.Read())
                {
                    var d = (TObj)FormatterServices.GetSafeUninitializedObject(typeof(TObj));
                    foreach (var s in AttributesOfClass<TObj>.ListBaseAttrE(providerName))
                    {
                        if (fied == null)
                            fied = Utils.ColumnExists(reader, s.ColumnNameAlias);

                        object e;
                        if (fied == true)
                            e = reader[s.ColumnNameAlias];
                        else
                            e =
                                reader[
                                    s.GetColumnName(providerName).Replace("`", string.Empty)
                                        .Replace("[", string.Empty)
                                        .Replace("]", string.Empty)];

                        var pr = AttributesOfClass<TObj>.PropertyInfoList.Value[s.PropertyName];
                        {
                            if (Utils.IsJsonType(pr.PropertyType))
                            {
                                AttributesOfClass<TObj>.SetValueE(providerName, pr.Name,d,
                                    e == DBNull.Value
                                        ? null
                                        : Utils.JsonToObject(e.ToString(), pr.PropertyType));
                            }
                            else if (pr.PropertyType == typeof(Image))
                            {
                                AttributesOfClass<TObj>.SetValueE(providerName, pr.Name,d,e == DBNull.Value ? null : Utils.ImageFromByte((byte[])e));
                            }
                            else if (pr.PropertyType == typeof(bool))
                            {
                                if (e == DBNull.Value)
                                {
                                    AttributesOfClass<TObj>.SetValueE(providerName, pr.Name,d, null);
                                }
                                else
                                {
                                    var b = Convert.ToBoolean(e);
                                    AttributesOfClass<TObj>.SetValueE(providerName, pr.Name,d, b);
                                }
                            }
                            else if (pr.PropertyType == typeof(DateTime))
                            {
                                var tt = e.GetType();
                                AttributesOfClass<TObj>.SetValueE(providerName, pr.Name, d, e == DBNull.Value ? null : e);
                            }
                            else if (pr.PropertyType.BaseType == typeof(Enum))
                            {
                                var eres = Enum.Parse(pr.PropertyType, e.ToString());
                                AttributesOfClass<TObj>.SetValueE(providerName, pr.Name, d, e == DBNull.Value ? null : eres);
                            }
                            else if (pr.PropertyType == typeof(Guid))
                            {
                                var eres = new Guid(e.ToString());
                                AttributesOfClass<TObj>.SetValueE(providerName, pr.Name,d,
                                    e == DBNull.Value ? Guid.Empty : eres);
                            }
                            else
                            {
                                var tt = e.GetType();
                                var tte = pr.PropertyType;

                                AttributesOfClass<TObj>.SetValueE(providerName, pr.Name, d, e == DBNull.Value ? null : e);
                            }
                        }
                    }

                    Utils.SetPersisten(d);
                    yield return d;
                }
            }
        }
    }
}