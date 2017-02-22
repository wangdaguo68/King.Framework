namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;

    [KingTable]
    public class SysReportMenuRole
    {
        [KingColumn(IsPrimaryKey=true)]
        public int MenuRoleId { get; set; }

        [KingColumn]
        public int? ReportId { get; set; }

        [KingColumn]
        public int? RoleId { get; set; }
    }
}

