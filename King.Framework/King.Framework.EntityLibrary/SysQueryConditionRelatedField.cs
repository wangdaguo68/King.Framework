namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;

    [Serializable, KingTable]
    public class SysQueryConditionRelatedField : ICachedEntity
    {
        [NonSerialized]
        private CacheContext _metaCache;

        void ICachedEntity.InitNavigationList()
        {
        }

        void ICachedEntity.SetContext(CacheContext context)
        {
            this._metaCache = context;
            this.Field = context.FindById<SysField>(this.FieldId);
            this.OwnerQueryCondition = context.FindById<SysQueryCondition>(this.ConditionId);
            if (this.OwnerQueryCondition != null)
            {
                this.OwnerQueryCondition.RelatedFields.Add(this);
            }
        }

        [KingColumn, KingRef(typeof(SysQueryCondition))]
        public virtual long? ConditionId { get; set; }

        [KingColumn(IsPrimaryKey=true, KeyOrder=1)]
        public virtual long ConditionRelationId { get; set; }

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
                return this.ConditionRelationId;
            }
        }

        [XmlIgnore]
        public virtual SysField Field { get; set; }

        [KingColumn, KingRef(typeof(SysField))]
        public virtual long? FieldId { get; set; }

        [XmlIgnore]
        public virtual SysQueryCondition OwnerQueryCondition { get; set; }
    }
}

