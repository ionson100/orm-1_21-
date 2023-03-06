using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;



namespace ORM_1_21_.Utils
{
    
    class GroupExpression<T>
    {
       
        public static Delegate Delegate(LambdaExpression lexp)
        {
            return Action(lexp, lexp.ReturnType);
        }
        private static Delegate Action(LambdaExpression lexp, Type type)
        {

            if (type == typeof(Guid)) { return (Func<T, Guid>)lexp.Compile(); }
            if (type == typeof(DateTime)) { return (Func<T, DateTime>)lexp.Compile(); }
            if (type == typeof(uint)) { return (Func<T, uint>)lexp.Compile(); }
            if (type == typeof(ulong)) { return (Func<T, ulong>)lexp.Compile(); }
            if (type == typeof(ushort)) { return (Func<T, ushort>)lexp.Compile(); }
            if (type == typeof(bool)) { return (Func<T, bool>)lexp.Compile(); }
            if (type == typeof(byte)) { return (Func<T, byte>)lexp.Compile(); }
            if (type == typeof(char)) { return (Func<T, char>)lexp.Compile(); }
            if (type == typeof(decimal)) { return (Func<T, decimal>)lexp.Compile(); }
            if (type == typeof(double)) { return (Func<T, double>)lexp.Compile(); }
            if (type == typeof(short)) { return (Func<T, short>)lexp.Compile(); }
            if (type == typeof(int)) { return (Func<T, int>)lexp.Compile(); }
            if (type == typeof(long)) { return (Func<T, long>)lexp.Compile(); }
            if (type == typeof(sbyte)) { return (Func<T, sbyte>)lexp.Compile(); }
            if (type == typeof(float)) { return (Func<T, float>)lexp.Compile(); }

            if (type == typeof(string))
            {
                return (Func<T, string>)lexp.Compile();
            }
            if (type == typeof(object)) { return (Func<T, object>)lexp.Compile(); }


            if (type == typeof(Guid?)) { return (Func<T, Guid?>)lexp.Compile(); }
            if (type == typeof(DateTime?)) { return (Func<T, DateTime?>)lexp.Compile(); }
            if (type == typeof(uint?)) { return (Func<T, uint?>)lexp.Compile(); }
            if (type == typeof(ulong?)) { return (Func<T, ulong?>)lexp.Compile(); }
            if (type == typeof(ushort?)) { return (Func<T, ushort?>)lexp.Compile(); }
            if (type == typeof(bool?)) { return (Func<T, bool?>)lexp.Compile(); }
            if (type == typeof(byte?)) { return (Func<T, byte?>)lexp.Compile(); }
            if (type == typeof(char?)) { return (Func<T, char?>)lexp.Compile(); }
            if (type == typeof(decimal?)) { return (Func<T, decimal?>)lexp.Compile(); }
            if (type == typeof(double?)) { return (Func<T, double?>)lexp.Compile(); }
            if (type == typeof(short?)) { return (Func<T, short?>)lexp.Compile(); }
            if (type == typeof(int?)) { return (Func<T, int?>)lexp.Compile(); }
            if (type == typeof(long?)) { return (Func<T, long?>)lexp.Compile(); }
            if (type == typeof(sbyte?)) { return (Func<T, sbyte?>)lexp.Compile(); }
            if (type == typeof(float?)) { return (Func<T, float?>)lexp.Compile(); }



            return (Func<T, object>)lexp.Compile();


        }
    }
    
    internal static class UtilsCore
    {
        private static readonly HashSet<Type> Hlam = new HashSet<Type>
        {
            typeof(uint),
            typeof(ulong),
            typeof(ushort),
            typeof(bool),
            typeof(byte),
            typeof(char),
            typeof(DateTime),
            typeof(decimal),
            typeof(double),
            typeof(short),
            typeof(int),
            typeof(long),
            typeof(sbyte),
            typeof(float),
            typeof(string),
            typeof(uint?),
            typeof(ulong?),
            typeof(ushort?),
            typeof(bool?),
            typeof(byte?),
            typeof(char?),
            typeof(DateTime?),
            typeof(decimal?),
            typeof(double?),
            typeof(short?),
            typeof(int?),
            typeof(long?),
            typeof(sbyte?),
            typeof(float?),
            typeof(Guid?),
            typeof(Guid),
            typeof(Enum),
            typeof(byte[]),
         
    };
        private static readonly Dictionary<Type, bool> SerializableTypeDictionary = new Dictionary<Type, bool>();
        private static readonly HashSet<Type> NumericTypes = new HashSet<Type>
        {

           typeof(byte),
           typeof(sbyte),
           typeof(ushort),
           typeof(uint),
           typeof(ulong),
           typeof(short),
           typeof(int),
           typeof(long),
           typeof(decimal),
           typeof(double),
           typeof(float),

        };

