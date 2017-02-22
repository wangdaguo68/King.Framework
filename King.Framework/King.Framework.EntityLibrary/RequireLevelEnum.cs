namespace King.Framework.EntityLibrary
{
    using System;
    using System.ComponentModel;

    public enum RequireLevelEnum
    {
        [Description("业务级")]
        Business = 3,
        [Description("平台级")]
        PlatForm = 1,
        [Description("可选级")]
        Selectable = 4,
        [Description("系统级")]
        System = 2
    }
}

