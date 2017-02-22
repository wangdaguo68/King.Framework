namespace King.Framework.WorkflowEngineCore.Extends
{
    using King.Framework.EntityLibrary;
    using System;
    using System.Runtime.CompilerServices;

    internal static class WorkItemExtend
    {
        public static void AssertHasGroup(this SysWorkItem wi)
        {
            if (!wi.ApproveGroupId.HasValue)
            {
                throw new ApplicationException("工作项没有Group");
            }
        }

        public static void SureStatus(this SysWorkItem wi)
        {
            if (!wi.Status.HasValue)
            {
                wi.Status = 0;
            }
        }
    }
}

