namespace King.Framework.EntityLibrary
{
    using System;
    using System.ComponentModel;

    public enum SysServiceCallState
    {
        [Description("执行错误")]
        ErrorRun = 2,
        [Description("未执行")]
        NotRun = 1,
        [Description("执行成功")]
        SuccessRun = 3
    }
}

