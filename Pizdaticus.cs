using ORM_1_21_.Linq;
using ORM_1_21_.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Text;

namespace ORM_1_21_
{
    internal static class Pizdaticus
    {
        public static IEnumerable<TObj> GetRiderToList<TObj>(IDataReader reader, ProviderName providerName)
        {
            var isLegalese = AttributesOfClass<TObj>.IsUssageActivator(providerName);
            bool? field = null;
            var res = new List<TObj>();
            while (reader.Read())
            {
                TObj d;
                if (isLegalese)
                {
                    d = Activator.CreateInstance<TObj>();
                }
                else
                {
                    d = (TObj)FormatterServices.GetSafeUninitializedObject(typeof(TObj));
                }

                foreach (var s in AttributesOfClass<TObj>.ListBaseAttrE(providerName))
                {
                    try
                    {
                        if (field == null)
                            field = UtilsCore.ColumnExists(reader, s.ColumnNameAlias);

                        var e = field == true ? reader[s.ColumnNameAlias] : reader[UtilsCore.ClearTrim(s.GetColumnName(providerName))];
                        var pr = AttributesOfClass<TObj>.PropertyInfoList.Value[s.PropertyName];
                        var resCore = MethodFree(providerName, pr.PropertyType, e);
                        AttributesOfClass<TObj>.SetValueE(providerName, pr.Name, d, resCore);
                    }
                    catch (Exception e)
                    {
                        MySqlLogger.Info($" {Environment.NewLine}Create Item{Environment.NewLine}{e}");
                        throw;
                    }

                }

                UtilsCore.SetPersistent(d);
                res.Add(d);
            }

            reader.Dispose();
            return res;
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
                        var val = MethodFree(providerName, t, reader[i]);
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

        public static void GetListAnonymousObjDistinct(IDataReader reader, object ss, IList list, ProviderName providerName)
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
                        var val = MethodFree(providerName, t, reader[i]);
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
                if (enumerable.Length == 1)
                {
                    isActive = true;

                    return enumerable.First();
                }

                isActive = true;
                return default;

            }

            if (oneComposite.Any(a => a.Operand == Evolution.Single && a.IsAggregate))
            {
                if (result.Count() == 1)
                {
                    isActive = true;
                    return enumerable.First();
                }

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
            var st = UtilsCore.GetSerializeType(type);
            if (st == SerializeType.Self)
            {
                if (e == DBNull.Value) return null;
                return UtilsCore.JsonToObject(e.ToString(), type);
            }
            if (st == SerializeType.User)
            {
                var o = Activator.CreateInstance(type);
                ((IMapSerializable)o).Deserialize(e.ToString());
                if (e == DBNull.Value) return null;
                return o;
            }

            if (type == typeof(Image))
            {

                return e == DBNull.Value ? null : UtilsCore.ImageFromByte((byte[])e);
            }

            if (type == typeof(bool))
            {
                if (e == DBNull.Value)
                {
                    return false;
                }
                else
                {
                    return Convert.ToBoolean(e);

                }
            }

            if (type == typeof(bool?))
            {
                if (e == DBNull.Value)
                {
                    return null;
                }
                else
                {
                    return Convert.ToBoolean(e);
                }
            }
            else if (type == typeof(DateTime))
            {
                return providerName == ProviderName.Sqlite ? DateTime.Parse(e.ToString()) : e;
            }
            else if (type == typeof(DateTime?))
            {
                if (providerName == ProviderName.Sqlite)
                {
                    DateTime? dateTime = null;
                    if (e != DBNull.Value)
                    {
                        dateTime = DateTime.Parse(e.ToString());
                    }

                    return dateTime;
                }

                return e;
            }
            else if (type.BaseType == typeof(Enum))
            {
                if (e == DBNull.Value)
                {
                    return -1;
                }

                return Enum.Parse(type, Convert.ToInt32(e).ToString());
            }
            else if (type == typeof(Guid))
            {
                if (e == DBNull.Value)
                {
                    return Guid.Empty;
                }

                if (e is Guid) return e;

                Guid guid = Guid.Empty;
                if (e is byte[] bytes)
                {
                    return bytes.Length == 16 ? new Guid(bytes) : new Guid(Encoding.ASCII.GetString(bytes));
                }

                return e is string ? new Guid(e.ToString()) : guid;
            }
            else if (type == typeof(Guid?))
            {
                return e == DBNull.Value ? (object)null : new Guid(e.ToString());
            }
            else if (type == typeof(float))
            {
                return e == DBNull.Value ? 0f : Convert.ToSingle(e);
            }
            else if (type == typeof(float?))
            {
                return e == DBNull.Value ? (object)null : Convert.ToSingle(e);
            }
            else if (type == typeof(double))
            {
                return e == DBNull.Value ? 0 : Convert.ToDouble(e);
            }
            else if (type == typeof(double?))
            {
                return e == DBNull.Value ? (object)null : Convert.ToDouble(e);
            }
            else if (type == typeof(decimal))
            {
                return e == DBNull.Value ? 0 : Convert.ToDecimal(e);
            }
            else if (type == typeof(decimal?))
            {
                return e == DBNull.Value ? (object)null : Convert.ToDecimal(e);
            }
            else if (type == typeof(int))
            {
                return e == DBNull.Value ? 0 : Convert.ToInt32(e);
            }
            else if (type == typeof(int?))
            {
                return e == DBNull.Value ? (object)null : Convert.ToInt32(e);
            }
            else if (type == typeof(uint))
            {
                return e == DBNull.Value ? 0 : Convert.ToUInt32(e);
            }
            else if (type == typeof(uint?))
            {
                return e == DBNull.Value ? (object)null : Convert.ToUInt32(e);
            }
            else if (type == typeof(ushort))
            {
                return e == DBNull.Value ? 0 : Convert.ToUInt16(e);
            }
            else if (type == typeof(ushort?))
            {
                return e == DBNull.Value ? (object)null : Convert.ToUInt16(e);
            }
            else if (type == typeof(ulong))
            {
                return e == DBNull.Value ? 0 : Convert.ToUInt64(e);
            }
            else if (type == typeof(ulong?))
            {
                return e == DBNull.Value ? (object)null : Convert.ToUInt64(e);
            }
            else if (type == typeof(byte))
            {
                return e == DBNull.Value ? 0 : Convert.ToByte(e);
            }
            else if (type == typeof(byte?))
            {
                return e == DBNull.Value ? (object)null : Convert.ToByte(e);
            }
            else if (type == typeof(sbyte))
            {
                return e == DBNull.Value ? 0 : Convert.ToSByte(e);
            }
            else if (type == typeof(sbyte?))
            {
                return e == DBNull.Value ? (object)null : Convert.ToSByte(e);
            }
            else if (type == typeof(char?))
            {
                return e == DBNull.Value ? (object)null : Convert.ToChar(e);
            }
            else if (type == typeof(short))
            {
                return e == DBNull.Value ? 0 : Convert.ToInt16(e);
            }
            else if (type == typeof(short?))
            {
                return e == DBNull.Value ? (object)null : Convert.ToInt16(e);
            }
            else if (type == typeof(Int64))
            {
                return e == DBNull.Value ? 0 : Convert.ToInt64(e);
            }
            else if (type == typeof(Int64?))
            {
                return e == DBNull.Value ? (object)null : Convert.ToInt64(e);
            }
            else if (type == typeof(char))
            {
                return e == DBNull.Value ? '\0' : Convert.ToChar(e);
            }
            else
            {
                return e == DBNull.Value ? null : e;
            }

           
        }

    }
}