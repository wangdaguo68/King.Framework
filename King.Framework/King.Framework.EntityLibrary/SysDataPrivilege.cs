namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;

    [Serializable, KingTable(Name="SysDataPrivilege")]
    public class SysDataPrivilege : ICachedEntity
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
        public virtual int? AuthType { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string CustomField { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string CustomValue { get; set; }

        [KingColumn(IsPrimaryKey=true, KeyOrder=1)]
        public virtual long DataPrivilegeId { get; set; }

        [KingColumn]
        public virtual long? DeptId { get; set; }

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
                return (long) this.GetHashCode();
            }
        }

        [KingColumn]
        public virtual long EntityId { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string EntityName { get; set; }

        [KingColumn]
        public virtual int? Operation { get; set; }

        [KingColumn]
        public virtual int OperationId { get; set; }

        [KingColumn]
        public virtual int? PrivilegeLevel { get; set; }

        [KingColumn]
        public virtual long? RoleId { get; set; }

        [KingColumn]
        public virtual long? UserId { get; set; }
    }
}

