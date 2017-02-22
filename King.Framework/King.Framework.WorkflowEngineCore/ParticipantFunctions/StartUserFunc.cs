namespace King.Framework.WorkflowEngineCore.ParticipantFunctions
{
    using King.Framework.EntityLibrary;
    using King.Framework.Interfaces;
    using King.Framework.Linq;
    using System;
    using System.Collections.Generic;

    internal class StartUserFunc : ParticiparntFuncBase
    {
        public StartUserFunc(DataContext ctx) : base(ctx)
        {
        }

        public IUser GetStartUser(SysProcessInstance pi)
        {
            if (pi == null)
            {
                throw new ArgumentNullException("pi");
            }
            if (!pi.StartUserId.HasValue)
            {
                throw new ApplicationException("启动用户为空");
            }
            return base._orgProxy.GetUserById(pi.StartUserId.Value);
        }

        public override IList<IUser> GetUsers(SysProcessParticipant part, SysProcessInstance pi, SysActivityInstance ai)
        {
            IUser startUser = this.GetStartUser(pi);
            return new List<IUser> { startUser };
        }
    }
}

