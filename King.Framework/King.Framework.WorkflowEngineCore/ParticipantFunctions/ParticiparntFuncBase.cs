namespace King.Framework.WorkflowEngineCore.ParticipantFunctions
{
    using King.Framework.EntityLibrary;
    using King.Framework.Interfaces;
    using King.Framework.Linq;
    using King.Framework.OrgLibrary;
    using System;
    using System.Collections.Generic;

    public abstract class ParticiparntFuncBase : IParticipantFunc
    {
        protected DataContext _context;
        protected IOrgProxy _orgProxy;

        public ParticiparntFuncBase(DataContext ctx)
        {
            this._context = ctx;
            this._orgProxy = OrgProxyFactory.GetProxy(ctx);
        }

        public abstract IList<IUser> GetUsers(SysProcessParticipant part, SysProcessInstance pi, SysActivityInstance ai);
    }
}

