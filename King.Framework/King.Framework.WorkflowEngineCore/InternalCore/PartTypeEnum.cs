namespace King.Framework.WorkflowEngineCore.InternalCore
{
    using System;
    using System.ComponentModel;

    internal enum PartTypeEnum
    {
        [Description("本活动所有参与人")]
        CurrentActivityParticipant = 0x66,
        [Description("本工作项负责人")]
        CurrentWorkItemOwner = 0x67,
        Custom_Net = 11,
        DEPARTMENT_ROLE = 7,
        DEPT_MANAGER = 4,
        DIRECT_MANAGER = 9,
        PARENT_DEPT = 5,
        [Description("上一活动参与人")]
        PreviousActivityParticipant = 100,
        [Description("之前所有活动参与人")]
        PreviousAllParticipant = 0x65,
        Python = 8,
        SPECIAL_DEPT = 3,
        SPECIAL_ROLE = 6,
        SPECIAL_USER = 1,
        SpecialLevelParentDepartment = 10,
        START_DEPT = 2,
        START_USER = 0
    }
}

