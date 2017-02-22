using King.Framework.Interfaces;

namespace King.Framework.WorkflowEngineCore.ParticipantFunctions
{
    using King.Framework.EntityLibrary;
    using System.Collections.Generic;

    internal interface IParticipantFunc
    {
        IList<IUser> GetUsers(SysProcessParticipant part, SysProcessInstance pi, SysActivityInstance ai);
    }
}

