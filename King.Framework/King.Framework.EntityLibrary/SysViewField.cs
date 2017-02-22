namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;

    [Serializable, KingTable]
    public class SysViewField : ICachedEntity, IKeyObject
    {
        [NonSerialized]
        private CacheContext _metaCache;

        void ICachedEntity.InitNavigationList()
        {
        }

        void ICachedEntity.SetContext(CacheContext context)
        {
            this._metaCache = context;
            this.Field = context.FindById<SysField>(this.FieldId);
            this.RefRelation = context.FindById<SysOneMoreRelation>(this.RelationId);
            this.OwnerView = context.FindById<SysView>(this.ViewId);
            this.ConditionView = context.FindById<SysView>(this.ConditionViewId);
            this.OwnerEntityJoinRelation = context.FindById<SysEntityJoinRelation>(this.EntityRelationId);
            if (this.OwnerView != null)
            {
                this.OwnerView.ViewFields.Add(this);
            }
        }

        public string GetKey()
        {
            return string.Format("{0}/{1}:{2}", this.OwnerView.GetKey(), base.GetType().Name, this.Field.FieldName);
        }

        public override string ToString()
        {
            try
            {
                return this.Field.ToString();
            }
            catch (Exception)
            {
                return string.Format("fieldid={0},viewid={1}", this.FieldId, this.ViewId);
            }
        }

        [XmlIgnore]
        public virtual SysView ConditionView { get; set; }

        [KingColumn]
        public virtual long? ConditionViewId { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string CustomDisplayText { get; set; }

        [KingColumn]
        public virtual int? DisplayWidth { get; set; }

        CacheContext ICachedEntity.MetaCache
        {
            get
            {
                return this._metaCache;
            }
        }

        long ICachedEntity.PrimaryKeyValue
        {
            get
            {
                return this.ViewFieldId;
            }
        }

        [KingColumn]
        public virtual bool? Enabled { get; set; }

        [KingColumn, KingRef(typeof(SysEntityJoinRelation))]
        public long? EntityRelationId { get; set; }

        [KingColumn(MaxLength=100)]
        public virtual string FalseText { get; set; }

        [XmlIgnore]
        public virtual SysField Field { get; set; }

        [KingColumn(MaxLength=200)]
        public string FieldAlias { get; set; }

        [KingColumn, KingRef(typeof(SysField))]
        public virtual long? FieldId { get; set; }

        [KingColumn(MaxLength=100)]
        public virtual string FormatString { get; set; }

        [KingColumn]
        public virtual int? FormatType { get; set; }

        [KingColumn]
        public virtual int? HorizontalAlign { get; set; }

        [KingColumn]
        public virtual bool? IsCustomDisplayText { get; set; }

        [KingColumn]
        public virtual bool? IsPinYinOrder { get; set; }

        [KingColumn]
        public virtual bool? IsUnDisplay { get; set; }

        [KingColumn]
        public virtual bool? IsWrap { get; set; }

        [KingColumn]
        public virtual int? MaxDisplayLength { get; set; }

        [KingColumn]
        public int? MobileIndex { get; set; }

        [KingColumn]
        public int? OrderIndex { get; set; }

        [XmlIgnore]
        public virtual SysEntityJoinRelation OwnerEntityJoinRelation { get; set; }

        [XmlIgnore]
        public virtual SysView OwnerView { get; set; }

        [XmlIgnore]
        public virtual SysOneMoreRelation RefRelation { get; set; }

        [KingRef(typeof(SysOneMoreRelation)), KingColumn]
        public virtual long? RelationId { get; set; }

        [KingColumn]
        public virtual bool? ShowAsImage { get; set; }

        [KingColumn(MaxLength=100)]
        public virtual string TrueText { get; set; }

        [KingColumn(IsPrimaryKey=true, KeyOrder=1)]
        public virtual long ViewFieldId { get; set; }

        [KingColumn, KingRef(typeof(SysView))]
        public virtual long? ViewId { get; set; }
    }
}

