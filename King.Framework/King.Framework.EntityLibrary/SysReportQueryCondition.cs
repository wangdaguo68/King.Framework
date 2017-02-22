namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;

    [KingTable]
    public class SysReportQueryCondition
    {
        [KingColumn]
        public int? CompareType { get; set; }

        [KingColumn(IsPrimaryKey=true)]
        public int ConditionId { get; set; }

        public string ControlName
        {
            get
            {
                return string.Format("{0}{1}", this.FieldName, this.ConditionId);
            }
        }

        [KingColumn]
        public int? DataType { get; set; }

        [KingColumn(MaxLength=500)]
        public string DisplayText { get; set; }

        [KingColumn]
        public long? FieldId { get; set; }

        [KingColumn(MaxLength=200)]
        public string FieldName { get; set; }

        [KingColumn]
        public bool? IsFullRow { get; set; }

        [KingColumn]
        public bool? IsSubQuery { get; set; }

        [KingColumn]
        public int? OrderIndex { get; set; }

        [KingColumn]
        public long? RelationId { get; set; }

        [KingColumn]
        public int? ReportId { get; set; }
    }
}

