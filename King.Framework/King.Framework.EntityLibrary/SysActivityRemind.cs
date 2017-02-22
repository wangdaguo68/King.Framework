namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;

    [KingTable]
    public class SysActivityRemind
    {
        [XmlIgnore]
        public virtual SysActivity Activity { get; set; }

        [KingColumn]
        public virtual long? ActivityId { get; set; }

        [KingColumn]
        public virtual long? ProcessId { get; set; }

        [KingColumn(IsPrimaryKey=true)]
        public virtual long RemindId { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string RemindName { get; set; }

        [XmlIgnore]
        public virtual ICollection<SysActivityRemindParticipant> RemindParticipants { get; set; }

        [KingColumn]
        public virtual int? RemindType { get; set; }

        [KingColumn]
        public virtual int? ResultType { get; set; }

        [KingColumn]
        public virtual int? UseTimeType { get; set; }
    }
}

