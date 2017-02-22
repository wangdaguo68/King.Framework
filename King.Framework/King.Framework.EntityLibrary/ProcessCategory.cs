namespace King.Framework.EntityLibrary
{
    using System;
    using System.ComponentModel;

    public enum ProcessCategory
    {
        [Description("签核流程")]
        Approve = 1,
        [Description("表单签核流程")]
        FormApprove = 2,
        [Description("普通流程")]
        Normal = 0
    }
}

