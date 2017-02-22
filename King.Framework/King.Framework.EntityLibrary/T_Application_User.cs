namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;

    [Serializable, KingTable]
    public class T_Application_User
    {
        [KingColumn]
        public string Action { get; set; }

        [KingColumn]
        public int Application_ID { get; set; }

        [KingColumn]
        public string Application_Name { get; set; }

        [KingColumn(IsPrimaryKey=true)]
        public int Application_User_Id { get; set; }

        [KingColumn]
        public string Application_User_Name { get; set; }

        [KingColumn]
        public int? ApplicationId { get; set; }

        [KingColumn]
        public string ApplicationPackage { get; set; }

        [KingColumn]
        public int? ApplicationType { get; set; }

        [KingColumn]
        public int? AppType { get; set; }

        [KingColumn]
        public DateTime? AuthorizationOperateTime { get; set; }

        [KingColumn]
        public int? CompanyId { get; set; }

        [KingColumn]
        public DateTime? CreateTime { get; set; }

        [KingColumn]
        public int? CreateUserId { get; set; }

        [KingColumn]
        public string DownloadPath { get; set; }

        [KingColumn]
        public bool? IsDelete { get; set; }

        [KingColumn]
        public bool? IsDelMobileData { get; set; }

        [KingColumn]
        public bool? IsDelMobileDB { get; set; }

        [KingColumn]
        public bool? IsUpdate { get; set; }

        [KingColumn]
        public bool? IsUse { get; set; }

        [KingColumn]
        public int? MsgCount { get; set; }

        [KingColumn]
        public int? OrderIndex { get; set; }

        [KingColumn]
        public int? OwnerId { get; set; }

        [KingColumn]
        public string Schemes { get; set; }

        [KingColumn]
        public int? State { get; set; }

        [KingColumn]
        public int? StateDetail { get; set; }

        [KingColumn]
        public int? TableVersion { get; set; }

        [KingColumn]
        public DateTime? UpdateTime { get; set; }

        [KingColumn]
        public int? UpdateUserId { get; set; }
    }
}

