namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;

    [Serializable, KingTable]
    public class SysPageViews
    {
        [KingColumn(MaxLength=200)]
        public string ClientBrowser { get; set; }

        [KingColumn(MaxLength=200)]
        public string ClientBrowserVersion { get; set; }

        [KingColumn(MaxLength=100)]
        public string ClientIP { get; set; }

        [KingColumn(MaxLength=100)]
        public string ClientOS { get; set; }

        [KingColumn]
        public DateTime? CreateTime { get; set; }

        [KingColumn]
        public int? CreateUserId { get; set; }

        [KingColumn(MaxLength=100)]
        public string LoginUserName { get; set; }

        [KingColumn]
        public int? OwnerId { get; set; }

        [KingColumn(MaxLength=200)]
        public string PageName { get; set; }

        [KingColumn(MaxLength=200)]
        public string PageOriginalName { get; set; }

        [KingColumn(IsPrimaryKey=true)]
        public int PageViews_Id { get; set; }

        [KingColumn(MaxLength=200)]
        public string PageViews_Name { get; set; }

        [KingColumn(MaxLength=200)]
        public string RefererPageName { get; set; }

        [KingColumn(MaxLength=200)]
        public string RefererPageOriginalName { get; set; }

        [KingColumn]
        public DateTime? RequestTime { get; set; }

        [KingColumn]
        public DateTime? ResponseTime { get; set; }

        [KingColumn]
        public int? State { get; set; }

        [KingColumn]
        public int? StateDetail { get; set; }
    }
}

