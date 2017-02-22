namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using King.Framework.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;

    [Serializable, KingTable]
    public class SysField : ICachedEntity, IKeyObject, IMetaField
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
                this.__OwnSetHasBuild = true;
            }
        }

        void ICachedEntity.InitNavigationList()
        {
            this.MultiRefs = new List<SysFieldMultiRef>();
        }

        void ICachedEntity.SetContext(CacheContext context)
        {
            this._metaCache = context;
            this.OwnerEntity = context.FindById<SysEntity>(this.EntityId);
            this.RefEntity = context.FindById<SysEntity>(this.RefEntityId);
            this.RefEnum = context.FindById<SysEnum>(this.EnumId);
            this.RefRelation = context.FindById<SysOneMoreRelation>(this.RefRelationId);
            if (this.OwnerEntity != null)
            {
                this.OwnerEntity.Fields.Add(this);
            }
        }

        public string GetKey()
        {
            if (this.OwnerEntity == null)
            {
                throw new ApplicationExceptionFormat("字段{0}未设置实体, entityid={1}", new object[] { this.ToString(), this.EntityId });
            }
            if (string.IsNullOrWhiteSpace(this.FieldName))
            {
                throw new ApplicationException("未设置字段名");
            }
            return string.Format("{0}/{1}:{2}", this.OwnerEntity.GetKey(), base.GetType().Name, this.FieldName.ToLower());
        }

        public string GetUniqueId()
        {
            return string.Format("{0}${1}", base.GetType().Name, this.FieldId);
        }

        public override string ToString()
        {
            if (this.OwnerEntity != null)
            {
                return string.Format("{0}.{1} ({2})", this.OwnerEntity.EntityName, this.FieldName, this.DisplayText);
            }
            return string.Format("{0}-{1}", this.FieldName, this.DisplayText);
        }

        [KingColumn]
        public virtual int? DataLength { get; set; }

        [KingColumn]
        public virtual int? DataScale { get; set; }

        [KingColumn]
        public virtual int? DataType { get; set; }

        [KingColumn(MaxLength=100)]
        public virtual string DefaultValue { get; set; }

        [KingColumn(MaxLength=0x200)]
        public virtual string Description { get; set; }

        [KingColumn(MaxLength=100)]
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
                return this.FieldId;
            }
        }

        string IMetaField.DisplayText
        {
            get
            {
                return this.DisplayText;
            }
        }

        long IMetaField.FieldId
        {
            get
            {
                return this.FieldId;
            }
        }

        string IMetaField.FieldName
        {
            get
            {
                return this.FieldName;
            }
        }

        [KingRef(typeof(SysEntity)), KingColumn]
        public virtual long? EntityId { get; set; }

        [KingColumn, KingRef(typeof(SysEnum))]
        public virtual long? EnumId { get; set; }

        [KingColumn(IsPrimaryKey=true, KeyOrder=1)]
        public virtual long FieldId { get; set; }

        [KingColumn(MaxLength=100)]
        public virtual string FieldName { get; set; }

        [KingColumn]
        public virtual bool? IsFormField { get; set; }

        [KingColumn]
        public virtual bool? IsFullText { get; set; }

        [KingColumn]
        public virtual bool? IsLocal { get; set; }

        [KingColumn]
        public virtual bool? IsNullable { get; set; }

        [KingColumn]
        public virtual bool? IsReference { get; set; }

        [XmlIgnore]
        public virtual ICollection<SysFieldMultiRef> MultiRefs { get; set; }

        [KingColumn]
        public virtual int? OrderIndex { get; set; }

        [XmlIgnore]
        public virtual SysEntity OwnerEntity { get; set; }

        public HashSet<string> OwnSet
        {
            get
            {
                return this._OwnSet;
            }
        }

        [XmlIgnore]
        public virtual SysEntity RefEntity { get; set; }

        [KingRef(typeof(SysEntity)), KingColumn]
        public virtual long? RefEntityId { get; set; }

        [XmlIgnore]
        public virtual SysEnum RefEnum { get; set; }

        [XmlIgnore]
        public virtual SysOneMoreRelation RefRelation { get; set; }

        [KingRef(typeof(SysOneMoreRelation)), KingColumn]
        public virtual long? RefRelationId { get; set; }

        public HashSet<string> RefSet
        {
            get
            {
                return this._RefSet;
            }
        }

        [KingColumn]
        public virtual int? RequiredLevel { get; set; }
    }
}

