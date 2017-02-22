namespace King.Framework.EntityLibrary
{
    using System;
    using System.ComponentModel;

    public enum FormFieldPrivilege
    {
        [Description("不可见")]
        Invisible = 0,
        [Description("只读")]
        ReadOnly = 1,
        [Description("可编辑")]
        ReadWrite = 2
    }
}

