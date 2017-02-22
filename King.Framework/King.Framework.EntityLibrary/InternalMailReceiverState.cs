namespace King.Framework.EntityLibrary
{
    using System;
    using System.ComponentModel;

    public enum InternalMailReceiverState
    {
        [Description("已删除")]
        IsDeleted = 2,
        [Description("已接收")]
        IsReceived = 1
    }
}

