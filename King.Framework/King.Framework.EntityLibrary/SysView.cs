namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;

    [Serializable, KingTable]
    public class SysView : ICachedEntity, IKeyObject
    {
        [NonSerialized]
        private CacheContext _metaCache;
        private readonly HashSet<string> _OwnSet = new HashSet<string>();
        private readonly HashSet<string> _RefSet = new HashSet<string>();

        void ICachedEntity.InitNavigationList()
        {
            this.ViewConditions = new List<SysViewCondition>();
            this.ViewFields = new List<SysViewField>();
            this.ViewOrderFields = new List<SysViewOrderField>();
        }

        void ICachedEntity.SetContext(CacheContext context)
        {
            this._metaCache = context;
            this.OwnerEntity = context.FindById<SysEntity>(this.EntityId);
            if (this.OwnerEntity != null)
            {
                this.OwnerEntity.Views.Add(this);
            }
        }

        public string GetKey()
        {
            return string.Format("{0}/{1}:{2}", this.OwnerEntity.GetKey(), base.GetType().Name, this.ViewName.ToLower());
        }

        public string GetUniqueId()
        {
            return string.Format("{0}${1}", base.GetType().Name, ((ICachedEntity) this).PrimaryKeyValue);
        }

        public override string ToString()
        {
            if (this.OwnerEntity != null)
            {
                return string.Format("视图:{0}/{1}-{2}", this.OwnerEntity.EntityName, this.ViewName, this.DisplayText);
            }
            return string.Format("视图:{0}-{1}", this.ViewName, this.DisplayText);
        }

        [KingColumn(MaxLength=0x3e8)]
        public virtual string Description { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string DisplayText { get; set; }

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
                return this.ViewId;
            }
        }

        [KingColumn, KingRef(typeof(SysEntity))]
        public virtual long? EntityId { get; set; }

        [KingColumn]
        public virtual bool? IsAllowPage { get; set; }

        [KingColumn]
        public virtual bool? IsDefault { get; set; }

        [KingColumn]
        public virtual bool? IsMobileLocal { get; set; }

        [XmlIgnore]
        public virtual SysEntity OwnerEntity { get; set; }

        public HashSet<string> OwnSet
        {
            get
            {
                return this._OwnSet;
            }
        }

        public HashSet<string> RefSet
        {
            get
            {
                return this._RefSet;
            }
        }

        [KingColumn]
        public virtual int? ShareStartType { get; set; }

        [XmlIgnore]
        public virtual ICollection<SysViewCondition> ViewConditions { get; set; }

        [XmlIgnore]
        public virtual ICollection<SysViewField> ViewFields { get; set; }

        [KingColumn(IsPrimaryKey=true, KeyOrder=1)]
        public virtual long ViewId { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string ViewName { get; set; }

        [XmlIgnore]
        public virtual ICollection<SysViewOrderField> ViewOrderFields { get; set; }
    }
}

