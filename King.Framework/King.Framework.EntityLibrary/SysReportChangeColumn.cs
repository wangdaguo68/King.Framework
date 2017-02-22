namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;

    [KingTable]
    public class SysReportChangeColumn
    {
        [KingColumn(IsPrimaryKey=true)]
        public int ChangeColumnId { get; set; }

        [KingColumn(MaxLength=200)]
        public string ColumnFieldAliases { get; set; }

        [KingColumn]
        public int? ReportId { get; set; }

        [KingColumn(MaxLength=200)]
        public string RowFieldAliases { get; set; }

        [KingColumn(MaxLength=200)]
        public string RowFieldDisPlayText { get; set; }

        [KingColumn(MaxLength=200)]
        public string ValueFieldAliases { get; set; }
    }
}

