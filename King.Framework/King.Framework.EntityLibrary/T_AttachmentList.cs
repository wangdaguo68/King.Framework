namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;

    [Serializable, KingTable]
    public class T_AttachmentList
    {
        public override bool Equals(object obj)
        {
            return (this.GetHashCode() == obj.GetHashCode());
        }

        public override int GetHashCode()
        {
            return (base.GetType().GetHashCode() ^ this.AttachmentListId.GetHashCode());
        }

        public override string ToString()
        {
            return (this.AttachmentList_Name ?? "");
        }

        public int? _mobileState { get; set; }

        [KingColumn]
        public int? Attachment_ID { get; set; }

        [KingColumn(MaxLength=200)]
        public string AttachmentList_Name { get; set; }

        [KingColumn(IsPrimaryKey=true)]
        public int AttachmentListId { get; set; }

        [KingColumn]
        public DateTime? CreateTime { get; set; }

        [KingColumn]
        public int? CreateUserId { get; set; }

        [KingColumn]
        public long? EntityId { get; set; }

        [KingColumn(MaxLength=200)]
        public string Keyword { get; set; }

        [KingColumn]
        public int? ObjectID { get; set; }

        [KingColumn]
        public int? OrderIndex { get; set; }

        [KingColumn]
        public int? OwnerId { get; set; }

        [KingColumn]
        public int? State { get; set; }

        [KingColumn]
        public int? StateDetail { get; set; }

        public T_AttachmentListStateEnum? StateEnum
        {
            get
            {
                if (this.State.HasValue)
                {
                   // return new T_AttachmentListStateEnum?(this.State.Value);
                }
                return null;
            }
            set
            {
                if (!value.HasValue)
                {
                    this.State = null;
                }
                //this.State = new int?(value.Value);
            }
        }

        [KingColumn]
        public int? TableVersion { get; set; }

        [KingColumn]
        public DateTime? UpdateTime { get; set; }

        [KingColumn]
        public int? UpdateUserId { get; set; }
    }
}

