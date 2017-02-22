namespace King.Framework.LiteQueryDef.Internal
{
    using System;
    using System.ComponentModel;

    public enum CompareTypeEnum
    {
        [Description("当前时间之后")]
        AfterNow = 0xce,
        [Description("当前时间之前")]
        BeforeNow = 0xcd,
        [Description("LIKE '%{0}%'")]
        Contains = 8,
        [Description("当前用户")]
        CurrentUser = 0x65,
        [Description("部门")]
        Department = 0x72,
        [Description("部门与子部门")]
        DepartmentAndSub = 0x73,
        [Description("LIKE '%{0}'")]
        EndWith = 6,
        [Description("= {0}")]
        Equal = 3,
        [Description("Contains({0})")]
        FullText = 10,
        [Description("> {0}")]
        GreaterThan = 5,
        [Description("大于{0}天+1")]
        GreaterThanOneDayPlugOne = 0x14e,
        [Description(">= {0}")]
        GreaterThanOrEqual = 4,
        [Description("IN")]
        IN = 20,
        [Description("IsNotNull")]
        IsNotNull = 0x70,
        [Description("IsNull")]
        IsNull = 0x6f,
        [Description("最近一月")]
        LastMonth = 0xde,
        [Description("最近一周")]
        LastWeek = 0xd4,
        [Description("最近{0}天")]
        LastXDays = 0xcb,
        [Description("最近{0}月")]
        LastXMonths = 0xdf,
        [Description("最近{0}周")]
        LastXWeeks = 0xd5,
        [Description("最近{0}年")]
        LastXYears = 0xe9,
        [Description("最近一年")]
        LastYear = 0xe8,
        [Description("< {0}")]
        LessThan = 1,
        [Description("小于{0}天+1")]
        LessThanOneDayPlusOne = 0x14d,
        [Description("<= {0}")]
        LessThanOrEqual = 2,
        [Description("未来1天")]
        NextDay = 0x12d,
        [Description("未来一月")]
        NextMonth = 0x141,
        [Description("未来一周")]
        NextWeek = 0x137,
        [Description("未来{0}天")]
        NextXDays = 0x12e,
        [Description("未来{0}月")]
        NextXMonths = 0x142,
        [Description("未来{0}周")]
        NextXWeeks = 0x138,
        [Description("未来{0}年")]
        NextXYears = 0x14c,
        [Description("未来一年")]
        NextYear = 0x14b,
        [Description("!= {0}")]
        NotEqual = 9,
        [Description("被共享")]
        Shared = 0x71,
        [Description("LIKE '{0}%'")]
        StartWith = 7,
        [Description("本月")]
        ThisMonth = 0xdd,
        [Description("本周内")]
        ThisWeek = 0xd3,
        [Description("本年度")]
        ThisYear = 0xe7,
        [Description("今天")]
        Today = 0xc9
    }
}
