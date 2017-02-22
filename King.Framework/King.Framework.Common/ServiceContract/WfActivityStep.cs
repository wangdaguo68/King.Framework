namespace King.Framework.Common.ServiceContract
{
    using System;
    using System.Runtime.CompilerServices;

    public class WfActivityStep
    {
        public string ConstValue { get; set; }

        public int ExpressionType { get; set; }

        public long OperationId { get; set; }

        public long ProcessId { get; set; }

        public long? SourceFieldId { get; set; }

        public long? SourceObjectId { get; set; }

        public long? SourceRelationId { get; set; }

        public long StepId { get; set; }

        public string StepName { get; set; }

        public long? TargetFieldId { get; set; }
    }
}
