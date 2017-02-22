namespace King.Framework.Common.ServiceContract
{
    using System;
    using System.Runtime.CompilerServices;

    public class WfActivityRemind
    {
        public long ActivityId { get; set; }

        public long ProcessId { get; set; }

        public long RemindId { get; set; }

        public string RemindName { get; set; }

        public int RemindType { get; set; }

        public int ResultType { get; set; }

        public int UseTimeType { get; set; }
    }
}
