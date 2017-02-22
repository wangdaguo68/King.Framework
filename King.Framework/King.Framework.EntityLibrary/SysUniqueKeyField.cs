namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;

    [Serializable, KingTable]
    public class SysUniqueKeyField : ICachedEntity, IKeyObject
    {
        [NonSerialized]
        private CacheContext _metaCache;

        void ICachedEntity.InitNavigationList()
        {
        }

        void ICachedEntity.SetContext(CacheContext context)
        {
            this._metaCache = context;
            this.SysField = context.FindById<King.Framework.EntityLibrary.SysField>(new long?(this.FieldId));
            this.OwnerUniqueKey = context.FindById<SysUniqueKey>(new long?(this.UniqueKeyId));
            if (this.OwnerUniqueKey != null)
            {
                this.OwnerUniqueKey.UniqueKeyFields.Add(this);
            }
        }

        public string GetKey()
        {
            return string.Format("{0}/{1}:{2}", this.OwnerUniqueKey.GetKey(), base.GetType().Name, this.SysField.GetKey());
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
                return (long) this.GetHashCode();
            }
        }

        [KingRef(typeof(King.Framework.EntityLibrary.SysField)), KingColumn(IsPrimaryKey=true, KeyOrder=2)]
        public virtual long FieldId { get; set; }

        [KingColumn]
        public virtual int? OrderIndex { get; set; }

        [XmlIgnore]
        public virtual SysUniqueKey OwnerUniqueKey { get; set; }

        [XmlIgnore]
        public virtual King.Framework.EntityLibrary.SysField SysField { get; set; }

        [KingColumn(IsPrimaryKey=true, KeyOrder=1), KingRef(typeof(SysUniqueKey))]
        public virtual long UniqueKeyId { get; set; }
    }
}

