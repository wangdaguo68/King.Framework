namespace King.Framework.Linq.Data.Mapping
{
    using King.Framework.Linq;
    using King.Framework.Linq.Data.Common;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;
    using System.Threading;

    public class AttributeMapping : AdvancedMapping
    {
        private Type contextType;
        private static readonly char[] dotSeparator = new char[] { '.' };
        private Dictionary<string, MappingEntity> entities = new Dictionary<string, MappingEntity>();
        private ReaderWriterLock rwLock = new ReaderWriterLock();
        private static readonly char[] separators = new char[] { ' ', ',', '|' };

        public AttributeMapping(Type contextType)
        {
            this.contextType = contextType;
        }

        private MappingEntity CreateEntity(Type elementType, string tableId, Type entityType)
        {
            if (tableId == null)
            {
                tableId = this.GetTableId(elementType);
            }
            HashSet<string> set = new HashSet<string>();
            List<AttributeMappingMember> mappingMembers = new List<AttributeMappingMember>();
            int index = tableId.IndexOf('.');
            string rootEntityId = (index > 0) ? tableId.Substring(0, index) : tableId;
            string path = (index > 0) ? tableId.Substring(index + 1) : "";
            IEnumerable<MappingAttribute> mappingAttributes = this.GetMappingAttributes(rootEntityId);
            IOrderedEnumerable<TableBaseAttribute> source = from ta in mappingAttributes.OfType<TableBaseAttribute>()
                orderby ta.Name
                select ta;
            TableAttribute attribute = source.OfType<TableAttribute>().FirstOrDefault<TableAttribute>();
            if (((attribute != null) && (attribute.EntityType != null)) && (entityType == elementType))
            {
                entityType = attribute.EntityType;
            }
            IOrderedEnumerable<MemberAttribute> enumerable3 = from ma in mappingAttributes.OfType<MemberAttribute>()
                where ma.Member.StartsWith(path)
                orderby ma.Member
                select ma;
            foreach (MemberAttribute attribute2 in enumerable3)
            {
                if (!string.IsNullOrEmpty(attribute2.Member))
                {
                    string str2 = (path.Length == 0) ? attribute2.Member : attribute2.Member.Substring(path.Length + 1);
                    MemberInfo mi = null;
                    MemberAttribute attribute3 = null;
                    AttributeMappingEntity nested = null;
                    if (str2.Contains<char>('.'))
                    {
                        string str3 = str2.Substring(0, str2.IndexOf('.'));
                        if (str3.Contains<char>('.') || set.Contains(str3))
                        {
                            continue;
                        }
                        set.Add(str3);
                        mi = this.FindMember(entityType, str3);
                        string entityID = tableId + "." + str3;
                        nested = (AttributeMappingEntity) this.GetEntity(King.Framework.Linq.TypeHelper.GetMemberType(mi), entityID);
                    }
                    else
                    {
                        if (set.Contains(str2))
                        {
                            throw new InvalidOperationException(string.Format("AttributeMapping: more than one mapping attribute specified for member '{0}' on type '{1}'", str2, entityType.Name));
                        }
                        mi = this.FindMember(entityType, str2);
                        attribute3 = attribute2;
                    }
                    mappingMembers.Add(new AttributeMappingMember(mi, attribute3, nested));
                }
            }
            return new AttributeMappingEntity(elementType, tableId, entityType, source, mappingMembers);
        }

        public override QueryMapper CreateMapper(QueryTranslator translator)
        {
            return new AttributeMapper(this, translator);
        }

        private MemberInfo FindMember(Type type, string path)
        {
            MemberInfo mi = null;
            string[] strArray = path.Split(dotSeparator);
            foreach (string str in strArray)
            {
                mi = type.GetMember(str, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase).FirstOrDefault<MemberInfo>();
                if (mi == null)
                {
                    throw new InvalidOperationException(string.Format("AttributMapping: the member '{0}' does not exist on type '{1}'", str, type.Name));
                }
                type = King.Framework.Linq.TypeHelper.GetElementType(King.Framework.Linq.TypeHelper.GetMemberType(mi));
            }
            return mi;
        }

        public override string GetAlias(MappingTable table)
        {
            return ((AttributeMappingTable) table).Attribute.Alias;
        }

        public override string GetAlias(MappingEntity entity, MemberInfo member)
        {
            AttributeMappingMember mappingMember = ((AttributeMappingEntity) entity).GetMappingMember(member.Name);
            return (((mappingMember != null) && (mappingMember.Column != null)) ? mappingMember.Column.Alias : null);
        }

        public override IEnumerable<MemberInfo> GetAssociationKeyMembers(MappingEntity entity, MemberInfo member)
        {
            AttributeMappingEntity entity2 = (AttributeMappingEntity) entity;
            AttributeMappingMember mappingMember = entity2.GetMappingMember(member.Name);
            if ((mappingMember != null) && (mappingMember.Association != null))
            {
                return this.GetReferencedMembers(entity2, mappingMember.Association.KeyMembers, "Association.KeyMembers", entity2.EntityType);
            }
            return base.GetAssociationKeyMembers(entity, member);
        }

        public override IEnumerable<MemberInfo> GetAssociationRelatedKeyMembers(MappingEntity entity, MemberInfo member)
        {
            AttributeMappingEntity entity2 = (AttributeMappingEntity) entity;
            AttributeMappingEntity relatedEntity = (AttributeMappingEntity) this.GetRelatedEntity(entity, member);
            AttributeMappingMember mappingMember = entity2.GetMappingMember(member.Name);
            if ((mappingMember != null) && (mappingMember.Association != null))
            {
                return this.GetReferencedMembers(relatedEntity, mappingMember.Association.RelatedKeyMembers, "Association.RelatedKeyMembers", entity2.EntityType);
            }
            return base.GetAssociationRelatedKeyMembers(entity, member);
        }

        public override string GetColumnDbType(MappingEntity entity, MemberInfo member)
        {
            AttributeMappingMember mappingMember = ((AttributeMappingEntity) entity).GetMappingMember(member.Name);
            if (!(((mappingMember == null) || (mappingMember.Column == null)) || string.IsNullOrEmpty(mappingMember.Column.DbType)))
            {
                return mappingMember.Column.DbType;
            }
            return null;
        }

        public override string GetColumnName(MappingEntity entity, MemberInfo member)
        {
            AttributeMappingMember mappingMember = ((AttributeMappingEntity) entity).GetMappingMember(member.Name);
            if (!(((mappingMember == null) || (mappingMember.Column == null)) || string.IsNullOrEmpty(mappingMember.Column.Name)))
            {
                return mappingMember.Column.Name;
            }
            return base.GetColumnName(entity, member);
        }

        public override MappingEntity GetEntity(MemberInfo contextMember)
        {
            Type elementType = King.Framework.Linq.TypeHelper.GetElementType(King.Framework.Linq.TypeHelper.GetMemberType(contextMember));
            return this.GetEntity(elementType, contextMember.Name);
        }

        public override MappingEntity GetEntity(Type type, string tableId)
        {
            return this.GetEntity(type, tableId, type);
        }

        private MappingEntity GetEntity(Type elementType, string tableId, Type entityType)
        {
            MappingEntity entity;
            this.rwLock.AcquireReaderLock(-1);
            if (!this.entities.TryGetValue(tableId, out entity))
            {
                this.rwLock.ReleaseReaderLock();
                this.rwLock.AcquireWriterLock(-1);
                if (!this.entities.TryGetValue(tableId, out entity))
                {
                    entity = this.CreateEntity(elementType, tableId, entityType);
                    this.entities.Add(tableId, entity);
                }
                this.rwLock.ReleaseWriterLock();
                return entity;
            }
            this.rwLock.ReleaseReaderLock();
            return entity;
        }

        public override IEnumerable<string> GetExtensionKeyColumnNames(MappingTable table)
        {
            ExtensionTableAttribute attribute = ((AttributeMappingTable) table).Attribute as ExtensionTableAttribute;
            if (attribute == null)
            {
                return new string[0];
            }
            return attribute.KeyColumns.Split(separators);
        }

        public override string GetExtensionRelatedAlias(MappingTable table)
        {
            ExtensionTableAttribute attribute = ((AttributeMappingTable) table).Attribute as ExtensionTableAttribute;
            return ((attribute != null) ? attribute.RelatedAlias : null);
        }

        public override IEnumerable<MemberInfo> GetExtensionRelatedMembers(MappingTable table)
        {
            AttributeMappingTable amt = (AttributeMappingTable) table;
            ExtensionTableAttribute attribute = amt.Attribute as ExtensionTableAttribute;
            if (attribute == null)
            {
                return new MemberInfo[0];
            }
            return (from n in attribute.RelatedKeyColumns.Split(separators) select this.GetMemberForColumn(amt.Entity, n));
        }

        public override IEnumerable<MemberInfo> GetMappedMembers(MappingEntity entity)
        {
            return ((AttributeMappingEntity) entity).MappedMembers;
        }

        protected virtual IEnumerable<MappingAttribute> GetMappingAttributes(string rootEntityId)
        {
            return (MappingAttribute[]) Attribute.GetCustomAttributes(this.FindMember(this.contextType, rootEntityId), typeof(MappingAttribute));
        }

        private MemberInfo GetMemberForColumn(MappingEntity entity, string columnName)
        {
            foreach (MemberInfo info in this.GetMappedMembers(entity))
            {
                if (this.IsNestedEntity(entity, info))
                {
                    MemberInfo memberForColumn = this.GetMemberForColumn(this.GetRelatedEntity(entity, info), columnName);
                    if (memberForColumn != null)
                    {
                        return memberForColumn;
                    }
                }
                else if (this.IsColumn(entity, info) && (string.Compare(this.GetColumnName(entity, info), columnName, true) == 0))
                {
                    return info;
                }
            }
            return null;
        }

        private MappingEntity GetReferencedEntity(Type elementType, string name, Type entityType, string source)
        {
            MappingEntity entity = this.GetEntity(elementType, name, entityType);
            if (entity == null)
            {
                throw new InvalidOperationException(string.Format("The entity '{0}' referenced in {1} of '{2}' does not exist", name, source, entityType.Name));
            }
            return entity;
        }

        private MemberInfo GetReferencedMember(AttributeMappingEntity entity, string name, string source, Type sourceType)
        {
            AttributeMappingMember mappingMember = entity.GetMappingMember(name);
            if (mappingMember == null)
            {
                throw new InvalidOperationException(string.Format("AttributeMapping: The member '{0}.{1}' referenced in {2} for '{3}' is not mapped or does not exist", new object[] { entity.EntityType.Name, name, source, sourceType.Name }));
            }
            return mappingMember.Member;
        }

        private IEnumerable<MemberInfo> GetReferencedMembers(AttributeMappingEntity entity, string names, string source, Type sourceType)
        {
            return (from n in names.Split(separators) select this.GetReferencedMember(entity, n, source, sourceType));
        }

        public override MappingEntity GetRelatedEntity(MappingEntity entity, MemberInfo member)
        {
            AttributeMappingMember mappingMember = ((AttributeMappingEntity) entity).GetMappingMember(member.Name);
            if (mappingMember != null)
            {
                if (mappingMember.Association != null)
                {
                    Type elementType = King.Framework.Linq.TypeHelper.GetElementType(King.Framework.Linq.TypeHelper.GetMemberType(member));
                    Type entityType = (mappingMember.Association.RelatedEntityType != null) ? mappingMember.Association.RelatedEntityType : elementType;
                    return this.GetReferencedEntity(elementType, mappingMember.Association.RelatedEntityID, entityType, "Association.RelatedEntityID");
                }
                if (mappingMember.NestedEntity != null)
                {
                    return mappingMember.NestedEntity;
                }
            }
            return base.GetRelatedEntity(entity, member);
        }

        public override string GetTableId(Type entityType)
        {
            if (this.contextType != null)
            {
                foreach (MemberInfo info in this.contextType.GetMembers(BindingFlags.Public | BindingFlags.Instance))
                {
                    FieldInfo info2 = info as FieldInfo;
                    if ((info2 != null) && (King.Framework.Linq.TypeHelper.GetElementType(info2.FieldType) == entityType))
                    {
                        return info2.Name;
                    }
                    PropertyInfo info3 = info as PropertyInfo;
                    if ((info3 != null) && (King.Framework.Linq.TypeHelper.GetElementType(info3.PropertyType) == entityType))
                    {
                        return info3.Name;
                    }
                }
            }
            return entityType.Name;
        }

        public override string GetTableName(MappingEntity entity)
        {
            AttributeMappingEntity entity2 = (AttributeMappingEntity) entity;
            MappingTable table = entity2.Tables.FirstOrDefault<MappingTable>();
            return this.GetTableName(table);
        }

        public override string GetTableName(MappingTable table)
        {
            AttributeMappingTable table2 = (AttributeMappingTable) table;
            return this.GetTableName(table2.Entity, table2.Attribute);
        }

        private string GetTableName(MappingEntity entity, TableBaseAttribute attr)
        {
            return (((attr != null) && !string.IsNullOrEmpty(attr.Name)) ? attr.Name : entity.TableId);
        }

        public override IList<MappingTable> GetTables(MappingEntity entity)
        {
            return ((AttributeMappingEntity) entity).Tables;
        }

        public override bool IsAssociationRelationship(MappingEntity entity, MemberInfo member)
        {
            AttributeMappingMember mappingMember = ((AttributeMappingEntity) entity).GetMappingMember(member.Name);
            return ((mappingMember != null) && (mappingMember.Association != null));
        }

        public override bool IsColumn(MappingEntity entity, MemberInfo member)
        {
            AttributeMappingMember mappingMember = ((AttributeMappingEntity) entity).GetMappingMember(member.Name);
            return ((mappingMember != null) && (mappingMember.Column != null));
        }

        public override bool IsComputed(MappingEntity entity, MemberInfo member)
        {
            AttributeMappingMember mappingMember = ((AttributeMappingEntity) entity).GetMappingMember(member.Name);
            return (((mappingMember != null) && (mappingMember.Column != null)) && mappingMember.Column.IsComputed);
        }

        public override bool IsExtensionTable(MappingTable table)
        {
            return (((AttributeMappingTable) table).Attribute is ExtensionTableAttribute);
        }

        public override bool IsGenerated(MappingEntity entity, MemberInfo member)
        {
            AttributeMappingMember mappingMember = ((AttributeMappingEntity) entity).GetMappingMember(member.Name);
            return (((mappingMember != null) && (mappingMember.Column != null)) && mappingMember.Column.IsGenerated);
        }

        public override bool IsMapped(MappingEntity entity, MemberInfo member)
        {
            return (((AttributeMappingEntity) entity).GetMappingMember(member.Name) != null);
        }

        public override bool IsNestedEntity(MappingEntity entity, MemberInfo member)
        {
            AttributeMappingMember mappingMember = ((AttributeMappingEntity) entity).GetMappingMember(member.Name);
            return ((mappingMember != null) && (mappingMember.NestedEntity != null));
        }

        public override bool IsPrimaryKey(MappingEntity entity, MemberInfo member)
        {
            AttributeMappingMember mappingMember = ((AttributeMappingEntity) entity).GetMappingMember(member.Name);
            return (((mappingMember != null) && (mappingMember.Column != null)) && mappingMember.Column.IsPrimaryKey);
        }

        public override bool IsRelationshipSource(MappingEntity entity, MemberInfo member)
        {
            AttributeMappingMember mappingMember = ((AttributeMappingEntity) entity).GetMappingMember(member.Name);
            return (((mappingMember != null) && (mappingMember.Association != null)) && !(!mappingMember.Association.IsForeignKey || typeof(IEnumerable).IsAssignableFrom(King.Framework.Linq.TypeHelper.GetMemberType(member))));
        }

        public override bool IsRelationshipTarget(MappingEntity entity, MemberInfo member)
        {
            AttributeMappingMember mappingMember = ((AttributeMappingEntity) entity).GetMappingMember(member.Name);
            return (((mappingMember != null) && (mappingMember.Association != null)) && !(mappingMember.Association.IsForeignKey && !typeof(IEnumerable).IsAssignableFrom(King.Framework.Linq.TypeHelper.GetMemberType(member))));
        }

        private class AttributeMapper : AdvancedMapper
        {
            public AttributeMapper(AttributeMapping mapping, QueryTranslator translator) : base(mapping, translator)
            {
            }
        }

        private class AttributeMappingEntity : MappingEntity
        {
            private Type elementType;
            private Type entityType;
            private Dictionary<string, AttributeMapping.AttributeMappingMember> mappingMembers;
            private string tableId;
            private ReadOnlyCollection<MappingTable> tables;

            internal AttributeMappingEntity(Type elementType, string tableId, Type entityType, IEnumerable<TableBaseAttribute> attrs, IEnumerable<AttributeMapping.AttributeMappingMember> mappingMembers)
            {
                Func<TableBaseAttribute, MappingTable> selector = null;
                this.tableId = tableId;
                this.elementType = elementType;
                this.entityType = entityType;
                if (selector == null)
                {
                    selector = a => new AttributeMapping.AttributeMappingTable(this, a);
                }
                this.tables = attrs.Select<TableBaseAttribute, MappingTable>(selector).ToReadOnly<MappingTable>();
                this.mappingMembers = mappingMembers.ToDictionary<AttributeMapping.AttributeMappingMember, string>(mm => mm.Member.Name);
            }

            internal AttributeMapping.AttributeMappingMember GetMappingMember(string name)
            {
                AttributeMapping.AttributeMappingMember member = null;
                this.mappingMembers.TryGetValue(name, out member);
                return member;
            }

            public override Type ElementType
            {
                get
                {
                    return this.elementType;
                }
            }

            public override Type EntityType
            {
                get
                {
                    return this.entityType;
                }
            }

            internal IEnumerable<MemberInfo> MappedMembers
            {
                get
                {
                    return (from mm in this.mappingMembers.Values select mm.Member);
                }
            }

            public override string TableId
            {
                get
                {
                    return this.tableId;
                }
            }

            internal ReadOnlyCollection<MappingTable> Tables
            {
                get
                {
                    return this.tables;
                }
            }
        }

        private class AttributeMappingMember
        {
            private MemberAttribute attribute;
            private MemberInfo member;
            private AttributeMapping.AttributeMappingEntity nested;

            internal AttributeMappingMember(MemberInfo member, MemberAttribute attribute, AttributeMapping.AttributeMappingEntity nested)
            {
                this.member = member;
                this.attribute = attribute;
                this.nested = nested;
            }

            internal AssociationAttribute Association
            {
                get
                {
                    return (this.attribute as AssociationAttribute);
                }
            }

            internal ColumnAttribute Column
            {
                get
                {
                    return (this.attribute as ColumnAttribute);
                }
            }

            internal MemberInfo Member
            {
                get
                {
                    return this.member;
                }
            }

            internal AttributeMapping.AttributeMappingEntity NestedEntity
            {
                get
                {
                    return this.nested;
                }
            }
        }

        private class AttributeMappingTable : MappingTable
        {
            private TableBaseAttribute attribute;
            private AttributeMapping.AttributeMappingEntity entity;

            internal AttributeMappingTable(AttributeMapping.AttributeMappingEntity entity, TableBaseAttribute attribute)
            {
                this.entity = entity;
                this.attribute = attribute;
            }

            public TableBaseAttribute Attribute
            {
                get
                {
                    return this.attribute;
                }
            }

            public AttributeMapping.AttributeMappingEntity Entity
            {
                get
                {
                    return this.entity;
                }
            }
        }
    }
}
