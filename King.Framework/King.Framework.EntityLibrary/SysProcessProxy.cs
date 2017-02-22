namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;

    [KingTable]
    public class SysProcessProxy
    {
        [KingColumn]
        public virtual DateTime? CreateTime { get; set; }

        [KingColumn]
        public virtual DateTime? DisableTime { get; set; }

        [KingColumn]
        public virtual DateTime? EndTime { get; set; }

        [KingColumn]
        public virtual int? OwnerId { get; set; }

        [XmlIgnore]
        public virtual SysProcess Process { get; set; }

        [KingColumn]
        public virtual long? ProcessId { get; set; }

        [KingColumn(IsPrimaryKey=true)]
        public virtual int ProcessProxyId { get; set; }

        [KingColumn]
        public virtual int? ProxyId { get; set; }

        [KingColumn]
        public virtual DateTime? StartTime { get; set; }

        [KingColumn]
        public virtual int? Status { get; set; }
    }
}

