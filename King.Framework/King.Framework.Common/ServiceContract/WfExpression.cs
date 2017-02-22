namespace King.Framework.Common.ServiceContract
{
    using System;
    using System.Runtime.CompilerServices;

    public class WfExpression
    {
        public long? ActivityId { get; set; }

        public string ConstValue { get; set; }

        public int DataType { get; set; }

        public string DisplayText { get; set; }

        public long ExpressionId { get; set; }

        public string ExpressionName { get; set; }

        public int ExpressionType { get; set; }

        public long? FieldId { get; set; }

        public long? LeftId { get; set; }

        public int OperationType { get; set; }

        public long ProcessId { get; set; }

        public long? RelationId { get; set; }

        public long? RightId { get; set; }

        public long? TransitionId { get; set; }
    }
}
