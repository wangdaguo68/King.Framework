namespace King.Framework.Common
{
    using King.Framework.EntityLibrary;
    using King.Framework.Ioc;
    using King.Framework.Linq;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.Common;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;

    public class BizDataContext : DataContext
    {
        public static readonly string SessionConnectionInfoName = "ConnectionInfo";

        public BizDataContext(bool openConnection = true) : base(CurrentConnectionInfo, openConnection)
        {
        }

        public BizDataContext(ConnectionInfo connectionInfo, bool openConnection = true) : base(connectionInfo, openConnection)
        {
        }

        public BizDataContext(string connectionStringName, bool openConnection = true) : base(connectionStringName, openConnection)
        {
        }

        public BizDataContext(string connectionString, string providerInvariantName, bool openConnectionn = true) : base(connectionString, providerInvariantName, openConnectionn)
        {
        }

        public override void Delete(object obj)
        {
            Type type = obj.GetType();
            string methodKey = string.Format("{0}${1}", type.Name, OperationEnum.Delete.ToString());
            if (OperationPluginFactory.IsEnableOperationPlugin)
            {
                IOperationPlugin operationPlugin = OperationPluginFactory.GetOperationPlugin(methodKey);
                if (operationPlugin != null)
                {
                    operationPlugin.InnerBeforeExecute(obj);
                    base.Delete(obj);
                    operationPlugin.InnerAfterExecute(obj);
                }
                else
                {
                    base.Delete(obj);
                }
            }
            else
            {
                base.Delete(obj);
            }
        }

        public Dictionary<string, object> DynamicFindById(SysEntity entity, object id)
        {
            Func<DataColumn, object> elementSelector = null;
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            SysField field = this.ValidateEntity(entity, null);
            string name = base.AddPrefixToParameterName(field.FieldName);
            DbParameter parameter = base.CreateParameter(name, id, typeof(int));
            string message = string.Format("select * from {0} where {1} = {2}", entity.EntityName, field.FieldName, name);
            Trace.WriteLine(message);
            DataTable dt = base.ExecuteDataTable(message, new DbParameter[] { parameter });
            if (dt.Rows.Count != 1)
            {
                return dictionary;
            }
            if (elementSelector == null)
            {
                elementSelector = p => (dt.Rows[0][p] == DBNull.Value) ? null : dt.Rows[0][p];
            }
            return dt.Columns.Cast<DataColumn>().ToDictionary<DataColumn, string, object>(p => p.ColumnName.ToLower(), elementSelector);
        }

        public int DynamicInsert(SysEntity entity, Dictionary<string, object> valueDict)
        {
            SysField field = this.ValidateEntity(entity, valueDict);
            int num = this.GetNextIdentity_Int(false);
            valueDict[field.FieldName] = num;
            List<DbParameter> list = new List<DbParameter>();
            StringBuilder builder = new StringBuilder(0x400);
            builder.AppendFormat("insert into {0} (", entity.EntityName);
            int num2 = 0;
            foreach (KeyValuePair<string, object> pair in valueDict)
            {
                if (num2 > 0)
                {
                    builder.Append(",");
                }
                builder.AppendFormat("{0}", pair.Key);
                num2++;
            }
            builder.Append(")values(");
            num2 = 0;
            foreach (KeyValuePair<string, object> pair in valueDict)
            {
                if (num2 > 0)
                {
                    builder.Append(",");
                }
                string str = base.AddPrefixToParameterName(pair.Key);
                builder.Append(str);
                object obj2 = pair.Value ?? DBNull.Value;
                int? type = null;
                DbParameter item = base.CreateParameter(str, obj2, type);
                list.Add(item);
                num2++;
            }
            builder.Append(")");
            Trace.WriteLine(builder.ToString());
            base.ExecuteNonQuery(builder.ToString(), list.ToArray());
            return num;
        }

        public void DynamicUpdate(SysEntity entity, object id, Dictionary<string, object> valueDict)
        {
            string str;
            DbParameter parameter;
            SysField field = this.ValidateEntity(entity, valueDict);
            if (valueDict.ContainsKey(field.FieldName))
            {
                valueDict.Remove(field.FieldName);
            }
            List<DbParameter> list = new List<DbParameter>();
            StringBuilder builder = new StringBuilder(0x400);
            builder.AppendFormat("update {0} set ", entity.EntityName);
            int num = 0;
            foreach (KeyValuePair<string, object> pair in valueDict)
            {
                if (pair.Key != field.FieldName)
                {
                    if (num > 0)
                    {
                        builder.Append(",");
                    }
                    str = base.AddPrefixToParameterName(pair.Key);
                    builder.AppendFormat("{0}={1}", pair.Key, str);
                    object obj2 = pair.Value ?? DBNull.Value;
                    int? type = null;
                    parameter = base.CreateParameter(str, obj2, type);
                    list.Add(parameter);
                    num++;
                }
            }
            builder.Append(" where ");
            str = base.AddPrefixToParameterName(field.FieldName);
            builder.AppendFormat("{0}={1}", field.FieldName, str);
            parameter = base.CreateParameter(str, id, typeof(int));
            list.Add(parameter);
            Trace.WriteLine(builder.ToString());
            base.ExecuteNonQuery(builder.ToString(), list.ToArray());
        }

        public override void Insert(object obj)
        {
            Type type = obj.GetType();
            string methodKey = string.Format("{0}${1}", type.Name, OperationEnum.Add.ToString());
            if (OperationPluginFactory.IsEnableOperationPlugin)
            {
                IOperationPlugin operationPlugin = OperationPluginFactory.GetOperationPlugin(methodKey);
                if (operationPlugin != null)
                {
                    operationPlugin.InnerBeforeExecute(obj);
                    base.Insert(obj);
                    operationPlugin.InnerAfterExecute(obj);
                }
                else
                {
                    base.Insert(obj);
                }
            }
            else
            {
                base.Insert(obj);
            }
        }

        public override void Update(object obj)
        {
            Type type = obj.GetType();
            string methodKey = string.Format("{0}${1}", type.Name, OperationEnum.Update.ToString());
            if (OperationPluginFactory.IsEnableOperationPlugin)
            {
                IOperationPlugin operationPlugin = OperationPluginFactory.GetOperationPlugin(methodKey);
                if (operationPlugin != null)
                {
                    operationPlugin.InnerBeforeExecute(obj);
                    base.Update(obj);
                    operationPlugin.InnerAfterExecute(obj);
                }
                else
                {
                    base.Update(obj);
                }
            }
            else
            {
                base.Update(obj);
            }
        }

        private SysField ValidateEntity(SysEntity entity, Dictionary<string, object> valueDict = null)
        {
            if (entity == null)
            {
                throw new Exception("实体不能为空");
            }
            List<SysField> source = base.Where<SysField>(p => p.EntityId == entity.EntityId);
            if (source.Count < 1)
            {
                throw new Exception("没有定义字段");
            }
            List<SysField> list2 = (from p in source
                where p.DataType == 1
                select p).ToList<SysField>();
            if (list2.Count == 0)
            {
                throw new Exception("主键未定义");
            }
            if (list2.Count > 1)
            {
                throw new Exception("不支持多个主键定义");
            }
            SysField field = list2.First<SysField>();
            if (valueDict != null)
            {
                if (valueDict.Count < 1)
                {
                    throw new Exception("没有提供值");
                }
                using (Dictionary<string, object>.KeyCollection.Enumerator enumerator = valueDict.Keys.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        Func<SysField, bool> predicate = null;
                        string key = enumerator.Current;
                        if (predicate == null)
                        {
                            predicate = p => p.FieldName == key;
                        }
                        int num = source.Where<SysField>(predicate).Count<SysField>();
                        if (num == 0)
                        {
                            throw new Exception(string.Format("字段{0}未定义", key));
                        }
                        if (num > 1)
                        {
                            throw new Exception(string.Format("字段{0}重复定义", key));
                        }
                    }
                }
            }
            return field;
        }

        public static ConnectionInfo CurrentConnectionInfo
        {
            get
            {
                if (UseConnectionProvider)
                {
                    if (!IocHelper.IsRegistered(typeof(IConnectionProvider)))
                    {
                        throw new ApplicationException("未注入IConnectionProvider的实现");
                    }
                    return IocHelper.Resolve<IConnectionProvider>().GetConnectionInfo();
                }
                return ConnectionInfo.Default;
            }
        }

        public static bool UseConnectionProvider
        {
            get
            {
                string str = ConfigurationManager.AppSettings["UseConnectionProvider"];
                return !(string.IsNullOrEmpty(str) || !(str.ToLower() == "true"));
            }
        }
    }
}
