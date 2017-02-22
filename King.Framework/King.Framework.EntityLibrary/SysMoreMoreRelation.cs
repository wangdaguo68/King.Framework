namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;

    [Serializable, KingTable]
    public class SysMoreMoreRelation : ICachedEntity, IKeyObject
    {
        [NonSerialized]
        private CacheContext _metaCache;

        void ICachedEntity.InitNavigationList()
        {
        }

        void ICachedEntity.SetContext(CacheContext context)
        {
            this._metaCache = context;
            this.ChildEntity = context.FindById<SysEntity>(this.ChildEntityId);
            this.ParentEntity = context.FindById<SysEntity>(this.ParentEntityId);
            if (this.ChildEntity != null)
            {
                this.ChildEntity.ChildMoreMoreRelations.Add(this);
            }
            if (this.ParentEntity != null)
            {
                this.ParentEntity.ParentMoreMoreRelations.Add(this);
            }
        }

        public string GetKey()
        {
            if (string.IsNullOrWhiteSpace(this.TableName))
            {
                throw new ApplicationException("TableName = null");
            }
            string str = this.ParentEntity.EntityName.ToLower();
            string strB = this.ChildEntity.EntityName.ToLower();
            if (str.CompareTo(strB) > 0)
            {
                return string.Format("{0}:{1}${2}${3}", new object[] { base.GetType().Name, str, strB, this.TableName.ToLower() });
            }
            return string.Format("{0}:{1}${2}${3}", new object[] { base.GetType().Name, strB, str, this.TableName.ToLower() });
        }

        public override string ToString()
        {
            if ((this.ParentEntity != null) && (this.ChildEntity != null))
            {
                string entityName = this.ParentEntity.EntityName;
                string str2 = this.ChildEntity.EntityName;
                return string.Format("{1}${2}${3}", entityName, str2, this.TableName);
            }
            return this.TableName;
        }

        [XmlIgnore]
        public virtual SysEntity ChildEntity { get; set; }

        [KingColumn, KingRef(typeof(SysEntity))]
        public virtual long? ChildEntityId { get; set; }

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

        [KingColumn]
        public virtual bool? IsCustomChildField { get; set; }

        [KingColumn]
        public virtual bool? IsCustomParentField { get; set; }

        [XmlIgnore]
        public virtual SysEntity ParentEntity { get; set; }

        [KingRef(typeof(SysEntity)), KingColumn]
        public virtual long? ParentEntityId { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string ParentFieldName { get; set; }

        [KingColumn(IsPrimaryKey=true)]
        public virtual long RelationId { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string RelationName { get; set; }

        [KingColumn]
        public virtual int? RequiredLevel { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string TableName { get; set; }
    }
}

