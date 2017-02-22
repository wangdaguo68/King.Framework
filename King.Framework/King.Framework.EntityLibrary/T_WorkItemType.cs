namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;

    [Serializable, KingTable]
    public class T_WorkItemType
    {
        [KingColumn]
        public bool? AllowAddToPlan { get; set; }

        [KingColumn]
        public bool? AllowAutoExecute { get; set; }

        [KingColumn]
        public bool? AllowDispatch { get; set; }

        [KingColumn]
        public bool? AllowEvaluate { get; set; }

        [KingColumn]
        public bool? AllowManualAdd { get; set; }

        [KingColumn(MaxLength=200)]
        public string CompletePageName { get; set; }

        [KingColumn]
        public int? CompletePageType { get; set; }

        [KingColumn(MaxLength=200)]
        public string CreatePageName { get; set; }

        [KingColumn]
        public int? CreatePageType { get; set; }

        [KingColumn(MaxLength=200)]
        public string CreatePageUserDefinePage { get; set; }

        [KingColumn]
        public DateTime? CreateTime { get; set; }

        [KingColumn]
        public int? CreateUserId { get; set; }

        [KingColumn(MaxLength=200)]
        public string DisplayText { get; set; }

        [KingColumn(MaxLength=200)]
        public string EntityName { get; set; }

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

        [KingColumn(IsPrimaryKey=true)]
        public int WorkItemType_Id { get; set; }

        [KingColumn(MaxLength=200)]
        public string WorkItemType_Name { get; set; }
    }
}

