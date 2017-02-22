namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;

    [KingTable(Name="SysMessageApproveUser")]
    public class MessageApproveUser : IApproveUser
    {
        public MessageApproveUser()
        {
        }

        public MessageApproveUser(IApproveUser user)
        {
            this.UserId = user.UserId;
            this.IsMajor = user.IsMajor;
            this.FlagInt = user.FlagInt;
            this.FlagString = user.FlagString;
        }

        [KingColumn]
        public int? FlagInt { get; set; }

        [KingColumn(MaxLength=200)]
        public string FlagString { get; set; }

        [KingColumn]
        public bool? IsMajor { get; set; }

        [KingColumn(IsPrimaryKey=true)]
        public int MessageApproveUserId { get; set; }

        [KingColumn]
        public int? UserId { get; set; }

        [KingColumn]
        public int WorkflowMessageId { get; set; }
    }
}

