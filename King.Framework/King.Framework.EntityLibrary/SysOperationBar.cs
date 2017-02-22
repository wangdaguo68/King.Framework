namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;

    [Serializable, KingTable(IsInherited=true)]
    public class SysOperationBar : SysPageControl
    {
        [KingColumn]
        public virtual int? CssClass { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string Description { get; set; }
    }
}

