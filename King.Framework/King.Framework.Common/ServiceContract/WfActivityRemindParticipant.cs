namespace King.Framework.Common.ServiceContract
{
    using System;
    using System.Runtime.CompilerServices;

    public class WfActivityRemindParticipant
    {
        public virtual long? ParticipantId { get; set; }

        public long ProcessId { get; set; }

        public virtual long? RemindId { get; set; }

        public virtual long RemindParticipantId { get; set; }

        public virtual long? TemplateId { get; set; }
    }
}
