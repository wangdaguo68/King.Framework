namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Xml.Serialization;

    [Serializable]
    public class SysModule : ICachedEntity, IKeyObject
    {
        [NonSerialized]
        private CacheContext _metaCache;

        void ICachedEntity.InitNavigationList()
        {
            this.Entities = new List<SysEntity>();
        }

        void ICachedEntity.SetContext(CacheContext context)
        {
            this._metaCache = context;
            this.EntityCategory = context.FindById<SysEntityCategory>(this.CategoryId);
            if (this.EntityCategory != null)
            {
                this.EntityCategory.Modules.Add(this);
            }
        }

        public string GetKey()
        {
            return string.Format("{0}/{1}:{2}", this.EntityCategory.GetKey(), base.GetType().Name, this.ModuleName.ToLower());
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(0x3f);
            if (this.EntityCategory != null)
            {
                builder.AppendFormat("Category:{0}", this.EntityCategory.CategoryName).AppendLine();
            }
            builder.AppendFormat("/Module: {0}", this.ModuleName).AppendLine();
            return builder.ToString();
        }

        [KingColumn, KingRef(typeof(SysEntityCategory))]
        public virtual long? CategoryId { get; set; }

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
                return this.ModuleId;
            }
        }

        [XmlIgnore]
        public ICollection<SysEntity> Entities { get; set; }

        [XmlIgnore]
        public virtual SysEntityCategory EntityCategory { get; set; }

        [KingColumn(IsPrimaryKey=true)]
        public virtual long ModuleId { get; set; }

        [KingColumn(MaxLength=100)]
        public virtual string ModuleName { get; set; }
    }
}

