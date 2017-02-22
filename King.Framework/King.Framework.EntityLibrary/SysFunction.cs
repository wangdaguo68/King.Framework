namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;

    [Serializable, KingTable]
    public class SysFunction : ICachedEntity
    {
        [NonSerialized]
        private CacheContext _metaCache;

        void ICachedEntity.InitNavigationList()
        {
        }

        void ICachedEntity.SetContext(CacheContext context)
        {
            this._metaCache = context;
            this.OwnerEntity = context.FindById<SysEntity>(this.Entity_ID);
            this.OwnerPage = context.FindById<SysPage>(this.PageId);
        }

        [KingColumn]
        public virtual long? AppId { get; set; }

        [KingColumn(MaxLength=500)]
        public virtual string AppURL { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string Category { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string ClassName { get; set; }

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
                return this.Function_ID;
            }
        }

        [KingColumn]
        public virtual long? Entity_ID { get; set; }

        [KingColumn(MaxLength=500)]
        public virtual string Function_Comment { get; set; }

        [KingColumn(IsPrimaryKey=true, KeyOrder=1)]
        public virtual long Function_ID { get; set; }

        [KingColumn]
        public virtual int? FunctionType { get; set; }

        [KingColumn]
        public virtual int? Is_Menu { get; set; }

        [KingColumn]
        public virtual int? IsRelateApp { get; set; }

        [KingColumn]
        public virtual int? OrderIndex { get; set; }

        [XmlIgnore]
        public virtual SysEntity OwnerEntity { get; set; }

        [XmlIgnore]
        public virtual SysPage OwnerPage { get; set; }

        [KingColumn]
        public virtual long? PageId { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string Permission_Name { get; set; }

        [KingColumn]
        public virtual long? Permission_Type { get; set; }

        [KingColumn]
        public virtual int? StyleImageID { get; set; }

        [KingColumn(MaxLength=500)]
        public virtual string URL { get; set; }
    }
}

