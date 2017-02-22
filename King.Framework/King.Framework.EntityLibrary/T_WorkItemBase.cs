namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using King.Framework.Interfaces;
    using System;
    using System.Runtime.CompilerServices;

    [KingTable]
    public class T_WorkItemBase : IWorkItemBase
    {
        [KingColumn]
        public int? AppId { get; set; }

        [KingColumn]
        public int? Classify { get; set; }

        [KingColumn(MaxLength=0x100)]
        public string CompletePageUrl { get; set; }

        [KingColumn]
        public DateTime? CreateTime { get; set; }

        [KingColumn]
        public DateTime? EarliestExecuteTime { get; set; }

        [KingColumn]
        public DateTime? EndTime { get; set; }

        public virtual T_Entity_Trace_Plan Entity_Trace_Plan { get; set; }

        [KingColumn]
        public long? EntityId { get; set; }

        [KingColumn]
        public bool? IsAllDay { get; set; }

        [KingColumn]
        public bool? IsAssigned { get; set; }

        [KingColumn]
        public bool? IsMust { get; set; }

        [KingColumn]
        public DateTime? LatestExecuteTime { get; set; }

        [KingColumn]
        public int? ObjectId { get; set; }

        [KingColumn]
        public int? OwnerId { get; set; }

        [KingColumn]
        public int? ParentWorkItemId { get; set; }

        [KingColumn]
        public DateTime? StartTime { get; set; }

        [KingColumn]
        public int? State { get; set; }

        [KingColumn(MaxLength=100)]
        public string Title { get; set; }

        [KingColumn(IsPrimaryKey=true, KeyOrder=1)]
        public int WorkItemBase_Id { get; set; }

        [KingColumn(MaxLength=100)]
        public string WorkItemBase_Name { get; set; }

        [KingColumn(MaxLength=100)]
        public string WorkItemId { get; set; }

        [KingColumn]
        public int? WorkSort { get; set; }
    }
}

