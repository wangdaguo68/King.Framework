using King.Framework.Interfaces;

namespace King.Framework.WorkflowEngineCore.ParticipantFunctions
{
    using King.Framework.EntityLibrary;
    using King.Framework.Linq;
    using System;
    using System.Collections.Generic;

    internal class SpecUserFunc : ParticiparntFuncBase
    {
        public SpecUserFunc(DataContext ctx) : base(ctx)
        {
        }

        public override IList<IUser> GetUsers(SysProcessParticipant part, SysProcessInstance pi, SysActivityInstance ai)
        {
            if (part == null)
            {
                throw new ArgumentNullException("part");
            }
            if (!part.Param_UserId.HasValue)
            {
                throw new ApplicationException("未指定参与人的用户参数");
            }
            return new List<IUser> { base._orgProxy.GetUserById(part.Param_UserId.Value) };
        }
    }
}

