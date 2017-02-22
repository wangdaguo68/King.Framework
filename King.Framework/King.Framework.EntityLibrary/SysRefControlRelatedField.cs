namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;

    [Serializable, KingTable]
    public class SysRefControlRelatedField : ICachedEntity
    {
        [NonSerialized]
        private CacheContext _metaCache;

        void ICachedEntity.InitNavigationList()
        {
        }

        void ICachedEntity.SetContext(CacheContext context)
        {
            this._metaCache = context;
            this.Field = context.FindById<SysField>(this.FieldId);
            SysPageField field = context.FindById<SysPageField>(this.PageFieldId);
            if (field != null)
            {
                field.RelatedFields.Add(this);
            }
            SysQueryCondition condition = context.FindById<SysQueryCondition>(this.PageFieldId);
            if (condition != null)
            {
                condition.RefRelatedFields.Add(this);
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
                return this.RefControlRelatedId;
            }
        }

        [XmlIgnore]
        public virtual SysField Field { get; set; }

        [KingRef(typeof(SysField)), KingColumn]
        public virtual long? FieldId { get; set; }

        [KingRef(typeof(SysPageField)), KingColumn]
        public virtual long? PageFieldId { get; set; }

        [KingColumn(IsPrimaryKey=true, KeyOrder=1)]
        public virtual long RefControlRelatedId { get; set; }
    }
}

