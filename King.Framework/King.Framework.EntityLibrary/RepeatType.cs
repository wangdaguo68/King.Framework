namespace King.Framework.EntityLibrary
{
    using System;
    using System.ComponentModel;

    public enum RepeatType
    {
        [Description("全部不重复")]
        NotAll = 2,
        [Description("本单据内不重复")]
        NotThis = 1,
        [Description("可重复")]
        Repeat = 0
    }
}

