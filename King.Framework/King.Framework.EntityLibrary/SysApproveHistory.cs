namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;

    [Serializable, KingTable(IsInherited=true)]
    public class SysApproveHistory : SysPageControl
    {
        [KingColumn]
        public virtual int? DisplayMode { get; set; }

        [KingColumn(MaxLength=500)]
        public virtual string ProcessName { get; set; }
    }
}

