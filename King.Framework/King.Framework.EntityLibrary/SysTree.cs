namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;

    [Serializable, KingTable(IsInherited=true)]
    public class SysTree : SysPageControl
    {
        public override void SetContext(CacheContext context)
        {
            base.SetContext(context);
            this.TreeRelation = context.FindById<SysOneMoreRelation>(this.TreeRelationId);
            this.OwnerSysView = context.FindById<SysView>(this.ViewId);
            this.FilterRelation = context.FindById<SysOneMoreRelation>(this.FilterRelationId);
        }

        [KingColumn(MaxLength=200)]
        public virtual string ClickBehavior { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string Delimiter { get; set; }

        [KingColumn]
        public virtual int? ExpandMethod { get; set; }

        [XmlIgnore]
        public virtual SysOneMoreRelation FilterRelation { get; set; }

        [KingColumn, KingRef(typeof(SysOneMoreRelation))]
        public virtual long? FilterRelationId { get; set; }

        [KingColumn]
        public virtual int? LoadMethod { get; set; }

        [XmlIgnore]
        public virtual SysView OwnerSysView { get; set; }

        [KingColumn]
        public virtual bool? ShowCheckBox { get; set; }

        [XmlIgnore]
        public virtual SysOneMoreRelation TreeRelation { get; set; }

        [KingColumn, KingRef(typeof(SysOneMoreRelation))]
        public virtual long? TreeRelationId { get; set; }

        [KingColumn]
        public virtual bool? TwoState { get; set; }

        [KingRef(typeof(SysView)), KingColumn]
        public virtual long? ViewId { get; set; }
    }
}

