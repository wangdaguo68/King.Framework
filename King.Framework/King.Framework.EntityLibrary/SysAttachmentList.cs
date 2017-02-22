namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;

    [Serializable, KingTable(IsInherited=true)]
    public class SysAttachmentList : SysPageControl
    {
        [KingColumn(MaxLength=0x200)]
        public virtual string DisplayText { get; set; }

        [KingColumn(MaxLength=0x200)]
        public virtual string FileFilter { get; set; }

        [KingColumn]
        public virtual bool? IsReadOnly { get; set; }

        [KingColumn(MaxLength=0x200)]
        public virtual string Keyword { get; set; }

        [KingColumn(MaxLength=0x200)]
        public virtual string MaxFileSize { get; set; }
    }
}

