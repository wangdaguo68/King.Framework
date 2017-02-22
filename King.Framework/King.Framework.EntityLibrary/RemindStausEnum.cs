namespace King.Framework.EntityLibrary
{
    using System;
    using System.ComponentModel;

    public enum RemindStausEnum
    {
        [Description("已取消")]
        Cancel = 2,
        [Description("已读")]
        Completed = 1,
        [Description("草稿")]
        Draft = 3,
        [Description("已删除")]
        IsDeleted = 0x270f,
        [Description("未读")]
        New = 0
    }
}

