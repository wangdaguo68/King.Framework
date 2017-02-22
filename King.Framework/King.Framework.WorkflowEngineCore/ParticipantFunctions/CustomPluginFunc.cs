using King.Framework.Interfaces;

namespace King.Framework.WorkflowEngineCore.ParticipantFunctions
{
    using King.Framework.Common;
    using King.Framework.EntityLibrary;
    using King.Framework.Linq;
    using System;
    using System.Collections.Generic;

    internal class CustomPluginFunc : ParticiparntFuncBase
    {
        public CustomPluginFunc(DataContext ctx) : base(ctx)
        {
        }

        public override IList<IUser> GetUsers(SysProcessParticipant part, SysProcessInstance pi, SysActivityInstance ai)
        {
            string assemblyName = part.Param_AssemblyName;
            string className = part.Param_ClassName;
            return CustomHandlerLoader.GetHandlerWithName<IParticipantFunc>(assemblyName, className, new object[0]).GetUsers(part, pi, ai);
        }
    }
}

