namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;

    [Serializable, KingTable]
    public class SysBusinessLogicOperation : ICachedEntity
    {
        [NonSerialized]
        private CacheContext _metaCache;

        void ICachedEntity.InitNavigationList()
        {
        }

        void ICachedEntity.SetContext(CacheContext context)
        {
            this._metaCache = context;
            this.OwnerEntity = context.FindById<SysEntity>(this.EntityId);
            if (this.OwnerEntity != null)
            {
                this.OwnerEntity.BusinessLogicOperations.Add(this);
            }
        }

        public CacheContext GetMetaCache()
        {
            return this._metaCache;
        }

        [KingColumn(MaxLength=100)]
        public virtual string ClassName { get; set; }

        [KingColumn(MaxLength=500)]
        public virtual string Description { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual byte[] DllContent { get; set; }

        [KingColumn(MaxLength=50)]
        public virtual string DllFileName { get; set; }

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
                return this.OperationId;
            }
        }

        [KingColumn, KingRef(typeof(SysEntity))]
        public virtual long? EntityId { get; set; }

        [KingColumn(MaxLength=100)]
        public virtual string MethodName { get; set; }

        [KingColumn(MaxLength=100)]
        public virtual string NameSpaceName { get; set; }

        [KingColumn(IsPrimaryKey=true)]
        public virtual long OperationId { get; set; }

        [KingColumn(MaxLength=100)]
        public virtual string OperationName { get; set; }

        [XmlIgnore]
        public virtual SysEntity OwnerEntity { get; set; }
    }
}

