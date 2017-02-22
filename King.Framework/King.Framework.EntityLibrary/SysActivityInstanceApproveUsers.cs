namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;

    [KingTable(Name="SysActivityInstanceApproveUser")]
    public class SysActivityInstanceApproveUsers : IApproveUser
    {
        public virtual SysActivityInstance CurrentActivityInstance { get; set; }

        [KingColumn]
        public virtual int? CurrentActivityInstanceId { get; set; }

        [KingColumn]
        public virtual int? FlagInt { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string FlagString { get; set; }

        [KingColumn(IsPrimaryKey=true)]
        public int Id { get; set; }

        [KingColumn]
        public virtual bool? IsMajor { get; set; }

        public virtual SysWorkItem OriginalWorkItem { get; set; }

        [KingColumn]
        public virtual int? OriginalWorkItemId { get; set; }

        [KingColumn]
        public virtual int? ProcessInstanceId { get; set; }

        [KingColumn]
        public virtual int? UserId { get; set; }
    }
}

