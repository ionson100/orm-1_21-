using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using ORM_1_21_.Linq;
using ORM_1_21_.Utils;

namespace ORM_1_21_
{
    internal static class AttributesOfClass<T>
    {
        internal static readonly object LockO = new object();

        public static Lazy<bool> IsSerializableObject = new Lazy<bool>(() =>
        {
            var t = typeof(T).GetCustomAttribute(typeof(MapSerializableAttribute));
            if (t != null) return true;
            return false;
        }, LazyThreadSafetyMode.PublicationOnly);

        public static Lazy<bool> IsUssageActivatorInner = new Lazy<bool>(() =>
        {
            var t = typeof(T).GetCustomAttribute(typeof(MapUsageActivatorAttribute), true);
            if (t != null) return true;
            return false;
        }, LazyThreadSafetyMode.PublicationOnly);

        private static readonly Lazy<Dictionary<string, string>> ColumnName = new Lazy<Dictionary<string, string>>(() =>
        {
            var list = new List<BaseAttribute>();
            AttributeDall.Value.ToList().ForEach(a => list.AddRange(a.Value));
            PrimaryKeyLazy.Value.ToList().ForEach(a => list.AddRange(a.Value));
            return list.Any()
                ? list.Select(a => new { a.PropertyName, ColumnName = a.GetColumnName(CurProviderName) })
                    .ToDictionary(s => s.PropertyName, d => d.ColumnName)
                : null;
        }, LazyThreadSafetyMode.PublicationOnly);

        private static readonly Lazy<Dictionary<Type, List<MapColumnNameAttribute>>> AttributeDall =
            new Lazy<Dictionary<Type, List<MapColumnNameAttribute>>>(ActivateDallAll,
                LazyThreadSafetyMode.PublicationOnly);

        private static readonly Lazy<Dictionary<Type, string>> TableNameAllLazy =
            new Lazy<Dictionary<Type, string>>(ActivateTableNameAll, LazyThreadSafetyMode.PublicationOnly);


        private static readonly Lazy<Dictionary<Type, string>> SqlWhereAllLazy =
            new Lazy<Dictionary<Type, string>>(ActivateSqlWhereAllLazy, LazyThreadSafetyMode.PublicationOnly);

        private static readonly Lazy<Dictionary<Type, List<MapPrimaryKeyAttribute>>> PrimaryKeyLazy =
            new Lazy<Dictionary<Type, List<MapPrimaryKeyAttribute>>>(ActivatePrimaryKeyLazy,
                LazyThreadSafetyMode.PublicationOnly);

        private static readonly Lazy<List<BaseAttribute>> ListBaseAttr = new Lazy<List<BaseAttribute>>(() =>
        {
            var l = new List<BaseAttribute>();
            AttributeDall.Value.ToList().ForEach(a => l.AddRange(a.Value));
            PrimaryKeyLazy.Value.ToList().ForEach(a => l.AddRange(a.Value));
            return l;
        }, LazyThreadSafetyMode.PublicationOnly);

        public static Lazy<Dictionary<string, PropertyInfo>> PropertyInfoList =
            new Lazy<Dictionary<string, PropertyInfo>>(() =>
            {
                var d = new Dictionary<string, PropertyInfo>();
                typeof(T).GetProperties().ToList().ForEach(a => d.Add(a.Name, a));
                return d;
            }, LazyThreadSafetyMode.PublicationOnly);

        private static readonly Lazy<Dictionary<string, Func<T, object>>> GetValue =
            new Lazy<Dictionary<string, Func<T, object>>>(
                () =>
                {
                    var pr = typeof(T).GetProperties();
                    var dictionary = new Dictionary<string, Func<T, object>>();
                    foreach (var propertyInfo in pr)
                    {
                        var instance = Expression.Parameter(typeof(T));
                        var property = Expression.Property(instance, propertyInfo);
                        var convert = Expression.TypeAs(property, typeof(object));
                        dictionary.Add(propertyInfo.Name,
                            Expression.Lambda<Func<T, object>>(convert, instance).Compile());
                    }

                    return dictionary;
                }, LazyThreadSafetyMode.PublicationOnly);

        private static readonly Lazy<Dictionary<string, Action<T, object>>> SetValue =
            new Lazy<Dictionary<string, Action<T, object>>>(
                () =>
                {
                    var list = new Dictionary<string, Action<T, object>>();

                    var pr = typeof(T).GetProperties()
                        .Where(a => a.GetCustomAttributes(typeof(BaseAttribute), true).Any());


                    foreach (var propertyInfo in pr)
                    {
                        var instance = Expression.Parameter(propertyInfo.DeclaringType, "i");
                        var argument = Expression.Parameter(typeof(object), "a");
                        var setterCall = Expression.Call(
                            instance,
                            propertyInfo.GetSetMethod(),
                            Expression.Convert(argument, propertyInfo.PropertyType));
                        var res = (Action<T, object>)Expression.Lambda(setterCall, instance, argument)
                            .Compile();
                        list.Add(propertyInfo.Name, res);
                    }

                    return list;
                }, LazyThreadSafetyMode.PublicationOnly);

