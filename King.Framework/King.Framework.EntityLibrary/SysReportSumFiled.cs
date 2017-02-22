namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;

    [KingTable]
    public class SysReportSumFiled
    {
        [KingColumn(MaxLength=200)]
        public string Aliases { get; set; }

        [KingColumn(MaxLength=200)]
        public string DefaultDisplayText { get; set; }

        [KingColumn(MaxLength=200)]
        public string DisplayText { get; set; }

        [KingColumn(MaxLength=200)]
        public string QueryFieldAliases { get; set; }

        [KingColumn]
        public int? ReportId { get; set; }

        [KingColumn(IsPrimaryKey=true)]
        public int SumFiledId { get; set; }

        [KingColumn]
        public int? SumType { get; set; }
    }
}

