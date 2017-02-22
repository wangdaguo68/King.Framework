namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;

    [Serializable, KingTable]
    public class SysPageControl : ICachedEntity, IKeyObject
    {
        [NonSerialized]
        private CacheContext _metaCache;

        public virtual string GetKey()
        {
            if (this.OwnerEntity != null)
            {
                return string.Format("{0}:/{1}-{2}-{3}", new object[] { this.OwnerEntity.GetKey(), this.Page.PageName.ToLower(), this.ControlType, this.ControlName });
            }
            return string.Empty;
        }

        public virtual void InitNavigationList()
        {
            this.Children = new List<SysPageControl>();
        }

        public virtual void SetContext(CacheContext context)
        {
            this._metaCache = context;
            this.OwnerEntity = context.FindById<SysEntity>(this.EntityId);
            this.Parent = context.FindById<SysPageControl>(this.ParentId);
            this.VisibleCondition = context.FindById<SysCondition>(this.VisibleConditionId);
            this.EnableCondition = context.FindById<SysCondition>(this.EnableConditionId);
            if (this.Parent != null)
            {
                this.Parent.Children.Add(this);
            }
            if (this.OwnerEntity != null)
            {
                this.OwnerEntity.PageControls.Add(this);
            }
            this.Page = context.FindById<SysPage>(this.PageId);
        }

        [XmlIgnore]
        public virtual ICollection<SysPageControl> Children { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string ControlExtension { get; set; }

        [KingColumn(IsPrimaryKey=true, KeyOrder=1)]
        public virtual long ControlId { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string ControlName { get; set; }

        [KingColumn]
        public virtual int ControlType { get; set; }

        [KingColumn]
        public virtual int? DisplayControlType { get; set; }

        CacheContext ICachedEntity.MetaCache
        {
            get
            {
                return this._metaCache;
            }
        }

        [XmlIgnore]
        public virtual SysCondition EnableCondition { get; set; }

        [KingColumn, KingRef(typeof(SysCondition))]
        public virtual long? EnableConditionId { get; set; }

        [KingRef(typeof(SysEntity)), KingColumn]
        public virtual long? EntityId { get; set; }

        [KingColumn]
        public virtual bool? IsHidden { get; set; }

        [KingColumn]
        public virtual bool? IsLock { get; set; }

        [KingColumn]
        public virtual int OrderIndex { get; set; }

        [XmlIgnore]
        public virtual SysEntity OwnerEntity { get; set; }

        [XmlIgnore]
        public virtual SysPage Page { get; set; }

        [KingColumn, KingRef(typeof(SysPage))]
        public virtual long? PageId { get; set; }

        [XmlIgnore]
        public virtual SysPageControl Parent { get; set; }

        [KingRef(typeof(SysPageControl)), KingColumn]
        public virtual long? ParentId { get; set; }

        public virtual long PrimaryKeyValue
        {
            get
            {
                return this.ControlId;
            }
        }

        [XmlIgnore]
        public virtual SysCondition VisibleCondition { get; set; }

        [KingRef(typeof(SysCondition)), KingColumn]
        public virtual long? VisibleConditionId { get; set; }
    }
}

