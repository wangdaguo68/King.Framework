namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;

    [KingTable]
    public class SysFormInstance
    {
        [KingColumn]
        public virtual DateTime? CreateTime { get; set; }

        [KingColumn]
        public virtual int? CreateUserId { get; set; }

        [XmlIgnore]
        public virtual SysForm Form { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string FormCode { get; set; }

        [KingColumn]
        public virtual DateTime? FormDate { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string FormDescription { get; set; }

        [KingColumn]
        public virtual long? FormId { get; set; }

        [KingColumn(IsPrimaryKey=true)]
        public virtual int FormInstanceId { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string FormTitle { get; set; }

        [KingColumn]
        public virtual int? ObjectId { get; set; }

        [KingColumn]
        public virtual int? OwnerId { get; set; }

        [KingColumn]
        public virtual int? State { get; set; }

        [KingColumn]
        public virtual DateTime? UpdateTime { get; set; }

        [KingColumn]
        public virtual int? UpdateUserId { get; set; }
    }
}

