namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;

    [Serializable, KingTable]
    public class SysSharedPrivilege
    {
        [KingColumn]
        public virtual DateTime? CreateTime { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string EntityName { get; set; }

        [KingColumn(IsPrimaryKey=true)]
        public virtual int Id { get; set; }

        [KingColumn]
        public virtual int? ObjectId { get; set; }

        [KingColumn]
        public virtual int? OwnerId { get; set; }

        [KingColumn]
        public virtual int? Privilege { get; set; }

        [KingColumn]
        public virtual int? ShareRoleId { get; set; }

        [KingColumn]
        public virtual int? ShareType { get; set; }

        [KingColumn]
        public virtual int? ShareUserId { get; set; }
    }
}

