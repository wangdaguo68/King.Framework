namespace King.Framework.EntityLibrary
{
    using System;
    using System.ComponentModel;

    [Serializable]
    public enum Entity_Trace_Plan_ExecuteTypeEnum
    {
        [Description("调用服务引擎")]
        ServiceEngine = 1,
        [Description("生成工作项")]
        WorkItem = 0
    }
}

