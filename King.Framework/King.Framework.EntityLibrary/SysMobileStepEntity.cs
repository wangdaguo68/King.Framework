namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;

    [Serializable, KingTable]
    public class SysMobileStepEntity : ICachedEntity
    {
        [NonSerialized]
        private CacheContext _metaCache;

        void ICachedEntity.InitNavigationList()
        {
        }

        void ICachedEntity.SetContext(CacheContext context)
        {
            this._metaCache = context;
            this.OwnerEntity = context.FindById<SysEntity>(this.EntityId);
            this.OwnerView = context.FindById<SysView>(this.ViewId);
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
                return this.StepEntityId;
            }
        }

        [KingColumn]
        public virtual long? EntityId { get; set; }

        [XmlIgnore]
        public virtual SysEntity OwnerEntity { get; set; }

        [XmlIgnore]
        public virtual SysView OwnerView { get; set; }

        [KingColumn]
        public virtual int? StepDataType { get; set; }

        [KingColumn(IsPrimaryKey=true)]
        public virtual long StepEntityId { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string SystemKey { get; set; }

        [KingColumn]
        public virtual long? ViewId { get; set; }
    }
}

