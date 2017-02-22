namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;

    [KingTable]
    public class SysActivityParticipant
    {
        [XmlIgnore]
        public virtual SysActivity Activity { get; set; }

        [XmlIgnore]
        public virtual SysActivityApproveGroup ActivityApproveGroup { get; set; }

        [KingColumn]
        public long? ActivityApproveGroupId { get; set; }

        [KingColumn]
        public virtual long? ActivityId { get; set; }

        [KingColumn(IsPrimaryKey=true)]
        public virtual long ActivityParticipantId { get; set; }

        [KingColumn]
        public virtual bool? IsMustApprove { get; set; }

        [KingColumn]
        public virtual int? MinPassNum { get; set; }

        [KingColumn]
        public virtual double? MinPassRatio { get; set; }

        [KingColumn]
        public virtual long? ParticipantId { get; set; }

        [KingColumn]
        public virtual int? PassType { get; set; }

        [KingColumn]
        public virtual long? ProcessId { get; set; }

        [XmlIgnore]
        public virtual SysProcessParticipant ProcessParticipant { get; set; }

        [KingColumn]
        public virtual bool? SumAfterAllComplete { get; set; }
    }
}

