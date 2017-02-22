namespace King.Framework.Common.ServiceContract
{
    using System;
    using System.Runtime.CompilerServices;

    public class WfProcessParticipant
    {
        public int FunctionType { get; set; }

        public string JScriptText { get; set; }

        public string Param_AssemblyName { get; set; }

        public string Param_ClassName { get; set; }

        public string Param_Constant { get; set; }

        public int? Param_DepartmentId { get; set; }

        public long? Param_ParticipantId { get; set; }

        public int? Param_RoleId { get; set; }

        public int? Param_UserId { get; set; }

        public long ParticipantId { get; set; }

        public string ParticipantName { get; set; }

        public int ParticipantType { get; set; }

        public long ProcessId { get; set; }
    }
}
