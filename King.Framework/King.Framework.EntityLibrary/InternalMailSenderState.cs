namespace King.Framework.EntityLibrary
{
    using System;
    using System.ComponentModel;

    public enum InternalMailSenderState
    {
        [Description("草稿")]
        Draft = 0,
        [Description("已删除")]
        IsDeleted = 2,
        [Description("已发送")]
        IsSended = 1
    }
}

