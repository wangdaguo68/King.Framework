namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;

    [KingTable]
    public class T_Email
    {
        [KingColumn]
        public DateTime? CreateTime { get; set; }

        [KingColumn]
        public int? CreateUserId { get; set; }

        [KingColumn(MaxLength=-1)]
        public string Email_Bcc { get; set; }

        [KingColumn(MaxLength=-1)]
        public string Email_Cc { get; set; }

        [KingColumn(MaxLength=-1)]
        public string Email_Content { get; set; }

        [KingColumn(IsPrimaryKey=true)]
        public int Email_Id { get; set; }

        [KingColumn(MaxLength=-1)]
        public string Email_Key { get; set; }

        [KingColumn(MaxLength=-1)]
        public string Email_Name { get; set; }

        [KingColumn]
        public DateTime? Email_ReceiveDate { get; set; }

        [KingColumn(MaxLength=-1)]
        public string Email_Receiver { get; set; }

        [KingColumn]
        public int? Email_ReceiveUserID { get; set; }

        [KingColumn]
        public DateTime? Email_SendDate { get; set; }

        [KingColumn(MaxLength=-1)]
        public string Email_Sender { get; set; }

        [KingColumn]
        public int? Email_Status { get; set; }

        public EmailStatusEnum? Email_StatusEnum { get; set; }

        [KingColumn(MaxLength=-1)]
        public string Email_Title { get; set; }

        [KingColumn]
        public int? Email_Type { get; set; }

        public EmailTypeEnum? Email_TypeEnum { get; set; }

        [KingColumn]
        public int? OwnerId { get; set; }

        [KingColumn]
        public int? State { get; set; }

        [KingColumn]
        public int? StateDetail { get; set; }

        [KingColumn]
        public DateTime? UpdateTime { get; set; }

        [KingColumn]
        public int? UpdateUserId { get; set; }
    }
}

