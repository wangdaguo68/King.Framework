namespace King.Framework.EntityLibrary
{
    using System;
    using System.ComponentModel;

    public enum ProcessRemindUseTimeType
    {
        [Description("流程被撤消")]
        ProcessCancel = 12,
        [Description("流程完成")]
        ProcessFinished = 11,
        [Description("流程启动")]
        ProcessStart = 10
    }
}