        private static readonly Lazy<Dictionary<string, Action<T, object>>> SetValueFreeSql =
            new Lazy<Dictionary<string, Action<T, object>>>(
                () =>
                {
                    var list = new Dictionary<string, Action<T, object>>();

                    var pr = typeof(T).GetProperties();


                    foreach (var propertyInfo in pr)
                    {
                        var instance = Expression.Parameter(propertyInfo.DeclaringType, "i");
                        var argument = Expression.Parameter(typeof(object), "a");
                        var setterCall = Expression.Call(
                            instance,
                            propertyInfo.GetSetMethod(),
                            Expression.Convert(argument, propertyInfo.PropertyType));
                        var res = (Action<T, object>)Expression.Lambda(setterCall, instance, argument)
                            .Compile();
                        list.Add(propertyInfo.Name, res);
                    }

                    return list;
                }, LazyThreadSafetyMode.PublicationOnly);

        private static readonly Lazy<bool> _isValid = new Lazy<bool>(() =>
                typeof(T).GetCustomAttributes(typeof(MapTableNameAttribute), false).Any(),
            LazyThreadSafetyMode.PublicationOnly);


        private static readonly Lazy<bool> _isReceiverFreeSql = new Lazy<bool>(
            () => { return typeof(T).GetCustomAttributes(typeof(MapReceiverFreeSqlAttribute), true).Any(); },
            LazyThreadSafetyMode.PublicationOnly);

        private static ProviderName CurProviderName
        {
            get
            {
                if (ProviderName == null)
                    return Configure.Provider;
                return ProviderName.Value;
            }
        }

        private static ProviderName Provider
        {
            set
            {
                lock (LockO)
                {
                    if (ProviderName == null) ProviderName = value;
                }
            }
        }

        public static string AllSqlWhereFromMap => GetSqlAll();

        internal static string SqlWhere => SqlWhereAllLazy.Value[typeof(T)];

        private static string SqlWhereBase =>
            SqlWhereAllLazy.Value.ContainsKey(typeof(T).BaseType)
                ? SqlWhereAllLazy.Value[typeof(T).BaseType]
                : string.Empty;

        public static bool IsValid => _isValid.Value;

        public static bool IsReceiverFreeSql => _isReceiverFreeSql.Value;


        private static ProviderName? ProviderName { get; set; }

        public static bool IsUssageActivator(ProviderName providerName)
        {
            Provider = providerName;
            return IsUssageActivatorInner.Value;
        }


        public static List<BaseAttribute> ListBaseAttrE(ProviderName providerName)
        {
            Provider = providerName;
            return ListBaseAttr.Value;
        }

        public static object GetValueE(ProviderName providerName, string k, T o)
        {
            Provider = providerName;
            return GetValue.Value[k](o);
        }


        public static void SetValueE(ProviderName providerName, string name, T t, object a)
        {
            Provider = providerName;
            SetValue.Value[name](t, a);
        }

        public static void SetValueFreeSqlE(ProviderName providerName, string name, T t, object a)
        {
            Provider = providerName;
            SetValueFreeSql.Value[name](t, a);
        }

        public static List<MapColumnNameAttribute> CurrentTableAttributeDall(ProviderName providerName)
        {
            Provider = providerName;
            return AttributeDall.Value[typeof(T)];
        }

        public static MapPrimaryKeyAttribute PkAttribute(ProviderName providerName)
        {
            Provider = providerName;
            return PrimaryKeyLazy.Value[typeof(T)].First();
        }

        public static string TableName(ProviderName providerName)
        {
            Provider = providerName;
            return TableNameAllLazy.Value[typeof(T)];
        }

        public static string TableNameRaw(ProviderName providerName)
        {
            return UtilsCore.ClearTrim(TableName(providerName));
        }

        public static string SimpleSqlSelect(ProviderName providerName)
        {
            Provider = providerName;
            return SimpleSelect(providerName);
        }

        private static Dictionary<Type, List<MapColumnNameAttribute>> ActivateDallAll()
        {
            var dictionary = new Dictionary<Type, List<MapColumnNameAttribute>>
                { { typeof(T), ActivateADall(CurProviderName) } };
            return dictionary;
        }

        private static Dictionary<Type, string> ActivateTableNameAll()
        {
            try
            {
                var providerName = CurProviderName;
                var dictionary = new Dictionary<Type, string>
                {
                    {
                        typeof(T),
                        ((MapTableNameAttribute)
                            typeof(T).GetCustomAttributes(typeof(MapTableNameAttribute), false).First())
                        .TableName(providerName)
                    }
                };
                if (typeof(T).BaseType != null)
                    RecursionTableName(dictionary, typeof(T).BaseType, providerName);
                return dictionary;
            }
            catch (Exception ex)
            {
                throw new Exception($"Perhaps you forgot to put the table name attribute (MapTableNameAttribute) in the type: {typeof(T)}", ex);
            }
           
        }

        public static string GetTypeTable(ProviderName providerName)
        {
            Provider = providerName;
            var o = typeof(T).GetCustomAttributes(typeof(MapTypeMysqlTableAttribute), true);
            if (o.Any()) return ((MapTypeMysqlTableAttribute)o.First()).TableType;

            return "";
        }

