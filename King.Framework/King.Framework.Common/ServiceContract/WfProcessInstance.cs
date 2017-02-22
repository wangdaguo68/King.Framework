namespace King.Framework.Common.ServiceContract
{
    using System;
    using System.Runtime.CompilerServices;

    public class WfProcessInstance
    {
        public DateTime? EndTime { get; set; }

        public int? InstanceStatus { get; set; }

        public int? ObjectId { get; set; }

        public long? ProcessId { get; set; }

        public int ProcessInstanceId { get; set; }

        public WfDepartment StartDept { get; set; }

        public DateTime? StartTime { get; set; }

        public WfUser StartUser { get; set; }

        public int? WwfInstanceId { get; set; }
    }
}
