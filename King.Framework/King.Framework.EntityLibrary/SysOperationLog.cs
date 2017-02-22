namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;

    [Serializable, Header(PrimaryKey="LogId", XmlRootName="Root")]
    public class SysOperationLog : ICachedEntity
    {
        [NonSerialized]
        private CacheContext _metaCache;

        void ICachedEntity.InitNavigationList()
        {
        }

        void ICachedEntity.SetContext(CacheContext context)
        {
            this._metaCache = context;
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
                return this.LogId;
            }
        }

        [KingColumn]
        public virtual long? EntityId { get; set; }

        [KingColumn]
        public virtual long? EnumId { get; set; }

        [KingColumn]
        public virtual int? InfluenceType { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string KeyFieldName { get; set; }

        [KingColumn]
        public virtual long? KeyValue { get; set; }

        [KingColumn(IsPrimaryKey=true)]
        public virtual long LogId { get; set; }

        [KingColumn]
        public virtual DateTime? OperationTime { get; set; }

        [KingColumn]
        public virtual int? OperationType { get; set; }

        [KingColumn]
        public virtual long? PageId { get; set; }

        [KingColumn]
        public virtual long? ParentViewId { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string TableName { get; set; }

        [KingColumn]
        public virtual long? ViewId { get; set; }
    }
}

