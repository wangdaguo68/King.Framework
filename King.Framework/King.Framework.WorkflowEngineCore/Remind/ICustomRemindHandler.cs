namespace King.Framework.WorkflowEngineCore.Remind
{
    using King.Framework.Common;
    using King.Framework.EntityLibrary;
    using King.Framework.Interfaces;
    using System;

    public interface ICustomRemindHandler
    {
        void Execute(IUser receiveUser, string title, string content, SysProcessInstance pi, SysActivityInstance ai, SysWorkItem wi, RemindTemplateModel model);
    }
}

