namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    [KingTable]
    public class SysWorkflowMessage
    {
        private bool _addUser = false;
        private readonly List<IApproveUser> _nextApproveUserList = new List<IApproveUser>();

        [KingColumn]
        public int ActivityId { get; set; }

        [KingColumn]
        public int ActivityInstanceId { get; set; }

        [KingColumn]
        public bool AddUser
        {
            get
            {
                return this._addUser;
            }
            set
            {
                this._addUser = value;
            }
        }

        [KingColumn]
        public int? AddUserId { get; set; }

        [KingColumn(MaxLength=200)]
        public string ApproveComment { get; set; }

        [KingColumn]
        public ApproveResultEnum ApproveResult { get; set; }

        [KingColumn]
        public DateTime CreateTime { get; set; }

        [KingColumn]
        public int Ext1 { get; set; }

        [KingColumn]
        public int Ext2 { get; set; }

        [KingColumn(MaxLength=200)]
        public string Ext3 { get; set; }

        [KingColumn(MaxLength=200)]
        public string Ext4 { get; set; }

        [KingColumn(MaxLength=200)]
        public string LastErrorMessage { get; set; }

        [KingColumn(IsPrimaryKey=true)]
        public int MessageId { get; set; }

        [KingColumn]
        public WorkflowMessageTypeEnum MessageType { get; set; }

        public List<IApproveUser> NextApproveUserList
        {
            get
            {
                return this._nextApproveUserList;
            }
        }

        [KingColumn]
        public int OperationUserId { get; set; }

        [KingColumn(MaxLength=200)]
        public string Param1 { get; set; }

        [KingColumn(MaxLength=200)]
        public string Param2 { get; set; }

        [KingColumn(MaxLength=200)]
        public string Param3 { get; set; }

        [KingColumn(MaxLength=200)]
        public string Param4 { get; set; }

        [KingColumn]
        public long ProcessId { get; set; }

        [KingColumn]
        public int ProcessInstanceId { get; set; }

        [KingColumn]
        public SysWorkflowMessageStateEnum State { get; set; }

        [KingColumn]
        public int WorkItemId { get; set; }
    }
}

