namespace King.Framework.Common
{
    using System;
    using System.ComponentModel;

    public enum EntityPrivilegeEnum
    {
        [Description("全部权限")]
        AllRights = 5,
        [Description("部门")]
        Department = 3,
        [Description("部门及子部门")]
        DepartmentAndSubSector = 4,
        [Description("无权限")]
        NoPermission = 1,
        [Description("个人")]
        Personal = 2
    }
}
