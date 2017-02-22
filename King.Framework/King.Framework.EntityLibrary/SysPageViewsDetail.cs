namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;

    [Serializable, KingTable]
    public class SysPageViewsDetail
    {
        [KingColumn(MaxLength=200)]
        public string ButtonName { get; set; }

        [KingColumn(MaxLength=500)]
        public string ButtonOriginalName { get; set; }

        [KingColumn]
        public DateTime? ClientRequestTime { get; set; }

        [KingColumn]
        public DateTime? ClientResponseTime { get; set; }

        [KingColumn]
        public DateTime? CreateTime { get; set; }

        [KingColumn]
        public int? CreateUserId { get; set; }

        [KingColumn]
        public int? OwnerId { get; set; }

        [KingColumn(MaxLength=200)]
        public string PageName { get; set; }

        [KingColumn(IsPrimaryKey=true)]
        public int PageViewsDetail_Id { get; set; }

        [KingColumn(MaxLength=200)]
        public string PageViewsDetail_Name { get; set; }

        [KingColumn]
        public int? PageViewsId { get; set; }

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