        private static void RecursionTableName(IDictionary<Type, string> dictionary, Type type,
            ProviderName providerName)
        {
            while (true)
            {
                var d = type.GetCustomAttributes(typeof(MapTableNameAttribute), false);
                if (!d.Any()) return;
                dictionary.Add(type, ((MapTableNameAttribute)d.First()).TableName(providerName));
                if (type.BaseType != null)
                {
                    type = type.BaseType;
                    continue;
                }

                break;
            }
        }

        private static Dictionary<Type, string> ActivateSqlWhereAllLazy()
        {
            var lazy = new Dictionary<Type, string>
            {
                {
                    typeof(T),
                    ((MapTableNameAttribute)
                        typeof(T).GetCustomAttributes(typeof(MapTableNameAttribute), false).First()).SqlWhere
                }
            };

            return lazy;
        }

        private static Dictionary<Type, List<MapPrimaryKeyAttribute>> ActivatePrimaryKeyLazy()
        {
            var lazy = new Dictionary<Type, List<MapPrimaryKeyAttribute>>();
            RecursionPrimaryKey(lazy, typeof(T), CurProviderName);
            return lazy;
        }

        private static void RecursionPrimaryKey(IDictionary<Type, List<MapPrimaryKeyAttribute>> dictionary, Type type,
            ProviderName
                providerName)
        {
            var l1 = new List<MapPrimaryKeyAttribute>();
            foreach (var f in type.GetProperties())
            {
                var oPk = f.GetCustomAttributes(typeof(MapPrimaryKeyAttribute), true);
                if (!oPk.Any()) continue;
                var pr = (MapPrimaryKeyAttribute)oPk[0];
                pr.IsBaseKey = false;
                pr.IsForeignKey = false;
                pr.PropertyName = f.Name;
                pr.DeclareType = type;
                pr.TypeColumn = f.PropertyType;
                pr.ColumnNameAlias = UtilsCore.GetAsAlias(TableNameAllLazy.Value[type], pr.GetColumnName(providerName));
                l1.Add(pr);
            }

            if (!l1.Any()) return;
            dictionary.Add(type, l1);
        }

        private static List<MapColumnNameAttribute> ActivateADall(ProviderName providerName)
        {
            var res = new List<MapColumnNameAttribute>();
            var fields = typeof(T).GetProperties();
            foreach (var f in fields)
            {
                var o3 = f.GetCustomAttributes(typeof(MapDefaultValueAttribute), true);
                var o1 = f.GetCustomAttributes(typeof(MapColumnNameAttribute), true);
                if (!o1.Any()) continue;

                var o2 = f.GetCustomAttributes(typeof(MapIndexAttribute), true);
                ((MapColumnNameAttribute)o1.First()).IsBaseKey = false;
                ((MapColumnNameAttribute)o1.First()).IsForeignKey = false;
                if (o2.Any()) ((MapColumnNameAttribute)o1.First()).IsIndex = true;

                ((MapColumnNameAttribute)o1.First()).PropertyName = f.Name;
                ((MapColumnNameAttribute)o1.First()).TypeColumn = f.PropertyType;
                ((MapColumnNameAttribute)o1.First()).DeclareType = typeof(T);
                if (o3.Any())
                    ((MapColumnNameAttribute)o1.First()).DefaultValue = ((MapDefaultValueAttribute)o3.First()).Value;

                ((MapColumnNameAttribute)o1.First()).ColumnNameAlias = UtilsCore.GetAsAlias(
                    TableNameAllLazy.Value[typeof(T)],
                    ((MapColumnNameAttribute)o1.First())
                    .GetColumnName(providerName));

                var o4 = f.GetCustomAttributes(typeof(MapColumnTypeAttribute), true);
                if (o4.Any())
                    ((MapColumnNameAttribute)o1.First()).TypeString = ((MapColumnTypeAttribute)o4.First()).TypeString;

                var o5 = f.GetCustomAttributes(typeof(MapNotInsertUpdateAttribute), true);
                if (o5.Any()) ((MapColumnNameAttribute)o1.First()).IsNotUpdateInsert = true;


                res.Add((MapColumnNameAttribute)o1.First());
            }

            return res;
        }

        private static string GetSqlAll()
        {
            var sb = new StringBuilder();
            if (!string.IsNullOrEmpty(SqlWhere)) sb.Append(SqlWhere);
            if (!string.IsNullOrEmpty(SqlWhere) && !string.IsNullOrEmpty(SqlWhereBase))
                sb.AppendFormat(" AND {0}", SqlWhereBase);
            if (string.IsNullOrEmpty(SqlWhere) && !string.IsNullOrEmpty(SqlWhereBase))
                sb.AppendFormat("{0}", SqlWhereBase);
            return sb.ToString();
        }

        public static string AddSqlWhere(string sqlWhere, ProviderName providerName)
        {
            Provider = providerName;
            sqlWhere = sqlWhere.Trim();
            if (!string.IsNullOrEmpty(sqlWhere) && string.IsNullOrEmpty(AllSqlWhereFromMap))
                return " WHERE " + sqlWhere;
            if (string.IsNullOrEmpty(sqlWhere) && !string.IsNullOrEmpty(AllSqlWhereFromMap))
                return " WHERE " + AllSqlWhereFromMap;
            if (!string.IsNullOrEmpty(sqlWhere) && !string.IsNullOrEmpty(AllSqlWhereFromMap))
                return " WHERE " + AllSqlWhereFromMap + " AND " + sqlWhere;
            return string.Empty;
        }

