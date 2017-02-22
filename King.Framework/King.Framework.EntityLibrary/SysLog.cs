namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;
    using System.Text;

    [KingTable]
    public class SysLog
    {
        public override string ToString()
        {
            return this.ToString(string.Empty);
        }

        public string ToString(string prefix)
        {
            StringBuilder builder = new StringBuilder(0x400);
            builder.Append(prefix).Append("LogId=").Append(this.LogId).Append(";ServerId=").Append(this.ServerId).Append(";AppId=").Append(this.AppId).Append(";CategoryId=").Append(this.CategoryId).Append(";OperationUserId=").Append(this.OperationUserId).Append(";LogType=").Append(this.LogType).Append(";CreateTime=").Append(this.CreateTime).Append(";Data=").Append(this.Data).Append(";Info=").Append(this.Info).Append(";Ext1=").Append(this.Ext1).Append(";Ext2=").Append(this.Ext2).Append(";Ext3=").Append(this.Ext3).Append(";Ext4=").Append(this.Ext4);
            return builder.ToString();
        }

        [KingColumn]
        public int AppId { get; set; }

        [KingColumn]
        public int CategoryId { get; set; }

        [KingColumn]
        public DateTime CreateTime { get; set; }

        [KingColumn(MaxLength=-1)]
        public string Data { get; set; }

        [KingColumn]
        public int Ext1 { get; set; }

        [KingColumn]
        public int Ext2 { get; set; }

        [KingColumn(MaxLength=-1)]
        public string Ext3 { get; set; }

        [KingColumn(MaxLength=-1)]
        public string Ext4 { get; set; }

        [KingColumn(MaxLength=-1)]
        public string Info { get; set; }

        [KingColumn(IsPrimaryKey=true, KeyOrder=1)]
        public int LogId { get; set; }

        [KingColumn]
        public LogTypeEnum LogType { get; set; }

        [KingColumn]
        public int OperationUserId { get; set; }

        [KingColumn]
        public int ServerId { get; set; }
    }
}

