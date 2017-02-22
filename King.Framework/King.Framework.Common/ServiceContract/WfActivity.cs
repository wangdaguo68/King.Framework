namespace King.Framework.Common.ServiceContract
{
    using System;
    using System.Runtime.CompilerServices;

    public class WfActivity
    {
        public long ActivityId { get; set; }

        public string ActivityName { get; set; }

        public int ActivityType { get; set; }

        public int ApproveResult { get; set; }

        public DateTime? DeadLine { get; set; }

        public bool DisableParticipantGroup { get; set; }

        public int ExecType { get; set; }

        public long? ExpressionId { get; set; }

        public bool IsAllowAddApproveUser { get; set; }

        public bool IsPassedWithNoParticipants { get; set; }

        public string JScriptText { get; set; }

        public double LeftPos { get; set; }

        public int MinPassNum { get; set; }

        public double MinPassRatio { get; set; }

        public long? PageId { get; set; }

        public int PassType { get; set; }

        public long ProcessId { get; set; }

        public bool SumAfterAllComplete { get; set; }

        public int? TimeLimit { get; set; }

        public string TimeOutScript { get; set; }

        public double TopPos { get; set; }

        public string VbExpression { get; set; }

        public int? WwfActivityId { get; set; }
    }
}
