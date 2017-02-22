namespace King.Framework.EntityLibrary
{
    using System;
    using System.ComponentModel;

    public enum SysServicePriority
    {
        [Description("高级")]
        Advanced = 3,
        [Description("中级")]
        Intermediate = 2,
        [Description("低级")]
        Low = 1
    }
}

