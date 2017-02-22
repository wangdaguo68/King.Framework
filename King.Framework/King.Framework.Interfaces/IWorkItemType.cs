namespace King.Framework.Interfaces
{
    using System;

    public interface IWorkItemType
    {
        bool? AllowManualAdd { get; set; }

        string CreatePageName { get; set; }

        int? CreatePageType { get; set; }

        string CreatePageUserDefinePage { get; set; }

        string EntityName { get; set; }

        int WorkItemType_Id { get; set; }

        string WorkItemType_Name { get; set; }
    }
}

