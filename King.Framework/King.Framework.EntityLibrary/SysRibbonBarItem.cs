namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;

    [Serializable, KingTable(IsInherited=true)]
    public class SysRibbonBarItem : SysPageControl
    {
        public override void SetContext(CacheContext context)
        {
            base.SetContext(context);
            this.RedirectEntity = context.FindById<SysEntity>(this.RedirectEntityId);
        }

        [KingColumn(MaxLength=200)]
        public virtual string DisplayText { get; set; }

        [KingColumn(MaxLength=400)]
        public virtual string ImageURL { get; set; }

        [XmlIgnore]
        public virtual SysEntity RedirectEntity { get; set; }

        [KingRef(typeof(SysEntity)), KingColumn]
        public virtual long? RedirectEntityId { get; set; }

        [KingColumn(MaxLength=400)]
        public virtual string RedirectPageName { get; set; }
    }
}

