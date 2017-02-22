namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;

    [Serializable, KingTable]
    public class SysPageParameter : ICachedEntity
    {
        [NonSerialized]
        private CacheContext _metaCache;

        void ICachedEntity.InitNavigationList()
        {
        }

        void ICachedEntity.SetContext(CacheContext context)
        {
            this._metaCache = context;
            this.OwnerPage = context.FindById<SysPage>(this.PageId);
            if (this.OwnerPage != null)
            {
                this.OwnerPage.PageParameters.Add(this);
            }
        }

        public CacheContext GetMetaCache()
        {
            return this._metaCache;
        }

        [KingColumn(MaxLength=0x3e8)]
        public virtual string Description { get; set; }

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
                return this.ParameterId;
            }
        }

        [XmlIgnore]
        public virtual SysPage OwnerPage { get; set; }

        [KingRef(typeof(SysPage)), KingColumn]
        public virtual long? PageId { get; set; }

        [KingColumn(IsPrimaryKey=true, KeyOrder=1)]
        public virtual long ParameterId { get; set; }

        [KingColumn(MaxLength=100)]
        public virtual string ParameterName { get; set; }

        [KingColumn]
        public virtual int? ParameterType { get; set; }
    }
}

