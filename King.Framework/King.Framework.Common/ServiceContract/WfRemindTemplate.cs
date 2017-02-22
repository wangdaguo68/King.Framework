namespace King.Framework.Common.ServiceContract
{
    using System;
    using System.Runtime.CompilerServices;

    public class WfRemindTemplate
    {
        public long? ActivityEntityId { get; set; }

        public long? ProcessEntityId { get; set; }

        public int? ResultType { get; set; }

        public long TemplateId { get; set; }

        public string TemplateName { get; set; }

        public int? TemplateType { get; set; }

        public int? UseTimeType { get; set; }
    }
}
