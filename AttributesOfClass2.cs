using ORM_1_21_.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;

namespace ORM_1_21_
{
    [SuppressMessage("ReSharper", "StaticMemberInGenericType")]
    internal static partial class AttributesOfClass<T>
    {
        private static readonly Lazy<List<Action<T, int, IDbCommand>>> InsertActionParam =
            new Lazy<List<Action<T, int, IDbCommand>>>(
                () =>
                {
                    string parName = UtilsCore.PrefParam(Provider);
                    const string par = "p";

                    List<Action<T, int, IDbCommand>> list = new List<Action<T, int, IDbCommand>>();

                    var pk = PrimaryKeyAttribute.Value;
                    if (pk.IsNotUpdateInsert || pk.Generator != Generator.Assigned)
                    {

                    }
                    else
                    {
                        void Command(T obj, int ip, IDbCommand dbCommand)
                        {
                            IDataParameter pr = dbCommand.CreateParameter();
                            pr.ParameterName = $"{parName}{par}{ip}";
                            pr.Value = GetValue.Value[pk.PropertyName](obj) ?? DBNull.Value;
                            //pr.DbType = pk.DbType();
                            dbCommand.Parameters.Add(pr);
                        }

                        list.Add(Command);
                    }

                    foreach (var rtp in AttributeDalList.Value)
                    {
                        if (rtp.IsNotUpdateInsert) continue;

                        if (rtp.PropertyType.BaseType == typeof(Enum))
                        {
                            void Command(T obj, int ip, IDbCommand dbCommand)
                            {
                                IDataParameter pr = dbCommand.CreateParameter();
                                pr.ParameterName = $"{parName}{par}{ip}";
                                pr.Value = (int)GetValue.Value[rtp.PropertyName](obj);
                                //pr.DbType = DbTypeConverter.ConvertFrom(typeof(int));
                                dbCommand.Parameters.Add(pr);
                            }
                            list.Add(Command);
                        }
                        else
                        {
                            void Command(T obj, int ip, IDbCommand dbCommand)
                            {
                                IDataParameter pr = dbCommand.CreateParameter();
                                pr.ParameterName = $"{parName}{par}{ip}";
                                pr.Value = GetValue.Value[rtp.PropertyName](obj) ?? DBNull.Value;
                                //pr.DbType = rtp.DbType();
                                dbCommand.Parameters.Add(pr);
                            }
                            list.Add(Command);
                        }
                    }

                    return list;
                }, LazyThreadSafetyMode.PublicationOnly);
        public static void CreateInsetCommandNew(IDbCommand command, T obj, ProviderName providerName)
        {
            Provider = providerName;
            var sql = InsertTemplate.Value;
            int i = 0;
            InsertActionParam.Value.ForEach(s => s.Invoke(obj, ++i, command));
            command.CommandText = sql;
        }

        private static readonly Lazy<List<Action<T, int, IDbCommand>>> UpdateMysqlActionParam =
           new Lazy<List<Action<T, int, IDbCommand>>>(
               () =>
               {
                   string parName = UtilsCore.PrefParam(Provider);


                   List<Action<T, int, IDbCommand>> list = new List<Action<T, int, IDbCommand>>();

                   foreach (var pra in AttributeDalList.Value)
                   {
                       if (pra.IsNotUpdateInsert) continue;



                       if (pra.PropertyType.BaseType == typeof(Enum))
                       {
                           void Command(T obj, int ip, IDbCommand dbCommand)
                           {
                               IDataParameter pr = dbCommand.CreateParameter();
                               pr.ParameterName = string.Format("{1}p{0}", ip, parName);
                               pr.Value = (int)GetValue.Value[pra.PropertyName](obj);
                               //pr.DbType = pra.DbType();
                               dbCommand.Parameters.Add(pr);
                           }

                           list.Add(Command);


                       }

                       else
                       {
                           void Command(T obj, int ip, IDbCommand dbCommand)
                           {
                               IDataParameter pr = dbCommand.CreateParameter();
                               pr.ParameterName = string.Format("{1}p{0}", ip, parName);
                               pr.Value = GetValue.Value[pra.PropertyName](obj) ?? DBNull.Value;
                               //pr.DbType = pra.DbType();
                               dbCommand.Parameters.Add(pr);
                           }
                           list.Add(Command);
                       }

                   }

                   var pk = PrimaryKeyAttribute.Value;
                   void CommandPk(T obj, int ip, IDbCommand dbCommand)
                   {
                       IDataParameter pr1 = dbCommand.CreateParameter();
                       pr1.ParameterName = string.Format("{1}p{0}", ip, parName);
                       pr1.Value = GetValue.Value[pk.PropertyName](obj);
                       //pr1.DbType = pk.DbType();
                       dbCommand.Parameters.Add(pr1);
                   }
                   list.Add(CommandPk);







                   return list;
               }, LazyThreadSafetyMode.PublicationOnly);

