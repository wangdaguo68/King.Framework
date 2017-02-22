namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;

    [Serializable, KingTable]
    public class SysInternalMailSender
    {
        [KingColumn]
        public virtual int? MailId { get; set; }

        [KingColumn(IsPrimaryKey=true)]
        public virtual int SenderId { get; set; }

        [KingColumn]
        public virtual DateTime? SendTime { get; set; }

        [KingColumn]
        public virtual int? State { get; set; }

        [KingColumn]
        public virtual int? UserId { get; set; }
    }
}

