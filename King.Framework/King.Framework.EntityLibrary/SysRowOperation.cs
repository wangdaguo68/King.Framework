namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;

    [Serializable, KingTable(IsInherited=true)]
    public class SysRowOperation : SysPageControl
    {
        public override void InitNavigationList()
        {
            base.InitNavigationList();
            this.OperationConditionRedirects = new List<SysOperationConditionRedirect>();
        }

        public override void SetContext(CacheContext context)
        {
            base.SetContext(context);
            this.RedirectEntity = context.FindById<SysEntity>(this.RedirectEntityId);
            this.RedirectPage = context.FindById<SysPage>(this.RedirectPageId);
            this.VisibleCondition = context.FindById<SysCondition>(this.VisibleConditionId);
            this.EnableCondition = context.FindById<SysCondition>(this.EnableConditionId);
            this.BeforeLogicOperation = context.FindById<SysBusinessLogicOperation>(this.BeforeLogicOperationId);
            this.AfterLogicOperation = context.FindById<SysBusinessLogicOperation>(this.AfterLogicOperationId);
        }

        [XmlIgnore]
        public virtual SysBusinessLogicOperation AfterLogicOperation { get; set; }

        [KingRef(typeof(SysBusinessLogicOperation)), KingColumn]
        public virtual long? AfterLogicOperationId { get; set; }

        public virtual SysBusinessLogicOperation BeforeLogicOperation { get; set; }

        [KingRef(typeof(SysBusinessLogicOperation)), KingColumn]
        public virtual long? BeforeLogicOperationId { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string Click { get; set; }

        [KingColumn(MaxLength=100)]
        public virtual string CssClassName { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string DisplayText { get; set; }

        [KingColumn]
        public virtual bool? EnablePermission { get; set; }

        [KingColumn]
        public virtual bool? IsTransaction { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string OnClientClick { get; set; }

        [XmlIgnore]
        public virtual ICollection<SysOperationConditionRedirect> OperationConditionRedirects { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string OperationName { get; set; }

        [KingColumn]
        public virtual int? OperationType { get; set; }

        [XmlIgnore]
        public virtual SysEntity RedirectEntity { get; set; }

        [KingColumn, KingRef(typeof(SysEntity))]
        public virtual long? RedirectEntityId { get; set; }

        [XmlIgnore]
        public virtual SysPage RedirectPage { get; set; }

        [KingColumn, KingRef(typeof(SysPage))]
        public virtual long? RedirectPageId { get; set; }

        [KingColumn(MaxLength=200)]
        public string RedirectURL { get; set; }
    }
}

