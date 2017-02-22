namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;

    [Serializable, KingTable(IsInherited=true)]
    public class SysParentViewControl : SysPageControl
    {
        public override void InitNavigationList()
        {
            base.InitNavigationList();
        }

        public override void SetContext(CacheContext context)
        {
            base.SetContext(context);
            this.OwnerMoreMoreRelation = context.FindById<SysMoreMoreRelation>(this.MoreRelationId);
            this.OwnerOneMoreRelation = context.FindById<SysOneMoreRelation>(this.RelationId);
            this.OwnerSysView = context.FindById<SysView>(this.ViewId);
            this.MultiRefField = context.FindById<SysField>(this.MultiRefFieldId);
            this.MultiRefEntity = context.FindById<SysEntity>(this.MultiRefEntityId);
        }

        [KingColumn(MaxLength=200)]
        public virtual string DisplayText { get; set; }

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

        [KingColumn, KingRef(typeof(SysOneMoreRelation))]
        public virtual long? RelationId { get; set; }

        [KingColumn]
        public virtual int? RelationType { get; set; }

        [KingColumn, KingRef(typeof(SysView))]
        public virtual long? ViewId { get; set; }
    }
}

