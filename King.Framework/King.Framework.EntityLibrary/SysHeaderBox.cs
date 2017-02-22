namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;

    [Serializable, KingTable(IsInherited=true)]
    public class SysHeaderBox : SysPageControl
    {
        [KingColumn]
        public virtual int? DisplayMode { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string DisplayText { get; set; }

        [KingColumn]
        public virtual bool? ShowFormInfo { get; set; }
    }
}