        public static IEnumerable<T> GetEnumerableObjects(IDataReader reader, ProviderName providerName)
        {
            Provider = providerName;
            Check.NotNull(reader, "IDataReader reader");
            var res = Pizdaticus.GetRiderToList<T>(reader, providerName);
            return res;
        }


        public static IEnumerable<T> GetEnumerableObjectsGroupBy<TT>(IDataReader reader, Delegate expDelegate,
            ProviderName providerName)
        {
            Provider = providerName;
            Check.NotNull(reader, "IDataReader reader");
            var rr = typeof(T).GetGenericArguments();
            var r = Pizdaticus.GetRiderToList<TT>(reader, providerName);
            var res = CallExp<T, TT>.GetTreeForGroupBy(r, expDelegate, rr[0]);
            return res;
        }

        public static string SimpleSelect(ProviderName providerName)
        {
            Provider = providerName;
            var sb = new StringBuilder();
            sb.Append(StringConst.Select + " ");
            foreach (var type in AttributeDall.Value.Keys.Reverse())
                if (providerName == ORM_1_21_.ProviderName.Postgresql)
                {
                    foreach (var a in AttributeDall.Value[type])
                        sb.AppendFormat($" {a.GetColumnName(providerName)},");

                    foreach (var pk in PrimaryKeyLazy.Value[type])
                        sb.AppendFormat($" {pk.GetColumnName(providerName)},");
                }
                else
                {
                    foreach (var a in AttributeDall.Value[type])
                        sb.AppendFormat(" {0}.{1} AS {2},",
                            TableNameAllLazy.Value[type],
                            a.GetColumnName(providerName),
                            UtilsCore.GetAsAlias(TableNameAllLazy.Value[type], a.GetColumnName(providerName)));

                    foreach (var pk in PrimaryKeyLazy.Value[type])
                        sb.AppendFormat(" {0}.{1} AS {2},",
                            TableNameAllLazy.Value[type],
                            pk.GetColumnName(providerName),
                            UtilsCore.GetAsAlias(TableNameAllLazy.Value[type], pk.GetColumnName(providerName)));
                }


            sb = new StringBuilder(sb.ToString().Trim(','));
            sb.AppendFormat(" FROM {0}", TableName(providerName));

            return sb.ToString();
        }


        public static void CreateUpdateCommandMysql(IDbCommand command, T item, ProviderName providerName,
            params AppenderWhere[] whereObjects)
        {
            Provider = providerName;
            var listType = TableNameAllLazy.Value.Keys.Reverse();
            var allSql = new StringBuilder();


            var i = 0;
            var sb = new StringBuilder();

            var enumerable = listType as Type[] ?? listType.ToArray();

            foreach (var type in enumerable)
            {
                sb.Clear();

                var par = new StringBuilder();
                foreach (var pra in AttributeDall.Value[type]
                             .Where(pra => !pra.IsBaseKey && !pra.IsForeignKey))
                {
                    if (pra.IsNotUpdateInsert) continue;
                    par.AppendFormat(" {0}.{1} = {3}p{2},", TableNameAllLazy.Value[type],
                        pra.GetColumnName(providerName), ++i,
                        UtilsCore.PrefParam(providerName));

                    IDataParameter pr = command.CreateParameter();
                    pr.ParameterName = string.Format("{1}p{0}", i, UtilsCore.PrefParam(providerName));
                    var prcore = item.GetType().GetProperties().First(a => a.Name == pra.PropertyName);
                    object vall;
                    var st = UtilsCore.GetSerializeType(prcore.PropertyType);
                    if (prcore.PropertyType == typeof(Image))
                        vall = UtilsCore.ImageToByte((Image)GetValue.Value[prcore.Name](item));
                    else if (st == SerializeType.Self)
                        vall = UtilsCore.ObjectToJson(GetValue.Value[prcore.Name](item));
                    else if (st == SerializeType.User)
                        vall = ((IMapSerializable)GetValue.Value[prcore.Name](item)).Serialize();
                    else if (prcore.PropertyType.BaseType == typeof(Enum))
                        vall = (int)GetValue.Value[prcore.Name](item);
                    else
                        vall = GetValue.Value[prcore.Name](item);

                    pr.Value = vall ?? DBNull.Value;
                    pr.DbType = pra.DbType();
                    command.Parameters.Add(pr);
                }


                sb.AppendFormat("UPDATE {0} SET {1} WHERE {0}.{2} = {4}p{3}  ", TableNameAllLazy.Value[type],
                    par.ToString().Trim(','),
                    PrimaryKeyLazy.Value[type].First().GetColumnName(providerName), ++i,
                    UtilsCore.PrefParam(providerName));

                IDataParameter pr1 = command.CreateParameter();
                pr1.ParameterName = string.Format("{1}p{0}", i, UtilsCore.PrefParam(providerName));
                var cname = PrimaryKeyLazy.Value[type].First();
                var sxs = item.GetType().GetProperties().First(a => a.Name == cname.PropertyName);


                var val1 = GetValue.Value[sxs.Name](item);
                //  var val1 = sxs.GetValue(item, null);
                pr1.Value = val1 ?? DBNull.Value;
                pr1.DbType = PrimaryKeyLazy.Value[type].First().DbType();
                command.Parameters.Add(pr1);
                allSql.Append(sb);
            }


            if (whereObjects != null && whereObjects.Length > 0)
                foreach (var o in whereObjects)
                {
                    var nameParam = $"{UtilsCore.PrefParam(providerName)}p{++i}";
                    allSql.Append($" AND {o.ColumnName} = {nameParam}");
                    dynamic d = command.Parameters;
                    d.AddWithValue(nameParam, o.Value);
                }

            command.CommandText = allSql + ";";
        }


