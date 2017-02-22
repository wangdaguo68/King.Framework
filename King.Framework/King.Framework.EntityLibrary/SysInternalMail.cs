namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;

    [Serializable, KingTable]
    public class SysInternalMail
    {
        [KingColumn(MaxLength=-1)]
        public virtual string Body { get; set; }

        [KingColumn]
        public virtual DateTime? CreateTime { get; set; }

        [KingColumn(IsPrimaryKey=true)]
        public virtual int MailId { get; set; }

        [KingColumn(MaxLength=0x100)]
        public virtual string Subject { get; set; }
    }
}

