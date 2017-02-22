namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;

    [KingTable]
    public class SysActivity
    {
        [KingColumn(IsPrimaryKey=true)]
        public virtual long ActivityId { get; set; }

        [XmlIgnore]
        public virtual ICollection<SysActivityInstance> ActivityInstances { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string ActivityName { get; set; }

        [XmlIgnore]
        public virtual ICollection<SysActivityOperation> ActivityOperations { get; set; }

        [XmlIgnore]
        public virtual ICollection<SysActivityParticipant> ActivityParticipants { get; set; }

        [XmlIgnore]
        public virtual ICollection<SysActivityRemind> ActivityReminds { get; set; }

        [KingColumn]
        public virtual int? ActivityType { get; set; }

        [KingColumn]
        public virtual int? ApproveResult { get; set; }

        [KingColumn]
        public virtual DateTime? DeadLine { get; set; }

        [KingColumn]
        public virtual bool? DisableParticipantGroup { get; set; }

        [KingColumn]
        public virtual int? DisplayOrder { get; set; }

        [KingColumn]
        public virtual int? ExecType { get; set; }

        [XmlIgnore]
        public virtual SysExpression Expression { get; set; }

        [KingColumn]
        public virtual long? ExpressionId { get; set; }

        [XmlIgnore]
        public virtual ICollection<SysExpression> Expressions { get; set; }

        [XmlIgnore]
        public virtual ICollection<SysTransition> FromTransitions { get; set; }

        [KingColumn]
        public virtual int? Height { get; set; }

        [KingColumn]
        public virtual bool? IsAllowAddApproveUser { get; set; }

        [KingColumn]
        public virtual bool? IsPassedWithNoParticipants { get; set; }

        [KingColumn(MaxLength=-1)]
        public virtual string JScriptText { get; set; }

        [KingColumn]
        public virtual double? LeftPos { get; set; }

        [KingColumn]
        public virtual int? MinPassNum { get; set; }

        [KingColumn]
        public virtual double? MinPassRatio { get; set; }

        [KingColumn]
        public virtual long? PageId { get; set; }

        [KingColumn]
        public virtual int? PassType { get; set; }

        [XmlIgnore]
        public virtual SysProcess Process { get; set; }

        [KingColumn]
        public virtual long? ProcessId { get; set; }

        [KingColumn]
        public virtual bool? SumAfterAllComplete { get; set; }

        [KingColumn]
        public virtual int? TimeLimit { get; set; }

        [KingColumn(MaxLength=-1)]
        public virtual string TimeOutScript { get; set; }

        [KingColumn]
        public virtual double? TopPos { get; set; }

        [XmlIgnore]
        public virtual ICollection<SysTransition> ToTransitions { get; set; }

        [KingColumn(MaxLength=-1)]
        public virtual string VbExpression { get; set; }

        [KingColumn]
        public virtual int? Width { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string WwfActivityId { get; set; }
    }
}

