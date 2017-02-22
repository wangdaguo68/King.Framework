namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;

    [KingTable]
    public class SysReportQueryField
    {
        [KingColumn(MaxLength=200)]
        public string Aliases { get; set; }

        [KingColumn(MaxLength=200)]
        public string DefaultDisplayText { get; set; }

        [KingColumn(MaxLength=200)]
        public string DisplayText { get; set; }

        [KingColumn]
        public long? Field1Id { get; set; }

        [KingColumn(MaxLength=200)]
        public string Field1Name { get; set; }

        [KingColumn]
        public long? Field2Id { get; set; }

        [KingColumn(MaxLength=200)]
        public string Field2Name { get; set; }

        [KingColumn]
        public int? FunctionType { get; set; }

        [KingColumn]
        public bool? IsParent { get; set; }

        [KingColumn(IsPrimaryKey=true)]
        public int QueryFieldId { get; set; }

        [KingColumn(MaxLength=200)]
        public string Relation1Id { get; set; }

        [KingColumn(MaxLength=200)]
        public string Relation2Id { get; set; }

        [KingColumn]
        public int? ReportId { get; set; }
    }
}

