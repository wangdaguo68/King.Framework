namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;

    [KingTable]
    public class SysWorkItem
    {
        [XmlIgnore]
        public virtual SysActivityInstance ActivityInstance { get; set; }

        [KingColumn]
        public virtual int? ActivityInstanceId { get; set; }

        [XmlIgnore]
        public virtual SysActivityParticipant ActivityParticipant { get; set; }

        [KingColumn]
        public virtual long? ActivityParticipantId { get; set; }

        [KingColumn]
        public virtual int? AddingUserId { get; set; }

        [XmlIgnore]
        public virtual SysWorkItemApproveGroup ApproveGroup { get; set; }

        [KingColumn]
        public virtual int? ApproveGroupId { get; set; }

        [KingColumn]
        public virtual DateTime? CreateTime { get; set; }

        [KingColumn]
        public virtual DateTime? DeadLine { get; set; }

        [KingColumn]
        public virtual DateTime? EndTime { get; set; }

        [KingColumn]
        public virtual int? FlagInt { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string FlagString { get; set; }

        [KingColumn]
        public virtual bool? IsAdded { get; set; }

        [KingColumn]
        public virtual bool? IsMajor { get; set; }

        [KingColumn]
        public virtual bool? IsProxy { get; set; }

        [KingColumn]
        public virtual int? OwnerId { get; set; }

        [KingColumn]
        public virtual long? ParticipantId { get; set; }

        [XmlIgnore]
        public virtual SysProcessInstance ProcessInstance { get; set; }

        [KingColumn]
        public virtual int? ProcessInstanceId { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string ProcessPageUrl { get; set; }

        [KingColumn]
        public virtual int? ProxyUserId { get; set; }

        [KingColumn]
        public virtual int? ProxyWorkItemId { get; set; }

        [KingColumn]
        public virtual int? RelativeObjectId { get; set; }

        [KingColumn]
        public virtual int? Status { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string TimeOutScript { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string Title { get; set; }

        [KingColumn]
        public virtual int? WorkItemBaseId { get; set; }

        [KingColumn(IsPrimaryKey=true)]
        public virtual int WorkItemId { get; set; }

        [KingColumn]
        public virtual int? WorkItemType { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string WwfBookmarkId { get; set; }
    }
}

