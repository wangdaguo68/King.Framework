namespace King.Framework.EntityLibrary
{
    using System;
    using System.ComponentModel;

    public enum FormState
    {
        [Description("草稿")]
        New = 0,
        [Description("已启用")]
        StartUsed = 1,
        [Description("停用")]
        StopUsed = 2
    }
}

