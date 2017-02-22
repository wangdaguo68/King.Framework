namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;

    [Serializable, KingTable]
    public class SysOneMoreRelation : ICachedEntity, IKeyObject
    {
        [NonSerialized]
        private CacheContext _metaCache;

        void ICachedEntity.InitNavigationList()
        {
            this.PatchMatchFields = new List<SysPatchMatchField>();
        }

        void ICachedEntity.SetContext(CacheContext context)
        {
            this._metaCache = context;
            this.ChildEntity = context.FindById<SysEntity>(this.ChildEntityId);
            this.ParentEntity = context.FindById<SysEntity>(this.ParentEntityId);
            this.ChildField = context.FindById<SysField>(this.ChildFieldId);
            this.ParentField = context.FindById<SysField>(this.ParentFieldId);
            if (this.ChildEntity != null)
            {
                this.ChildEntity.ChildOneMoreRelations.Add(this);
            }
            if (this.ParentEntity != null)
            {
                this.ParentEntity.ParentOneMoreRelations.Add(this);
            }
        }

        public string GetKey()
        {
            return string.Format("{0}/{1}:{2}", this.ChildEntity.GetKey(), base.GetType().Name, this.ChildField.FieldName);
        }

        public override string ToString()
        {
            try
            {
                return string.Format("{0}.{1}", this.ChildEntity.EntityName, this.ChildField.FieldName);
            }
            catch (Exception)
            {
                return string.Format("{0}-{1}", this.ChildFieldName, this.ChildField);
            }
        }

        [KingColumn]
        public virtual int? CascadeDeleteFlag { get; set; }

        [XmlIgnore]
        public virtual SysEntity ChildEntity { get; set; }

        [KingColumn, KingRef(typeof(SysEntity))]
        public virtual long? ChildEntityId { get; set; }

        [XmlIgnore]
        public virtual SysField ChildField { get; set; }

        [KingColumn, KingRef(typeof(SysField))]
        public virtual long? ChildFieldId { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string ChildFieldName { get; set; }

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

        [KingColumn(MaxLength=200)]
        public virtual string IndexName { get; set; }

        [KingColumn]
        public virtual bool? Is_Tree { get; set; }

        [KingColumn]
        public virtual bool? IsCreateIndex { get; set; }

        [KingColumn]
        public virtual bool? IsParentChild { get; set; }

        [XmlIgnore]
        public virtual SysEntity ParentEntity { get; set; }

        [KingColumn, KingRef(typeof(SysEntity))]
        public virtual long? ParentEntityId { get; set; }

        [XmlIgnore]
        public virtual SysField ParentField { get; set; }

        [KingRef(typeof(SysField)), KingColumn]
        public virtual long? ParentFieldId { get; set; }

        [XmlIgnore]
        public virtual ICollection<SysPatchMatchField> PatchMatchFields { get; set; }

        [KingColumn(IsPrimaryKey=true, KeyOrder=1)]
        public virtual long RelationId { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string RelationName { get; set; }

        [KingColumn]
        public virtual int? RequiredLevel { get; set; }
    }
}

