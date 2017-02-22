namespace King.Framework.EntityLibrary
{
    using System;
    using System.ComponentModel;

    public enum ReportChartTypeEnum
    {
        [Description("柱状图")]
        Column = 1,
        [Description("折线图")]
        Line = 2,
        [Description("饼图")]
        Pie = 4,
        [Description("曲线图")]
        Spline = 3
    }
}

