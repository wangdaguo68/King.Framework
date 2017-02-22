namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;

    [Serializable, KingTable]
    public class SysChangeLog : ICachedEntity
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

        [KingColumn]
        public virtual DateTime? ChangeTime { get; set; }

        [KingColumn]
        public virtual int? ChangeUserId { get; set; }

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

        [KingColumn(MaxLength=200)]
        public virtual string EntityName { get; set; }

        [KingColumn(IsPrimaryKey=true)]
        public virtual long LogId { get; set; }

        [KingColumn]
        public virtual int? ObjectId { get; set; }
    }
}

