namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;

    [Serializable, KingTable(IsInherited=true)]
    public class SysViewControl : SysPageControl
    {
        public override void InitNavigationList()
        {
            base.InitNavigationList();
            this.ViewSumFields = new List<SysViewSumFields>();
            this.ViewListItems = new List<SysViewListItem>();
        }

        public override void SetContext(CacheContext context)
        {
            base.SetContext(context);
            this.OwnerOneMoreRelation = context.FindById<SysOneMoreRelation>(this.RelationId);
            this.OwnerMoreMoreRelation = context.FindById<SysMoreMoreRelation>(this.MoreRelationId);
            this.GroupField = context.FindById<SysViewField>(this.GroupFieldId);
            this.OwnerSysView = context.FindById<SysView>(this.ViewId);
            this.ExportView = context.FindById<SysView>(this.ExportViewId);
            this.FilterOneMoreRelation = context.FindById<SysOneMoreRelation>(this.FilterRelationId);
            this.FilterMoreMoreRelation = context.FindById<SysMoreMoreRelation>(this.FilterMoreRelationId);
            this.FilterPageParameter = context.FindById<SysPageParameter>(this.FilterPageParameterId);
            this.MultiRefField = context.FindById<SysField>(this.MultiRefFieldId);
            this.MultiRefEntity = context.FindById<SysEntity>(this.MultiRefEntityId);
            this.FilterControl = context.FindById<SysPageControl>(this.FilterControlId);
        }

        [KingColumn]
        public virtual bool? DisableCheckBox { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string DisplayText { get; set; }

        [KingColumn]
        public virtual bool? Enabled { get; set; }

        [KingColumn]
        public virtual bool? EnableMutiSelect { get; set; }

        [XmlIgnore]
        public virtual SysView ExportView { get; set; }

        [KingColumn, KingRef(typeof(SysView))]
        public virtual long? ExportViewId { get; set; }

        [XmlIgnore]
        public virtual SysPageControl FilterControl { get; set; }

        [KingColumn, KingRef(typeof(SysPageControl))]
        public virtual long? FilterControlId { get; set; }

        [XmlIgnore]
        public virtual SysMoreMoreRelation FilterMoreMoreRelation { get; set; }

        [KingRef(typeof(SysMoreMoreRelation)), KingColumn]
        public virtual long? FilterMoreRelationId { get; set; }

        [XmlIgnore]
        public virtual SysOneMoreRelation FilterOneMoreRelation { get; set; }

        [XmlIgnore]
        public virtual SysPageParameter FilterPageParameter { get; set; }

        [KingRef(typeof(SysPageParameter)), KingColumn]
        public virtual long? FilterPageParameterId { get; set; }

        [KingColumn, KingRef(typeof(SysOneMoreRelation))]
        public virtual long? FilterRelationId { get; set; }

        [KingColumn]
        public virtual int? FilterRelationType { get; set; }

        [XmlIgnore]
        public virtual SysViewField GroupField { get; set; }

        [KingRef(typeof(SysField)), KingColumn]
        public virtual long? GroupFieldId { get; set; }

        [KingColumn]
        public virtual bool? IsHideTitle { get; set; }

        [KingColumn]
        public virtual bool? IsShowExport { get; set; }

        [KingColumn]
        public virtual bool? IsShowRefresh { get; set; }

        [KingColumn]
        public virtual int? MobileTemplate { get; set; }

        [KingRef(typeof(SysMoreMoreRelation)), KingColumn]
        public virtual long? MoreRelationId { get; set; }

        [XmlIgnore]
        public virtual SysEntity MultiRefEntity { get; set; }

        [KingColumn, KingRef(typeof(SysEntity))]
        public virtual long? MultiRefEntityId { get; set; }

        [XmlIgnore]
        public virtual SysField MultiRefField { get; set; }

        [KingColumn, KingRef(typeof(SysField))]
        public virtual long? MultiRefFieldId { get; set; }

        [XmlIgnore]
        public virtual SysMoreMoreRelation OwnerMoreMoreRelation { get; set; }

        [XmlIgnore]
        public virtual SysOneMoreRelation OwnerOneMoreRelation { get; set; }

        [XmlIgnore]
        public virtual SysView OwnerSysView { get; set; }

        [KingColumn]
        public virtual int? PageSize { get; set; }

        [KingRef(typeof(SysOneMoreRelation)), KingColumn]
        public virtual long? RelationId { get; set; }

        [KingColumn]
        public virtual int? RelationType { get; set; }

        [KingRef(typeof(SysView)), KingColumn]
        public virtual long? ViewId { get; set; }

        [XmlIgnore]
        public virtual ICollection<SysViewListItem> ViewListItems { get; set; }

        [XmlIgnore]
        public virtual ICollection<SysViewSumFields> ViewSumFields { get; set; }
    }
}

