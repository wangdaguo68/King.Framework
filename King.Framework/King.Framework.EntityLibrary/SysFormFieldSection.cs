namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;

    [KingTable]
    public class SysFormFieldSection
    {
        [KingColumn]
        public virtual DateTime? CreateTime { get; set; }

        [KingColumn]
        public virtual int? CreateUserId { get; set; }

        [KingColumn]
        public virtual int? DisplayOrder { get; set; }

        [XmlIgnore]
        public virtual SysForm Form { get; set; }

        [KingColumn]
        public virtual long? FormId { get; set; }

        [KingColumn(IsPrimaryKey=true)]
        public virtual long FormSectionId { get; set; }
    }
}

