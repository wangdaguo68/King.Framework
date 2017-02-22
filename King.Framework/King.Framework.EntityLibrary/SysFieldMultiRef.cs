namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;

    [Serializable, KingTable]
    public class SysFieldMultiRef : ICachedEntity
    {
        [NonSerialized]
        private CacheContext _metaCache;

        void ICachedEntity.InitNavigationList()
        {
        }

        void ICachedEntity.SetContext(CacheContext context)
        {
            this._metaCache = context;
            this.OwnerField = context.FindById<SysField>(this.FieldId);
            this.RefEntity = context.FindById<SysEntity>(this.EntityId);
            if (this.OwnerField != null)
            {
                this.OwnerField.MultiRefs.Add(this);
            }
        }

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
                return this.MultiRefId;
            }
        }

        [KingRef(typeof(SysEntity)), KingColumn]
        public virtual long? EntityId { get; set; }

        [KingRef(typeof(SysField)), KingColumn]
        public virtual long? FieldId { get; set; }

        [KingColumn(IsPrimaryKey=true)]
        public virtual long MultiRefId { get; set; }

        [XmlIgnore]
        public virtual SysField OwnerField { get; set; }

        [XmlIgnore]
        public virtual SysEntity RefEntity { get; set; }
    }
}

