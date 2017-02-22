namespace King.Framework.EntityLibrary
{
    using System;
    using System.ComponentModel;

    [Serializable]
    public enum EmailTypeEnum
    {
        [Description("收件箱")]
        ReceiveBox = 1,
        [Description("发件箱")]
        SendBox = 0
    }
}

