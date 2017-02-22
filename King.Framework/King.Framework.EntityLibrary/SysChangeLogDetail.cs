namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;

    [Serializable, KingTable]
    public class SysChangeLogDetail : ICachedEntity
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

        [KingColumn(MaxLength=200)]
        public virtual string CurrentValue { get; set; }

        [KingColumn]
        public virtual int? DataType { get; set; }

        [KingColumn(IsPrimaryKey=true)]
        public virtual long DetailId { get; set; }

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
                return this.DetailId;
            }
        }

        [KingColumn(MaxLength=200)]
        public virtual string FieldName { get; set; }

        [KingColumn]
        public virtual long? LogId { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string OriginalValue { get; set; }
    }
}

