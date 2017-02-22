namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using King.Framework.LiteQueryDef.Internal;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Xml.Serialization;

    [Serializable, KingTable]
    public class SysCondition : ICachedEntity, IKeyObject
    {
        [NonSerialized]
        private CacheContext _metaCache;

        void ICachedEntity.InitNavigationList()
        {
            this.ChildConditions = new List<SysCondition>();
        }

        void ICachedEntity.SetContext(CacheContext context)
        {
            this._metaCache = context;
            this.Field = context.FindById<SysField>(this.FieldId);
            this.RefRelation = context.FindById<SysOneMoreRelation>(this.RelationId);
            this.ParentCondition = context.FindById<SysCondition>(this.ParentId);
            this.OwnerEntity = context.FindById<SysEntity>(this.EntityId);
            if (this.OwnerEntity != null)
            {
                this.OwnerEntity.Conditions.Add(this);
            }
            if (this.ParentCondition != null)
            {
                this.ParentCondition.ChildConditions.Add(this);
            }
        }

        private void GetChildKey(StringBuilder sb, SysCondition child)
        {
            if (child.FilterType == 2)
            {
                sb.AppendFormat("{0} {1} {2}", child.Field.FieldName, (CompareTypeEnum) child.CompareType.Value, child.ConditionValue);
            }
            else
            {
                int num = 0;
                foreach (SysCondition condition in child.ChildConditions)
                {
                    sb.Append("(");
                    this.GetChildKey(sb, condition);
                    sb.Append(")");
                    num++;
                    if (num < child.ChildConditions.Count)
                    {
                        sb.AppendFormat(" {0} ", (FilterTypeEnum) child.FilterType.Value);
                    }
                }
            }
        }

        public string GetKey()
        {
            if (this.ParentCondition == null)
            {
                StringBuilder sb = new StringBuilder(0x80);
                sb.Append(this.OwnerEntity.GetKey());
                this.GetChildKey(sb, this);
                return sb.ToString();
            }
            return this.ParentCondition.GetKey();
        }

        [XmlIgnore]
        public virtual ICollection<SysCondition> ChildConditions { get; set; }

        [KingColumn]
        public virtual int? CompareType { get; set; }

        [KingColumn(IsPrimaryKey=true)]
        public virtual long ConditionId { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string ConditionValue { get; set; }

        CacheContext ICachedEntity.MetaCache
        {
            get
            {
                return this._metaCache;
            }
        }

        long ICachedEntity.PrimaryKeyValue
        {
            get
            {
                return this.ConditionId;
            }
        }

        [KingColumn(MaxLength=0x7d0)]
        public virtual string EFCompareString { get; set; }

        [KingColumn, KingRef(typeof(SysEntity))]
        public virtual long? EntityId { get; set; }

        [XmlIgnore]
        public virtual SysField Field { get; set; }

        [KingRef(typeof(SysField)), KingColumn]
        public virtual long? FieldId { get; set; }

        [KingColumn]
        public virtual int? FilterType { get; set; }

        [KingColumn]
        public virtual int? OrderIndex { get; set; }

        [XmlIgnore]
        public virtual SysEntity OwnerEntity { get; set; }

        [XmlIgnore]
        public virtual SysCondition ParentCondition { get; set; }

        [KingRef(typeof(SysCondition)), KingColumn]
        public virtual long? ParentId { get; set; }

        [XmlIgnore]
        public virtual SysOneMoreRelation RefRelation { get; set; }

        [KingColumn, KingRef(typeof(SysOneMoreRelation))]
        public virtual long? RelationId { get; set; }
    }
}

