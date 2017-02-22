namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;

    [Serializable, KingTable]
    public class SysEnum : INamedObject, ICachedEntity, IKeyObject
    {
        private Dictionary<int, string> _itemDict = null;
        [NonSerialized]
        private CacheContext _metaCache;

        void ICachedEntity.InitNavigationList()
        {
            this.EnumItems = new List<SysEnumItem>();
        }

        void ICachedEntity.SetContext(CacheContext context)
        {
            this._metaCache = context;
        }

        public string GetItemText(int item_value)
        {
            if (this._itemDict == null)
            {
                this._itemDict = new Dictionary<int, string>();
                List<SysEnumItem> list = this.EnumItems.ToList<SysEnumItem>();
                foreach (SysEnumItem item in list)
                {
                    this._itemDict.Add(item.ItemValue.Value, item.DisplayText);
                }
            }
            if (this._itemDict.ContainsKey(item_value))
            {
                return this._itemDict[item_value];
            }
            return "未知";
        }

        public string GetKey()
        {
            if (string.IsNullOrWhiteSpace(this.EnumName))
            {
                throw new ApplicationException("未设置EnumName");
            }
            return string.Format("{0}:{1}", base.GetType().Name, this.EnumName.ToLower());
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
                return this.EnumId;
            }
        }

        string INamedObject.DisplayText
        {
            get
            {
                return this.DisplayText;
            }
            set
            {
                this.DisplayText = value;
            }
        }

        long INamedObject.Id
        {
            get
            {
                return this.EnumId;
            }
            set
            {
                this.EnumId = value;
            }
        }

        string INamedObject.Name
        {
            get
            {
                return this.EnumName;
            }
            set
            {
                this.EnumName = value;
            }
        }

        [KingColumn(IsPrimaryKey=true, KeyOrder=1)]
        public virtual long EnumId { get; set; }

        [XmlIgnore]
        public virtual ICollection<SysEnumItem> EnumItems { get; set; }

        [KingColumn(MaxLength=100)]
        public virtual string EnumName { get; set; }
    }
}

