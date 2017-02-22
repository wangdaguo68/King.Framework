namespace King.Framework.Common.ServiceContract
{
    using System;
    using System.Runtime.CompilerServices;

    public class WfProcess
    {
        public long? ActivityEntityId { get; set; }

        public string ActivityEntityName { get; set; }

        public string CancelScript { get; set; }

        public bool CanDelete { get; set; }

        public long? EntityId { get; set; }

        public string EntityName { get; set; }

        public int ProcessCategory { get; set; }

        public string ProcessDescription { get; set; }

        public double ProcessHeight { get; set; }

        public long ProcessId { get; set; }

        public string ProcessName { get; set; }

        public int ProcessStatus { get; set; }

        public string ProcessTemplate { get; set; }

        public long ProcessType { get; set; }

        public string ProcessVersion { get; set; }

        public double ProcessWidth { get; set; }

        public byte[] TemplateId { get; set; }

        public int? WwfId { get; set; }
    }
}
