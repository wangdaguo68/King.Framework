namespace King.Framework.EntityLibrary
{
    using System;
    using System.ComponentModel;

    public enum ParticipantFunctionType
    {
        [Description("自定义C#")]
        Custom_Net = 11,
        [Description("部门经理")]
        DepartmentManager = 4,
        [Description("部门角色")]
        DepartmentRole = 7,
        [Description("直属主管")]
        DIRECT_MANAGER = 9,
        [Description("上级部门")]
        ParentDepartment = 5,
        [Description("自定义Python")]
        Python = 8,
        [Description("具体部门")]
        SpecialDepartment = 3,
        [Description("指定层级的上级部门")]
        SpecialLevelParentDepartment = 10,
        [Description("具体角色")]
        SpecialRole = 6,
        [Description("具体人员")]
        SpecialUser = 1,
        [Description("流程启动部门")]
        StartDepartment = 2,
        [Description("流程启动人")]
        StartUser = 0
    }
}

