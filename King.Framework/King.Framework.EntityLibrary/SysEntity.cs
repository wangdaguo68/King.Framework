namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;

    [Serializable, KingTable]
    public class SysEntity : INamedObject, ICachedEntity, IKeyObject
    {
        private bool __OwnSetHasBuild = false;
        [NonSerialized]
        private CacheContext _metaCache;
        private readonly HashSet<string> _OwnSet = new HashSet<string>();
        private readonly HashSet<string> _RefSet = new HashSet<string>();

        internal void BuildOwnSet()
        {
            if (!this.__OwnSetHasBuild)
            {
                string uniqueId = this.GetUniqueId();
                this.OwnSet.Add(uniqueId);
                this._metaCache.idSet.Add(uniqueId);
                this._metaCache.idDict.Add(uniqueId, this);
                foreach (SysField field in this.Fields)
                {
                    field.BuildOwnSet();
                    this.OwnSet.UnionWith(field.OwnSet);
                }
                foreach (SysView view in this.Views)
                {
                }
                this.__OwnSetHasBuild = true;
            }
        }

        void ICachedEntity.InitNavigationList()
        {
            this.Fields = new List<SysField>();
            this.ParentMoreMoreRelations = new List<SysMoreMoreRelation>();
            this.ParentOneMoreRelations = new List<SysOneMoreRelation>();
            this.ChildMoreMoreRelations = new List<SysMoreMoreRelation>();
            this.ChildOneMoreRelations = new List<SysOneMoreRelation>();
            this.Operations = new List<SysOperation>();
            this.UniqueKeys = new List<SysUniqueKey>();
            this.Views = new List<SysView>();
            this.PageControls = new List<SysPageControl>();
            this.Conditions = new List<SysCondition>();
            this.BusinessLogicOperations = new List<SysBusinessLogicOperation>();
            this.Pages = new List<SysPage>();
            this.EntityJoinRelations = new List<SysEntityJoinRelation>();
        }

        void ICachedEntity.SetContext(CacheContext context)
        {
            this._metaCache = context;
            this.EntityCategory = context.FindById<SysEntityCategory>(this.EntityCategoryId);
            this.OwnerModule = context.FindById<SysModule>(this.ModuleId);
            this.EntitySchema = context.FindById<SysEntitySchema>(this.EntitySchemaId);
            if (this.EntityCategory != null)
            {
                this.EntityCategory.Entities.Add(this);
            }
            if (this.OwnerModule != null)
            {
                this.OwnerModule.Entities.Add(this);
            }
            if (this.EntitySchema != null)
            {
                this.EntitySchema.EntityList.Add(this);
            }
        }

        public string GetKey()
        {
            if (string.IsNullOrWhiteSpace(this.EntityName))
            {
                throw new ApplicationException("EntityName == null");
            }
            return string.Format("{0}:{1}", base.GetType().Name, this.EntityName.ToLower());
        }

        public CacheContext GetMetaCache()
        {
            return this._metaCache;
        }

        public string GetUniqueId()
        {
            return string.Format("{0}${1}", base.GetType().Name, this.EntityId);
        }

        public override string ToString()
        {
            return string.Format("{0}-{1}", this.EntityName, this.DisplayText);
        }

        [XmlIgnore]
        public virtual ICollection<SysBusinessLogicOperation> BusinessLogicOperations { get; set; }

        [XmlIgnore]
        public virtual ICollection<SysMoreMoreRelation> ChildMoreMoreRelations { get; set; }

        [XmlIgnore]
        public virtual ICollection<SysOneMoreRelation> ChildOneMoreRelations { get; set; }

        [XmlIgnore]
        public virtual ICollection<SysCondition> Conditions { get; set; }

        [KingColumn]
        public virtual DateTime? CreateTime { get; set; }

        [KingColumn(MaxLength=0x100)]
        public virtual string Description { get; set; }

        [KingColumn(MaxLength=100)]
        public virtual string DisplayFieldName { get; set; }

        [KingColumn(MaxLength=50)]
        public virtual string DisplayText { get; set; }

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
                return this.EntityId;
            }
        }

        string INamedObject.DisplayText
        {
            get
            {
                return this.DisplayText;
            }
            set
            {
                this.DisplayText = value;
            }
        }

        long INamedObject.Id
        {
            get
            {
                return this.EntityId;
            }
            set
            {
                this.EntityId = value;
            }
        }

        string INamedObject.Name
        {
            get
            {
                return this.EntityName;
            }
            set
            {
                this.EntityName = value;
            }
        }

        [XmlIgnore]
        public virtual SysEntityCategory EntityCategory { get; set; }

        [KingColumn]
        public virtual long? EntityCategoryId { get; set; }

        [KingColumn(IsPrimaryKey=true, KeyOrder=1)]
        public virtual long EntityId { get; set; }

        [XmlIgnore]
        public virtual ICollection<SysEntityJoinRelation> EntityJoinRelations { get; set; }

        [KingColumn(MaxLength=50)]
        public virtual string EntityName { get; set; }

        [XmlIgnore]
        public virtual SysEntitySchema EntitySchema { get; set; }

        [KingColumn]
        public virtual long? EntitySchemaId { get; set; }

        [XmlIgnore]
        public virtual ICollection<SysField> Fields { get; set; }

        [KingColumn]
        public virtual bool? IsCustomDisplayField { get; set; }

        [KingColumn]
        public virtual bool? IsCustomKeyField { get; set; }

        [KingColumn]
        public virtual bool? IsFormEntity { get; set; }

        [KingColumn]
        public virtual bool? IsFullText { get; set; }

        [KingColumn]
        public virtual bool? IsHistory { get; set; }

        [KingColumn]
        public virtual bool? IsLocal { get; set; }

        [KingColumn]
        public virtual bool? IsNotCreateTable { get; set; }

        [KingColumn]
        public virtual bool? IsStep { get; set; }

        [KingColumn(MaxLength=100)]
        public virtual string KeyFieldName { get; set; }

        [KingColumn, KingRef(typeof(SysModule))]
        public virtual long? ModuleId { get; set; }

        [XmlIgnore]
        public virtual ICollection<SysOperation> Operations { get; set; }

        [XmlIgnore]
        public virtual SysModule OwnerModule { get; set; }

        public HashSet<string> OwnSet
        {
            get
            {
                return this._OwnSet;
            }
        }

        [XmlIgnore]
        public virtual ICollection<SysPageControl> PageControls { get; set; }

        [XmlIgnore]
        public virtual ICollection<SysPage> Pages { get; set; }

        [XmlIgnore]
        public virtual ICollection<SysMoreMoreRelation> ParentMoreMoreRelations { get; set; }

        [XmlIgnore]
        public virtual ICollection<SysOneMoreRelation> ParentOneMoreRelations { get; set; }

        [KingColumn]
        public virtual int? PrivilegeMode { get; set; }

        public HashSet<string> RefSet
        {
            get
            {
                return this._RefSet;
            }
        }

        [KingColumn]
        public virtual int? RequiredLevel { get; set; }

        [XmlIgnore]
        public virtual ICollection<SysSubSystem> SubSystems { get; set; }

        [KingColumn]
        public virtual int? TableVersion { get; set; }

        [XmlIgnore]
        public virtual ICollection<SysUniqueKey> UniqueKeys { get; set; }

        [KingColumn]
        public virtual DateTime? UpdateTime { get; set; }

        [XmlIgnore]
        public virtual ICollection<SysView> Views { get; set; }
    }
}

