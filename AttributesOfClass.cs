using ORM_1_21_.geo;
using ORM_1_21_.Linq;
using ORM_1_21_.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace ORM_1_21_
{

    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    [SuppressMessage("ReSharper", "StaticMemberInGenericType")]
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    internal static partial class AttributesOfClass<T>
    {
        internal static readonly object LockO = new object();

        public static Dictionary<Type, TypeReturning> JsonTypeReturning = new Dictionary<Type, TypeReturning>();
        public static Dictionary<string, TypeReturning> JsonTypeReturningName = new Dictionary<string, TypeReturning>();


        public static TypeReturning? IsJsonTypeName(string name)
        {
            if (JsonTypeReturningName.ContainsKey(name))
            {
                return JsonTypeReturningName[name];
            }

            return null;

        }

        public static bool IsJsonName(string name)
        {
            var res = JsonTypeReturningName.ContainsKey(name);
            return res;
        }


        public static bool IsJson(Type type)
        {
            return JsonTypeReturning.ContainsKey(type);
        }

        public static Lazy<bool> IsUsageActivatorInner = new Lazy<bool>(() =>
        {
            var t = typeof(T).GetCustomAttribute(typeof(MapUsageActivatorAttribute), false);
            if (t != null) return true;
            return false;
        }, LazyThreadSafetyMode.PublicationOnly);


        public static Lazy<bool> IsUsagePersistent = new Lazy<bool>(() =>
        {
            var t = typeof(T).GetCustomAttribute(typeof(MapUsagePersistentAttribute), false);
            if (t != null) return true;
            return false;
        }, LazyThreadSafetyMode.ExecutionAndPublication);


        private static readonly Lazy<Dictionary<string, string>> ColumnName = new Lazy<Dictionary<string, string>>(() =>
        {
            var list = new List<BaseAttribute>();
            AttributeDalList.Value.ToList().ForEach(a => list.Add(a));
            list.Add(PrimaryKeyAttribute.Value);
            return list.Count > 0
                ? list.Select(a => new { a.PropertyName, ColumnName = a.GetColumnName(Provider) })
                    .ToDictionary(s => s.PropertyName, d => d.ColumnName)
                : null;
        }, LazyThreadSafetyMode.PublicationOnly);

        private static readonly Lazy<Dictionary<string, bool>> ColumnIsJson = new Lazy<Dictionary<string, bool>>(() =>
        {
            Dictionary<string, bool> resDictionary = new Dictionary<string, bool>();
            AttributeDalList.Value.ToList().ForEach(a => { resDictionary.Add(a.PropertyName, a.IsJson); });
            return resDictionary;
        }, LazyThreadSafetyMode.PublicationOnly);


        private static readonly Lazy<List<MapColumnAttribute>> AttributeDalList =
            new Lazy<List<MapColumnAttribute>>(GetListActivateDallAll, LazyThreadSafetyMode.PublicationOnly);


        private static readonly Lazy<MapTableAttribute> TableAttribute =
            new Lazy<MapTableAttribute>(GetTableAttribute, LazyThreadSafetyMode.PublicationOnly);


        private static readonly Lazy<string> SqlWhereLazy =
            new Lazy<string>(ActivateSqlWhere, LazyThreadSafetyMode.PublicationOnly);

        private static readonly Lazy<MapPrimaryKeyAttribute> PrimaryKeyAttribute =
            new Lazy<MapPrimaryKeyAttribute>(GetPrimaryKeyAttribute, LazyThreadSafetyMode.ExecutionAndPublication);

        private static readonly Lazy<List<BaseAttribute>> ListBaseAttr = new Lazy<List<BaseAttribute>>(() =>
        {
            var l = new List<BaseAttribute> { PrimaryKeyAttribute.Value };
            AttributeDalList.Value.ToList().ForEach(a => l.Add(a));

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
                        dictionary.Add(propertyInfo.Name, Expression.Lambda<Func<T, object>>(convert, instance).Compile());
                    }

                    return dictionary;
                }, LazyThreadSafetyMode.PublicationOnly);

        private static readonly Lazy<Dictionary<string, Action<T, object>>> SetValue =
            new Lazy<Dictionary<string, Action<T, object>>>(
                () =>
                {
                    var list = new Dictionary<string, Action<T, object>>();

                    var pr = typeof(T).GetProperties()
                        .Where(a => a.GetCustomAttributes(typeof(BaseAttribute), true).Length > 0);


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
                        MethodInfo method = UnaryConverter.GetMethodInfo(Provider, propertyInfo.PropertyType);
                        UnaryExpression unary = Expression.Convert(argument, propertyInfo.PropertyType, method);
                        var setterCall = Expression.Call(instance, propertyInfo.GetSetMethod(), unary);
                        var res = (Action<T, object>)Expression.Lambda(setterCall, instance, argument)
                            .Compile();
                        list.Add(propertyInfo.Name, res);
                    }

                    return list;
                }, LazyThreadSafetyMode.PublicationOnly);

        private static readonly Lazy<bool> IsValid = new Lazy<bool>(() =>
                typeof(T).GetCustomAttribute<MapTableAttribute>(true) != null,
            LazyThreadSafetyMode.PublicationOnly);


        private static readonly Lazy<bool> IsReceiverFreeSql = new Lazy<bool>(
            () => typeof(T).GetCustomAttributes(typeof(MapReceiverFreeSqlAttribute), true).Length > 0,
            LazyThreadSafetyMode.PublicationOnly);

        static AttributesOfClass()
        {
            StorageTypeAttribute.DictionaryAttribute.TryAdd(typeof(T), new ProxyAttribute<T>());
        }



        private static ProviderName Provider
        {
            get
            {
                if (ProviderName == null)
                {
                    throw new Exception("Provider null..");
                }

                return ProviderName.Value;
            }
            set
            {
                if (ProviderName == null)
                {
                    lock (LockO)
                    {

                        ProviderName = value;
                    }
                }
            }
        }

        public static string AllSqlWhereFromMap => GetSqlAll();

        internal static string SqlWhere => SqlWhereLazy.Value;

        private static string SqlWhereBase => SqlWhere;

        public static bool IsValidInner => IsValid.Value;

        public static bool IsReceiverFreeSqlInner => IsReceiverFreeSql.Value;

        private static ProviderName? ProviderName { get; set; }

        public static object GetValuePrimaryKey(T t)
        {
            var p = PkAttribute(Provider);
            return GetValueE(Provider, p.PropertyName, t);
        }

        private static List<MapColumnAttribute> GetListActivateDallAll()
        {
            var res = new List<MapColumnAttribute>();
            var fields = typeof(T).GetProperties();
            foreach (var f in fields)
            {
                var o3 = f.GetCustomAttribute<MapDefaultValueAttribute>(true);
                var columnAttribute = f.GetCustomAttribute<MapColumnAttribute>(true);
                if (columnAttribute == null) continue;

                var s = columnAttribute.GetColumnNameRaw();
                if (string.IsNullOrWhiteSpace(s))
                {
                    var n = f.Name;
                    columnAttribute.SetColumnNameRaw(n);
                }

                var o2 = f.GetCustomAttribute<MapIndexAttribute>(true);
                var o6 = f.GetCustomAttribute<MapColumnTypeJson>(true);
                columnAttribute.IsBaseKey = false;
                columnAttribute.IsForeignKey = false;
                if (o2 != null) columnAttribute.IsIndex = true;


                columnAttribute.PropertyName = f.Name;
                columnAttribute.PropertyType = f.PropertyType;
                columnAttribute.IsInheritIGeoShape = columnAttribute.PropertyType == typeof(IGeoShape);//    columnAttribute.PropertyType.GetInterfaces().Contains(typeof(IGeoShape));
                columnAttribute.DeclareType = typeof(T);
                if (o3 != null)
                    columnAttribute.DefaultValue = o3.Value;

                columnAttribute.ColumnNameAlias = UtilsCore.GetAsAlias(
                    TableAttribute.Value.TableName(Provider),
                    columnAttribute
                        .GetColumnName(Provider));

                var o4 = f.GetCustomAttribute<MapColumnTypeAttribute>(true);
                if (o4 != null)
                    columnAttribute.TypeString = o4.TypeString;

                var o5 = f.GetCustomAttribute<MapNotInsertUpdateAttribute>(true);
                if (o5 != null) columnAttribute.IsNotUpdateInsert = true;

                if (o6 != null)
                {


                    columnAttribute.IsJson = true;
                    columnAttribute.TypeReturning = o6.Returning;
                    if (columnAttribute.PropertyType == typeof(string))
                    {
                        throw new Exception(
                            $"Propery: {columnAttribute.PropertyName} type: {typeof(T)} сannot be a string type, perhaps you wanted to use the Object type");
                    }
                    if (columnAttribute.PropertyType != typeof(Object) && columnAttribute.TypeReturning == TypeReturning.AsString)
                    {
                        throw new Exception(
                            $"I can't return an object of type {columnAttribute.PropertyType} as a string." +
                            $"for property: {columnAttribute.PropertyName}, type: {typeof(T)}");
                    }
                    // todo 3.0.5
                    if (JsonTypeReturning.ContainsKey(columnAttribute.PropertyType) == false)
                    {
                        JsonTypeReturning.Add(columnAttribute.PropertyType, columnAttribute.TypeReturning);
                    }

                    JsonTypeReturningName.Add(columnAttribute.GetColumnNameRaw(), columnAttribute.TypeReturning);
                    JsonTypeReturningName.Add(columnAttribute.PropertyName, columnAttribute.TypeReturning);
                    //UtilsCore.JsonTypeReturning.Add(columnAttribute.PropertyType,columnAttribute.TypeReturning);
                    //UtilsCore.HashSetJsonType.Add(columnAttribute.PropertyType);

                }
                res.Add(columnAttribute);
            }

            return res;
        }

        private static MapTableAttribute GetTableAttribute()
        {

            var d = typeof(T).GetCustomAttribute<MapTableAttribute>(true);

            if (d == null) throw new Exception($"There is no attribute: MapTableAttribute that defines the name of the table for type: {typeof(T).Name}");
            if (d.IsTableNameEmpty()) d.SetTableName(typeof(T).Name);
            return d;
        }

        private static string ActivateSqlWhere()
        {
            var r = typeof(T).GetCustomAttribute<MapTableAttribute>(true);
            if (r == null) return string.Empty;
            return r.SqlWhere;
        }

        private static MapPrimaryKeyAttribute GetPrimaryKeyAttribute()
        {
            foreach (var f in typeof(T).GetProperties())
            {
                var pr = f.GetCustomAttribute<MapPrimaryKeyAttribute>(true);
                if (pr == null) continue;
                if (string.IsNullOrWhiteSpace(pr.GetColumnNameRaw())) pr.SetColumnNameRaw(f.Name);
                pr.IsBaseKey = false;
                pr.IsForeignKey = false;
                pr.PropertyName = f.Name;
                pr.PropertyType = f.PropertyType;
                pr.DeclareType = typeof(T);
                var o3 = f.GetCustomAttribute<MapDefaultValueAttribute>(true);
                if (o3 != null)
                    pr.DefaultValue = o3.Value;
                pr.ColumnNameAlias =
                    UtilsCore.GetAsAlias(TableName(Provider), pr.GetColumnName(Provider));
                var o4 = f.GetCustomAttribute<MapColumnTypeAttribute>(true);
                if (o4 != null)
                    pr.TypeString = o4.TypeString;
                return pr;
            }

            throw new Exception("Perhaps you forgot to use the MapPrimaryKeyAttribute, for a class property");
        }

        public static bool IsUsageActivator(ProviderName providerName)
        {
            Provider = providerName;
            return IsUsageActivatorInner.Value;
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

        public static void SetValueFreeSqlE(ProviderName providerName, string name, T t, object o)
        {
            Provider = providerName;
           
            if (SetValueFreeSql.Value.TryGetValue(name, out Action<T, object> value))
            {
                value(t, o);
            }
        }

        public static List<MapColumnAttribute> CurrentTableAttributeDal(ProviderName providerName)
        {
            Provider = providerName;
            return AttributeDalList.Value;
        }

        public static MapPrimaryKeyAttribute PkAttribute(ProviderName providerName)
        {
            Provider = providerName;
            return PrimaryKeyAttribute.Value;
        }

        public static string TableName(ProviderName providerName)
        {
            Provider = providerName;
            return TableAttribute.Value.TableName(providerName);
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


        public static string GetTypeTable(ProviderName providerName)
        {
            Provider = providerName;
            var o = typeof(T).GetCustomAttribute<MapTypeMysqlTableAttribute>(true);
            return o != null ? o.TableType : string.Empty;
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

        public static IEnumerable<T> GetEnumerableObjects(IDataReader reader, ProviderName providerName, bool isFree = false)
        {
            Provider = providerName;
            Check.NotNull(reader, "IDataReader reader");
            var res = Pizdaticus.GetRiderToList<T>(reader, providerName, isFree);
            return res;
        }


        public static string SimpleSelect(ProviderName providerName)
        {
            Provider = providerName;
            var sb = new StringBuilder();
            sb.Append(StringConst.Select + " ");
            var pk = PrimaryKeyAttribute.Value;
            if (providerName == ORM_1_21_.ProviderName.PostgreSql)
            {
                foreach (var a in AttributeDalList.Value)
                    sb.AppendFormat($" {a.GetColumnName(providerName)},");
                sb.AppendFormat($" {pk.GetColumnName(providerName)},");
            }
            else
            {
                sb.AppendFormat(" {0}.{1} AS {2},",
                    TableAttribute.Value.TableName(providerName),
                    pk.GetColumnName(providerName),
                    UtilsCore.GetAsAlias(TableAttribute.Value.TableName(providerName),
                        pk.GetColumnName(providerName)));
                foreach (var a in AttributeDalList.Value)
                {
                    if (a.IsInheritIGeoShape && Provider == ORM_1_21_.ProviderName.MsSql)
                    {
                        string colName = $"{TableAttribute.Value.TableName(providerName)}.{a.GetColumnName(providerName)}";
                        string asE = UtilsCore.GetAsAlias(TableAttribute.Value.TableName(providerName),
                            a.GetColumnName(providerName));
                        sb.Append($"{UtilsCore.SqlConcat(colName, providerName)} AS {asE},");
                       
                    }
                    else
                    {
                        sb.AppendFormat(" {0}.{1} AS {2},",
                            TableAttribute.Value.TableName(providerName),
                            a.GetColumnName(providerName),
                            UtilsCore.GetAsAlias(TableAttribute.Value.TableName(providerName),
                                a.GetColumnName(providerName)));
                    }

                }

            }

            sb = new StringBuilder(sb.ToString().Trim(','));
            sb.AppendFormat(" FROM {0}", TableName(providerName));

            return sb.ToString();
        }

        private static readonly Lazy<string> UpdateTemplateMysql =
            new Lazy<string>(CreateUpdateTemplateMysql, LazyThreadSafetyMode.PublicationOnly);

        private static string CreateUpdateTemplateMysql()
        {
            string parName = UtilsCore.PrefParam(Provider);
            var allSql = new StringBuilder();
            var i = 0;
            var sb = new StringBuilder();
            var par = new StringBuilder();

            foreach (var pra in AttributeDalList.Value
                         .Where(pra => !pra.IsBaseKey && !pra.IsForeignKey))
            {
                if (pra.IsNotUpdateInsert) continue;
                if (pra.IsInheritIGeoShape)
                {
                    if (Provider == ORM_1_21_.ProviderName.MsSql)
                    {
                        par.AppendFormat(" {0}.{1} = geometry::STGeomFromText({3}p{2},{4}),", TableAttribute.Value.TableName(Provider),
                            pra.GetColumnName(Provider), ++i, parName, $"{parName}srid{i}");
                    }
                    else
                    {
                        par.AppendFormat(" {0}.{1} = ST_GeomFromText({3}p{2},{4}),", TableAttribute.Value.TableName(Provider),
                            pra.GetColumnName(Provider), ++i, parName, $"{parName}srid{i}");
                    }

                }
                else
                {
                    par.AppendFormat(" {0}.{1} = {3}p{2},", TableAttribute.Value.TableName(Provider),
                        pra.GetColumnName(Provider), ++i, parName);
                }

            }

            var pk = PrimaryKeyAttribute.Value;
            sb.AppendFormat("UPDATE {0} SET {1} WHERE {0}.{2} = {4}p{3}  ",
                TableAttribute.Value.TableName(Provider),
                par.ToString().Trim(','),
                pk.GetColumnName(Provider), ++i, parName);

            return allSql.Append(sb).ToString();
        }

        public static void CreateUpdateCommandMysql(IDbCommand command, T item, ProviderName providerName,
           params AppenderWhere[] whereObjects)
        {
            Provider = providerName;
            var sql = UpdateTemplateMysql.Value;
            string parName = UtilsCore.PrefParam(providerName);
            var i = 0;
            foreach (var pra in AttributeDalList.Value
                         .Where(pra => !pra.IsBaseKey && !pra.IsForeignKey))
            {
                if (pra.IsNotUpdateInsert) continue;
                command.AddParameter(string.Format("{1}p{0}", ++i, parName), GetValue.Value[pra.PropertyName](item));
            }

            var pk = PrimaryKeyAttribute.Value;
            command.AddParameter(string.Format("{1}p{0}", ++i, parName), GetValue.Value[pk.PropertyName](item));

            if (whereObjects != null && whereObjects.Length > 0)
            {
                StringBuilder builder = new StringBuilder(sql);
                foreach (var o in whereObjects)
                {
                    var nameParam = $"{parName}p{++i}";
                    builder.Append($" AND {o.ColumnName} = {nameParam}");
                    dynamic d = command.Parameters;
                    d.AddWithValue(nameParam, o.Value);
                }
                command.CommandText = builder.Append(';').ToString();
            }
            else
            {
                command.CommandText = sql + ";";
            }

        }

        public static readonly Lazy<string> TemplateUpdatePostgres =
            new Lazy<string>(CreateTemplateUpdatePostgres, LazyThreadSafetyMode.PublicationOnly);

        private static string CreateTemplateUpdatePostgres()
        {
            string parName = UtilsCore.PrefParam(Provider);
            var allSql = new StringBuilder();
            var sb = new StringBuilder();
            var i = 0;

            var par = new StringBuilder();
            foreach (var pra in AttributeDalList.Value
                         .Where(pra => !pra.IsBaseKey && !pra.IsForeignKey))
            {
                if (pra.IsNotUpdateInsert) continue;
                if (pra.IsJson)//todo geo
                {
                    if (Provider == ORM_1_21_.ProviderName.SqLite)
                    {
                        par.AppendFormat(" {0} = {2}p{1},", pra.GetColumnName(Provider), ++i, parName);
                    }
                    else
                    {
                        par.AppendFormat(" {0} = CAST({2}p{1} AS JSON),", pra.GetColumnName(Provider), ++i, parName);
                    }

                }
                else if (pra.IsInheritIGeoShape)
                {
                    par.AppendFormat(" {0} = ST_GeomFromText({2}p{1},{3}),", pra.GetColumnName(Provider), ++i, parName, $"{parName}srid{i}");
                }
                else
                {
                    par.AppendFormat(" {0} = {2}p{1},", pra.GetColumnName(Provider), ++i, parName);
                }

            }

            var pk = PrimaryKeyAttribute.Value;
            sb.AppendFormat("UPDATE {0} SET {1} WHERE {2} = {4}p{3} ", TableAttribute.Value.TableName(Provider),
                par.ToString().Trim(','),
                pk.GetColumnName(Provider), ++i, parName);

            return allSql.Append(sb).ToString();
        }

        

        public static string CreateCommandLimitForMySql(List<OneComposite> listOne, ProviderName providerName)
        {
            Provider = providerName;

            var si = SimpleSqlSelect(providerName);
            var sb = new StringBuilder();
            foreach (var oneComposite in listOne.Where(a => a.Operand == Evolution.Update))
            {
                sb.Append($" {oneComposite.Body} ");

            }

            var where = new StringBuilder();
            foreach (var v in listOne.Where(a => a.Operand == Evolution.Where)) where.AppendFormat(" {0} AND", v.Body);
            if (!string.IsNullOrEmpty(where.ToString()))
                where = new StringBuilder(" WHERE " + where.ToString().Trim("AND".ToArray()));
            var from = si.Substring(si.IndexOf("FROM", StringComparison.Ordinal) + 4);

            if (providerName == ORM_1_21_.ProviderName.PostgreSql || providerName == ORM_1_21_.ProviderName.SqLite)
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
            foreach (var oneComposite in listOne.Where(a => a.Operand == Evolution.Update))
            {
                r.Append($" {oneComposite.Body} ");
            }

            var where = new StringBuilder();

            foreach (var v in listOne.Where(a => a.Operand == Evolution.Where)) where.AppendFormat(" {0} AND", v.Body);
            if (!string.IsNullOrEmpty(where.ToString()))
                where = new StringBuilder(" WHERE " + where.ToString().Trim("AND".ToArray()));


            var from = si.Substring(si.IndexOf("FROM", StringComparison.Ordinal));


            return string.Format(" UPDATE {0} SET {1} {2} {3}", TableAttribute.Value.TableName(providerName),
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
            var sbWhere = new StringBuilder();
            var sbOrderBy = new StringBuilder();
            var oneComposites = where as OneComposite[] ?? where.ToArray();
            foreach (var oneComposite in oneComposites)
            {
                if (oneComposite == oneComposites.First())
                {
                    sbWhere.AppendFormat(" {0} ", oneComposite.Body);
                    continue;
                }
                sbWhere.AppendFormat("AND  {0} ", oneComposite.Body);
            }

            var orderBy = listOne.Where(a => a.Operand == Evolution.OrderBy);
            foreach (var oneComposite in orderBy)
                sbOrderBy.AppendFormat("{0},", oneComposite.Body);
            var ss = SimpleSqlSelect(providerName).Replace(StringConst.Select, "") +
                     AddSqlWhere(sbWhere.ToString(), providerName);
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
                sbOrderBy.AppendFormat(" {0}.{1} ", TableAttribute.Value.TableName(providerName),
                    PrimaryKeyAttribute.Value.GetColumnName(providerName));


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


        public static void RedefiningPrimaryKey(T item, object val, ProviderName providerName)
        {
            Provider = providerName;
            var e = PrimaryKeyAttribute.Value;
            if (e.Generator != Generator.Native) return;
            //var valCore = UtilsCore.ConverterPrimaryKeyType(e.PropertyType, Convert.ToDecimal(val));
            var valCore = UtilsCore.ConverterPrimaryKeyType(e.PropertyType, val);
            SetValue.Value[e.PropertyName](item, valCore);
        }



        public static string GetNameFieldForQuery(string member, ProviderName providerName)
        {
            Provider = providerName;
            return TableAttribute.Value.TableName(providerName) + "." + ColumnName.Value[member];
        }

        public static bool GetIsJson(string member, ProviderName providerName)
        {
            Provider = providerName;
            return ColumnIsJson.Value.ContainsKey(member);
        }

        public static string GetNameSimpleColumnForQuery(string member, ProviderName providerName)
        {
            Provider = providerName;
            return ColumnName.Value[member];
        }

        public static void Init()
        {
        }

        private static readonly Lazy<string> InsertTemplate =
            new Lazy<string>(CreateInsertTemplate, LazyThreadSafetyMode.PublicationOnly);

        private static string CreateInsertTemplate()
        {
          
            string parName = UtilsCore.PrefParam(Provider);

            var sb = new StringBuilder();
            const string par = "p";
            var i = 0;
            var values = new StringBuilder(" VALUES (");

            sb.AppendFormat("INSERT INTO {0} (", TableAttribute.Value.TableName(Provider));
            var pk = PrimaryKeyAttribute.Value;

            if (pk.IsNotUpdateInsert || pk.Generator != Generator.Assigned)
            {

            }
            else
            {
                if (Provider == ORM_1_21_.ProviderName.PostgreSql ||
                    Provider == ORM_1_21_.ProviderName.SqLite)
                    sb.AppendFormat($"{pk.GetColumnName(Provider)}, ");
                else
                    sb.AppendFormat("{0}.{1}, ", TableAttribute.Value.TableName(Provider),
                        pk.GetColumnName(Provider));

                values.AppendFormat("{0}{1}{2},", parName, par, ++i);

            }
            foreach (var rtp in AttributeDalList.Value)
            {
                if (rtp.IsNotUpdateInsert) continue;

                if (Provider == ORM_1_21_.ProviderName.PostgreSql || Provider == ORM_1_21_.ProviderName.SqLite)
                    sb.AppendFormat($"{rtp.GetColumnName(Provider)}, ");
                else
                    sb.AppendFormat("{0}.{1},", TableAttribute.Value.TableName(Provider), rtp.GetColumnName(Provider));

                if (rtp.IsJson)
                {
                    if (Provider == ORM_1_21_.ProviderName.SqLite || Provider == ORM_1_21_.ProviderName.MsSql)
                    {
                        values.AppendFormat("{0}{1}{2},", parName, par, ++i);
                    }
                    else
                    {
                        values.AppendFormat("CAST({0}{1}{2} AS json),", parName, par, ++i);//todo geo
                    }

                }
                else if (rtp.IsInheritIGeoShape)
                {
                    if (Provider == ORM_1_21_.ProviderName.MsSql)
                    {
                        values.AppendFormat("geometry::STGeomFromText({0}{1}{2},{3}),", parName, par, ++i, $"{parName}srid{i}");
                    }
                    else
                    {
                        values.AppendFormat("ST_GeomFromText({0}{1}{2}, {3}),", parName, par, ++i, $"{parName}srid{i}");
                    }

                }
                else
                {
                    values.AppendFormat("{0}{1}{2},", parName, par, ++i);
                }
               


            }

            string s = sb.ToString().Trim(' ', ',');
            sb.Clear().Append(s).Append(") ");

            sb.Append(values.ToString().Trim(' ', ',')).Append(") ");

            if (PkAttribute(Provider).Generator == Generator.Native)
                switch (Provider)
                {
                    case ORM_1_21_.ProviderName.MsSql:
                        {
                            sb.Append($" SELECT IDENT_CURRENT ('{TableAttribute.Value.TableName(Provider)}')");
                            break;
                        }
                    case ORM_1_21_.ProviderName.PostgreSql:
                        {
                            sb.Append($" RETURNING {PkAttribute(Provider).GetColumnName(Provider)}");
                            break;
                        }
                    case ORM_1_21_.ProviderName.SqLite:
                        {
                            sb.Append(";select last_insert_rowid()");
                            break;
                        }
                    case ORM_1_21_.ProviderName.MySql:
                        {
                            sb.Append("; SELECT LAST_INSERT_ID()");
                            break;
                        }
                    default:
                        throw new ArgumentOutOfRangeException($"Database type is not defined:{Provider}");
                }

            sb.Append(';');
        
            return sb.ToString();
        }

        public static void CreateInsetCommand(IDbCommand command, T obj, ProviderName providerName)
        {
       

            Provider = providerName;
            var ssq = InsertTemplate.Value;

          

            string parName = UtilsCore.PrefParam(providerName);
            const string par = "p";
            var i = 0;
            var pk = PrimaryKeyAttribute.Value;
            if (pk.IsNotUpdateInsert || pk.Generator != Generator.Assigned)
            {

            }
            else
            {
                command.AddParameter($"{parName}{par}{++i}", GetValue.Value[pk.PropertyName](obj));
            }
            foreach (var rtp in AttributeDalList.Value)
            {
                if (rtp.IsNotUpdateInsert) continue;
                command.AddParameter($"{parName}{par}{++i}", GetValue.Value[rtp.PropertyName](obj));
            }
            command.CommandText = ssq;
            
        }
    }
}