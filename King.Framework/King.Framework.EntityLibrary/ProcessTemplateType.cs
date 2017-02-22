namespace King.Framework.EntityLibrary
{
    using System;
    using System.ComponentModel;

    public enum ProcessTemplateType
    {
        [Description("自定义")]
        Custom = 2,
        [Description("邮件")]
        Email = 0,
        [Description("短信")]
        Message = 1
    }
}

