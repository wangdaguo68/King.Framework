namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;

    [Serializable, KingTable(IsInherited=true)]
    public class SysActivityDataSection : SysPageControl
    {
        private readonly HashSet<string> _OwnSet = new HashSet<string>();
        private readonly HashSet<string> _RefSet = new HashSet<string>();

        public string GetUniqueId()
        {
            return string.Format("{0}${1}", base.GetType().Name, this.PrimaryKeyValue);
        }

        public override void SetContext(CacheContext context)
        {
            base.SetContext(context);
            this.Relation = context.FindById<SysOneMoreRelation>(this.RelationId);
        }

        [KingColumn]
        public virtual int? ColumnNum { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string DisplayText { get; set; }

        public HashSet<string> OwnSet
        {
            get
            {
                return this._OwnSet;
            }
        }

        public HashSet<string> RefSet
        {
            get
            {
                return this._RefSet;
            }
        }

        [XmlIgnore]
        public virtual SysOneMoreRelation Relation { get; set; }

        [KingRef(typeof(SysOneMoreRelation)), KingColumn]
        public virtual long? RelationId { get; set; }
    }
}

