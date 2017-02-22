namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;

    [Serializable, KingTable]
    public class SysInternalMailReceiver
    {
        [KingColumn]
        public virtual bool? IsRead { get; set; }

        [KingColumn(IsPrimaryKey=true)]
        public virtual int ReceiverId { get; set; }

        [KingColumn]
        public virtual DateTime? ReceiveTime { get; set; }

        [KingColumn]
        public virtual int? SenderId { get; set; }

        [KingColumn]
        public virtual int? State { get; set; }

        [KingColumn]
        public virtual int? UserId { get; set; }
    }
}

