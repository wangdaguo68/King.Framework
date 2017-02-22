namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;

    [KingTable]
    public class SysActivityRemindParticipant
    {
        [XmlIgnore]
        public virtual SysProcessParticipant Participant { get; set; }

        [KingColumn]
        public virtual long? ParticipantId { get; set; }

        [KingColumn]
        public virtual long? ProcessId { get; set; }

        [XmlIgnore]
        public virtual SysActivityRemind Remind { get; set; }

        [KingColumn]
        public virtual long? RemindId { get; set; }

        [KingColumn(IsPrimaryKey=true)]
        public virtual long RemindParticipantId { get; set; }

        [XmlIgnore]
        public virtual SysProcessRemindTemplate RemindTemplate { get; set; }

        [KingColumn]
        public virtual long? TemplateId { get; set; }
    }
}

