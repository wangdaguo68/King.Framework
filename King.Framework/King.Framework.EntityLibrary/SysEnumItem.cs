namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Xml.Serialization;

    [Serializable, KingTable]
    public class SysEnumItem : ICachedEntity, IKeyObject
    {
        [NonSerialized]
        private CacheContext _metaCache;

        void ICachedEntity.InitNavigationList()
        {
        }

        void ICachedEntity.SetContext(CacheContext context)
        {
            this._metaCache = context;
            this.OwnerEnum = context.FindById<SysEnum>(this.EnumId);
            if (this.OwnerEnum != null)
            {
                this.OwnerEnum.EnumItems.Add(this);
            }
        }

        public string GetKey()
        {
            if (string.IsNullOrWhiteSpace(this.ItemName))
            {
                throw new ApplicationException("未设置ItemName");
            }
            return string.Format("{0}/{1}:{2}", this.OwnerEnum.GetKey(), base.GetType().Name, this.ItemName);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(0xff);
            if (this.OwnerEnum != null)
            {
                builder.AppendFormat("/enum:{0}-{1}", this.OwnerEnum.DisplayText, this.OwnerEnum.EnumName).AppendLine();
            }
            builder.AppendFormat("/item: {0}-{1}-{2}", this.ItemName, this.ItemValue, this.DisplayText).AppendLine();
            return builder.ToString();
        }

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
                return this.ItemId;
            }
        }

        [KingColumn, KingRef(typeof(SysEnum))]
        public virtual long? EnumId { get; set; }

        [KingColumn(IsPrimaryKey=true, KeyOrder=1)]
        public virtual long ItemId { get; set; }

        [KingColumn(MaxLength=100)]
        public virtual string ItemName { get; set; }

        [KingColumn]
        public virtual int? ItemValue { get; set; }

        [XmlIgnore]
        public virtual SysEnum OwnerEnum { get; set; }
    }
}

