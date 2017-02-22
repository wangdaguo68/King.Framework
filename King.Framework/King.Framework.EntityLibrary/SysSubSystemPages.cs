namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;

    [KingTable]
    public class SysSubSystemPages : ICachedEntity
    {
        [NonSerialized]
        private CacheContext _metaCache;

        void ICachedEntity.InitNavigationList()
        {
        }

        void ICachedEntity.SetContext(CacheContext context)
        {
            this._metaCache = context;
            this.Page = context.FindById<SysPage>(this.PageId);
            this.SubSystem = context.FindById<SysSubSystem>(this.SubSystemId);
            if ((this.Page != null) && (this.SubSystem != null))
            {
                this.Page.SubSystems.Add(this.SubSystem);
                this.SubSystem.Pages.Add(this.Page);
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
                return this.Id;
            }
        }

        [KingColumn(IsPrimaryKey=true)]
        public virtual long Id { get; set; }

        [XmlIgnore]
        public virtual SysPage Page { get; set; }

        [KingRef(typeof(SysPage)), KingColumn]
        public virtual long? PageId { get; set; }

        [XmlIgnore]
        public virtual SysSubSystem SubSystem { get; set; }

        [KingColumn, KingRef(typeof(SysSubSystem))]
        public virtual long? SubSystemId { get; set; }
    }
}

