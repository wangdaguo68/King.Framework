namespace King.Framework.EntityLibrary
{
    using System;
    using System.ComponentModel;

    [Serializable]
    public enum T_WorkItemBaseStateEnum
    {
        [Description("新建")]
        Added = 0,
        [Description("被取消")]
        Cancelled = 2,
        [Description("已执行")]
        Completed = 1,
        [Description("已删除")]
        IsDeleted = 0x270f,
        [Description("已接收")]
        Received = 4
    }
}

