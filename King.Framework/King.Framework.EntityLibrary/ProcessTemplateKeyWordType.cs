namespace King.Framework.EntityLibrary
{
    using System;
    using System.ComponentModel;

    public enum ProcessTemplateKeyWordType
    {
        [Description("活动实体字段")]
        ActivityEntityProperty = 1,
        [Description("审核页面")]
        ApprovePageUrl = 4,
        [Description("当前时间")]
        CurrentTime = 5,
        [Description("收件人")]
        Participant = 4,
        [Description("流程详情")]
        ProcessDetailUrl = 5,
        [Description("流程实体字段")]
        ProcessEntityProperty = 0
    }
}

