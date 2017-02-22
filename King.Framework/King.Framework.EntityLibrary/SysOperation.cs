namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;

    [Serializable]
    public class SysOperation : ICachedEntity
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
                this.OwnerEntity.Operations.Add(this);
            }
        }

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
                return this.OperationPrivilegeID;
            }
        }

        [KingColumn]
        public virtual long? EntityId { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string OperationDisplayText { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string OperationName { get; set; }

        [KingColumn(IsPrimaryKey=true, KeyOrder=1)]
        public virtual long OperationPrivilegeID { get; set; }

        [XmlIgnore]
        public virtual SysEntity OwnerEntity { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string PageName { get; set; }

        [KingColumn]
        public virtual int? State { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string ViewName { get; set; }
    }
}

