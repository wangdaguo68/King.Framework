namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;

    [Serializable, KingTable]
    public class SysPageName
    {
        [KingColumn(MaxLength=200)]
        public string ApplicationName { get; set; }

        [KingColumn(IsPrimaryKey=true)]
        public int PageNameId { get; set; }

        [KingColumn(MaxLength=100)]
        public string PageTitle { get; set; }

        [KingColumn(MaxLength=0x200)]
        public string PhysicalDirectory { get; set; }

        [KingColumn(MaxLength=0x200)]
        public string PhysicalName { get; set; }

        [KingColumn(MaxLength=0x200)]
        public string PhysicalPath { get; set; }
    }
}

