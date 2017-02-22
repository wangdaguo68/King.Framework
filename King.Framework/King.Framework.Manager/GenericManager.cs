namespace King.Framework.Manager
{
    using King.Framework.Common;
    using King.Framework.EntityLibrary;
    using King.Framework.Interfaces;
    using King.Framework.Manager.Exceptions;
    using King.Framework.Repository.Schemas;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.Common;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Transactions;

    public class GenericManager
    {
        private IAttachHandler _attachHandler;
        private BizDataContext _context;
        private IUserIdentity _currentUser;
        private static readonly string AttachmentInterfacePath = "AttatchInterface";
        private static readonly string SaveAttachmentEntityKey = "SaveAttachmentEntity";
        private static readonly string SystemLevelCode = "SystemLevelCode";
        private static readonly string T_AttachmentListTable = "T_AttachmentList";
        private static readonly string T_AttachmentTable = "T_Attachment";
        private static readonly string TableVersion = "TableVersion";

        public GenericManager(BizDataContext _ctx, IUserIdentity _user)
        {
            this._context = _ctx;
            this._currentUser = _user;
            this._attachHandler = GetAttachHandler(_ctx);
        }

        private string AutoSaveSystemLevelCode(IEntitySchema es, object entity)
        {
            string str = null;
            if (!string.IsNullOrEmpty(es.TreeRelationFieldName))
            {
                object keyValue = es.GetKeyValue(entity);
                object obj3 = entity.GetPropertyValue(es.TreeRelationFieldName, null);
                if (obj3 != null)
                {
                    object obj5 = this._context.FindById(es.EntityType, new object[] { obj3 }).GetPropertyValue(SystemLevelCode, null);
                    str = string.Format("{0}{1}-", obj5, keyValue);
                    entity.SetPropertyValue(SystemLevelCode, str, null);
                    return str;
                }
                str = string.Format("{0}-", keyValue);
                entity.SetPropertyValue(SystemLevelCode, str, null);
            }
            return str;
        }

        private void AutoUpdateSystemLevelCode(IEntitySchema es, object entity)
        {
            if (!string.IsNullOrEmpty(es.TreeRelationFieldName))
            {
                object oldEntity = this.GetOldEntity(es, entity);
                object obj3 = entity.GetPropertyValue(es.TreeRelationFieldName, null);
                object obj4 = oldEntity.GetPropertyValue(es.TreeRelationFieldName, null);
                if (obj3 != obj4)
                {
                    string str = this.AutoSaveSystemLevelCode(es, entity);
                    object keyValue = es.GetKeyValue(entity);
                    string str2 = Convert.ToString(oldEntity.GetPropertyValue(SystemLevelCode, null));
                    DbParameter parameter = this._context.CreateParameter();
                    parameter.ParameterName = this._context.AddPrefixToParameterName(SystemLevelCode);
                    parameter.Value = string.Format("{0}%", str2);
                    string condition = string.Format("{0} like {1} and {2} != {3}", new object[] { SystemLevelCode, parameter.ParameterName, es.KeyName, keyValue });
                    IList list = this._context.Where(es.EntityType, condition, new DbParameter[] { parameter });
                    foreach (object obj6 in list)
                    {
                        string str4 = Convert.ToString(obj6.GetPropertyValue(SystemLevelCode, null));
                        string str5 = str + str4.Substring(str2.Length);
                        obj6.SetPropertyValue(SystemLevelCode, str5, null);
                        this._context.Update(obj6);
                    }
                }
            }
        }

        public void Delete(object entity)
        {
            IEntitySchema es = IEntitySchemaHelper.Get(entity.GetType());
            IOperationManager opm = new DefaultOperationManager(this._context, this._currentUser);
            object obj2 = entity.GetPropertyValue(es.KeyName, null);
            opm.AUD_OperationCheck(es, entity, EntityOperationEnum.Delete);
            if (es.EntityName == T_AttachmentTable)
            {
                this._attachHandler.DeleteAttachment(entity as T_Attachment);
            }
            else
            {
                Dictionary<string, DeleteInfo> deleteEntitys = new Dictionary<string, DeleteInfo>();
                DeleteInfo info = new DeleteInfo {
                    Id = obj2,
                    EntitySchema = es
                };
                deleteEntitys.Add("current_delete_entity", info);
                Dictionary<string, DeleteInfo> setNullEntitys = new Dictionary<string, DeleteInfo>();
                List<string> mmDeleteSqls = new List<string>();
                Action<string, object, object> action = null;
                action = delegate (string entityname, object delEntity, object refValue) {
                    IEntitySchema schema = IEntitySchemaHelper.Get(entityname);
                    foreach (ReferencedObject refObj in from t in schema.ReferencedObjectList
                        where t.ReferenceType == ReferenceType.OneMore
                        orderby t.DeleteFlag
                        select t)
                    {
                        this.IfNotAllowed(entityname, refObj, refValue);
                        this.IfCascadeDelete(entityname, refObj, refValue, deleteEntitys, action);
                        this.IfSetNull(entityname, refObj, refValue, setNullEntitys, action);
                    }
                    string format = "DELETE FROM {0} WHERE {1} = {2}";
                    foreach (ReferencedObject refObj in from t in schema.ReferencedObjectList
                        where t.ReferenceType == ReferenceType.MoreMore
                        orderby t.DeleteFlag
                        select t)
                    {
                        string item = string.Format(format, schema.MmTables[refObj.ReferenceField], schema.KeyName, schema.GetKeyValue(delEntity));
                        mmDeleteSqls.Add(item);
                    }
                };
                action(es.EntityName, entity, obj2);
                using (TransactionScope scope = new TransactionScope())
                {
                    foreach (string str in mmDeleteSqls)
                    {
                        if (!string.IsNullOrEmpty(str))
                        {
                            this._context.ExecuteNonQuery(str, new DbParameter[0]);
                        }
                    }
                    foreach (IGrouping<string, DeleteInfo> grouping in from x in setNullEntitys.Values group x by x.Group)
                    {
                        Console.WriteLine("更新 " + grouping.Key);
                        this.SetEntityFiledNull(grouping.ToList<DeleteInfo>());
                    }
                    deleteEntitys.Remove("current_delete_entity");
                    foreach (IGrouping<string, DeleteInfo> grouping in from x in deleteEntitys.Values group x by x.Group)
                    {
                        Console.WriteLine("删除 " + grouping.Key);
                        this.DeleteEntity(grouping.ToList<DeleteInfo>());
                    }
                    this.DeleteRoleFunction(info, false);
                    this._context.Delete(entity);
                    scope.Complete();
                }
            }
        }

        private void DeleteEntity(List<DeleteInfo> infoList)
        {
            if (infoList.Count > 0)
            {
                DeleteInfo info = infoList[0];
                if (info.EntitySchema.EntityName == "T_Role")
                {
                    foreach (DeleteInfo info2 in infoList)
                    {
                        this.DeleteRoleFunction(info2, true);
                    }
                }
                object[] values = (from s in infoList select s.Id).ToArray<object>();
                string str = string.Join(",", values);
                string sql = string.Format("DELETE FROM [{0}] WHERE {1} in ({2});", info.EntitySchema.EntityName, info.EntitySchema.KeyName, str);
                int num = this._context.ExecuteNonQuery(sql, new DbParameter[0]);
                Console.WriteLine(sql);
                Console.WriteLine(num);
            }
        }

        private void DeleteRoleFunction(DeleteInfo dinfo, bool isRole = false)
        {
            if (isRole || dinfo.EntitySchema.IsT_RoleTable())
            {
                string sql = string.Format("DELETE FROM T_Role_Function WHERE Role_ID = {0}", dinfo.Id);
                this._context.ExecuteNonQuery(sql, new DbParameter[0]);
            }
        }

        public T FindById<T>(object id) where T: class
        {
            int num = id.ToInt();
            if (typeof(T).Name == T_AttachmentTable)
            {
                return (this._attachHandler.GetAttachment(num) as T);
            }
            return this._context.FindById<T>(new object[] { num });
        }

        public object FindById(Type type, object id)
        {
            int num = id.ToInt();
            if (type.Name == T_AttachmentTable)
            {
                return this._attachHandler.GetAttachment(num);
            }
            return this._context.FindById(type, new object[] { num });
        }

        private IList FindReferenceds(string sourceEntity, ReferencedObject refObj, object refValue)
        {
            string str = this._context.AddPrefixToParameterName("refFieldValue");
            DbParameter parameter = this._context.CreateParameter();
            parameter.ParameterName = str;
            parameter.Value = refValue;
            string condition = string.Format("{0} = {1}", refObj.ReferenceField, str);
            return this._context.Where(refObj.EntityType, condition, new DbParameter[] { parameter });
        }

        public static IAttachHandler GetAttachHandler(BizDataContext ctx)
        {
            string str = ConfigurationManager.AppSettings[AttachmentInterfacePath];
            if (string.IsNullOrWhiteSpace(str))
            {
                return new AttachmentManager(ctx);
            }
            return CustomHandlerLoader.GetHandlerWithConfiguration<IAttachHandler>(str, new object[] { ctx });
        }

        private string GetEntityNameById(long entityId)
        {
            return IEntitySchemaHelper.Get(entityId).EntityName;
        }

        public int GetNextIdentity()
        {
            return this._context.GetNextIdentity_Int(false);
        }

        private object GetOldEntity(IEntitySchema es, object entity)
        {
            int id = entity.GetPropertyValue(es.KeyName, null).ToInt();
            if (es.EntityName == T_AttachmentTable)
            {
                return this._attachHandler.GetAttachment(id);
            }
            return this._context.FindById(es.EntityType, new object[] { id });
        }

        private void IfCascadeDelete(string sourceEntity, ReferencedObject refObj, object refValue, Dictionary<string, DeleteInfo> deleteEntitys, Action<string, object, object> action)
        {
            if (refObj.DeleteFlag == DeleteFlag.CascadeDelete)
            {
                string name = refObj.EntityType.Name;
                IEntitySchema schema = IEntitySchemaHelper.Get(name);
                IList list = this.FindReferenceds(name, refObj, refValue);
                foreach (object obj2 in list)
                {
                    object keyValue = schema.GetKeyValue(obj2);
                    DeleteInfo info = new DeleteInfo {
                        Id = keyValue,
                        EntitySchema = schema,
                        RefObject = refObj
                    };
                    Console.WriteLine(string.Format("找到需要删除的数据:{0}", info.Key));
                    if (deleteEntitys["current_delete_entity"].Equals(info))
                    {
                        Console.WriteLine("父对象，不添加");
                    }
                    else if (deleteEntitys.ContainsKey(info.Key))
                    {
                        Console.WriteLine("已存在,不添加");
                    }
                    else
                    {
                        deleteEntitys.Add(info.Key, info);
                        Console.WriteLine("添加");
                        action(name, obj2, keyValue);
                    }
                }
            }
        }

        private void IfNotAllowed(string sourceEntity, ReferencedObject refObj, object refFieldValue)
        {
            if (refObj.DeleteFlag == DeleteFlag.NotAllowed)
            {
                string str = this._context.AddPrefixToParameterName("refFieldValue");
                DbParameter parameter = this._context.CreateParameter();
                parameter.ParameterName = str;
                parameter.Value = refFieldValue;
                if (this._context.ExecuteDataTable(string.Format(" {0} = {1}", refObj.ReferenceField, str), new DbParameter[] { parameter }).Rows.Count != 0)
                {
                    throw new DeleteException(string.Format("对象 {0},Id : {1} 的被引用项 {2} 不允许级联操作", sourceEntity, refFieldValue, refObj.EntityType.Name));
                }
            }
        }

        private void IfSetNull(string sourceEntity, ReferencedObject refObj, object refValue, Dictionary<string, DeleteInfo> setNullEntitys, Action<string, object, object> action)
        {
            if (refObj.DeleteFlag == DeleteFlag.SetNull)
            {
                string name = refObj.EntityType.Name;
                IEntitySchema schema = IEntitySchemaHelper.Get(name);
                IList list = this.FindReferenceds(name, refObj, refValue);
                foreach (object obj2 in list)
                {
                    object keyValue = schema.GetKeyValue(obj2);
                    DeleteInfo info = new DeleteInfo {
                        Id = keyValue,
                        EntitySchema = schema,
                        RefObject = refObj
                    };
                    Console.WriteLine(string.Format("找到置为空的数据:{0}", info.Key));
                    if (setNullEntitys.ContainsKey(info.Key))
                    {
                        Console.WriteLine("已存在,不添加");
                    }
                    else
                    {
                        setNullEntitys.Add(info.Key, info);
                        Console.WriteLine("添加");
                    }
                }
            }
        }

        public void MMDelete(Type entityType, int parentId, string parentEntityName, List<object> childIds)
        {
            IOperationManager opm = new DefaultOperationManager(this._context, this._currentUser);
            IEntitySchema es = IEntitySchemaHelper.Get(entityType);
            IEntitySchema schema2 = IEntitySchemaHelper.Get(parentEntityName);
            object entity = this._context.FindById(schema2.EntityType, new object[] { parentId });
            if (entity == null)
            {
                throw new DeleteException(es, parentId, string.Format("父实体[{0}]记录不存在", parentEntityName));
            }
            opm.AUD_OperationCheck(schema2, entity, EntityOperationEnum.Update);
            string str = string.Format("{0}s", es.EntityName);
            string entityName = schema2.MmTables[str];
            IEntitySchema schema3 = IEntitySchemaHelper.Get(entityName);
            using (TransactionScope scope = new TransactionScope())
            {
                foreach (object obj3 in childIds)
                {
                    string condition = string.Format("{0} = {1} and {2} = {3}", new object[] { es.KeyName, obj3, schema2.KeyName, parentId });
                    IList list = this._context.Where(schema3.EntityType, condition, new DbParameter[0]);
                    foreach (object obj4 in list)
                    {
                        this._context.Delete((dynamic) obj4);
                    }
                }
                scope.Complete();
            }
        }

        public void MMDelete(Type entityType, int parentId, string parentEntityName, object childId)
        {
            this.MMDelete(entityType, parentId, parentEntityName, new List<object> { childId });
        }

        public void MMSave(Type entityType, int parentId, string parentEntityName, List<string> childIds)
        {
            IOperationManager opm = new DefaultOperationManager(this._context, this._currentUser);
            IEntitySchema es = IEntitySchemaHelper.Get(entityType);
            IEntitySchema schema2 = IEntitySchemaHelper.Get(parentEntityName);
            object entity = this._context.FindById(schema2.EntityType, new object[] { parentId });
            if (entity == null)
            {
                throw new DeleteException(es, parentId, string.Format("父实体[{0}]记录不存在", parentEntityName));
            }
            opm.AUD_OperationCheck(schema2, entity, EntityOperationEnum.Update);
            string str = string.Format("{0}s", es.EntityName);
            string entityName = schema2.MmTables[str];
            IEntitySchema schema3 = IEntitySchemaHelper.Get(entityName);
            string condition = string.Format("{0} = {1}", schema2.KeyName, parentId);
            IList list = this._context.Where(schema3.EntityType, condition, new DbParameter[0]);
            List<int> first = (from o in childIds select o.ToInt()).ToList<int>();
            List<int> second = new List<int>();
            foreach (object obj3 in list)
            {
                int item = (int) obj3.GetPropertyValue(es.KeyName, null);
                second.Add(item);
            }
            List<int> list4 = first.Except<int>(second).ToList<int>();
            using (TransactionScope scope = new TransactionScope())
            {
                foreach (int num in list4)
                {
                    object targetObj = schema3.CreateInstance();
                    targetObj.SetPropertyValue(schema3.KeyName, this.GetNextIdentity(), null);
                    targetObj.SetPropertyValue(schema2.KeyName, parentId, null);
                    targetObj.SetPropertyValue(es.KeyName, num, null);
                    this._context.Insert(targetObj);
                }
                scope.Complete();
            }
        }

        public void Save(object entity)
        {
            IEntitySchema es = IEntitySchemaHelper.Get(entity.GetType());
            IOperationManager opm = new DefaultOperationManager(this._context, this._currentUser);
            if (Convert.ToInt32(es.GetKeyValue(entity)) <= 0)
            {
                int nextIdentity = this.GetNextIdentity();
                entity.SetPropertyValue(es.KeyName, nextIdentity, null);
            }
            if (es.RequiredLevel() != RequireLevelEnum.PlatForm)
            {
                entity.SetPropertyValue("CreateTime", DateTime.Now, null);
                entity.SetPropertyValue("CreateUserId", opm.CurrentUser.User_ID, null);
                object obj2 = entity.GetPropertyValue("OwnerId", null);
                if ((obj2 == null) || (Convert.ToInt32(obj2) <= 0))
                {
                    entity.SetPropertyValue("OwnerId", opm.CurrentUser.User_ID, null);
                }
                object obj3 = entity.GetPropertyValue("State", null);
                if ((obj3 == null) || (Convert.ToInt32(obj3) <= 0))
                {
                    entity.SetPropertyValue("State", 0, null);
                }
                entity.SetPropertyValue("StateDetail", 0, null);
            }
            this.SetDefaultValues(es, entity);
            opm.AUD_OperationCheck(es, entity, EntityOperationEnum.Add);
            if (es.EntityName == T_AttachmentTable)
            {
                this._attachHandler.SaveAttachment(entity as T_Attachment);
            }
            else
            {
                this.SaveActionUniqueKeyCheck(es, entity);
                this.AutoSaveSystemLevelCode(es, entity);
                this.UpdateTableVersion(es, entity);
                this._context.Insert(entity);
                this.SaveAttachmentOwner(es, entity);
            }
        }

        private bool SaveActionUniqueKeyCheck(IEntitySchema es, object entity)
        {
            Dictionary<string, List<string>> uniqueKeyDict = es.UniqueKeyDict;
            foreach (KeyValuePair<string, List<string>> pair in uniqueKeyDict)
            {
                if (pair.Value.Count > 0)
                {
                    List<string> values = new List<string>();
                    List<DbParameter> list2 = new List<DbParameter>();
                    foreach (string str in pair.Value)
                    {
                        object obj2 = entity.GetPropertyValue(str, null);
                        if (obj2 != null)
                        {
                            string str2 = this._context.AddPrefixToParameterName(str);
                            values.Add(string.Format("{0} = {1}", str, str2));
                            DbParameter item = this._context.CreateParameter();
                            item.ParameterName = str2;
                            item.Value = obj2;
                            list2.Add(item);
                        }
                        else
                        {
                            values.Add(string.Format("{0} is Null", str));
                        }
                    }
                    string str3 = string.Join(" and ", values);
                    string sql = string.Format("select * from {0} where {1}", es.EntityName, str3);
                    if (this._context.ExecuteDataTable(sql, list2.ToArray()).Rows.Count != 0)
                    {
                        string str5 = string.Empty;
                        foreach (string str6 in pair.Value)
                        {
                            str5 = string.Format("{0} {1}:{2}", str5, str6, entity.GetPropertyValue(str6, null));
                        }
                        throw new ApplicationException(string.Format("违反唯一键约束 {0} ", str5));
                    }
                }
            }
            return true;
        }

        private void SaveAttachmentOwner(IEntitySchema es, object entity)
        {
            string str = ConfigurationManager.AppSettings[SaveAttachmentEntityKey];
            if ((string.IsNullOrEmpty(str) || (str.ToLower() == "true")) && (es.EntityName != T_AttachmentTable))
            {
                if (es.EntityName == T_AttachmentListTable)
                {
                    this.SaveAttachmentOwnerForAttachmentList(es, entity);
                }
                else
                {
                    this.SaveAttachmentOwnerForCommonEntities(es, entity);
                }
            }
        }

        private void SaveAttachmentOwnerForAttachmentList(IEntitySchema es, object entity)
        {
            T_AttachmentList list = entity as T_AttachmentList;
            if ((list.Attachment_ID.HasValue && list.EntityId.HasValue) && list.ObjectID.HasValue)
            {
                string entityNameById = this.GetEntityNameById(list.EntityId.Value);
                int num = list.ObjectID.Value;
                T_Attachment attach = this._attachHandler.GetAttachment(list.Attachment_ID.Value);
                if (attach != null)
                {
                    attach.OwnerEntityType = entityNameById;
                    attach.OwnerObjectId = new int?(num);
                    this._attachHandler.UpdateAttachment(attach, p => new { OwnerEntityType = p.OwnerEntityType, OwnerObjectId = p.OwnerObjectId });
                }
            }
        }

        private void SaveAttachmentOwnerForCommonEntities(IEntitySchema es, object entity)
        {
            if (es.FilePropertys.Count > 0)
            {
                foreach (string str in es.FilePropertys)
                {
                    int? nullable = Convert.ToString(entity.GetPropertyValue(str, null)).ToIntNullable();
                    if (nullable.HasValue)
                    {
                        T_Attachment attach = this._attachHandler.GetAttachment(nullable.Value);
                        if (attach != null)
                        {
                            attach.OwnerEntityType = es.EntityName;
                            attach.OwnerObjectId = Convert.ToString(entity.GetPropertyValue(es.KeyName, null)).ToIntNullable();
                            this._attachHandler.UpdateAttachment(attach, p => new { OwnerEntityType = p.OwnerEntityType, OwnerObjectId = p.OwnerObjectId });
                        }
                    }
                }
            }
        }

        private void SetDefaultValues(IEntitySchema es, object entity)
        {
            foreach (KeyValuePair<string, string> pair in es.DefaultValues)
            {
                if (entity.GetPropertyValue(pair.Key, null) == null)
                {
                    entity.SetPropertyValue(pair.Key, pair.Value, null);
                }
            }
        }

        private void SetEntityFiledNull(List<DeleteInfo> infoList)
        {
            if (infoList.Count > 0)
            {
                object[] values = (from s in infoList select s.Id).ToArray<object>();
                string str = string.Join(",", values);
                string sql = string.Format("UPDATE [{0}] SET {1} = NULL WHERE {2} in ({3});", new object[] { infoList[0].EntitySchema.EntityName, infoList[0].RefObject.ReferenceField, infoList[0].EntitySchema.KeyName, str });
                int num = this._context.ExecuteNonQuery(sql, new DbParameter[0]);
                Console.WriteLine(sql);
                Console.WriteLine(num);
            }
        }

        public void Update(object entity)
        {
            IEntitySchema es = IEntitySchemaHelper.Get(entity.GetType());
            IOperationManager opm = new DefaultOperationManager(this._context, this._currentUser);
            List<string> modifiedPropertys = (from p in es.PropertyTypes select p.Key).ToList<string>();
            opm.AUD_OperationCheck(es, entity, EntityOperationEnum.Update);
            this.UpdateActionUniqueKeyCheck(es, entity, modifiedPropertys);
            if (es.PrivilegeModel() == PrivilegeModel.Persional)
            {
                entity.SetPropertyValue("UpdateTime", DateTime.Now, null);
                entity.SetPropertyValue("UpdateUserId", opm.CurrentUser.User_ID, null);
            }
            if (es.EntityName == T_AttachmentTable)
            {
                this._attachHandler.UpdateAttachment(entity as T_Attachment);
            }
            else
            {
                this.AutoUpdateSystemLevelCode(es, entity);
                this.UpdateTableVersion(es, entity);
                this.UpdateEntityChangeLog(es, entity, modifiedPropertys);
                this._context.Update(entity);
                this.SaveAttachmentOwner(es, entity);
            }
        }

        private bool UpdateActionUniqueKeyCheck(IEntitySchema es, object entity, List<string> modifiedPropertys)
        {
            Dictionary<string, List<string>> uniqueKeyDict = es.UniqueKeyDict;
            if (uniqueKeyDict.Keys.Count > 0)
            {
                foreach (KeyValuePair<string, List<string>> pair in uniqueKeyDict)
                {
                    if (pair.Value.Count == 0)
                    {
                        throw new ApplicationException(string.Format("[{0}]配置了唯一键，但未配置唯一键字段", es.EntityName));
                    }
                    object oldEntity = this.GetOldEntity(es, entity);
                    List<string> values = new List<string>();
                    List<DbParameter> list2 = new List<DbParameter>();
                    string str = this._context.AddPrefixToParameterName(es.KeyName);
                    values.Add(string.Format("{0} != {1}", es.KeyName, str));
                    DbParameter item = this._context.CreateParameter();
                    item.ParameterName = str;
                    item.Value = es.GetKeyValue(entity);
                    list2.Add(item);
                    foreach (string str2 in pair.Value)
                    {
                        object obj3 = null;
                        if (modifiedPropertys.Contains(str2))
                        {
                            obj3 = entity.GetPropertyValue(str2, null);
                        }
                        else
                        {
                            obj3 = oldEntity.GetPropertyValue(str2, null);
                        }
                        if (str2 != es.KeyName)
                        {
                            if (obj3 != null)
                            {
                                string str3 = this._context.AddPrefixToParameterName(str2);
                                values.Add(string.Format("{0} = {1}", str2, str3));
                                DbParameter parameter2 = this._context.CreateParameter();
                                parameter2.ParameterName = str3;
                                parameter2.Value = obj3;
                                list2.Add(parameter2);
                            }
                            else
                            {
                                values.Add(string.Format("{0} is Null", str2));
                            }
                        }
                    }
                    string str4 = string.Join(" and ", values);
                    string sql = string.Format("select * from {0} where {1}", es.EntityName, str4);
                    if (this._context.ExecuteDataTable(sql, list2.ToArray()).Rows.Count != 0)
                    {
                        string str6 = string.Empty;
                        foreach (string str7 in pair.Value)
                        {
                            str6 = string.Format("{0} {1}:{2}", str6, str7, entity.GetPropertyValue(str7, null));
                        }
                        throw new ApplicationException(string.Format("更新违反唯一键约束 {0} ", str6));
                    }
                }
            }
            return true;
        }

        private void UpdateEntityChangeLog(IEntitySchema es, object entity, List<string> modifiedColumns)
        {
            if (es.IsHistory)
            {
                int num = Convert.ToInt32(es.GetKeyValue(entity));
                SysChangeLog log = new SysChangeLog {
                    LogId = this.GetNextIdentity(),
                    EntityName = es.EntityName,
                    ChangeTime = new DateTime?(DateTime.Now),
                    ChangeUserId = new int?(this._currentUser.User_ID),
                    ObjectId = new int?(num)
                };
                this._context.Insert(log);
                object oldEntity = this.GetOldEntity(es, entity);
                foreach (string str in modifiedColumns)
                {
                    object obj3 = oldEntity.GetPropertyValue(str, null);
                    object obj4 = entity.GetPropertyValue(str, null);
                    if ((((obj3 != null) && (obj4 == null)) || ((obj3 == null) && (obj4 != null))) || (((obj3 != null) && (obj4 != null)) && !obj3.Equals(obj4)))
                    {
                        SysChangeLogDetail detail = new SysChangeLogDetail {
                            DetailId = this.GetNextIdentity(),
                            LogId = new long?(log.LogId),
                            CurrentValue = Convert.ToString(obj4),
                            OriginalValue = Convert.ToString(obj3),
                            DataType = new int?(es.PropertyTypeEnums[str]),
                            FieldName = str
                        };
                        this._context.Insert(detail);
                    }
                }
            }
        }

        public void UpdatePartial<T>(object entity, Expression<Func<T, object>> columns) where T: class
        {
            Type entityType = typeof(T);
            IEntitySchema es = IEntitySchemaHelper.Get(entityType);
            IOperationManager opm = new DefaultOperationManager(this._context, this._currentUser);
            List<string> modifiedPropertys = (from p in columns.Body.Type.GetProperties() select p.Name).ToList<string>();
            opm.AUD_OperationCheck(es, entity, EntityOperationEnum.Update);
            this.UpdateActionUniqueKeyCheck(es, entity, modifiedPropertys);
            if (es.EntityName == T_AttachmentTable)
            {
                this._attachHandler.UpdateAttachment(entity as T_Attachment, columns as Expression<Func<T_Attachment, object>>);
            }
            else
            {
                this.AutoUpdateSystemLevelCode(es, entity);
                if (!(!this.UpdateTableVersion(es, entity) || modifiedPropertys.Contains(TableVersion)))
                {
                    modifiedPropertys.Add(TableVersion);
                }
                this.UpdateEntityChangeLog(es, entity, modifiedPropertys);
                this._context.UpdatePartial(entity, modifiedPropertys);
                this.SaveAttachmentOwner(es, entity);
            }
        }

        private bool UpdateTableVersion(IEntitySchema es, object entity)
        {
            if (es.IsChangeTableVersion)
            {
                string sql = string.Format("select max({0}) from {1}", TableVersion, es.EntityName);
                object obj2 = this._context.ExecuteScalar(sql, new DbParameter[0]);
                if ((obj2 == null) || (obj2 == DBNull.Value))
                {
                    entity.SetPropertyValue(TableVersion, 1, null);
                }
                else
                {
                    int num = Convert.ToInt32(obj2) + 1;
                    entity.SetPropertyValue(TableVersion, num, null);
                }
                return true;
            }
            return false;
        }

        public List<T> Where<T>(Expression<Func<T, bool>> predicate)
        {
            if (typeof(T).Name == T_AttachmentTable)
            {
                return (this._attachHandler.GetAttachment(predicate as Expression<Func<T_Attachment, bool>>) as List<T>);
            }
            return this._context.Where<T>(predicate);
        }
    }
}
