namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;

    [KingTable]
    public class SysExpression
    {
        [XmlIgnore]
        public virtual SysActivity Activity { get; set; }

        [KingColumn]
        public virtual long? ActivityId { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string ConstValue { get; set; }

        [KingColumn]
        public virtual int? DataType { get; set; }

        [KingColumn(MaxLength=-1)]
        public virtual string DisplayText { get; set; }

        [KingColumn(IsPrimaryKey=true)]
        public virtual long ExpressionId { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string ExpressionName { get; set; }

        [KingColumn]
        public virtual int? ExpressionType { get; set; }

        [XmlIgnore]
        public virtual SysField Field { get; set; }

        [KingColumn]
        public virtual long? FieldId { get; set; }

        [KingColumn]
        public virtual long? LeftId { get; set; }

        [KingColumn]
        public virtual int? OperationType { get; set; }

        [XmlIgnore]
        public virtual SysProcess Process { get; set; }

        [KingColumn]
        public virtual long? ProcessId { get; set; }

        [XmlIgnore]
        public virtual SysOneMoreRelation Relation { get; set; }

        [KingColumn]
        public virtual long? RelationId { get; set; }

        [KingColumn]
        public virtual long? RightId { get; set; }

        [XmlIgnore]
        public virtual SysTransition Transition { get; set; }

        [KingColumn]
        public virtual long? TransitionId { get; set; }
    }
}

