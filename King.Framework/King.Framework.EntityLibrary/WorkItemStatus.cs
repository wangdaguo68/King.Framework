namespace King.Framework.EntityLibrary
{
    using System;

    public enum WorkItemStatus
    {
        Cancelled = 2,
        CancelledBySystem = 3,
        Completed = 1,
        Completing = 0x63,
        Created = 0
    }
}

