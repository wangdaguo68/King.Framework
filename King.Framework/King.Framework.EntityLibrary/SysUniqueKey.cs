using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using King.Framework.DAL;

namespace King.Framework.EntityLibrary
{
    public class SysUniqueKey : ICachedEntity, IKeyObject
    {
        [NonSerialized] private CacheContext _metaCache;

        // Methods
        public SysUniqueKey() {}
        void ICachedEntity.InitNavigationList() { }
        void ICachedEntity.SetContext(CacheContext context) { }
        public string GetKey()
        {
            return "";
        }
        //public override string ToString();

        // Properties
        [KingColumn(MaxLength = 200)]
        public virtual string Description { get; set; }

        [KingColumn(MaxLength = 200)]
        public virtual string DisplayText { get; set; }

        CacheContext ICachedEntity.MetaCache { get; }
        long ICachedEntity.PrimaryKeyValue { get; }

        [KingRef(typeof (SysEntity)), KingColumn]
        public virtual long? EntityId { get; set; }

        [XmlIgnore]
        public virtual SysEntity SysEntity { get; set; }

        [XmlIgnore]
        public virtual ICollection<SysUniqueKeyField> UniqueKeyFields { get; set; }

        [KingColumn(IsPrimaryKey = true)]
        public virtual long UniqueKeyId { get; set; }

        [KingColumn(MaxLength = 200)]
        public virtual string UniqueKeyName { get; set; }
    }

}
