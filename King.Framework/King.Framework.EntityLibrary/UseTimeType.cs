namespace King.Framework.EntityLibrary
{
    using System;
    using System.ComponentModel;

    public enum UseTimeType
    {
        [Description("活动结束")]
        ActivityEnd = 1,
        [Description("活动开始")]
        ActivityStart = 0,
        [Description("流程被撤消")]
        ProcessCancel = 12,
        [Description("流程完成")]
        ProcessFinished = 11,
        [Description("流程启动")]
        ProcessStart = 10,
        [Description("工作项产生")]
        WorkItemCreate = 2,
        [Description("工作项完成")]
        WorkItemFinished = 3
    }
}

