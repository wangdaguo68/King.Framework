namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;

    [KingTable]
    public class SysProcessParticipant
    {
        [XmlIgnore]
        public virtual ICollection<SysActivityParticipant> ActivityParticipants { get; set; }

        [KingColumn]
        public virtual int? FunctionType { get; set; }

        [KingColumn(MaxLength=-1)]
        public string JScriptText { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string Param_AssemblyName { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string Param_ClassName { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string Param_Constant { get; set; }

        [KingColumn]
        public virtual int? Param_DepartmentId { get; set; }

        [XmlIgnore]
        public virtual SysProcessParticipant Param_Participant { get; set; }

        [KingColumn]
        public virtual long? Param_ParticipantId { get; set; }

        [KingColumn]
        public virtual int? Param_RoleId { get; set; }

        [KingColumn]
        public virtual int? Param_UserId { get; set; }

        [KingColumn(IsPrimaryKey=true)]
        public virtual long ParticipantId { get; set; }

        [KingColumn(MaxLength=200)]
        public string ParticipantName { get; set; }

        [KingColumn]
        public virtual int? ParticipantType { get; set; }

        [XmlIgnore]
        public virtual SysProcess Process { get; set; }

        [KingColumn]
        public virtual long? ProcessId { get; set; }
    }
}

