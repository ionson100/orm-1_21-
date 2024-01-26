using ORM_1_21_.geo;
using ORM_1_21_.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace ORM_1_21_.Utils
{
    internal static class UtilsCore
    {
        internal const string Bungalo = "____";


        public static void ErrorAlert()
        {
            throw new Exception("in Sqlite database, spatial operations are not implemented");
        }

       

        //public static HashSet<Type> HashSetJsonType = new HashSet<Type>();

        public static string SqlConcat(string column, ProviderName provider)
        {
            switch (provider)
            {
                case ProviderName.MsSql:
                    return $" ISNULL(CONCAT('SRID=',{column}.STSrid,';',{column}.STAsText()),NULL)";
                case ProviderName.MySql:
                    return $" IFNULL(CONCAT('SRID=',ST_SRID({column}),';',ST_AsText({column})),NULL)";
                case ProviderName.PostgreSql:
                    return $" coalesce(CONCAT('SRID=',ST_SRID({column}),';',ST_AsText({column})),null)";
                case ProviderName.SqLite:
                    return $" IFNULL(CONCAT('SRID=',ST_SRID({column}),';',ST_AsText({column})),NULL)";
                default:
                    throw new ArgumentOutOfRangeException(nameof(provider), provider, null);
            }
        }

       
        public static bool IsGeo(Type type)
        {
            if (type == typeof(IGeoShape) || type == typeof(GeoObject))
                return true;
            return false;
        }


        public static void AddParameter(this IDbCommand command, string name, object o)
        {
            IDataParameter p = command.CreateParameter();
            p.ParameterName = name;
            p.Value = AddParam(o);
            command.Parameters.Add(p);

        }

        static object AddParam(object o)
        {
            if (o == null) return DBNull.Value;
            if (o.GetType().BaseType == typeof(Enum))
            {
                return (int)o;
            }

            return o;
        }

        public static void SpotRider<T>(this IDataReader reader, ProviderName providerName, T d)
        {
            AttributesOfClass<T>.SpotRider(reader, providerName, d);
        }




        public static void SpotRiderFree<T>(this IDataReader reader, ProviderName providerName, T d)
        {
            AttributesOfClass<T>.SpotRiderFree(reader, providerName, d);
        }

        private static readonly HashSet<Evolution> EvolutionsList = new HashSet<Evolution>
        {
            Evolution.FreeSql,
            Evolution.TableCreate,
            Evolution.TableExists,
            Evolution.ExecuteScalar,
            Evolution.TruncateTable,
            Evolution.ExecuteNonQuery,
            Evolution.DataTable,
            Evolution.DropTable
        };


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
            typeof(byte[])
        };

        internal static readonly HashSet<Type> NumericTypes = new HashSet<Type>
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
            typeof(float)
        };

        public static bool IsReceiverFreeSql<T>()
        {
            if (Hlam.Contains(typeof(T))) return false;
            if (typeof(T).IsGenericType) return false;
            return AttributesOfClass<T>.IsReceiverFreeSqlInner;
        }
        private static readonly Dictionary<Type, Func<NewExpression, object>> ConvertorPkDictionaryX =
         new Dictionary<Type, Func<NewExpression, object>>
         {
                {typeof(uint),val=>Expression.Lambda<Func<uint>>(val).Compile()()},
               {typeof(ulong),val=>Expression.Lambda<Func<ulong>>(val).Compile()()},
               {typeof(ushort),val=>Expression.Lambda<Func<ushort>>(val).Compile()()},
               {typeof(bool),val=>Expression.Lambda<Func<bool>>(val).Compile()()},
               {typeof(byte),val=>Expression.Lambda<Func<byte>>(val).Compile()()},
               {typeof(char),val=>Expression.Lambda<Func<char>>(val).Compile()()},
               {typeof(DateTime),val=>Expression.Lambda<Func<DateTime>>(val).Compile()()},
               {typeof(decimal),val=>Expression.Lambda<Func<decimal>>(val).Compile()()},
               {typeof(double),val=>Expression.Lambda<Func<double>>(val).Compile()()},
               {typeof(short),val=>Expression.Lambda<Func<short>>(val).Compile()()},
               {typeof(int),val=>Expression.Lambda<Func<int>>(val).Compile()()},
               {typeof(long),val=>Expression.Lambda<Func<long>>(val).Compile()()},
               {typeof(sbyte),val=>Expression.Lambda<Func<sbyte>>(val).Compile()()},
               {typeof(float),val=>Expression.Lambda<Func<float>>(val).Compile()()},
               {typeof(string),val=>Expression.Lambda<Func<string>>(val).Compile()()},
               {typeof(uint?),val=>Expression.Lambda<Func<uint?>>(val).Compile()()},
              {typeof(ulong?),val=>Expression.Lambda<Func<ulong?>>(val).Compile()()},
              {typeof(ushort?),val=>Expression.Lambda<Func<ushort ?>>(val).Compile()()},
              {typeof(bool?),val=>Expression.Lambda<Func<bool?>>(val).Compile()()},
              {typeof(byte?),val=>Expression.Lambda<Func<byte ?>>(val).Compile()()},
              {typeof(char?),val=>Expression.Lambda<Func<char?>>(val).Compile()()},
              {typeof(DateTime?),val=>Expression.Lambda<Func<DateTime?>>(val).Compile()()},
              {typeof(decimal?),val=>Expression.Lambda<Func<decimal ?>>(val).Compile()()},
              {typeof(double?),val=>Expression.Lambda<Func<double?>>(val).Compile()()},
              {typeof(short?),val=>Expression.Lambda<Func<short ?>>(val).Compile()()},
              {typeof(int?),val=>Expression.Lambda<Func<int?>>(val).Compile()()},
              {typeof(long?),val=>Expression.Lambda<Func<long ?>>(val).Compile()()},
              {typeof(sbyte?),val=>Expression.Lambda<Func<sbyte?>>(val).Compile()()},
              {typeof(float?),val=>Expression.Lambda<Func<float ?>>(val).Compile()()},
              {typeof(Guid?),val=>Expression.Lambda<Func<Guid ?>>(val).Compile()()},
              {typeof(Guid),val=>Expression.Lambda<Func<Guid>>(val).Compile()()},
              {typeof(Enum),val=>Expression.Lambda<Func<Enum>>(val).Compile()()},
              {typeof(byte[]),val=>Expression.Lambda<Func<byte[]>>(val).Compile()()}




         };

        public static object CompileNewExpression(NewExpression expression)
        {
            if (ConvertorPkDictionaryX.TryGetValue(expression.Type, out Func<NewExpression, object> value))
                return value.Invoke(expression);
            throw new Exception("I can't find the type to compile, call the developer");
        }

        public static bool IsCompile(Type type)
        {
            return Hlam.Contains(type);
        }
        public static bool IsCompileExpression(NewExpression expression, out object val)
        {
            val = CompileNewExpression(expression);
            return true;
        }

        public static bool IsValid<T>()
        {
            if (Hlam.Contains(typeof(T))) return false;
            if (typeof(T).IsGenericType) return false;
            return AttributesOfClass<T>.IsValidInner;
        }

        public static Type GetCoreType(Type t)
        {
            var s = Nullable.GetUnderlyingType(t);
            if (s != null)
            {
                return s;
            }

            return t;
        }


        internal static bool IsNumericType(Type type)
        {
            return NumericTypes.Contains(type) ||
                   NumericTypes.Contains(Nullable.GetUnderlyingType(type));
        }

        internal static string PrefParam(ProviderName providerName)
        {
            switch (providerName)
            {
                case ProviderName.MsSql:
                    return "@";
                case ProviderName.MySql:
                    return "?";
                case ProviderName.PostgreSql:
                    return "@";
                case ProviderName.SqLite:
                    return "@";
                default:
                    throw new ArgumentOutOfRangeException($"Database type is not defined:{providerName}");
            }
        }


        internal static bool IsAnonymousType(Type type)
        {
            return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
                   && type.IsGenericType && type.Name.Contains("AnonymousType")
                   && (type.Name.StartsWith("<>", StringComparison.OrdinalIgnoreCase) ||
                       type.Name.StartsWith("VB$", StringComparison.OrdinalIgnoreCase));
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


        private static readonly Dictionary<Type, Func<object, object>> ConvertorPkDictionary =
            new Dictionary<Type, Func<object, object>>
            {
                {typeof(string),val=>val.ToString()},
                {typeof(Guid),val=>(Guid)val},
                {typeof(decimal),val=>Convert.ToDecimal(val)},
                {typeof(short),val=>Convert.ToInt16(val)},
                {typeof(int),val=>Convert.ToInt32(val)},
                {typeof(long),val=>Convert.ToInt64(val)},
                {typeof(ushort),val=>Convert.ToUInt16(val)},
                {typeof(uint),val=>Convert.ToUInt32(val)},
                {typeof(ulong),val=>Convert.ToUInt64(val)},
                {typeof(double),val=>Convert.ToDouble(val)},
                {typeof(float),val=>Convert.ToSingle(val)},
            };

        internal static object ConverterPrimaryKeyType(Type type, object o)
        {
            //if(convertorPkDictionary.ContainsKey(type))
            //   return convertorPkDictionary[type].Invoke(o);
            if (ConvertorPkDictionary.TryGetValue(type, out Func<object, object> value))
                return value.Invoke(o);

            throw new Exception($"Can't find type to convert primary key {type} {o}");
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
            var list = new List<string>();
            str = str.Trim(' ', ',');
            var builder = new StringBuilder();
            var stop = 0;
            foreach (var c in str)
            {
                if (c == '(') stop = ++stop;
                if (c == ')') stop = --stop;
                if (stop > 0)
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

            if (builder.Length > 0) list.Add(builder.ToString());

            return list.ToArray();
        }

        public static void AddParamsSqlParam(IDbCommand com, List<SqlParam> list)
        {
            foreach (SqlParam sqlParam in list)
            {
                dynamic d = com.Parameters;
                d.AddWithValue(sqlParam.Name, sqlParam.Value ?? DBNull.Value);
            }
        }

        public static void AddParam(IDbCommand com, ProviderName providerName, object[] param)
        {
            if (param == null) return;
            if (param.Length == 0) return;
            var sql = com.CommandText;

            var regex = new Regex(@"\@([\w.$]+|""[^""]+""|'[^']+')");
            if (providerName == ProviderName.MySql) regex = new Regex(@"\?([\w.$]+|""[^""]+""|'[^']+')");
            MatchCollection matches = regex.Matches(sql);

            for (var index = 0; index < matches.Count; index++)
                if (param[index] is IDbDataParameter)
                {
                    com.Parameters.Add((IDbDataParameter)param[index]);
                }
                else
                {
                    dynamic d = com.Parameters;
                    d.AddWithValue(matches[index].Value, param[index] ?? DBNull.Value);
                }
        }

        public static Action CancelRegistr(IDbCommand com, CancellationToken cancellationToken,
            Transactionale transactionale, ProviderName providerName)
        {
            return () =>
            {
                try
                {
                    if (providerName == ProviderName.MySql)
                    {
                        dynamic s = com;
                        s.ExecuteNonQuery(cancellationToken);
                    }
                    else
                    {
                        com.Cancel();
                    }
                }
                catch (Exception ex)
                {
                    MySqlLogger.Error("cancellationToken", ex);
                }
                finally
                {
                    transactionale.isError = true;
                    cancellationToken.ThrowIfCancellationRequested();
                }
            };
        }


        public static string CheckAny(List<OneComposite> listOne)
        {
            foreach (var evolution in EvolutionsList)
            {
                var ss = listOne.FirstOrDefault(a => a.Operand == evolution);
                if (ss != null)
                    return ss.Body;
            }

            return null;
        }

        public static string ColumnBuilder(string type, bool isPk = false)
        {
            switch (type.ToLower().Trim())
            {
                case "blob":
                    {
                        if (isPk) return "Guid";
                        return "byte[]";
                    }
                case "text":
                    {
                        if (isPk) return "Guid";
                        return "string";
                    }
                case "integer": return "int";
                case "real": return "Int16";
                case "numeric": return "decimal";
                case "uuid": return "Guid";
                case "bit": return "bool";
                case "bigint": return "long";
                case "smallint": return "Int16";
                case "money": return "decimal";
                case "smallmoney": return "decimal";
                case "mediumint": return "int";
                case "tinyint": return "bool";
                case "boolean": return "bool";
            }

            if (type.ToLower().Contains("text")) return "string";
            if (type.ToLower().Contains("double")) return "double";
            if (type.ToLower().Contains("decimal")) return "decimal";
            if (type.ToLower().Contains("character")) return "string";
            if (type.ToLower().Contains("timestamp")) return "DateTime";
            if (type.ToLower().Contains("uniqueidentifier")) return "Guid";
            if (type.ToLower().Contains("varchar")) return "string";
            if (type.ToLower().Contains("datetime")) return "DateTime";

            return type;
        }

        private static readonly Dictionary<Type, Func<object, object>> DictionaryConvertor =
            new Dictionary<Type, Func<object, object>>
            {
                {typeof(uint), ob => Convert.ToUInt32(ob) },
                {typeof(ulong), ob => Convert.ToUInt64(ob) },
                {typeof(ushort), ob => Convert.ToUInt16(ob) },
                {typeof(bool), ob =>Convert.ToBoolean(ob)  },
                {typeof(byte), ob => Convert.ToByte(ob) },
                {typeof(char), ob =>Convert.ToChar(ob)  },
                {typeof(DateTime), ob => Convert.ToDateTime(ob) },
                {typeof(decimal), ob =>Convert.ToDecimal(ob)  },
                {typeof(double), ob =>Convert.ToDouble(ob)  },
                {typeof(short), ob => Convert.ToInt16(ob) },
                {typeof(int), ob => Convert.ToInt32(ob) },
                {typeof(long), ob => Convert.ToInt64(ob) },
                {typeof(sbyte), ob =>Convert.ToSByte(ob)  },
                {typeof(float), ob => Convert.ToSingle(ob) },
                {typeof(string), ob =>Convert.ToString(ob)  },
                {typeof(byte[]), ob => (byte[])ob },
                {typeof(Guid), ob =>
                { if (ob is Guid) return ob;
                    var guid = Guid.Empty;
                    if (ob is byte[] bytes)
                        return bytes.Length == 16 ? new Guid(bytes) : new Guid(Encoding.ASCII.GetString(bytes));

                    return ob is string ? new Guid(ob.ToString()) : guid;

                } },

            };
        internal static object Convertor(object ob, Type type)
        {
            try
            {
                if (ob is DBNull)
                    return null;
                type = GetCoreType(type);

                if (type.BaseType == typeof(Enum))
                {
                    return Enum.Parse(type, Convert.ToInt32(ob).ToString());
                }

                return DictionaryConvertor[type].Invoke(ob);
            }
            catch (Exception e)
            {
                var message = string.Format(CultureInfo.CurrentCulture, "Can't convert type {0} as {1}",
                    ob.GetType().FullName, type);
                throw new Exception(message, e);
            }
        }

        internal static bool IsNullableType(Type type)
        {
            return type != null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
        internal static bool IsPersistent<T>(T obj) where T : class
        {
            if (AttributesOfClass<T>.IsUsagePersistent.Value == false)
            {
                throw new Exception("The object type you are trying to modify does not have an attribute: MapUsagePersistentAttribute");
            }
            return TypeDescriptor.GetAttributes(obj).Contains(new PersistentAttribute());
        }


        internal static void SetPersistent(object obj)
        {
            TypeDescriptor.AddAttributes(obj, new PersistentAttribute());
        }


    }

}