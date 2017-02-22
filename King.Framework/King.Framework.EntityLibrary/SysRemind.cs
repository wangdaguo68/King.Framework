namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;

    [KingTable]
    public class SysRemind
    {
        [KingColumn(MaxLength=-1)]
        public virtual string Content { get; set; }

        [KingColumn]
        public virtual DateTime? CreateTime { get; set; }

        [KingColumn]
        public virtual int? CreateUserId { get; set; }

        [KingColumn]
        public virtual DateTime? DeadLine { get; set; }

        [KingColumn]
        public virtual int? OwnerId { get; set; }

        [KingColumn(IsPrimaryKey=true)]
        public virtual int RemindId { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string RemindName { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string RemindURL { get; set; }

        [KingColumn]
        public virtual int? State { get; set; }
    }
}