        public static void CreateUpdateCommandMysqlNew(IDbCommand command, T item, ProviderName providerName,
         params AppenderWhere[] whereObjects)
        {
            Provider = providerName;
            var sql = UpdateTemplateMysql.Value;
            string parName = UtilsCore.PrefParam(Provider);
            int i = 0;

            UpdateMysqlActionParam.Value.ForEach(a => a.Invoke(item, ++i, command));
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
                command.CommandText = builder.Append(";").ToString();
            }
            else
            {
                command.CommandText = sql + ";";
            }

        }

        private static readonly Lazy<string> DeleteTemplate =
            new Lazy<string>(() =>
            {
                var pk = PrimaryKeyAttribute.Value;
                return string.Format(" DELETE FROM {0} WHERE {0}.{1} = {2}p1",
                    TableAttribute.Value.TableName(Provider),
                    pk.GetColumnName(Provider), UtilsCore.PrefParam(Provider));
            }, LazyThreadSafetyMode.PublicationOnly);
        public static void CreateDeleteCommand(IDbCommand command, T obj, ProviderName providerName)
        {
            Provider = providerName;

            command.CommandText = DeleteTemplate.Value;
            IDataParameter pr = command.CreateParameter();
            pr.ParameterName = $"{UtilsCore.PrefParam(providerName)}p1";
            pr.Value = GetValue.Value[PrimaryKeyAttribute.Value.PropertyName](obj); ;
            command.Parameters.Add(pr);
        }

        private static readonly Lazy<List<Action<T, int, IDbCommand>>> UpdatePostgresActionParam =
          new Lazy<List<Action<T, int, IDbCommand>>>(
              () =>
              {
                  List<Action<T, int, IDbCommand>> list = new List<Action<T, int, IDbCommand>>();
                 
              

                  foreach (var pra in AttributeDalList.Value)
                  {
                      if (pra.IsNotUpdateInsert) continue;

                      if (pra.PropertyType.BaseType == typeof(Enum))
                      {
                          void Command(T obj, int ip, IDbCommand dbCommand)
                          {
                              IDataParameter pr = dbCommand.CreateParameter();
                              pr.ParameterName = string.Format("{1}p{0}", ip, UtilsCore.PrefParam(Provider));
                              pr.Value=  (int)GetValue.Value[pra.PropertyName](obj);
                              dbCommand.Parameters.Add(pr);
                          }
                          list.Add(Command);
                      }
                      else
                      {
                          
                              void Command(T obj, int ip, IDbCommand dbCommand)
                              {
                                  IDataParameter pr = dbCommand.CreateParameter();
                                  pr.ParameterName = string.Format("{1}p{0}", ip, UtilsCore.PrefParam(Provider));
                                  pr.Value = GetValue.Value[pra.PropertyName](obj) ?? DBNull.Value;
                                  dbCommand.Parameters.Add(pr);
                              }
                              list.Add(Command);
                          
                          
                      }
                    
                  }
                  void Command2(T obj, int ip, IDbCommand dbCommand)
                  {
                     
                      var pk = PrimaryKeyAttribute.Value;
                      IDataParameter pr = dbCommand.CreateParameter();
                      pr.ParameterName = string.Format("{1}p{0}", ip, UtilsCore.PrefParam(Provider));
                      pr.Value = GetValue.Value[pk.PropertyName](obj);
                      dbCommand.Parameters.Add(pr);
                  }
                  list.Add(Command2);
                  return list;
              }, LazyThreadSafetyMode.PublicationOnly);

        public static void CreateUpdateCommandPostgresNew(IDbCommand command, T item, ProviderName providerName,
        params AppenderWhere[] whereObjects)
        {
            Provider = providerName;
            var sql = TemplateUpdatePostgres.Value;
            var parName = UtilsCore.PrefParam(providerName);
            var i = 0;
            UpdatePostgresActionParam.Value.ForEach(a=>a.Invoke(item,++i,command));


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
                command.CommandText = builder.Append(";").ToString();
            }
            else
            {
                command.CommandText = sql + ";";
            }
        }

    }
}
