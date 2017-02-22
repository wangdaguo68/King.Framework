namespace King.Framework.WorkflowEngineCore.InternalCore
{
    using King.Framework.Common;
    using King.Framework.EntityLibrary;
    using King.Framework.DAL;
    using King.Framework.Linq;
    using King.Framework.WorkflowEngineCore.Cache;
    using IronPython.Hosting;
    using Microsoft.Scripting.Hosting;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Linq;

    public class DynamicManager
    {
        protected DataContext context;

        public DynamicManager(DataContext ctx)
        {
            this.context = ctx;
        }

        internal static object EntityDataToObject(EntityData data)
        {
            Type tableType = TableCache.GetTableType(data.EntityName);
            object obj2 = Activator.CreateInstance(tableType);
            foreach (Column column in TableCache.GetTableOrCreate(tableType).Columns)
            {
                column.SetAction(obj2, data[column.ColumnName]);
            }
            return obj2;
        }

        public DataTable ExecuteDataTable(string sql, params DbParameter[] parameters)
        {
            return this.context.ExecuteDataTable(sql, parameters);
        }

        public void ExecuteNonQuery(string sql, params DbParameter[] parameters)
        {
            this.context.ExecuteNonQuery(sql, parameters);
        }

        public List<int> ExecutePython(SysProcessInstance pi, string scriptText)
        {
            List<int> list = new List<int>();
            if (!string.IsNullOrEmpty(scriptText))
            {
                EntityCache cache = new EntityCache(this);
                SysEntity processEntity = pi.Process.ProcessEntity;
                EntityData data = cache.GetObject(processEntity, pi.ObjectId);
                try
                {
                    ScriptEngine engine = Python.CreateEngine();
                    ScriptScope scope = engine.CreateScope();
                    scope.SetVariable("manager", this);
                    scope.SetVariable("pi_data", data);
                    scope.SetVariable("userIdList", list);
                    engine.CreateScriptSourceFromString(scriptText).Execute(scope);
                }
                catch (Exception exception)
                {
                    throw new ApplicationException("Python执行错误:" + exception.Message, exception);
                }
            }
            return list;
        }

        public int GetNextIdentity()
        {
            return this.context.GetNextIdentity_Int(false);
        }

        public void Insert(EntityData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (!data.ObjectId.HasValue || (data.ObjectId.Value <= 0))
            {
                throw new ApplicationException("未设置对象ID");
            }
            if (data.Count == 0)
            {
                throw new ApplicationException("没有要新的字段");
            }
            if (data.Entity == null)
            {
                throw new ApplicationException(string.Format("实体{0}不存在", data.EntityName));
            }
            object obj2 = EntityDataToObject(data);
            this.context.Insert(obj2);
        }

        internal EntityData LoadFullWithEntity(SysEntity entity, int object_id, ProcessInstanceCacheFactory piCacheFactory)
        {
            EntityData data = null;
            Type tableType = TableCache.GetTableType(entity.EntityName);
            object obj2 = this.context.FindById(tableType, new object[] { object_id });
            if (obj2 != null)
            {
                data = ObjectToEntityData(obj2, entity);
            }
            if (data != null)
            {
                EntityCache cache = new EntityCache(this);
                using (IEnumerator<SysField> enumerator = entity.Fields.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        Func<SysEnumItem, bool> predicate = null;
                        SysField p = enumerator.Current;
                        if (p.DataType == 12)
                        {
                            if ((p.RefEntity != null) && (data[p.FieldName] != null))
                            {
                                string displayFieldName = p.RefEntity.GetDisplayFieldName();
                                EntityData da = cache.GetObject(p.RefEntity, (int) data[p.FieldName]);
                                if (da == null)
                                {
                                    throw new ApplicationException(string.Format("[{0}]-[{1}]的引用字段[{2}]有值[{3}]，但是对应的[{4}]不存在", new object[] { entity.EntityName, object_id, p.FieldName, data[p.FieldName], p.RefEntity.EntityName }));
                                }
                                if (da.ContainsKey(displayFieldName))
                                {
                                    da[p.FieldName] = data[displayFieldName];
                                }
                            }
                        }
                        else
                        {
                            if (p.DataType == 13)
                            {
                                if (((p.RefEnum != null) && (p.RefEnum.EnumItems != null)) && (data[p.FieldName] != null))
                                {
                                    if (predicate == null)
                                    {
                                        predicate = delegate (SysEnumItem i) {
                                            int? itemValue = i.ItemValue;
                                            int num = (int) data[p.FieldName];
                                            return (itemValue.GetValueOrDefault() == num) && itemValue.HasValue;
                                        };
                                    }
                                    SysEnumItem item = p.RefEnum.EnumItems.FirstOrDefault<SysEnumItem>(predicate);
                                    if (item != null)
                                    {
                                        data[p.FieldName] = item.DisplayText;
                                    }
                                }
                                continue;
                            }
                            if (p.DataType == 11)
                            {
                                if (data[p.FieldName].ToInt() == 0)
                                {
                                    data[p.FieldName] = "否";
                                }
                                else
                                {
                                    data[p.FieldName] = "是";
                                }
                                continue;
                            }
                            if ((p.DataType == 15) && (data[p.FieldName] != null))
                            {
                                string[] source = data[p.FieldName].ToString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                if (source.Count<string>() != 2)
                                {
                                    throw new ApplicationException(string.Format("[{0}]中[entity_id]或者[object_id]缺失", data[p.FieldName]));
                                }
                                long entityId = source[0].ToLong();
                                int num2 = source[1].ToInt();
                                SysEntity entityCache = piCacheFactory.PCacheFactory.GetEntityCache(entityId);
                                if (entityCache != null)
                                {
                                    string key = entityCache.GetDisplayFieldName();
                                    EntityData data2 = cache.GetObject(entityCache, num2);
                                    if (data2 == null)
                                    {
                                        throw new ApplicationException(string.Format("[{0}]-[{1}]的引用字段[{2}]有值[{3}]，但是对应的[{4}]不存在", new object[] { entity.EntityName, num2, p.FieldName, data[p.FieldName], entityCache.EntityName }));
                                    }
                                    if (data2.ContainsKey(key))
                                    {
                                        data[p.FieldName] = data2[key];
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return data;
        }

        internal EntityData LoadWithEntity(SysEntity entity, int object_id)
        {
            EntityData data = null;
            Type tableType = TableCache.GetTableType(entity.EntityName);
            object obj2 = this.context.FindById(tableType, new object[] { object_id });
            if (obj2 != null)
            {
                data = ObjectToEntityData(obj2, entity);
            }
            return data;
        }

        internal static EntityData ObjectToEntityData(object obj, SysEntity entity)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }
            Table tableOrCreate = TableCache.GetTableOrCreate(obj.GetType());
            EntityData data = new EntityData(entity);
            foreach (Column column in tableOrCreate.Columns)
            {
                data[column.ColumnName] = column.GetFunction(obj);
            }
            object obj2 = tableOrCreate.KeyColumns[0].GetFunction(obj);
            data.ObjectId = new int?(Convert.ToInt32(obj2));
            return data;
        }

        public void Update(EntityData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (data.Count == 0)
            {
                throw new ApplicationException("没有要新的字段");
            }
            if (data.Entity == null)
            {
                throw new ApplicationException(string.Format("实体{0}不存在", data.EntityName));
            }
            object obj2 = EntityDataToObject(data);
            this.context.Update(obj2);
        }
    }
}

