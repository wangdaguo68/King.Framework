namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;

    [KingTable]
    public class SysApproveActivityData
    {
        [KingColumn]
        public int? ActivityInstanceId { get; set; }

        [KingColumn]
        public int? ActivityParticipantId { get; set; }

        [KingColumn]
        public int? AddingUserId { get; set; }

        [KingColumn(MaxLength=500)]
        public string ApproveComment { get; set; }

        [KingColumn]
        public int? ApproveGroupId { get; set; }

        [KingColumn]
        public int? ApproveResult { get; set; }

        [KingColumn]
        public DateTime? ApproveTime { get; set; }

        [KingColumn]
        public int? ApproveUserId { get; set; }

        [KingColumn(IsPrimaryKey=true, KeyOrder=1)]
        public int DataId { get; set; }

        [KingColumn]
        public bool? IsAdded { get; set; }

        [KingColumn]
        public bool? IsProxy { get; set; }

        [KingColumn]
        public int? ProxyUserId { get; set; }

        [KingColumn]
        public int? WorkItemId { get; set; }
    }
}

