namespace King.Framework.Interfaces
{
    using System;

    public interface IWorkItemBase
    {
        string CompletePageUrl { get; set; }

        DateTime? CreateTime { get; set; }

        DateTime? EndTime { get; set; }

        bool? IsAllDay { get; set; }

        int? OwnerId { get; set; }

        DateTime? StartTime { get; set; }

        int? State { get; set; }

        string Title { get; set; }

        int WorkItemBase_Id { get; set; }

        string WorkItemBase_Name { get; set; }

        string WorkItemId { get; set; }
    }
}

