namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;

    [KingTable]
    public class SysProcessRemind
    {
        [XmlIgnore]
        public virtual SysProcess Process { get; set; }

        [KingColumn]
        public virtual long? ProcessId { get; set; }

        [KingColumn(IsPrimaryKey=true)]
        public virtual long RemindId { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string RemindName { get; set; }

        [XmlIgnore]
        public virtual ICollection<SysProcessRemindParticipant> RemindParticipants { get; set; }

        [KingColumn]
        public virtual int? RemindType { get; set; }

        [KingColumn]
        public virtual int? UseTimeType { get; set; }
    }
}

