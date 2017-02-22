namespace King.Framework.WorkflowEngineCore.WorkflowMessages
{
    using King.Framework.EntityLibrary;
    using System;

    internal abstract class AsyncMessageHandler
    {
        protected AsyncMessageHandler()
        {
        }

        public abstract void ProcessMessage(SysWorkflowMessage msg);
    }
}

