namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;

    [Serializable, KingTable]
    public class SysViewSumFields : ICachedEntity
    {
        [NonSerialized]
        private CacheContext _metaCache;

        void ICachedEntity.InitNavigationList()
        {
        }

        void ICachedEntity.SetContext(CacheContext context)
        {
            this._metaCache = context;
            this.ViewField = context.FindById<SysViewField>(this.FieldId);
            this.OwnerSysViewControl = context.FindById<SysViewControl>(this.ViewControlId);
            if (this.OwnerSysViewControl != null)
            {
                this.OwnerSysViewControl.ViewSumFields.Add(this);
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
                return this.SumFieldsId;
            }
        }

        [KingColumn, KingRef(typeof(SysField))]
        public virtual long? FieldId { get; set; }

        [XmlIgnore]
        public virtual SysViewControl OwnerSysViewControl { get; set; }

        [KingColumn(IsPrimaryKey=true, KeyOrder=1)]
        public virtual long SumFieldsId { get; set; }

        [KingColumn]
        public virtual int? SumType { get; set; }

        [KingRef(typeof(SysViewControl)), KingColumn]
        public virtual long? ViewControlId { get; set; }

        [XmlIgnore]
        public virtual SysViewField ViewField { get; set; }
    }
}

