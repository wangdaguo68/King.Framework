namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;

    [KingTable("SysSubSystemEntity")]
    public class SysSubSystemEntity : ICachedEntity
    {
        [NonSerialized]
        private CacheContext _metaCache;

        void ICachedEntity.InitNavigationList()
        {
        }

        void ICachedEntity.SetContext(CacheContext context)
        {
            this._metaCache = context;
            this.Entity = context.FindById<SysEntity>(this.EntityId);
            this.SubSystem = context.FindById<SysSubSystem>(this.SubSystemId);
            if ((this.Entity != null) && (this.SubSystem != null))
            {
                this.Entity.SubSystems.Add(this.SubSystem);
                this.SubSystem.Entitys.Add(this.Entity);
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
                return this.Id;
            }
        }

        [XmlIgnore]
        public virtual SysEntity Entity { get; set; }

        [KingRef(typeof(SysPage)), KingColumn]
        public virtual long? EntityId { get; set; }

        [KingColumn(IsPrimaryKey=true)]
        public virtual long Id { get; set; }

        [XmlIgnore]
        public virtual SysSubSystem SubSystem { get; set; }

        [KingRef(typeof(SysSubSystem)), KingColumn]
        public virtual long? SubSystemId { get; set; }
    }
}

