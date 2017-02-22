namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;

    [KingTable]
    public class SysPath
    {
        [KingColumn(MaxLength=-1)]
        public string Description { get; set; }

        [KingColumn(MaxLength=0x200)]
        public string Path { get; set; }

        [KingColumn(IsPrimaryKey=true)]
        public int PathId { get; set; }
    }
}

