namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;

    [Serializable, KingTable]
    public class SysIdSequence
    {
        [KingColumn]
        public int? CurrentId { get; set; }

        [KingColumn(IsPrimaryKey=true, MaxLength=100)]
        public string IdKey { get; set; }

        [KingColumn]
        public DateTime? UpdateTime { get; set; }
    }
}

