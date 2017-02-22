namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;

    [KingTable]
    public class SysProcess
    {
        [XmlIgnore]
        public virtual ICollection<SysActivity> Activities { get; set; }

        [XmlIgnore]
        public virtual SysEntity ActivityEntity { get; set; }

        [KingColumn]
        public virtual long? ActivityEntityId { get; set; }

        [KingColumn(MaxLength=-1)]
        public virtual string CancelScript { get; set; }

        [KingColumn]
        public virtual long? EntityId { get; set; }

        [XmlIgnore]
        public virtual ICollection<SysExpression> Expressions { get; set; }

        [XmlIgnore]
        public virtual SysForm Form { get; set; }

        [KingColumn]
        public virtual long? FormId { get; set; }

        [KingColumn]
        public virtual int? ProcessCategory { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string ProcessDescription { get; set; }

        [XmlIgnore]
        public virtual SysEntity ProcessEntity { get; set; }

        [KingColumn]
        public virtual double? ProcessHeight { get; set; }

        [KingColumn(IsPrimaryKey=true)]
        public virtual long ProcessId { get; set; }

        [XmlIgnore]
        public virtual ICollection<SysProcessInstance> ProcessInstances { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string ProcessName { get; set; }

        [XmlIgnore]
        public virtual ICollection<SysProcessParticipant> ProcessParticipants { get; set; }

        [XmlIgnore]
        public virtual ICollection<SysProcessProxy> ProcessProxies { get; set; }

        [XmlIgnore]
        public virtual ICollection<SysProcessRemind> ProcessReminds { get; set; }

        [KingColumn]
        public virtual int? ProcessStatus { get; set; }

        [KingColumn]
        public virtual long? ProcessType { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string ProcessVersion { get; set; }

        [KingColumn]
        public virtual double? ProcessWidth { get; set; }

        [KingColumn]
        public virtual byte[] TemplateId { get; set; }

        [XmlIgnore]
        public virtual ICollection<SysTransition> Transitions { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string WwfId { get; set; }
    }
}

