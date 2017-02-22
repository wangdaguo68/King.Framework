namespace King.Framework.Mvc
{
    using System;
    using System.ComponentModel;

    public enum DateEnum
    {
        [Description("日期")]
        Date = 1,
        [Description("日期时间")]
        DateTime = 2,
        [Description("时间")]
        Time = 3
    }
}

