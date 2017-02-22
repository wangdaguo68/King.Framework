namespace King.Framework.WorkflowEngineCore.Extends
{
    using King.Framework.EntityLibrary;
    using System;
    using System.Runtime.CompilerServices;

    internal static class ActivityExtend
    {
        public static void SureMinPassNum(this SysActivity activity)
        {
            if (!activity.MinPassNum.HasValue)
            {
                activity.MinPassNum = 1;
            }
        }

        public static void SureMinPassRatio(this SysActivity activity)
        {
            if (!activity.MinPassRatio.HasValue)
            {
                activity.MinPassRatio = 100.0;
            }
        }

        public static void SurePassType(this SysActivity activity)
        {
            if (!activity.PassType.HasValue)
            {
                activity.PassType = 0;
            }
        }
    }
}

