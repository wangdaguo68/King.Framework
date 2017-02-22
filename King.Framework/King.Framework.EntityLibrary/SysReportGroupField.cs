namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;

    [KingTable]
    public class SysReportGroupField
    {
        [KingColumn(IsPrimaryKey=true)]
        public int GroupFieldId { get; set; }

        [KingColumn(MaxLength=200)]
        public string QueryFieldAliases { get; set; }

        [KingColumn(MaxLength=200)]
        public string QueryFieldDisplayText { get; set; }

        [KingColumn]
        public int? ReportId { get; set; }
    }
}

