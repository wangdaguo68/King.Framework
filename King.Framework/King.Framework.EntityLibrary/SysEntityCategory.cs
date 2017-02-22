namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;

    [Serializable, KingTable]
    public class SysEntityCategory : INamedObject, ICachedEntity, IKeyObject
    {
        [NonSerialized]
        private CacheContext _metaCache;

        void ICachedEntity.InitNavigationList()
        {
            this.Entities = new List<SysEntity>();
            this.Modules = new List<SysModule>();
        }

        void ICachedEntity.SetContext(CacheContext context)
        {
            this._metaCache = context;
        }

        public string GetKey()
        {
            if (string.IsNullOrWhiteSpace(this.CategoryName))
            {
                throw new ApplicationException("CategoryName为空");
            }
            return string.Format("{0}:{1}", base.GetType().Name, this.CategoryName.ToLower());
        }

        public override string ToString()
        {
            return this.CategoryName;
        }

        [KingColumn(IsPrimaryKey=true)]
        public virtual long CategoryId { get; set; }

        [KingColumn(MaxLength=100)]
        public virtual string CategoryName { get; set; }

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
                return this.CategoryId;
            }
        }

        string INamedObject.DisplayText
        {
            get
            {
                return this.CategoryName;
            }
            set
            {
                this.CategoryName = value;
            }
        }

        long INamedObject.Id
        {
            get
            {
                return this.CategoryId;
            }
            set
            {
                this.CategoryId = value;
            }
        }

        string INamedObject.Name
        {
            get
            {
                return this.CategoryName;
            }
            set
            {
                this.CategoryName = value;
            }
        }

        [XmlIgnore]
        public ICollection<SysEntity> Entities { get; set; }

        [XmlIgnore]
        public ICollection<SysModule> Modules { get; set; }
    }
}

