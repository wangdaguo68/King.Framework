namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;

    [KingTable]
    public class SysReportHavingField
    {
        [KingColumn]
        public int? CompareType { get; set; }

        [KingColumn]
        public decimal? CompareValue { get; set; }

        [KingColumn(IsPrimaryKey=true)]
        public int HavingFieldId { get; set; }

        [KingColumn]
        public int? ReportId { get; set; }

        [KingColumn(MaxLength=200)]
        public string SumFieldAliases { get; set; }

        [KingColumn(MaxLength=200)]
        public string SumFieldDisplayText { get; set; }
    }
}

