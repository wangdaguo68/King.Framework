namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;

    [Serializable, KingTable(IsInherited=true)]
    public class SysConditionBox : SysPageControl
    {
        [KingColumn(MaxLength=40)]
        public virtual string btnQueryDisplayText { get; set; }

        [KingColumn]
        public virtual int? ColumnNum { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string DisplayText { get; set; }

        [KingColumn]
        public virtual bool? IsCanPageSize { get; set; }

        [KingColumn]
        public virtual bool? IsCanShrink { get; set; }

        [KingColumn]
        public virtual bool? IsQueryButtonInner { get; set; }
    }
}

