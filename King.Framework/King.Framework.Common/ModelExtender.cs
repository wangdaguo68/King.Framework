namespace King.Framework.Common
{
    using King.Framework.EntityLibrary;
    using King.Framework.Linq;
    using System;
    using System.Runtime.CompilerServices;

    public static class ModelExtender
    {
        public static string GetChildFieldName(this SysMoreMoreRelation rel, DataContext context)
        {
            bool? isCustomChildField = rel.IsCustomChildField;
            if (isCustomChildField.HasValue ? isCustomChildField.GetValueOrDefault() : false)
            {
                return rel.ChildFieldName;
            }
            return context.FindById<SysEntity>(new object[] { rel.ChildEntityId }).GetKeyFieldName();
        }

        public static string GetDefaultDisplayFieldName(this SysEntity entity)
        {
            string entityName = entity.EntityName;
            if (entityName.StartsWith("T_"))
            {
                entityName = entityName.Remove(0, 2);
            }
            return string.Format("{0}_Name", entityName);
        }

        public static long GetDefaultKeyFieldId(this SysEntity entity, DataContext context)
        {
            int fieldType = Convert.ToInt32(DataTypeEnum.pkey);
            SysField field = context.FirstOrDefault<SysField>(f => (f.EntityId == entity.EntityId) && (f.DataType == fieldType));
            if (field == null)
            {
                throw new ApplicationException(string.Format("实体{0}未定义主键！", entity.EntityName));
            }
            return field.FieldId;
        }

        public static string GetDefaultKeyFieldName(this SysEntity entity)
        {
            string entityName = entity.EntityName;
            if (entityName.ToUpper().StartsWith("T_"))
            {
                entityName = entityName.Remove(0, 2);
            }
            return string.Format("{0}_Id", entityName);
        }

        public static SysField GetDisplayField(this SysEntity entity, DataContext context)
        {
            string key_name = entity.GetDisplayFieldName();
            if (entity.Fields == null)
            {
                throw new ApplicationException("entity.Fieds没有加载");
            }
            SysField field = context.FirstOrDefault<SysField>(f => (f.EntityId == entity.EntityId) && (f.FieldName == key_name));
            if (field == null)
            {
                throw new ApplicationException(string.Format("没有找到显示字段:{0} -{1}", entity.EntityName, key_name));
            }
            return field;
        }

        public static string GetDisplayFieldName(this SysEntity entity)
        {
            bool? isCustomDisplayField = entity.IsCustomDisplayField;
            if (isCustomDisplayField.HasValue ? isCustomDisplayField.GetValueOrDefault() : false)
            {
                return entity.DisplayFieldName;
            }
            return entity.GetDefaultDisplayFieldName();
        }

        public static SysField GetField(this SysEntity entity, string field_name, DataContext context)
        {
            if (entity.Fields == null)
            {
                throw new ApplicationException("entity.Fieds没有加载");
            }
            return context.FirstOrDefault<SysField>(f => (f.EntityId == entity.EntityId) && (f.FieldName == field_name));
        }

        public static SysField GetKeyField(this SysEntity entity, DataContext context)
        {
            string key_name = entity.GetKeyFieldName();
            SysField field = context.FirstOrDefault<SysField>(f => (f.EntityId == entity.EntityId) && (f.FieldName == key_name));
            if (field == null)
            {
                throw new ApplicationException(string.Format("没有找到主键字段:{0} -{1}", entity.EntityName, key_name));
            }
            return field;
        }

        public static string GetKeyFieldName(this SysEntity entity)
        {
            bool? isCustomKeyField = entity.IsCustomKeyField;
            if (isCustomKeyField.HasValue ? isCustomKeyField.GetValueOrDefault() : false)
            {
                return entity.KeyFieldName;
            }
            return entity.GetDefaultKeyFieldName();
        }

        public static string GetParentFieldName(this SysMoreMoreRelation rel, DataContext context)
        {
            bool? isCustomParentField = rel.IsCustomParentField;
            if (isCustomParentField.HasValue ? isCustomParentField.GetValueOrDefault() : false)
            {
                return rel.ParentFieldName;
            }
            return context.FindById<SysEntity>(new object[] { rel.ParentEntityId }).GetKeyFieldName();
        }
    }
}
