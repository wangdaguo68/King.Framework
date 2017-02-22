namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;

    [KingTable]
    public class SysReportDisplayField
    {
        [KingColumn(MaxLength=200)]
        public string Aliases { get; set; }

        [KingColumn(IsPrimaryKey=true)]
        public int DisplayFieldId { get; set; }

        [KingColumn(MaxLength=200)]
        public string DisplayText { get; set; }

        [KingColumn]
        public int? OrderIndex { get; set; }

        [KingColumn]
        public int? ReportId { get; set; }
    }
}