        internal static bool IsNumericType(Type type)
        {
            return NumericTypes.Contains(type) ||
                   NumericTypes.Contains(Nullable.GetUnderlyingType(type));
        }

        internal const string Bungalo = "____";
        

        internal static string Pref(ProviderName providerName)
        {

            switch (providerName)
            {
                case ProviderName.MsSql:
                    return "SELECT IDENT_CURRENT ('{1}');";
                case ProviderName.MySql:
                    return "SELECT LAST_INSERT_ID();";
                case ProviderName.Postgresql:
                    return "RETURNING {2};";
                case ProviderName.Sqlite:
                    return ";select last_insert_rowid();";
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

        internal static string PrefParam(ProviderName providerName)
        {

            switch (providerName)
            {
                case ProviderName.MsSql:
                    return "@";
                case ProviderName.MySql:
                    return "?";
                case ProviderName.Postgresql:
                    return "@";
                case ProviderName.Sqlite:
                    return "@";
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

        internal static string Table1AliasForJoin = "tt1";

        internal static bool IsAnonymousType(Type type)
        {
            return System.Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
                   && type.IsGenericType && type.Name.Contains("AnonymousType")
                   && (type.Name.StartsWith("<>", StringComparison.OrdinalIgnoreCase) ||
                       type.Name.StartsWith("VB$", StringComparison.OrdinalIgnoreCase))
                   && (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic;
        }

     
        internal static byte[] ObjectToByteArray(object obj)
        {
            if (obj == null)
                return null;
            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        internal static object ByteArrayToObject(byte[] arrBytes)
        {
            if (arrBytes == null) return null;
            using (var memStream = new MemoryStream())
            {
                var binForm = new BinaryFormatter();
                memStream.Write(arrBytes, 0, arrBytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                var obj = binForm.Deserialize(memStream);
                return obj;
            }
        }

        internal static string GetAsAlias(string table, string field)
        {
            return string.Concat(table.Trim(), Bungalo, field.Trim()).Replace("`", "").Replace("[", "").Replace("]", "")
                .Trim().ToLower();
        }

        internal static byte[] ImageToByte(Image img)
        {
            if (img == null) return null;
            var converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }

       
        internal static Image ImageFromByte(byte[] bytes)
        {
            if (bytes == null) return null;
            var converter = new ImageConverter();
            return (Image)converter.ConvertFrom(bytes);
        }


        internal static bool IsPersistent(object obj)
        {
            return TypeDescriptor.GetAttributes(obj).Contains(new PersistentAttribute());
        }


        internal static void SetPersistent(object obj)
        {
            TypeDescriptor.AddAttributes(obj, new PersistentAttribute());
        }

        internal static object Convertor<TR>(object ob)
        {
            if (ob is DBNull)
            {
                if (typeof(TR).IsGenericType) return null;

                if (!typeof(TR).IsValueType) return (TR)(object)null;
                if (typeof(TR) == typeof(DateTime))
                {
                    object o = DateTime.MinValue;
                    return (TR)o;
                }

                return Activator.CreateInstance<TR>();
            }

            if (typeof(TR) == typeof(uint)) return (TR)(object)Convert.ToUInt32(ob, CultureInfo.InvariantCulture);
            if (typeof(TR) == typeof(ulong)) return (TR)(object)Convert.ToUInt64(ob, CultureInfo.InvariantCulture);
            if (typeof(TR) == typeof(ushort)) return (TR)(object)Convert.ToUInt16(ob, CultureInfo.InvariantCulture);
            if (typeof(TR) == typeof(bool)) return (TR)(object)Convert.ToBoolean(ob, CultureInfo.InvariantCulture);
            if (typeof(TR) == typeof(byte)) return (TR)(object)Convert.ToByte(ob, CultureInfo.InvariantCulture);

            if (typeof(TR) == typeof(char)) return (TR)(object)Convert.ToChar(ob, CultureInfo.InvariantCulture);
            if (typeof(TR) == typeof(DateTime))
                return (TR)(object)Convert.ToDateTime(ob, CultureInfo.InvariantCulture);
            if (typeof(TR) == typeof(decimal)) return (TR)(object)Convert.ToDecimal(ob, CultureInfo.InvariantCulture);
            if (typeof(TR) == typeof(double)) return (TR)(object)Convert.ToDouble(ob, CultureInfo.InvariantCulture);
            if (typeof(TR) == typeof(short)) return (TR)(object)Convert.ToInt16(ob, CultureInfo.InvariantCulture);
            if (typeof(TR) == typeof(int)) return (TR)(object)Convert.ToInt32(ob, CultureInfo.InvariantCulture);
            if (typeof(TR) == typeof(long)) return (TR)(object)Convert.ToInt64(ob, CultureInfo.InvariantCulture);
            if (typeof(TR) == typeof(sbyte)) return (TR)(object)Convert.ToSByte(ob, CultureInfo.InvariantCulture);
            if (typeof(TR) == typeof(float)) return (TR)(object)Convert.ToSingle(ob, CultureInfo.InvariantCulture);
            if (typeof(TR) == typeof(string)) return (TR)(object)Convert.ToString(ob, CultureInfo.InvariantCulture);
            ////
            if (typeof(TR) == typeof(uint?)) return (TR)(object)Convert.ToUInt32(ob, CultureInfo.InvariantCulture);
            if (typeof(TR) == typeof(ulong?)) return (TR)(object)Convert.ToUInt64(ob, CultureInfo.InvariantCulture);

            if (typeof(TR) == typeof(ushort?)) return (TR)(object)Convert.ToUInt16(ob, CultureInfo.InvariantCulture);
            if (typeof(TR) == typeof(bool?)) return (TR)(object)Convert.ToBoolean(ob, CultureInfo.InvariantCulture);
            if (typeof(TR) == typeof(byte?)) return (TR)(object)Convert.ToByte(ob, CultureInfo.InvariantCulture);
            if (typeof(TR) == typeof(char?)) return (TR)(object)Convert.ToChar(ob, CultureInfo.InvariantCulture);
            if (typeof(TR) == typeof(DateTime?))
                return (TR)(object)Convert.ToDateTime(ob, CultureInfo.InvariantCulture);
            if (typeof(TR) == typeof(decimal?))
                return (TR)(object)Convert.ToDecimal(ob, CultureInfo.InvariantCulture);
            if (typeof(TR) == typeof(double?)) return (TR)(object)Convert.ToDouble(ob, CultureInfo.InvariantCulture);
            if (typeof(TR) == typeof(short?)) return (TR)(object)Convert.ToInt16(ob, CultureInfo.InvariantCulture);
            if (typeof(TR) == typeof(int?)) return (TR)(object)Convert.ToInt32(ob, CultureInfo.InvariantCulture);
            if (typeof(TR) == typeof(long?)) return (TR)(object)Convert.ToInt64(ob, CultureInfo.InvariantCulture);
            if (typeof(TR) == typeof(sbyte?)) return (TR)(object)Convert.ToSByte(ob, CultureInfo.InvariantCulture);
            if (typeof(TR) == typeof(float?)) return (TR)(object)Convert.ToSingle(ob, CultureInfo.InvariantCulture);
            if (typeof(TR) == typeof(byte[])) return (TR)ob;
            if (typeof(TR) == typeof(object)) return (TR)ob;
            //var t = typeof(TR);
            //if (typeof(TR) == typeof(Guid))
            //{
            //    return new Guid(ob.ToString());
            //}

            string message = string.Format(CultureInfo.CurrentCulture, "Can't convert type {0} as {1}",
                ob.GetType().FullName, typeof(TR));
            throw new Exception(message);
        }

       





        internal static string TanslycatorFieldParam1(string nameField, string table1)
        {
            var matsup =
                new Regex(@"[aA-zZаА-яЯ\d[_]*]*\.[aA-zZаА-яЯ\d[_]*]*").Matches(nameField.Replace("`", "")
                    .Replace("[", "").Replace("]", ""));
            var dd = matsup[0].ToString().Split('.');
            return string.Format("{0}.{1}", table1, dd[1]);
        }

        internal static object ConverterPrimaryKeyType(Type typeColumn, object val)
        {
            if (typeColumn == typeof(Guid))
                return (Guid)val;
            if (typeColumn == typeof(decimal))
                return val;
            if (typeColumn == typeof(short))
                return decimal.ToInt16((decimal)val);
            if (typeColumn == typeof(int))
                return decimal.ToInt32((decimal)val);
            if (typeColumn == typeof(long))
                return decimal.ToInt64((decimal)val);
            if (typeColumn == typeof(ushort))
                return decimal.ToUInt16((decimal)val);
            if (typeColumn == typeof(uint))
                return decimal.ToUInt32((decimal)val);
            if (typeColumn == typeof(ulong))
                return decimal.ToUInt64((decimal)val);

            if (typeColumn == typeof(double))
                return decimal.ToDouble((decimal)val);
            if (typeColumn == typeof(double))
                return decimal.ToSByte((sbyte)val);
            if (typeColumn == typeof(float))
                return decimal.ToSingle((sbyte)val);
            throw new Exception($"Can't find type to convert primary key {typeColumn} {val}");
        }

        internal static bool ColumnExists(IDataReader reader, string columnName)
        {
            for (var i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i) == columnName) return true;
            }

            return false;
        }

        private static readonly Dictionary<Type, SerializeType> SerializeTypes = new Dictionary<Type, SerializeType>();
        private static bool IsSerializable(Type type)
        {
            if (Hlam.Contains(type)) return false;
            if (SerializableTypeDictionary.ContainsKey(type) == false)
            {
                var t = type.GetCustomAttribute(typeof(MapSerializableAttribute));
                SerializableTypeDictionary.Add(type, t != null);
            }

            return SerializableTypeDictionary[type];
        }

        internal static bool IsJsonType(Type type)
        {
           
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
                return true;
            if (IsSerializable(type))
            {
                return true;
            }
       
            return false;
        }

        internal static SerializeType GetSerializeType(Type type)
        {
            if (Hlam.Contains(type))
                return SerializeType.None;
            if (SerializeTypes.ContainsKey(type) == false)
            {
                var t = type.GetCustomAttribute(typeof(MapSerializableAttribute));
                if (t != null)
                {
                    SerializeTypes.Add(type,SerializeType.Self);
                } else if (type.GetInterfaces().Contains(typeof(IMapSerializable)))
                {
                    SerializeTypes.Add(type, SerializeType.User);
                }else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
                {
                    SerializeTypes.Add(type, SerializeType.Self);
                }
                else
                {
                    SerializeTypes.Add(type, SerializeType.None);
                }
               
                
            }

            return SerializeTypes[type];
        }

        public static string ObjectToJson(object o)
        {
            string json = JsonSerializer.Serialize(o);
            return json.Replace("'", "''");
        }

        public static object JsonToObject(string o, Type type)
        {
            if (string.IsNullOrWhiteSpace(o)) return  null;
            return JsonSerializer.Deserialize(o, type);
        }

        public static string ClearTrim(string tableName)
        {
            return tableName.Trim(' ', '`', '\'', '[', ']', '"');
        }

        public static string GetStringSql(IDbCommand command)
        {
            var stringBuilder = new StringBuilder(command.CommandText);
            if (command.Parameters.Count > 0)
            {
                stringBuilder.Append(" params: ");
                foreach (DbParameter commandParameter in command.Parameters)
                    stringBuilder.Append($" {commandParameter.ParameterName} - {commandParameter.Value} ");
            }


            return stringBuilder.ToString();
        }


        public static string[] MySplit(string str)
        {
            List<string> list = new List<string>();
            str = str.Trim(' ', ',');
            StringBuilder builder = new StringBuilder();
            int stop = 0;
            foreach (char c in str)
            {
                if (c == '(') stop = ++stop;
                if (c == ')') stop = --stop;
                if (stop>0)
                {
                    builder.Append(c);
                    continue;
                }
                if (c != ',')
                {
                    builder.Append(c);
                }
                else
                {
                    list.Add(builder.ToString());
                    builder.Clear();
                }
            }

            if (builder.Length > 0)
            {
                list.Add(builder.ToString());
            }

            return list.ToArray();

        }


        public static T Clone<T>(T ob) where T : class
        {
            var str = JsonSerializer.Serialize(ob);
            return JsonSerializer.Deserialize<T>(str);
        }
    }

    internal enum SerializeType
    {
        None,Self,User
    }
}

