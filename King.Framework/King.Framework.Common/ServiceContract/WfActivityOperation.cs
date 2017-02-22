namespace King.Framework.Common.ServiceContract
{
    using System;
    using System.Runtime.CompilerServices;

    public class WfActivityOperation
    {
        public long ActivityId { get; set; }

        public long? EntityId { get; set; }

        public string JScriptText { get; set; }

        public long? ObjectId { get; set; }

        public long OperationId { get; set; }

        public string OperationName { get; set; }

        public int OperationType { get; set; }

        public long ProcessId { get; set; }
    }
}
