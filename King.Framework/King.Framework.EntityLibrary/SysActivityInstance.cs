namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;

    [KingTable]
    public class SysActivityInstance
    {
        [XmlIgnore]
        public virtual SysActivity Activity { get; set; }

        [KingColumn]
        public virtual long? ActivityId { get; set; }

        [KingColumn(IsPrimaryKey=true)]
        public virtual int ActivityInstanceId { get; set; }

        [XmlIgnore]
        public virtual ICollection<SysWorkItemApproveGroup> ApproveGroups { get; set; }

        [KingColumn]
        public virtual int? ApproveResult { get; set; }

        [KingColumn]
        public virtual DateTime? EndTime { get; set; }

        [KingColumn]
        public virtual int? ExpressionValue { get; set; }

        [XmlIgnore]
        public virtual ICollection<SysTransitionInstance> FromTransitionInstances { get; set; }

        [KingColumn]
        public virtual int InstanceStatus { get; set; }

        [XmlIgnore]
        public virtual SysProcessInstance ProcessInstance { get; set; }

        [KingColumn]
        public virtual int? ProcessInstanceId { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string Remark { get; set; }

        [KingColumn]
        public virtual DateTime? StartTime { get; set; }

        [XmlIgnore]
        public virtual ICollection<SysTransitionInstance> ToTransitionInstances { get; set; }

        [XmlIgnore]
        public virtual ICollection<SysActivityInstanceApproveUsers> UserDefinedApproveUsers { get; set; }

        [XmlIgnore]
        public virtual ICollection<SysWorkItem> WorkItems { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string WwfActivityInstanceId { get; set; }
    }
}

