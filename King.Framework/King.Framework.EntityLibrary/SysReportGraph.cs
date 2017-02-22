namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;

    [KingTable]
    public class SysReportGraph
    {
        [KingColumn]
        public int? ChartType { get; set; }

        [KingColumn(IsPrimaryKey=true)]
        public int ReportGraphId { get; set; }

        [KingColumn]
        public int? ReportId { get; set; }

        [KingColumn(MaxLength=200)]
        public string SeriesMember { get; set; }

        [KingColumn(MaxLength=200)]
        public string XMember { get; set; }

        [KingColumn(MaxLength=200)]
        public string YMember { get; set; }
    }
}

