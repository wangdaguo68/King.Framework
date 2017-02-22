namespace King.Framework.EntityLibrary
{
    using System;
    using System.ComponentModel;

    [Serializable]
    public enum EmailStatusEnum
    {
        [Description("未发送")]
        NotSend = 0,
        [Description("已接收")]
        Received = 3,
        [Description("发送失败")]
        SendFailure = 2,
        [Description("发送成功")]
        SendSuccess = 1
    }
}

