namespace King.Framework.EntityLibrary
{
    using System;
    using System.ComponentModel;

    public enum ActivityPassType
    {
        [Description("默认优先")]
        FirstDefault = 0,
        [Description("通过数")]
        UsingMinNum = 1,
        [Description("通过百分比")]
        UsingMinRatio = 2
    }
}

