namespace King.Framework.Common.ServiceContract
{
    using System;
    using System.Runtime.CompilerServices;

    public class WfActivityParticipant
    {
        public long ActivityId { get; set; }

        public long ActivityParticipantId { get; set; }

        public bool IsMustApprove { get; set; }

        public int MinPassNum { get; set; }

        public double MinPassRatio { get; set; }

        public long ParticipantId { get; set; }

        public int PassType { get; set; }

        public long ProcessId { get; set; }

        public bool SumAfterAllComplete { get; set; }
    }
}
