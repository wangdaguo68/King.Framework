namespace King.Framework.EntityLibrary
{
    using System;
    using System.ComponentModel;

    public enum TemplateType
    {
        [Description("自定义")]
        Custom = 2,
        [Description("邮件")]
        Mail = 0,
        [Description("短信")]
        ShortMessage = 1
    }
}

