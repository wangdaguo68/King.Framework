namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;

    [Serializable, KingTable]
    public class SysOperationConditionRedirect : ICachedEntity
    {
        [NonSerialized]
        private CacheContext _metaCache;

        void ICachedEntity.InitNavigationList()
        {
            this.RedirectPageParaMeterSets = new List<SysRedirectPageParaMeterSet>();
        }

        void ICachedEntity.SetContext(CacheContext context)
        {
            this._metaCache = context;
            this.OwnerCondition = context.FindById<SysCondition>(this.ConditionId);
            this.RedirectEntity = context.FindById<SysEntity>(this.RedirectEntityId);
            SysRowOperation operation = context.FindById<SysRowOperation>(this.ControlId);
            if (operation != null)
            {
                operation.OperationConditionRedirects.Add(this);
            }
        }

        [KingRef(typeof(SysCondition)), KingColumn]
        public virtual long? ConditionId { get; set; }

        [KingColumn, KingRef(typeof(SysPageControl))]
        public virtual long? ControlId { get; set; }

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
                return this.RedirectId;
            }
        }

        [XmlIgnore]
        public virtual SysCondition OwnerCondition { get; set; }

        [XmlIgnore]
        public virtual SysEntity RedirectEntity { get; set; }

        [KingRef(typeof(SysEntity)), KingColumn]
        public virtual long? RedirectEntityId { get; set; }

        [KingColumn(IsPrimaryKey=true, KeyOrder=1)]
        public virtual long RedirectId { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string RedirectPageName { get; set; }

        [XmlIgnore]
        public virtual ICollection<SysRedirectPageParaMeterSet> RedirectPageParaMeterSets { get; set; }
    }
}

