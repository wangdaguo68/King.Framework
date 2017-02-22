using King.Framework.Interfaces;

namespace King.Framework.WorkflowEngineCore.ParticipantFunctions
{
    using King.Framework.EntityLibrary;
    using King.Framework.Linq;
    using King.Framework.WorkflowEngineCore.InternalCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class PythonFunc : ParticiparntFuncBase
    {
        private DynamicManager _manager;

        internal PythonFunc(DataContext ctx) : base(ctx)
        {
            this._manager = new DynamicManager(ctx);
        }

        public override IList<IUser> GetUsers(SysProcessParticipant part, SysProcessInstance pi, SysActivityInstance ai)
        {
            if (part == null)
            {
                throw new ArgumentNullException("part");
            }
            if (string.IsNullOrEmpty(part.JScriptText))
            {
                throw new ApplicationException("未指定参与人的Python脚本");
            }
            List<IUser> list = new List<IUser>();
            foreach (int num in this._manager.ExecutePython(pi, part.JScriptText).Distinct<int>())
            {
                list.Add(base._orgProxy.GetUserById(num));
            }
            return list;
        }
    }
}

