namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;

    [KingTable]
    public class SysActivityStep
    {
        [XmlIgnore]
        public virtual SysActivityOperation ActivityOperation { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string ConstValue { get; set; }

        [KingColumn]
        public virtual int? ExpressionType { get; set; }

        [KingColumn]
        public virtual long? OperationId { get; set; }

        [KingColumn]
        public virtual long? ProcessId { get; set; }

        [KingColumn]
        public virtual long? SourceFieldId { get; set; }

        [KingColumn]
        public virtual long? SourceObjectId { get; set; }

        [KingColumn]
        public virtual long? SourceRelationId { get; set; }

        [KingColumn(IsPrimaryKey=true)]
        public virtual long StepId { get; set; }

        [KingColumn(MaxLength=200)]
        public string StepName { get; set; }

        [KingColumn]
        public virtual long? TargetFieldId { get; set; }
    }
}

