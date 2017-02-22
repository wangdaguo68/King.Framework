namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;

    [Serializable, KingTable(IsInherited=true)]
    public class SysApproveItem : SysPageControl
    {
        [KingColumn(MaxLength=200)]
        public virtual string DisplayText { get; set; }

        [KingColumn(MaxLength=400)]
        public virtual string UserSelectURL { get; set; }
    }
}

