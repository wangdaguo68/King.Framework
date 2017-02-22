namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;

    [KingTable]
    public class SysEntitySchema : ICachedEntity
    {
        [NonSerialized]
        private CacheContext _metaCache;

        void ICachedEntity.InitNavigationList()
        {
            this.EntityList = new List<SysEntity>();
        }

        void ICachedEntity.SetContext(CacheContext context)
        {
            this._metaCache = context;
            this.DbSchema = context.FindById<SysDbSchema>(this.DbSchemaId);
        }

        [XmlIgnore]
        public virtual SysDbSchema DbSchema { get; set; }

        [KingRef(typeof(SysDbSchema)), KingColumn]
        public virtual long? DbSchemaId { get; set; }

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
                return this.EntitySchemaId;
            }
        }

        [XmlIgnore]
        public virtual ICollection<SysEntity> EntityList { get; set; }

        [KingColumn(IsPrimaryKey=true)]
        public virtual long EntitySchemaId { get; set; }

        [KingColumn(MaxLength=100)]
        public virtual string EntitySchemaName { get; set; }
    }
}

