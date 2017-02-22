namespace King.Framework.EntityLibrary
{
    using System;
    using System.ComponentModel;

    [Serializable]
    public enum Department_StateEnum
    {
        [Description("已删除")]
        Deleted = 0x270f,
        [Description("停用")]
        Disable = 1,
        [Description("启用")]
        Enable = 0
    }
}