        public static void CreateUpdateCommandPostgres(IDbCommand command, T item, ProviderName providerName,
            params AppenderWhere[] whereObjects)
        {
            Provider = providerName;
            var listType = TableNameAllLazy.Value.Keys.Reverse();
            var allSql = new StringBuilder();

            var sb = new StringBuilder();
            var i = 0;
            var enumerable = listType as Type[] ?? listType.ToArray();

            foreach (var type in enumerable)
            {
                sb.Clear();

                var par = new StringBuilder();
                foreach (var pra in AttributeDall.Value[type]
                             .Where(pra => !pra.IsBaseKey && !pra.IsForeignKey))
                {
                    if (pra.IsNotUpdateInsert) continue;
                    par.AppendFormat(" {0} = {2}p{1},", pra.GetColumnName(providerName), ++i,
                        UtilsCore.PrefParam(providerName));

                    IDataParameter pr = command.CreateParameter();
                    pr.ParameterName = string.Format("{1}p{0}", i, UtilsCore.PrefParam(providerName));
                    var prCore =
                        item.GetType()
                            .GetProperties()
                            .First(a => a.Name == pra.PropertyName);
                    object val;
                    var st = UtilsCore.GetSerializeType(prCore.PropertyType);
                    if (prCore.PropertyType == typeof(Image))
                        val = UtilsCore.ImageToByte((Image)GetValue.Value[prCore.Name](item));

                    else if (st == SerializeType.Self)
                        val = UtilsCore.ObjectToJson(GetValue.Value[prCore.Name](item));
                    else if (st == SerializeType.User)
                        val = ((IMapSerializable)GetValue.Value[prCore.Name](item)).Serialize();
                    else if (prCore.PropertyType.BaseType == typeof(Enum))
                        val = (int)GetValue.Value[prCore.Name](item);
                    else
                        val = GetValue.Value[prCore.Name](item);

                    pr.Value = val ?? DBNull.Value;
                    if (prCore.PropertyType.BaseType == typeof(Enum))
                        pr.DbType = DbTypeConverter.ConvertFrom(typeof(int));
                    else
                        pr.DbType = pra.DbType();

                    if (prCore.PropertyType == typeof(Guid) && providerName == ORM_1_21_.ProviderName.Sqlite)
                    {
                        pr.DbType = DbTypeConverter.ConvertFrom(typeof(string));
                        pr.Value = GetValue.Value[prCore.Name](item).ToString();
                    }

                    command.Parameters.Add(pr);
                }


                sb.AppendFormat("UPDATE {0} SET {1} WHERE {2} = {4}p{3} ", TableNameAllLazy.Value[type],
                    par.ToString().Trim(','),
                    PrimaryKeyLazy.Value[type].First().GetColumnName(providerName), ++i,
                    UtilsCore.PrefParam(providerName));

                IDataParameter pr1 = command.CreateParameter();
                pr1.ParameterName = string.Format("{1}p{0}", i, UtilsCore.PrefParam(providerName));
                var pk = PrimaryKeyLazy.Value[type].First();
                //var sxs = item.GetType().GetProperties().First(a => a.Name == cname.PropertyName);
                var val1 = GetValue.Value[pk.PropertyName](item);
                pr1.Value = val1 ?? DBNull.Value;
                pr1.DbType = PrimaryKeyLazy.Value[type].First().DbType();
                command.Parameters.Add(pr1);
                allSql.Append(sb);
            }

            if (whereObjects != null && whereObjects.Length > 0)
                foreach (var o in whereObjects)
                {
                    var nameParam = $"{UtilsCore.PrefParam(providerName)}p{++i}";
                    allSql.Append($" AND {o.ColumnName} = {nameParam}");
                    dynamic d = command.Parameters;
                    d.AddWithValue(nameParam, o.Value);
                }

            command.CommandText = allSql + ";";
        }

