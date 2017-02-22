namespace King.Framework.Linq
{
    using King.Framework.DAL;
    using King.Framework.Linq.Data.Common;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    internal class KingMapping : BasicMapping
    {
        private static readonly object lockObj = new object();

        private Dictionary<MemberInfo, Column> GetColumnMapper(MappingEntity entity)
        {
            Dictionary<MemberInfo, Column> dictionary = new Dictionary<MemberInfo, Column>();
            lock (lockObj)
            {
                Table tableOrCreate = TableCache.GetTableOrCreate(entity.EntityType);
                foreach (Column column in tableOrCreate.Columns)
                {
                    dictionary.Add(column.PropertyInfo, column);
                }
            }
            return dictionary;
        }

        public override string GetColumnName(MappingEntity entity, MemberInfo member)
        {
            return this.GetColumnMapper(entity)[member].ColumnName;
        }

        public override MappingEntity GetEntity(Type type)
        {
            Table tableOrCreate = TableCache.GetTableOrCreate(type);
            return this.GetEntity(type, tableOrCreate.TableName);
        }

        public override IEnumerable<MemberInfo> GetMappedMembers(MappingEntity entity)
        {
            return this.GetColumnMapper(entity).Keys;
        }

        public override IEnumerable<MemberInfo> GetPrimaryKeyMembers(MappingEntity entity)
        {
            return TableCache.GetTableOrCreate(entity.EntityType).KeyMembers;
        }

        public override string GetTableId(Type type)
        {
            return TableCache.GetTableOrCreate(type).TableName;
        }

        public override string GetTableName(MappingEntity entity)
        {
            Table tableOrCreate = TableCache.GetTableOrCreate(entity.EntityType);
            return string.Format("{0}.{1}", tableOrCreate.SchemaName, tableOrCreate.TableName);
        }

        public override bool IsPrimaryKey(MappingEntity entity, MemberInfo member)
        {
            return this.GetColumnMapper(entity)[member].IsPrimaryKey;
        }

        public override bool IsRelationshipSource(MappingEntity entity, MemberInfo member)
        {
            return false;
        }

        public override bool IsRelationshipTarget(MappingEntity entity, MemberInfo member)
        {
            return false;
        }
    }
}
