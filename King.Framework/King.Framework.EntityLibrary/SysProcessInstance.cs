namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;

    [KingTable]
    public class SysProcessInstance
    {
        [XmlIgnore]
        public virtual ICollection<SysActivityInstance> ActivityInstances { get; set; }

        [KingColumn]
        public virtual int? ApproveResult { get; set; }

        [KingColumn]
        public virtual DateTime? EndTime { get; set; }

        [XmlIgnore]
        public virtual SysFormInstance FormInstance { get; set; }

        [KingColumn]
        public virtual int? FormInstanceId { get; set; }

        [KingColumn]
        public virtual int InstanceStatus { get; set; }

        [KingColumn]
        public virtual int ObjectId { get; set; }

        [XmlIgnore]
        public virtual SysProcess Process { get; set; }

        [KingColumn]
        public virtual long? ProcessId { get; set; }

        [KingColumn(IsPrimaryKey=true)]
        public virtual int ProcessInstanceId { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string Remark { get; set; }

        [KingColumn]
        public virtual int? StartDeptId { get; set; }

        [KingColumn]
        public virtual DateTime? StartTime { get; set; }

        [KingColumn]
        public virtual int? StartUserId { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string WwfInstanceId { get; set; }
    }
}