        public static string CreateCommandLimitForMySql(List<OneComposite> listOne, ProviderName providerName)
        {
            Provider = providerName;
            var si = SimpleSqlSelect(providerName);
            var sb = new StringBuilder();
            foreach (var oneComposite in listOne.Where(a => a.Operand == Evolution.Update))
            {
                var ee = UtilsCore.MySplit(oneComposite.Body); // .Trim().Trim(' ',',').Split(',');
                var eee = ee.ToList().Split(2);
                foreach (var s in eee)
                {
                    var enumerable = s as string[] ?? s.ToArray();
                    if (enumerable.Any()) sb.AppendFormat(" {0} = {1},", enumerable.First(), enumerable.Last());
                }
            }

            var where = new StringBuilder();
            foreach (var v in listOne.Where(a => a.Operand == Evolution.Where)) where.AppendFormat(" {0} AND", v.Body);
            if (!string.IsNullOrEmpty(where.ToString()))
                where = new StringBuilder(" WHERE " + where.ToString().Trim("AND".ToArray()));
            var from = si.Substring(si.IndexOf("FROM", StringComparison.Ordinal) + 4);

            if (providerName == ORM_1_21_.ProviderName.Postgresql || providerName == ORM_1_21_.ProviderName.Sqlite)
            {
                var str = string.Format("UPDATE {0} SET {1}  {2};", from, sb.ToString().Trim(','),
                    where.ToString().Trim(','));
                return str.Replace($"{TableName(providerName)}.", "");
            }

            var res = string.Format("UPDATE {0} SET {1}  {2};", from, sb.ToString().Trim(','),
                where.ToString().Trim(','));
            return res;
        }

        public static string CreateCommandUpdateFreeForMsSql(List<OneComposite> listOne, ProviderName providerName)
        {
            Provider = providerName;
            var si = SimpleSqlSelect(providerName);


            var r = new StringBuilder();
            foreach (var oneComprosite in listOne.Where(a => a.Operand == Evolution.Update))
            {
                var ee = UtilsCore.MySplit(oneComprosite.Body); // oneComprosite.Body.Trim().Trim(',').Split(',');
                //  var eee = ee.Split(2).ToList();
                foreach (var s in ee.Split(2))
                {
                    var enumerable = s as string[] ?? s.ToArray();
                    if (enumerable.ToList().Any())
                        r.AppendFormat(" {0} = {1},", enumerable.First(), enumerable.Last());
                }
            }

            var where = new StringBuilder();

            foreach (var v in listOne.Where(a => a.Operand == Evolution.Where)) where.AppendFormat(" {0} AND", v.Body);
            if (!string.IsNullOrEmpty(where.ToString()))
                where = new StringBuilder(" WHERE " + where.ToString().Trim("AND".ToArray()));


            var from = si.Substring(si.IndexOf("FROM", StringComparison.Ordinal));


            return string.Format(" UPDATE {0} SET {1} {2} {3}", TableNameAllLazy.Value[typeof(T)],
                r.ToString().Trim(','), from, where.ToString().Trim(','));
        }

        public static string CreateCommandLimitForMsSql(List<OneComposite> listOne, string doSql,
            ProviderName providerName)
        {
            Provider = providerName;
            const string table = "tt1";

            var dd = listOne.Single(a => a.Operand == Evolution.Limit).Body.Replace("LIMIT", "").Trim(' ').Split(',');

            var start = int.Parse(dd[0]);
            var count = int.Parse(dd[1]);


            var where = listOne.Where(a => a.Operand == Evolution.Where);
            var sbwhere = new StringBuilder();
            var sbOrderBy = new StringBuilder();
            var oneComprosites = where as OneComposite[] ?? where.ToArray();
            foreach (var oneComprosite in oneComprosites)
            {
                if (oneComprosite == oneComprosites.First())
                {
                    sbwhere.AppendFormat(" {0} ", oneComprosite.Body);
                    continue;
                }

                sbwhere.AppendFormat("AND  {0} ", oneComprosite.Body);
            }

            var ordrby = listOne.Where(a => a.Operand == Evolution.OrderBy);
            foreach (var oneComprosite in ordrby)
                sbOrderBy.AppendFormat("{0},", oneComprosite.Body);
            var ss = SimpleSqlSelect(providerName).Replace(StringConst.Select, "") +
                     AddSqlWhere(sbwhere.ToString(), providerName);
            var mat4 = new Regex(@"AS[^,]*").Matches(doSql.Substring(0, doSql.IndexOf("FROM", StringComparison.Ordinal))
                .Replace(StringConst.Select, "") + ",");
            var d = new StringBuilder();
            if (mat4.Count > 0)
            {
                foreach (var v in mat4) d.Append(v.ToString().Replace("AS", "") + " ,");
            }
            else
            {
                var sss =
                    doSql.Substring(0, doSql.IndexOf("FROM", StringComparison.Ordinal)).Replace(StringConst.Select, "")
                        .Split(',');
                foreach (var s in sss)
                    d.Append(s.Replace("]", "").Replace("[", "").Replace(".", UtilsCore.Bungalo).ToLower() + " ,");
            }

            start += 1;

            if (string.IsNullOrEmpty(sbOrderBy.ToString()))
                sbOrderBy.AppendFormat(" {0}.{1} ", TableNameAllLazy.Value[typeof(T)],
                    PrimaryKeyLazy.Value[typeof(T)].First().GetColumnName(providerName));


            if (listOne.Any(a => a.Operand == Evolution.ElementAtOrDefault || a.Operand == Evolution.ElementAt))
                //start += 1;
                count = 1; //= start;

            var ff = string.Format("SELECT {0} FROM (SELECT ROW_NUMBER() " +
                                   "OVER(ORDER BY {3} ) AS rownum, {1} ) " +
                                   "AS {2} WHERE rownum BETWEEN {4} AND {5}",
                d.ToString().Trim(','),
                ss,
                table,
                sbOrderBy.ToString().Trim(','),
                start,
                start + count - 1);


            return ff;
        }

