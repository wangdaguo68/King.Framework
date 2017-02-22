namespace King.Framework.EntityLibrary
{
    using System;
    using System.ComponentModel;

    public enum FormInstanceState
    {
        [Description("审核中")]
        Approving = 1,
        [Description("新增")]
        New = 0,
        [Description("审核通过")]
        Pass = 2,
        [Description("驳回")]
        Reject = 3
    }
}

