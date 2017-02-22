namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;

    [KingTable]
    public class SysReport
    {
        [KingColumn(MaxLength=200)]
        public string EntityDisPlayText { get; set; }

        [KingColumn(MaxLength=200)]
        public string EntityName { get; set; }

        [KingColumn]
        public int? HavingRelation { get; set; }

        [KingColumn]
        public bool? IsChangeColumn { get; set; }

        [KingColumn]
        public bool? IsGraph { get; set; }

        [KingColumn]
        public bool? IsGroup { get; set; }

        [KingColumn]
        public long? MenuId { get; set; }

        [KingColumn(IsPrimaryKey=true)]
        public int ReportId { get; set; }

        [KingColumn]
        public int? ReportModel { get; set; }

        [KingColumn(MaxLength=200)]
        public string ReportName { get; set; }

        [KingColumn(MaxLength=-1)]
        public string SQLScript { get; set; }

        [KingColumn]
        public int? State { get; set; }

        [KingColumn]
        public int? TopN { get; set; }
    }
}

