namespace King.Framework.Linq.Data.Common
{
    using King.Framework.Linq;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public abstract class AdvancedMapping : BasicMapping
    {
        protected AdvancedMapping()
        {
        }

        public override object CloneEntity(MappingEntity entity, object instance)
        {
            object obj2 = base.CloneEntity(entity, instance);
            foreach (MemberInfo info in this.GetMappedMembers(entity))
            {
                if (this.IsNestedEntity(entity, info))
                {
                    MappingEntity relatedEntity = this.GetRelatedEntity(entity, info);
                    if (info.GetValue(instance) != null)
                    {
                        object obj4 = this.CloneEntity(relatedEntity, info.GetValue(instance));
                        info.SetValue(obj2, obj4);
                    }
                }
            }
            return obj2;
        }

        public override QueryMapper CreateMapper(QueryTranslator translator)
        {
            return new AdvancedMapper(this, translator);
        }

        public abstract string GetAlias(MappingTable table);
        public abstract string GetAlias(MappingEntity entity, MemberInfo member);
        public abstract IEnumerable<string> GetExtensionKeyColumnNames(MappingTable table);
        public abstract string GetExtensionRelatedAlias(MappingTable table);
        public abstract IEnumerable<MemberInfo> GetExtensionRelatedMembers(MappingTable table);
        public abstract string GetTableName(MappingTable table);
        public abstract IList<MappingTable> GetTables(MappingEntity entity);
        public abstract bool IsExtensionTable(MappingTable table);
        public override bool IsModified(MappingEntity entity, object instance, object original)
        {
            if (base.IsModified(entity, instance, original))
            {
                return true;
            }
            foreach (MemberInfo info in this.GetMappedMembers(entity))
            {
                if (this.IsNestedEntity(entity, info))
                {
                    MappingEntity relatedEntity = this.GetRelatedEntity(entity, info);
                    if (this.IsModified(relatedEntity, info.GetValue(instance), info.GetValue(original)))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public abstract bool IsNestedEntity(MappingEntity entity, MemberInfo member);
        public override bool IsRelationship(MappingEntity entity, MemberInfo member)
        {
            return (base.IsRelationship(entity, member) || this.IsNestedEntity(entity, member));
        }
    }
}
