namespace King.Framework.EntityLibrary
{
    using System;
    using System.ComponentModel;

    [Serializable]
    public enum User_StateEnum
    {
        [Description("审核中")]
        Auditting = 1,
        [Description("已删除")]
        Deleted = 0x270f,
        [Description("停用")]
        Disabled = 3,
        [Description("已生效")]
        Enabled = 2,
        [Description("有效用户")]
        NO = 0
    }
}

