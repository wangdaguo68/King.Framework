namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;

    [KingTable]
    public class SysActivityOperation
    {
        [XmlIgnore]
        public SysActivity Activity { get; set; }

        [KingColumn]
        public virtual long? ActivityId { get; set; }

        [XmlIgnore]
        public ICollection<SysActivityStep> ActivitySteps { get; set; }

        [KingColumn]
        public virtual long? EntityId { get; set; }

        [KingColumn(MaxLength=0x200)]
        public virtual string JScriptText { get; set; }

        [KingColumn]
        public virtual long? ObjectId { get; set; }

        [KingColumn(IsPrimaryKey=true)]
        public virtual long OperationId { get; set; }

        [KingColumn(MaxLength=200)]
        public string OperationName { get; set; }

        [KingColumn]
        public virtual int? OperationType { get; set; }

        [KingColumn]
        public virtual long? ProcessId { get; set; }
    }
}

