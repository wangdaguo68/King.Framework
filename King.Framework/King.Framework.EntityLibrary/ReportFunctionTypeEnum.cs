namespace King.Framework.EntityLibrary
{
    using System;
    using System.ComponentModel;

    public enum ReportFunctionTypeEnum
    {
        [Description("+")]
        Add = 1,
        [Description("/")]
        Div = 4,
        [Description("短日期")]
        GetDate = 7,
        [Description("月份")]
        GetMonth = 6,
        [Description("年份")]
        GetYear = 5,
        [Description("字段")]
        JustAdd = 0,
        [Description("-")]
        Minus = 2,
        [Description("*")]
        Mul = 3,
        [Description("反除")]
        ReDiv = 8,
        [Description("反减")]
        ReMinus = 9
    }
}

