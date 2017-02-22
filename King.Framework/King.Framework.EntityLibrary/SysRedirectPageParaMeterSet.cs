namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;

    [Serializable, KingTable]
    public class SysRedirectPageParaMeterSet : ICachedEntity
    {
        [NonSerialized]
        private CacheContext _metaCache;

        void ICachedEntity.InitNavigationList()
        {
        }

        void ICachedEntity.SetContext(CacheContext context)
        {
            this._metaCache = context;
            this.OwnerOperationConditionRedirect = context.FindById<SysOperationConditionRedirect>(this.RedirectId);
            if (this.OwnerOperationConditionRedirect != null)
            {
                this.OwnerOperationConditionRedirect.RedirectPageParaMeterSets.Add(this);
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
                return this.SetId;
            }
        }

        [XmlIgnore]
        public virtual SysOperationConditionRedirect OwnerOperationConditionRedirect { get; set; }

        [KingColumn, KingRef(typeof(SysOperationConditionRedirect))]
        public virtual long? RedirectId { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string RedirectPageParaMeterName { get; set; }

        [KingColumn(IsPrimaryKey=true, KeyOrder=1)]
        public virtual long SetId { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string ValueName { get; set; }

        [KingColumn]
        public virtual int? ValueType { get; set; }
    }
}

