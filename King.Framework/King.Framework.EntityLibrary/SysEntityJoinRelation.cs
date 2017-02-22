namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;

    [Serializable, KingTable]
    public class SysEntityJoinRelation : ICachedEntity, IKeyObject
    {
        [NonSerialized]
        private CacheContext _metaCache;

        void ICachedEntity.InitNavigationList()
        {
        }

        void ICachedEntity.SetContext(CacheContext context)
        {
            this._metaCache = context;
            this.OwnerEntity = context.FindById<SysEntity>(this.EntityId);
            this.ParentEntityJoinRelation = context.FindById<SysEntityJoinRelation>(this.ParentId);
            this.LeftField = context.FindById<SysField>(this.LeftFieldId);
            this.RightField = context.FindById<SysField>(this.RightFieldId);
        }

        public string GetKey()
        {
            if (this.LeftFieldId.HasValue && this.RightFieldId.HasValue)
            {
                if (this.LeftField == null)
                {
                    throw new ApplicationException("LeftField不正确");
                }
                if (this.RightField == null)
                {
                    throw new ApplicationException("RightField不正确");
                }
                return string.Format("{0}/{1}:{2}={3}", new object[] { this.RootOwnerEntity.GetKey(), base.GetType().Name, this.LeftField.FieldName, this.RightField.FieldName });
            }
            return string.Format("{0}/{1}:{2}", this.RootOwnerEntity.GetKey(), base.GetType().Name, this.TableAlias);
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
                return this.RelationId;
            }
        }

        [KingRef(typeof(SysEntity)), KingColumn]
        public long? EntityId { get; set; }

        [XmlIgnore]
        public SysField LeftField { get; set; }

        [KingColumn, KingRef(typeof(SysField))]
        public long? LeftFieldId { get; set; }

        [XmlIgnore]
        public virtual SysEntity OwnerEntity { get; set; }

        [XmlIgnore]
        public virtual SysEntityJoinRelation ParentEntityJoinRelation { get; set; }

        [KingColumn, KingRef(typeof(SysEntityJoinRelation))]
        public long? ParentId { get; set; }

        [KingColumn, KingRef(typeof(SysOneMoreRelation))]
        public long? RefRelationId { get; set; }

        [KingColumn(IsPrimaryKey=true)]
        public long RelationId { get; set; }

        [KingColumn(MaxLength=200)]
        public string RelationName { get; set; }

        [XmlIgnore]
        public SysField RightField { get; set; }

        [KingRef(typeof(SysField)), KingColumn]
        public long? RightFieldId { get; set; }

        [XmlIgnore]
        public virtual SysEntity RootOwnerEntity { get; set; }

        [KingColumn(MaxLength=200)]
        public string TableAlias { get; set; }
    }
}

