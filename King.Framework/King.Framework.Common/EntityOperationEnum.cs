namespace King.Framework.Common
{
    using System;
    using System.ComponentModel;

    public enum EntityOperationEnum
    {
        [Description("添加权限")]
        Add = 1,
        [Description("删除权限")]
        Delete = 2,
        [Description("无操作")]
        None = 0,
        [Description("查询权限")]
        Query = 4,
        [Description("更改权限")]
        Update = 3
    }
}
