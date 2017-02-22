namespace King.Framework.EntityLibrary
{
    using System;
    using System.ComponentModel;

    public enum ReportStateEnum
    {
        [Description("未发布")]
        NotPublish = 1,
        [Description("已发布")]
        Published = 2
    }
}

