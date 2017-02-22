namespace King.Framework.EntityLibrary
{
    using System;
    using System.ComponentModel;

    public enum ProcessTemplateResultType
    {
        [Description("通过")]
        Pass = 1,
        [Description("驳回")]
        Reject = 0
    }
}

