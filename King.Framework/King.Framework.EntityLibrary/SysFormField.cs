namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;

    [KingTable]
    public class SysFormField
    {
        [KingColumn]
        public virtual DateTime? CreateTime { get; set; }

        [KingColumn]
        public virtual int? CreateUserId { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string CustomLabel { get; set; }

        [KingColumn]
        public virtual int? DataType { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string DefaultValue { get; set; }

        [KingColumn]
        public virtual int? DisplayOrder { get; set; }

        [KingColumn]
        public virtual int? DisplayType { get; set; }

        [XmlIgnore]
        public virtual SysEntity Entity { get; set; }

        [KingColumn]
        public virtual long? EntityId { get; set; }

        [XmlIgnore]
        public virtual SysField Field { get; set; }

        [KingColumn]
        public virtual long? FieldId { get; set; }

        [XmlIgnore]
        public virtual SysForm Form { get; set; }

        [KingColumn(IsPrimaryKey=true)]
        public virtual long FormFieldId { get; set; }

        [KingColumn]
        public virtual long? FormId { get; set; }

        [XmlIgnore]
        public virtual SysFormFieldSection FormSection { get; set; }

        [KingColumn]
        public virtual long? FormSectionId { get; set; }

        [KingColumn]
        public virtual bool? IsNullable { get; set; }

        [KingColumn]
        public virtual int? MaxLength { get; set; }

        [KingColumn]
        public virtual decimal? MaxValue { get; set; }

        [KingColumn]
        public virtual int? MinLength { get; set; }

        [KingColumn]
        public virtual decimal? MinValue { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string PlaceHolder { get; set; }

        [XmlIgnore]
        public virtual SysOneMoreRelation Relation { get; set; }

        [KingColumn]
        public virtual long? RelationId { get; set; }
    }
}

