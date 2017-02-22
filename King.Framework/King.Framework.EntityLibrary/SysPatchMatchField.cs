namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;

    [Serializable, KingTable]
    public class SysPatchMatchField : ICachedEntity
    {
        [NonSerialized]
        private CacheContext _metaCache;

        void ICachedEntity.InitNavigationList()
        {
        }

        void ICachedEntity.SetContext(CacheContext context)
        {
            this._metaCache = context;
            this.ParentField = context.FindById<SysField>(this.ParentFieldId);
            this.ChildField = context.FindById<SysField>(this.ChildFieldId);
            this.OwnRelation = context.FindById<SysOneMoreRelation>(this.RelationId);
            if (this.OwnRelation != null)
            {
                this.OwnRelation.PatchMatchFields.Add(this);
            }
        }

        [XmlIgnore]
        public virtual SysField ChildField { get; set; }

        [KingRef(typeof(SysField)), KingColumn]
        public virtual long? ChildFieldId { get; set; }

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
                return this.MatchFieldId;
            }
        }

        [KingColumn(IsPrimaryKey=true)]
        public virtual long MatchFieldId { get; set; }

        [XmlIgnore]
        public virtual SysOneMoreRelation OwnRelation { get; set; }

        [XmlIgnore]
        public virtual SysField ParentField { get; set; }

        [KingRef(typeof(SysField)), KingColumn]
        public virtual long? ParentFieldId { get; set; }

        [KingRef(typeof(SysOneMoreRelation)), KingColumn]
        public virtual long? RelationId { get; set; }
    }
}

