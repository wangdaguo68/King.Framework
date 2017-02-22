namespace King.Framework.EntityLibrary
{
    using System;
    using System.ComponentModel;

    public enum ReportModelEnum
    {
        [Description("监控统计")]
        Monitoring_Statistics = 3,
        [Description("页面统计")]
        Page_Statistics = 1,
        [Description("订阅统计")]
        Subscribe_Statistics = 2
    }
}

