namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;

    [Serializable, KingTable(IsInherited=true)]
    public class SysUserControl : SysPageControl
    {
        [KingColumn(MaxLength=200)]
        public virtual string ControlURL { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string DisplayText { get; set; }

        [KingColumn]
        public virtual int? UserControlType { get; set; }
    }
}

