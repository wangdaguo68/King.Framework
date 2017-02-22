namespace King.Framework.WorkflowEngineCore.ParticipantFunctions
{
    using King.Framework.EntityLibrary;
    using King.Framework.Interfaces;
    using King.Framework.Linq;
    using System;
    using System.Collections.Generic;

    internal class CurrentWorkItemOwnerFunc : ParticiparntFuncBase
    {
        private int? _wiOwnerId;

        public CurrentWorkItemOwnerFunc(DataContext ctx, int? wiOwnerId) : base(ctx)
        {
            this._wiOwnerId = wiOwnerId;
        }

        public override IList<IUser> GetUsers(SysProcessParticipant part, SysProcessInstance pi, SysActivityInstance ai)
        {
            if (!this._wiOwnerId.HasValue)
            {
                throw new ApplicationException("计划工作项负责人参与人失败");
            }
            IUser userById = base._orgProxy.GetUserById(this._wiOwnerId.Value);
            return new List<IUser> { userById };
        }
    }
}

