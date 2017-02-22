namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;

    [KingTable]
    public class SysActivityApproveGroup
    {
        [XmlIgnore]
        public virtual SysActivity Activity { get; set; }

        [KingColumn(IsPrimaryKey=true)]
        public virtual long ActivityApproveGroupId { get; set; }

        [KingColumn]
        public virtual long? ActivityId { get; set; }

        [XmlIgnore]
        public virtual ICollection<SysActivityParticipant> ActivityParticipants { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string GroupName { get; set; }

        [KingColumn]
        public virtual bool? IsDefault { get; set; }

        [KingColumn]
        public virtual bool? IsMustApprove { get; set; }

        [KingColumn]
        public virtual int? MinPassNum { get; set; }

        [KingColumn]
        public virtual double? MinPassRatio { get; set; }

        [KingColumn]
        public virtual int? PassType { get; set; }

        [KingColumn]
        public long? ProcessId { get; set; }

        [KingColumn]
        public virtual bool? SumAfterAllComplete { get; set; }
    }
}

