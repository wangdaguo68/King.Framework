namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;

    [Serializable, KingTable]
    public class SysInternalMailAttachment
    {
        [KingColumn]
        public virtual int? AttachmentId { get; set; }

        [KingColumn(IsPrimaryKey=true)]
        public virtual int Id { get; set; }

        [KingColumn]
        public virtual int? MailId { get; set; }
    }
}

