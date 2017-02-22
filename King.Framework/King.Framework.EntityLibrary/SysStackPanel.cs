namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;

    [Serializable, KingTable(IsInherited=true)]
    public class SysStackPanel : SysPageControl
    {
        [KingColumn]
        public virtual int? Orientation { get; set; }
    }
}

