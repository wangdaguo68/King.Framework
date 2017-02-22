namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Xml.Serialization;

    [Serializable, KingTable]
    public class SysSubSystem : ICachedEntity, IKeyObject
    {
        [NonSerialized]
        private CacheContext _metaCache;

        void ICachedEntity.InitNavigationList()
        {
            this.Pages = new List<SysPage>();
        }

        void ICachedEntity.SetContext(CacheContext context)
        {
            this._metaCache = context;
        }

        public string GetKey()
        {
            return string.Format("{0}:{1}", base.GetType().Name, this.SubSystemName.ToLower());
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(0xff);
            builder.AppendFormat("/SubSystemName={0}", this.SubSystemName).AppendLine();
            builder.AppendFormat("/RelativePath={0}", this.RelativePath).AppendLine();
            builder.AppendFormat("/WebCsprojName={0}", this.WebCsprojName).AppendLine();
            return builder.ToString();
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
                return this.SubSystemId;
            }
        }

        [XmlIgnore]
        public virtual ICollection<SysEntity> Entitys { get; set; }

        [XmlIgnore]
        public virtual ICollection<SysPage> Pages { get; set; }

        [KingColumn]
        public virtual int PageType { get; set; }

        [KingColumn(MaxLength=0x200)]
        public virtual string RelativePath { get; set; }

        [KingColumn(IsPrimaryKey=true)]
        public virtual long SubSystemId { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string SubSystemName { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string WebCsprojName { get; set; }
    }
}

