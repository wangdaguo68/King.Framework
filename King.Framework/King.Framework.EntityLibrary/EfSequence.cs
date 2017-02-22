namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;

    [Serializable, KingTable("T_EfSequence")]
    public class EfSequence
    {
        [KingColumn(IsPrimaryKey=true, KeyOrder=1, IsIdentity=true)]
        public long Id { get; set; }

        [KingColumn]
        public long Temp { get; set; }
    }
}