        public static void CreateUpdateCommand(IDbCommand command, T item, ProviderName providerName)
        {
            Provider = providerName;
            var listType = TableNameAllLazy.Value.Keys.Reverse();
            var allSb = new StringBuilder();

            const string par = "p";
            var i = 0;

            var enumerable = listType as Type[] ?? listType.ToArray();
            foreach (var type in enumerable)
            {
                var sb = new StringBuilder();
                var sbvalues = new StringBuilder("");
                sb.AppendFormat("UPDATE {0} SET ", TableNameAllLazy.Value[type]);
                foreach (var pk in PrimaryKeyLazy.Value[type])
                {
                    if (pk.Generator == Generator.Native) continue;
                    if (pk.Generator == Generator.Assigned) continue;
                    sbvalues.AppendFormat(" {0}.{1}={2}{3}{4},", TableNameAllLazy.Value[type],
                        pk.GetColumnName(providerName),
                        UtilsCore.PrefParam(providerName), par, ++i);
                    IDataParameter pr = command.CreateParameter();
                    pr.ParameterName = string.Format("{0}{1}{2}", UtilsCore.PrefParam(providerName), par, i);
                    var val = GetValue.Value[pr.ParameterName](item);
                    pr.Value = val ?? DBNull.Value;
                    pr.DbType = pk.DbType();
                    command.Parameters.Add(pr);
                }

                foreach (var rtp in AttributeDall.Value[type])
                {
                    sbvalues.AppendFormat(" {0}.{1}={2}{3}{4},", TableNameAllLazy.Value[type],
                        rtp.GetColumnName(providerName),
                        UtilsCore.PrefParam(providerName), par, ++i);
                    IDataParameter pr = command.CreateParameter();
                    pr.ParameterName = string.Format("{0}{1}{2}", UtilsCore.PrefParam(providerName), par, i);
                    var val = GetValue.Value[rtp.PropertyName](item);
                    pr.Value = val ?? DBNull.Value;
                    pr.DbType = rtp.DbType();
                    command.Parameters.Add(pr);
                }

                sb.Append(sbvalues.ToString().Trim(',') + " ");
                sb.Append(SimpleSelect(providerName)
                    .Substring(SimpleSelect(providerName).IndexOf("FROM", StringComparison.Ordinal)));
                sb.Append(";");
                allSb.Append(sb);
            }

            command.CommandText = allSb.ToString();
        }

        public static void RedefiningPrimaryKey(T item, object val, ProviderName providerName)
        {
            Provider = providerName;
            var e = PrimaryKeyLazy.Value[typeof(T)].First();
            if (e.Generator != Generator.Native) return;
            var valCore = UtilsCore.ConverterPrimaryKeyType(e.TypeColumn, Convert.ToDecimal(val));
            item.GetType().GetProperty(e.PropertyName).SetValue(item, valCore, null);
        }

