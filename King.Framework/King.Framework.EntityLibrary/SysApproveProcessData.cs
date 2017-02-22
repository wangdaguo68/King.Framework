namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;

    [KingTable]
    public class SysApproveProcessData
    {
        [KingColumn]
        public DateTime? ApproveCompleteTime { get; set; }

        [KingColumn]
        public int? ApproveResult { get; set; }

        [KingColumn]
        public int? Attachment1 { get; set; }

        [KingColumn]
        public int? Attachment2 { get; set; }

        [KingColumn]
        public int? ContentType { get; set; }

        [KingColumn]
        public DateTime? CreateTime { get; set; }

        [KingColumn]
        public int? CreateUserId { get; set; }

        [KingColumn(MaxLength=0x100)]
        public string FormContent { get; set; }

        [KingColumn(IsPrimaryKey=true)]
        public int FormInstanceId { get; set; }

        [KingColumn]
        public long? ProcessId { get; set; }

        [KingColumn]
        public int? ProcessInstanceId { get; set; }

        [KingColumn]
        public int? RequestDepartmentId { get; set; }

        [KingColumn]
        public DateTime? RequestSubmitTime { get; set; }

        [KingColumn]
        public DateTime? RequestTime { get; set; }

        [KingColumn]
        public int? RequestUserId { get; set; }

        [KingColumn(MaxLength=0xff)]
        public string ReserveField1 { get; set; }

        [KingColumn(MaxLength=0xff)]
        public string ReserveField2 { get; set; }

        [KingColumn]
        public int? State { get; set; }
    }
}

