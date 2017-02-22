namespace King.Framework.EntityLibrary
{
    using System;
    using System.ComponentModel;

    public enum SumType
    {
        [Description("求平均")]
        Avg = 4,
        [Description("求数量")]
        Count = 1,
        [Description("求最大")]
        Max = 8,
        [Description("求最小")]
        Min = 0x10,
        [Description("求和")]
        Sum = 2
    }
}

