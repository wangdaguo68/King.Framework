namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;

    [Serializable, KingTable(IsInherited=true)]
    public class SysPageField : SysPageControl
    {
        public override void InitNavigationList()
        {
            base.InitNavigationList();
            this.RelatedFields = new List<SysRefControlRelatedField>();
        }

        public override void SetContext(CacheContext context)
        {
            base.SetContext(context);
            this.Field = context.FindById<SysField>(this.FieldId);
            this.RefRelation = context.FindById<SysOneMoreRelation>(this.RelationId);
            this.PopPage = context.FindById<SysPage>(this.PopPageId);
            this.RelationPageField = context.FindById<SysPageField>(this.RelationPageFieldID);
            this.FilterRelation = context.FindById<SysOneMoreRelation>(this.FilterRelationId);
            this.ConditionView = context.FindById<SysView>(this.ConditionViewId);
            this.MultiRefEntity = context.FindById<SysEntity>(this.MultiRefEntityId);
            this.OwnerEntityJoinRelation = context.FindById<SysEntityJoinRelation>(this.EntityRelationId);
            this.ItemForAddPage = context.FindById<SysPage>(this.ItemForAddPageId);
        }

        [KingColumn]
        public bool? AutoShowImage { get; set; }

        [KingColumn]
        public virtual int? ColumnNum { get; set; }

        [XmlIgnore]
        public virtual SysView ConditionView { get; set; }

        [KingRef(typeof(SysView)), KingColumn]
        public virtual long? ConditionViewId { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string CustomLabel { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string DefaultValue { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string Delimiter { get; set; }

        [KingColumn]
        public virtual int? DisplayAlign { get; set; }

        [KingColumn]
        public virtual int? DisplayType { get; set; }

        [KingColumn, KingRef(typeof(SysEntityJoinRelation))]
        public long? EntityRelationId { get; set; }

        [KingColumn(MaxLength=100)]
        public virtual string FalseText { get; set; }

        [XmlIgnore]
        public virtual SysField Field { get; set; }

        [KingColumn(MaxLength=200)]
        public string FieldAlias { get; set; }

        [KingRef(typeof(SysField)), KingColumn]
        public virtual long? FieldId { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string FileFilter { get; set; }

        [XmlIgnore]
        public virtual SysOneMoreRelation FilterRelation { get; set; }

        [KingColumn, KingRef(typeof(SysOneMoreRelation))]
        public virtual long? FilterRelationId { get; set; }

        [KingColumn(MaxLength=100)]
        public virtual string FormatString { get; set; }

        [KingColumn]
        public virtual int? FormatType { get; set; }

        [KingColumn(MaxLength=500)]
        public virtual string HtmlAfter { get; set; }

        [KingColumn(MaxLength=500)]
        public virtual string HtmlBefore { get; set; }

        [KingColumn]
        public virtual int? ImageHeight { get; set; }

        [KingColumn]
        public virtual int? ImageWidth { get; set; }

        [KingColumn]
        public virtual bool? IsCustomLabel { get; set; }

        [KingColumn]
        public virtual bool? IsNotUpdate { get; set; }

        [KingColumn]
        public virtual bool? IsNotValidate { get; set; }

        [KingColumn]
        public virtual bool? IsNullable { get; set; }

        [KingColumn]
        public virtual bool? IsReadOnly { get; set; }

        [KingColumn]
        public virtual bool? IsWholeRow { get; set; }

        [KingColumn(MaxLength=0x200)]
        public string ItemForAddName { get; set; }

        [XmlIgnore]
        public virtual SysPage ItemForAddPage { get; set; }

        [KingRef(typeof(SysPage)), KingColumn]
        public long? ItemForAddPageId { get; set; }

        [KingColumn(MaxLength=40)]
        public virtual string MaxFileSize { get; set; }

        [KingColumn]
        public virtual int? MaxLength { get; set; }

        [KingColumn]
        public virtual decimal? MaxValue { get; set; }

        [KingColumn]
        public virtual int? MinLength { get; set; }

        [KingColumn]
        public virtual decimal? MinValue { get; set; }

        [XmlIgnore]
        public virtual SysEntity MultiRefEntity { get; set; }

        [KingColumn, KingRef(typeof(SysEntity))]
        public virtual long? MultiRefEntityId { get; set; }

        [XmlIgnore]
        public virtual SysEntityJoinRelation OwnerEntityJoinRelation { get; set; }

        [KingColumn(MaxLength=500)]
        public virtual string PlaceHolder { get; set; }

        [XmlIgnore]
        public virtual SysPage PopPage { get; set; }

        [KingRef(typeof(SysPage)), KingColumn]
        public virtual long? PopPageId { get; set; }

        [KingColumn]
        public virtual int? RefDisplayType { get; set; }

        [XmlIgnore]
        public virtual SysOneMoreRelation RefRelation { get; set; }

        [XmlIgnore]
        public virtual ICollection<SysRefControlRelatedField> RelatedFields { get; set; }

        [KingRef(typeof(SysOneMoreRelation)), KingColumn]
        public virtual long? RelationId { get; set; }

        [XmlIgnore]
        public virtual SysPageField RelationPageField { get; set; }

        [KingRef(typeof(SysPageField)), KingColumn]
        public virtual long? RelationPageFieldID { get; set; }

        [KingColumn]
        public virtual int? SelectHeight { get; set; }

        [KingColumn]
        public virtual int? SelectWidth { get; set; }

        [KingColumn]
        public virtual bool? ShowAsImage { get; set; }

        [KingColumn]
        public bool? ShowItemForAdd { get; set; }

        [KingColumn(MaxLength=100)]
        public virtual string TrueText { get; set; }

        [KingColumn(MaxLength=100)]
        public virtual string ValidateMessage { get; set; }

        [KingColumn(MaxLength=0x200)]
        public virtual string ValidateRex { get; set; }
    }
}