        public static void CreateInsetCommand(IDbCommand command, T obj, ProviderName providerName)
        {
            Provider = providerName;
            var listType = TableNameAllLazy.Value.Keys.Reverse();
            var allSb = new StringBuilder();
            var declare = new StringBuilder();
            const string par = "p";
            var i = 0;

            var enumerable = listType as Type[] ?? listType.ToArray();
            foreach (var type in enumerable)
            {
                var sb = new StringBuilder();
                var values = new StringBuilder(" VALUES (");
                sb.AppendFormat("INSERT INTO {0} (", TableNameAllLazy.Value[type]);
                foreach (var pk in PrimaryKeyLazy.Value[type])
                {
                    if (pk.IsNotUpdateInsert) continue;

                    if (pk.Generator != Generator.Assigned) continue;

                    if (providerName == ORM_1_21_.ProviderName.Postgresql ||
                        providerName == ORM_1_21_.ProviderName.Sqlite)
                        sb.AppendFormat($"{pk.GetColumnName(providerName)}, ");
                    else
                        sb.AppendFormat("{0}.{1}, ", TableNameAllLazy.Value[type], pk.GetColumnName(providerName));
                    if (pk.IsForeignKey)
                    {
                        values.Append(" @basekey, ");
                    }
                    else
                    {
                        values.AppendFormat("{0}{1}{2},", UtilsCore.PrefParam(providerName), par, ++i);
                        IDataParameter pr = command.CreateParameter(); // 
                        pr.ParameterName = string.Format("{0}{1}{2}", UtilsCore.PrefParam(providerName), par, i);
                        // var val = obj.GetType().GetProperty(pk.PropertyName).GetValue(obj, null);
                        var val = GetValue.Value[pk.PropertyName](obj);

                        pr.Value = val ?? DBNull.Value;
                        pr.DbType = pk.DbType();
                        command.Parameters.Add(pr);
                    }
                }


                foreach (var rtp in AttributeDall.Value[type])
                {
                    if (rtp.IsNotUpdateInsert) continue;

                    if (providerName == ORM_1_21_.ProviderName.Postgresql ||
                        providerName == ORM_1_21_.ProviderName.Sqlite)
                        sb.AppendFormat($"{rtp.GetColumnName(providerName)}, ");
                    else
                        sb.AppendFormat("{0}.{1},", TableNameAllLazy.Value[type], rtp.GetColumnName(providerName));

                    if (rtp.IsForeignKey)
                    {
                        values.Append(" @basekey, ");
                    }
                    else
                    {
                        var isEnum = false;
                        values.AppendFormat("{0}{1}{2},", UtilsCore.PrefParam(providerName), par, ++i);
                        IDataParameter pr = command.CreateParameter(); // ProviderFactories.GetParameter(providerName);
                        pr.ParameterName = $"{UtilsCore.PrefParam(providerName)}{par}{i}";
                        var prCore = obj.GetType().GetProperty(rtp.PropertyName);
                        object val;
                        var st = UtilsCore.GetSerializeType(prCore.PropertyType);
                        if (prCore.PropertyType == typeof(Image))
                        {
                            val = UtilsCore.ImageToByte((Image)GetValue.Value[prCore.Name](obj));
                        }
                        else if (prCore.PropertyType.BaseType == typeof(Enum))
                        {
                            val = (int)GetValue.Value[prCore.Name](obj);
                            isEnum = true;
                        }
                        else if (st == SerializeType.Self)
                        {
                            val = UtilsCore.ObjectToJson(GetValue.Value[prCore.Name](obj));
                        }
                        else if (st == SerializeType.User)
                        {
                            val = ((IMapSerializable)GetValue.Value[prCore.Name](obj)).Serialize();
                        }
                        else
                        {
                            val = GetValue.Value[prCore.Name](obj);
                        }

                        if (prCore.PropertyType == typeof(Guid) && providerName == ORM_1_21_.ProviderName.Sqlite)
                        {
                            pr.Value = val.ToString();
                            pr.DbType = DbTypeConverter.ConvertFrom(typeof(string));
                        }
                        else
                        {
                            pr.Value = val ?? DBNull.Value;
                            pr.DbType = isEnum ? DbTypeConverter.ConvertFrom(typeof(int)) : rtp.DbType();
                        }


                        command.Parameters.Add(pr);
                    }
                }

                sb = new StringBuilder(sb.ToString().Trim(' ', ',') + ") ");
                sb.Append(values.ToString().Trim(' ', ',') + ") ");
                allSb.Append(sb);
            }

            string ss = allSb.ToString();

            if (PkAttribute(providerName).Generator == Generator.Native)
            {
                //allSb.Append(";");
                switch (providerName)
                {
                    case ORM_1_21_.ProviderName.MsSql:
                    {
                       
                        allSb.Append($" SELECT IDENT_CURRENT ('{TableNameAllLazy.Value[typeof(T)]}')");
                        break;
                    }
                    case ORM_1_21_.ProviderName.Postgresql:
                    {
                        allSb.Append($" RETURNING {PkAttribute(providerName).GetColumnName(providerName)}");
                        break;
                    }
                    case ORM_1_21_.ProviderName.Sqlite:
                    {
                        allSb.Append(";select last_insert_rowid()");
                        break;
                    }
                    case ORM_1_21_.ProviderName.MySql:
                    {
                        allSb.Append("; SELECT LAST_INSERT_ID()");
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            

            command.CommandText = allSb+";";
        }

        public static void CreateDeleteCommand(IDbCommand command, T obj, ProviderName providerName)
        {
            Provider = providerName;
            command.CommandText = string.Format(" DELETE FROM {0} WHERE {0}.{1} = {2}p1",
                TableNameAllLazy.Value[typeof(T)],
                PrimaryKeyLazy.Value[typeof(T)].First().GetColumnName(providerName), UtilsCore.PrefParam(providerName));
            IDataParameter pr = command.CreateParameter();
            pr.ParameterName = string.Format("{0}p1", UtilsCore.PrefParam(providerName));
            var prr = obj.GetType().GetProperty(PrimaryKeyLazy.Value[typeof(T)].First().PropertyName);
            var val = GetValue.Value[prr.Name](obj);

            // .GetValue(obj, null);
            pr.Value = val ?? DBNull.Value;
            pr.DbType = PrimaryKeyLazy.Value[typeof(T)].First().DbType();
            command.Parameters.Add(pr);
        }

        public static string GetNameFieldForQuery(string member, Type type, ProviderName providerName)
        {
            Provider = providerName;
            return TableNameAllLazy.Value[type] + "." + ColumnName.Value[member];
        }

        //private static readonly Lazy<bool> _isUserSerialization = new Lazy<bool>(() =>
        //{
        //   return typeof(IMapSerializable).IsAssignableFrom(typeof(T));
        //}, LazyThreadSafetyMode.PublicationOnly);
        //
        //public static bool IsUserSerialization = _isUserSerialization.Value;
    }
}