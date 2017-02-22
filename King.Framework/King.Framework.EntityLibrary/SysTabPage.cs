namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;

    [Serializable, KingTable(IsInherited=true)]
    public class SysTabPage : SysPageControl
    {
        [KingColumn(MaxLength=200)]
        public virtual string DisplayText { get; set; }

        [KingColumn(MaxLength=400)]
        public virtual string ImageURL { get; set; }

        [KingColumn]
        public virtual bool? IsShowCaption { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string TabPageName { get; set; }
    }
}

