namespace King.Framework.EntityLibrary
{
    using System;
    using System.ComponentModel;

    [Serializable]
    public enum T_Application_UserStateEnum
    {
        [Description("已删除")]
        IsDeleted = 0x270f,
        [Description("否")]
        No = 0,
        [Description("是")]
        Yes = 1
    }
}

