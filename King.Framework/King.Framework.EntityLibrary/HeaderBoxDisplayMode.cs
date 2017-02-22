namespace King.Framework.EntityLibrary
{
    using System;
    using System.ComponentModel;

    public enum HeaderBoxDisplayMode
    {
        [Description("详情")]
        Detail = 2,
        [Description("新增")]
        FormInput = 0,
        [Description("列表")]
        Table = 1
    }
}

