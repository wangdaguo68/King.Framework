namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;

    [Serializable, KingTable(IsInherited=true)]
    public class SysPageSection : SysPageControl
    {
        [KingColumn(MaxLength=200)]
        public virtual string Caption { get; set; }

        [KingColumn]
        public virtual int? ColumnNum { get; set; }

        [KingColumn]
        public virtual bool? IsShowCaption { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string SectionName { get; set; }
    }
}

