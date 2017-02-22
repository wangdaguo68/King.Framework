namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;

    [KingTable]
    public class SysReportFixedCondition
    {
        [KingColumn]
        public int? CompareType { get; set; }

        [KingColumn(IsPrimaryKey=true)]
        public int ConditionId { get; set; }

        [KingColumn(MaxLength=200)]
        public string ConditionValue { get; set; }

        [KingColumn(MaxLength=200)]
        public string DisPlayText { get; set; }

        [KingColumn]
        public long? FieldId { get; set; }

        [KingColumn(MaxLength=200)]
        public string FieldName { get; set; }

        [KingColumn]
        public int? FilterType { get; set; }

        [KingColumn]
        public int? ParentId { get; set; }

        [KingColumn]
        public long? RelationId { get; set; }

        [KingColumn]
        public int? ReportId { get; set; }
    }
}

