using King.Framework.Interfaces;

namespace King.Framework.WorkflowEngineCore.ParticipantFunctions
{
    using King.Framework.EntityLibrary;
    using King.Framework.Linq;
    using System;
    using System.Collections.Generic;

    internal class SpecRoleFunc : ParticiparntFuncBase
    {
        public SpecRoleFunc(DataContext ctx) : base(ctx)
        {
        }

        public override IList<IUser> GetUsers(SysProcessParticipant part, SysProcessInstance pi, SysActivityInstance ai)
        {
            if (part == null)
            {
                throw new ArgumentNullException("part");
            }
            if (!part.Param_RoleId.HasValue)
            {
                throw new ApplicationException("具体角色参与人未指定角色参数");
            }
            return base._orgProxy.GetRoleUsers(part.Param_RoleId.Value);
        }
    }
}

