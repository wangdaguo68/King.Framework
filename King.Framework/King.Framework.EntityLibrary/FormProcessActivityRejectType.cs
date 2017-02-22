namespace King.Framework.EntityLibrary
{
    using System;
    using System.ComponentModel;

    public enum FormProcessActivityRejectType
    {
        [Description("结束流程")]
        EndActivity = 0,
        [Description("跳转到指定活动")]
        SelectActivity = 1,
        [Description("审核时用户指定")]
        SelectActivityWhenRunning = 2
    }
}

