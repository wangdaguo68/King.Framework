namespace King.Framework.Common.ServiceContract
{
    using System;
    using System.Runtime.CompilerServices;

    public class WfTransition
    {
        public int Direction { get; set; }

        public string DisplayText { get; set; }

        public long? ExpressionId { get; set; }

        public long PostActivityId { get; set; }

        public long PreActivityId { get; set; }

        public long ProcessId { get; set; }

        public long TransitionId { get; set; }

        public int? WwfTransitionId { get; set; }
    }
}
