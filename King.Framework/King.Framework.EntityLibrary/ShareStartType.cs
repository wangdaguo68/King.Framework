namespace King.Framework.EntityLibrary
{
    using System;
    using System.ComponentModel;

    public enum ShareStartType
    {
        [Description("条件")]
        ConditionShare = 2,
        [Description("全局")]
        GlobalShare = 1
    }
}

