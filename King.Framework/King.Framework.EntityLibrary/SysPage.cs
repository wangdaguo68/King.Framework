namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;

    [Serializable, KingTable(IsInherited=true)]
    public class SysPage : SysPageControl, INamedObject
    {
        public override string GetKey()
        {
            return string.Format("{0}/{1}:{2}", this.OwnerEntity.GetKey(), base.GetType().Name, this.PageName.ToLower());
        }

        public override void InitNavigationList()
        {
            base.InitNavigationList();
            this.PageParameters = new List<SysPageParameter>();
            this.SubSystems = new List<SysSubSystem>();
        }

        public override void SetContext(CacheContext context)
        {
            base.SetContext(context);
            if (this.OwnerEntity != null)
            {
                this.OwnerEntity.Pages.Add(this);
            }
            this.OwnerModule = context.FindById<SysModule>(this.ModuleId);
        }

        [KingColumn(MaxLength=200)]
        public virtual string DisplayText { get; set; }

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
                return this.ControlId;
            }
            set
            {
                this.ControlId = value;
            }
        }

        string INamedObject.Name
        {
            get
            {
                return this.ControlName;
            }
            set
            {
                this.ControlName = value;
            }
        }

        [KingColumn]
        public virtual bool? IsDefault { get; set; }

        [KingColumn]
        public virtual bool? IsNotRelease { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string MasterName { get; set; }

        [KingRef(typeof(SysModule)), KingColumn]
        public virtual long? ModuleId { get; set; }

        [XmlIgnore]
        public virtual SysModule OwnerModule { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string PageName { get; set; }

        [XmlIgnore]
        public virtual ICollection<SysPageParameter> PageParameters { get; set; }

        [KingColumn]
        public virtual int? PageType { get; set; }

        [XmlIgnore]
        public virtual ICollection<SysSubSystem> SubSystems { get; set; }
    }
}

