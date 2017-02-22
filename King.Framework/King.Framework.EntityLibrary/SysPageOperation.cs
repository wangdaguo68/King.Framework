namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;

    [Serializable, KingTable(IsInherited=true)]
    public class SysPageOperation : SysPageControl
    {
        public override void SetContext(CacheContext context)
        {
            base.SetContext(context);
            this.BatchRelation = context.FindById<SysOneMoreRelation>(this.RelationId);
            this.RedirectEntity = context.FindById<SysEntity>(this.RedirectEntityId);
            this.RedirectPage = context.FindById<SysPage>(this.RedirectPageId);
            this.BeforeLogicOperation = context.FindById<SysBusinessLogicOperation>(this.BeforeLogicOperationId);
            this.AfterLogicOperation = context.FindById<SysBusinessLogicOperation>(this.AfterLogicOperationId);
        }

        [XmlIgnore]
        public virtual SysBusinessLogicOperation AfterLogicOperation { get; set; }

        [KingColumn, KingRef(typeof(SysBusinessLogicOperation))]
        public virtual long? AfterLogicOperationId { get; set; }

        [XmlIgnore]
        public virtual SysOneMoreRelation BatchRelation { get; set; }

        [XmlIgnore]
        public virtual SysBusinessLogicOperation BeforeLogicOperation { get; set; }

        [KingRef(typeof(SysBusinessLogicOperation)), KingColumn]
        public virtual long? BeforeLogicOperationId { get; set; }

        [KingColumn(MaxLength=500)]
        public virtual string Click { get; set; }

        [KingColumn(MaxLength=100)]
        public virtual string CssClassName { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string DisplayText { get; set; }

        [KingColumn]
        public virtual bool? EnablePermission { get; set; }

        [KingColumn]
        public virtual bool IsNotValidate { get; set; }

        [KingColumn]
        public virtual bool? IsTransaction { get; set; }

        [KingColumn(MaxLength=500)]
        public virtual string OnClientClick { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string OperationName { get; set; }

        [KingColumn]
        public virtual int OperationType { get; set; }

        [XmlIgnore]
        public virtual SysProcess OwnerProcess { get; set; }

        [KingColumn, KingRef(typeof(SysProcess))]
        public virtual long? ProcessId { get; set; }

        [KingColumn(MaxLength=0x200)]
        public virtual string ProcessName { get; set; }

        [XmlIgnore]
        public virtual SysEntity RedirectEntity { get; set; }

        [KingColumn, KingRef(typeof(SysEntity))]
        public virtual long? RedirectEntityId { get; set; }

        [XmlIgnore]
        public virtual SysPage RedirectPage { get; set; }

        [KingRef(typeof(SysPage)), KingColumn]
        public virtual long? RedirectPageId { get; set; }

        [KingColumn(MaxLength=0x200)]
        public virtual string RedirectURL { get; set; }

        [KingRef(typeof(SysOneMoreRelation)), KingColumn]
        public virtual long? RelationId { get; set; }

        [KingColumn]
        public virtual int? RepeatType { get; set; }
    }
}

