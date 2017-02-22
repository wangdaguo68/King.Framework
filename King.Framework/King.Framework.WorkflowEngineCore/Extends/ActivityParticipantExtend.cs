namespace King.Framework.WorkflowEngineCore.Extends
{
    using King.Framework.EntityLibrary;
    using System;
    using System.Runtime.CompilerServices;

    internal static class ActivityParticipantExtend
    {
        public static ActivityPassType GetPassType(this SysActivityParticipant ap)
        {
            if (!ap.PassType.HasValue)
            {
                ap.PassType = 0;
            }
            else if (ap.PassType == 1)
            {
                if (!ap.MinPassNum.HasValue)
                {
                    ap.MinPassNum = 1;
                }
            }
            else
            {
                if (ap.PassType != 2)
                {
                    throw new ApplicationException("不正确的PassType");
                }
                if (!ap.MinPassRatio.HasValue)
                {
                    ap.MinPassRatio = 100.0;
                }
            }
            return (ActivityPassType)ap.PassType.Value;
        }
    }
}

