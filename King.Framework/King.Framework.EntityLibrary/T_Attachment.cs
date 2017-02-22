namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;

    [Serializable, KingTable]
    public class T_Attachment
    {
        public override bool Equals(object obj)
        {
            return (this.GetHashCode() == obj.GetHashCode());
        }

        public override int GetHashCode()
        {
            return (base.GetType().GetHashCode() ^ this.Attachment_ID.GetHashCode());
        }

        public override string ToString()
        {
            return (this.Attachment_Name ?? "");
        }

        public int? _mobileState { get; set; }

        [KingColumn(IsPrimaryKey=true)]
        public int Attachment_ID { get; set; }

        [KingColumn(MaxLength=200)]
        public string Attachment_Name { get; set; }

        [KingColumn(MaxLength=200)]
        public string ContentType { get; set; }

        [KingColumn]
        public DateTime? CreateTime { get; set; }

        [KingColumn]
        public int? CreateUserId { get; set; }

        [KingColumn(MaxLength=200)]
        public string CurrentPosition { get; set; }

        [KingColumn(MaxLength=200)]
        public string DisplayName { get; set; }

        [KingColumn(MaxLength=-1)]
        public byte[] FileData { get; set; }

        [KingColumn(MaxLength=200)]
        public string FileDesc { get; set; }

        [KingColumn(MaxLength=200)]
        public string FileExtension { get; set; }

        [KingColumn(MaxLength=200)]
        public string FileName { get; set; }

        [KingColumn(MaxLength=500)]
        public string FilePath { get; set; }

        [KingColumn]
        public int? FileSize { get; set; }

        [KingColumn]
        public int? MediaPlaytime { get; set; }

        [KingColumn(MaxLength=200)]
        public string MobileFilePath { get; set; }

        [KingColumn(MaxLength=200)]
        public string OwnerEntityType { get; set; }

        [KingColumn]
        public int? OwnerId { get; set; }

        [KingColumn]
        public int? OwnerObjectId { get; set; }

        [KingColumn]
        public int? State { get; set; }

        [KingColumn]
        public int? StateDetail { get; set; }

        public T_AttachmentStateEnum? StateEnum
        {
            get
            {
                if (this.State.HasValue)
                {
                    //return new T_AttachmentStateEnum?(this.State.Value);
                }
                return null;
            }
            set
            {
               // this.State = new int?(value.Value);
            }
        }

        [KingColumn(MaxLength=200)]
        public long? TableVersion { get; set; }

        [KingColumn]
        public DateTime? UpdateTime { get; set; }

        [KingColumn]
        public int? UpdateUserId { get; set; }
    }
}

