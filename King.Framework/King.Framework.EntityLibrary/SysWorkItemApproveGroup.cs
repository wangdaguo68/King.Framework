namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;

    [KingTable]
    public class SysWorkItemApproveGroup
    {
        [KingColumn]
        public long? ActivityApproveGroupId { get; set; }

        [XmlIgnore]
        public SysActivityInstance ActivityInstance { get; set; }

        [KingColumn]
        public int? ActivityInstanceId { get; set; }

        [KingColumn]
        public long? ActivityParticipantId { get; set; }

        [KingColumn(IsPrimaryKey=true)]
        public int ApproveGroupId { get; set; }

        [KingColumn]
        public int? ApproveResult { get; set; }

        [KingColumn]
        public DateTime? ApproveTime { get; set; }

        [KingColumn]
        public DateTime? CreateTime { get; set; }

        [KingColumn]
        public long? ParticipantId { get; set; }

        [KingColumn]
        public virtual int? ProcessInstanceId { get; set; }

        [XmlIgnore]
        public ICollection<SysWorkItem> WorkItems { get; set; }
    }
}

