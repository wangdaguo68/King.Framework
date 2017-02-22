namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;

    [Serializable, KingTable(IsInherited=true)]
    public class SysQueryCondition : SysPageControl
    {
        public override void InitNavigationList()
        {
            base.InitNavigationList();
            this.RelatedFields = new List<SysQueryConditionRelatedField>();
            this.RefRelatedFields = new List<SysRefControlRelatedField>();
        }

        public override void SetContext(CacheContext context)
        {
            base.SetContext(context);
            this.OwnerOneMoreRelation = context.FindById<SysOneMoreRelation>(this.RelationId);
            this.OwnerField = context.FindById<SysField>(this.FieldId);
            this.PopPage = context.FindById<SysPage>(this.PopPageId);
            this.FilterRelation = context.FindById<SysOneMoreRelation>(this.FilterRelationId);
            this.RelationCondition = context.FindById<SysQueryCondition>(this.RelationConditionID);
            this.ConditionView = context.FindById<SysView>(this.ConditionViewId);
            this.MultiRefEntity = context.FindById<SysEntity>(this.MultiRefEntityId);
            this.OwnerEntityJoinRelation = context.FindById<SysEntityJoinRelation>(this.EntityRelationId);
        }

        [KingColumn(MaxLength=200)]
        public virtual int? ColumnNum { get; set; }

        [KingColumn]
        public virtual int? CompareType { get; set; }

        [XmlIgnore]
        public virtual SysView ConditionView { get; set; }

        [KingRef(typeof(SysView)), KingColumn]
        public virtual long? ConditionViewId { get; set; }

        [KingColumn(MaxLength=500)]
        public virtual string CustomCaption { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string DefaultValue { get; set; }

        [KingColumn(MaxLength=200)]
        public string Delimiter { get; set; }

        [KingColumn]
        public virtual int? DisplayAlign { get; set; }

        [KingColumn]
        public virtual int? DisplayType { get; set; }

        [KingColumn, KingRef(typeof(SysEntityJoinRelation))]
        public long? EntityRelationId { get; set; }

        [KingColumn(MaxLength=100)]
        public virtual string FalseText { get; set; }

        [KingColumn(MaxLength=200)]
        public string FieldAlias { get; set; }

        [KingColumn, KingRef(typeof(SysField))]
        public virtual long? FieldId { get; set; }

        [XmlIgnore]
        public virtual SysOneMoreRelation FilterRelation { get; set; }

        [KingColumn, KingRef(typeof(SysOneMoreRelation))]
        public virtual long? FilterRelationId { get; set; }

        [KingColumn(MaxLength=100)]
        public virtual string FormatString { get; set; }

        [KingColumn]
        public virtual int? FormatType { get; set; }

        [KingColumn]
        public virtual int? FullTextType { get; set; }

        [KingColumn]
        public virtual bool? IsCustomCaption { get; set; }

        [KingColumn]
        public virtual bool? IsNotCondition { get; set; }

        [KingColumn]
        public virtual bool? IsReadOnly { get; set; }

        [KingColumn]
        public virtual bool? IsRequired { get; set; }

        [KingColumn]
        public virtual bool? IsWholeRow { get; set; }

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

        [KingRef(typeof(SysEntity)), KingColumn]
        public virtual long? MultiRefEntityId { get; set; }

        [XmlIgnore]
        public virtual SysEntityJoinRelation OwnerEntityJoinRelation { get; set; }

        [XmlIgnore]
        public virtual SysField OwnerField { get; set; }

        [XmlIgnore]
        public virtual SysOneMoreRelation OwnerOneMoreRelation { get; set; }

        [XmlIgnore]
        public virtual SysPage PopPage { get; set; }

        [KingColumn, KingRef(typeof(SysPage))]
        public virtual long? PopPageId { get; set; }

        [KingColumn]
        public virtual int? RefDisplayType { get; set; }

        [XmlIgnore]
        public virtual ICollection<SysRefControlRelatedField> RefRelatedFields { get; set; }

        [XmlIgnore]
        public virtual ICollection<SysQueryConditionRelatedField> RelatedFields { get; set; }

        [XmlIgnore]
        public virtual SysQueryCondition RelationCondition { get; set; }

        [KingColumn, KingRef(typeof(SysQueryCondition))]
        public virtual long? RelationConditionID { get; set; }

        [KingRef(typeof(SysOneMoreRelation)), KingColumn]
        public virtual long? RelationId { get; set; }

        [KingColumn]
        public virtual int? SelectHeight { get; set; }

        [KingColumn]
        public virtual int? SelectWidth { get; set; }

        [KingColumn(MaxLength=100)]
        public virtual string TrueText { get; set; }
    }
}

