namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;

    [Serializable, KingTable]
    public class SysViewOrderField : ICachedEntity, IKeyObject
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
            this.OwnerEntityJoinRelation = context.FindById<SysEntityJoinRelation>(this.EntityRelationId);
            if (this.OwnerView != null)
            {
                this.OwnerView.ViewOrderFields.Add(this);
            }
        }

        public string GetKey()
        {
            return string.Format("{0}/{1}:{2}-ordery:{3}", new object[] { this.OwnerView.GetKey(), base.GetType().Name, this.Field.FieldName, this.OrderMethod });
        }

        public override string ToString()
        {
            try
            {
                return string.Format("{0} {1}", this.Field.ToString(), ((OrderMethodEnum) this.OrderMethod.Value).ToString());
            }
            catch (Exception)
            {
                return string.Format("fieldid={0} - ordermethod={1} ", this.FieldId, this.OrderMethod);
            }
        }

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
                return this.OrderFieldId;
            }
        }

        [KingColumn, KingRef(typeof(SysEntityJoinRelation))]
        public long? EntityRelationId { get; set; }

        [XmlIgnore]
        public virtual SysField Field { get; set; }

        [KingColumn(MaxLength=200)]
        public string FieldAlias { get; set; }

        [KingColumn, KingRef(typeof(SysField))]
        public virtual long? FieldId { get; set; }

        [KingColumn(IsPrimaryKey=true, KeyOrder=1)]
        public virtual long OrderFieldId { get; set; }

        [KingColumn]
        public virtual int? OrderIndex { get; set; }

        [KingColumn]
        public virtual int? OrderMethod { get; set; }

        [XmlIgnore]
        public virtual SysEntityJoinRelation OwnerEntityJoinRelation { get; set; }

        [XmlIgnore]
        public virtual SysView OwnerView { get; set; }

        [XmlIgnore]
        public virtual SysOneMoreRelation RefRelation { get; set; }

        [KingColumn, KingRef(typeof(SysOneMoreRelation))]
        public virtual long? RelationId { get; set; }

        [KingRef(typeof(SysView)), KingColumn]
        public virtual long? ViewId { get; set; }
    }
}

