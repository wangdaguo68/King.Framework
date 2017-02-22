namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;

    [KingTable]
    public class SysProcessRemindTemplate : ICachedEntity
    {
        [NonSerialized]
        private CacheContext _metaCache;

        void ICachedEntity.InitNavigationList()
        {
        }

        void ICachedEntity.SetContext(CacheContext context)
        {
            this._metaCache = context;
            this.ProcessEntity = context.FindById<SysEntity>(this.ProcessEntityId);
            this.ActivityEntity = context.FindById<SysEntity>(this.ActivityEntityId);
        }

        [XmlIgnore]
        public SysEntity ActivityEntity { get; set; }

        [KingColumn]
        public virtual long? ActivityEntityId { get; set; }

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
                return this.TemplateId;
            }
        }

        [KingColumn(MaxLength=-1)]
        public virtual string OriginalContentText { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string OriginalTitleText { get; set; }

        [XmlIgnore]
        public SysEntity ProcessEntity { get; set; }

        [KingColumn]
        public virtual long? ProcessEntityId { get; set; }

        [KingColumn]
        public virtual int? ResultType { get; set; }

        [KingColumn]
        public virtual int? State { get; set; }

        [KingColumn(MaxLength=-1)]
        public virtual string TemplateContentText { get; set; }

        [KingColumn(IsPrimaryKey=true)]
        public virtual long TemplateId { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string TemplateName { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string TemplateTitleText { get; set; }

        [KingColumn]
        public virtual int? TemplateType { get; set; }

        [KingColumn]
        public virtual int? UseTimeType { get; set; }
    }
}

