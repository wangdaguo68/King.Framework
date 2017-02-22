namespace King.Framework.EntityLibrary
{
    using System;
    using System.ComponentModel;

    public enum ReportCompareTypeEnum
    {
        [Description("LIKE '%'+{0}+'%'")]
        Contains = 9,
        [Description("当前用户")]
        CurrentUser = 0x65,
        [Description("LIKE '%'+{0}")]
        EndWith = 7,
        [Description("= {0}")]
        Equal = 3,
        [Description("> {0}")]
        GreaterThan = 5,
        [Description(">= {0}")]
        GreaterThanOrEqual = 4,
        [Description("< {0}")]
        LessThan = 1,
        [Description("<= {0}")]
        LessThanOrEqual = 2,
        [Description("!= {0}")]
        NotEqual = 6,
        [Description("LIKE {0}+'%'")]
        StartWith = 8,
        [Description("本月")]
        ThisMonth = 0xdd,
        [Description("本年度")]
        ThisYear = 0xe7,
        [Description("今天")]
        Today = 0xc9
    }
}

