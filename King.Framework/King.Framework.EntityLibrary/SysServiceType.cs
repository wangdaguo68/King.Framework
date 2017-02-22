namespace King.Framework.EntityLibrary
{
    using System;
    using System.ComponentModel;

    public enum SysServiceType
    {
        [Description("ashx")]
        ashx = 3,
        [Description("dll")]
        dll = 1,
        [Description("exe")]
        exe = 2
    }
}

