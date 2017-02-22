namespace King.Framework.EntityLibrary
{
    using System;
    using System.ComponentModel;

    public enum TreeLoadMethod
    {
        [Description("全部加载")]
        All = 0,
        [Description("异步加载")]
        FirstLevel = 1
    }
}

