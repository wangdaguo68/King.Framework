namespace King.Framework.EntityLibrary
{
    using System;
    using System.ComponentModel;

    public enum ReportGraphTypeEnum
    {
        [Description("柱状图")]
        Columnar = 1,
        [Description("曲线图")]
        Curve = 2,
        [Description("饼状图")]
        Pie = 3
    }
}

