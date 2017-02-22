namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;

    [Serializable, KingTable(IsInherited=true)]
    public class SysViewListItem : SysPageControl
    {
        public override void SetContext(CacheContext context)
        {
            base.SetContext(context);
            this.OwnerViewControl = context.FindById<SysViewControl>(this.ViewControlId);
            if (this.OwnerViewControl != null)
            {
                this.OwnerViewControl.ViewListItems.Add(this);
            }
        }

        [XmlIgnore]
        public virtual SysViewControl OwnerViewControl { get; set; }

        [KingColumn, KingRef(typeof(SysViewControl))]
        public virtual long? ViewControlId { get; set; }
    }
}

