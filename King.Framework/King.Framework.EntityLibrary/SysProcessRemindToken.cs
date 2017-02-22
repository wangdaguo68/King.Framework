namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;

    [KingTable]
    public class SysProcessRemindToken
    {
        [XmlIgnore]
        public virtual SysActivity Activity { get; set; }

        [KingColumn]
        public virtual long? ActivityId { get; set; }

        [KingColumn]
        public virtual DateTime? FailTime { get; set; }

        [XmlIgnore]
        public virtual SysProcess Process { get; set; }

        [KingColumn]
        public virtual long? ProcessId { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string Token { get; set; }

        [KingColumn(IsPrimaryKey=true)]
        public virtual long TokenId { get; set; }

        [KingColumn]
        public virtual int? UserId { get; set; }

        [KingColumn]
        public virtual int? WorkItemBaseId { get; set; }

        [KingColumn]
        public virtual int? WorkItemId { get; set; }
    }
}

