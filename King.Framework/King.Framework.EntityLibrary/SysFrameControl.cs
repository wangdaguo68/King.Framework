namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;

    [Serializable, KingTable(IsInherited=true)]
    public class SysFrameControl : SysPageControl
    {
        [KingColumn(MaxLength=200)]
        public virtual string ContentUrl { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string DisplayText { get; set; }
    }
}

