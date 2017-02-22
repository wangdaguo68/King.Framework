namespace King.Framework.EntityLibrary
{
    using System;
    using System.ComponentModel;

    public enum ActivityType
    {
        [Description("一般活动")]
        Activity = 3,
        [Description("签核活动")]
        Approve = 6,
        [Description("分支活动")]
        Decision = 2,
        [Description("结束活动")]
        End = 4,
        [Description("开始活动")]
        Start = 1
    }
}

