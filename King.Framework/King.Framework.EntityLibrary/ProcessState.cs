namespace King.Framework.EntityLibrary
{
    using System;
    using System.ComponentModel;

    public enum ProcessState
    {
        [Description("已新建")]
        Created = 0,
        [Description("已启用")]
        StartUsed = 1,
        [Description("已停用")]
        Stoped = 3,
        [Description("已升级")]
        Updated = 2
    }
}

