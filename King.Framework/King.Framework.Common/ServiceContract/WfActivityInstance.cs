namespace King.Framework.Common.ServiceContract
{
    using System;
    using System.Runtime.CompilerServices;

    public class WfActivityInstance
    {
        public WfActivityEntityInstance ActivityEntityInstance { get; set; }

        public long? ActivityId { get; set; }

        public int ActivityInstanceId { get; set; }

        public DateTime? EndTime { get; set; }

        public int? ExpressionValue { get; set; }

        public int? InstanceStatus { get; set; }

        public DateTime? StartTime { get; set; }

        public int? WwfActivityInstanceId { get; set; }
    }
}
