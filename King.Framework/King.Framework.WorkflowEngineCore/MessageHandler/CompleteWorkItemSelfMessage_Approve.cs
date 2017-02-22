namespace King.Framework.WorkflowEngineCore.MessageHandler
{
    using King.Framework.EntityLibrary;
    using King.Framework.WorkflowEngineCore.InternalCore;
    using System;
    using System.Collections.Generic;

    internal class CompleteWorkItemSelfMessage_Approve : CompleteWorkItemMessage_Approve
    {
        public CompleteWorkItemSelfMessage_Approve(ProcessEngine engine, SysWorkItem wi) : base(engine, wi)
        {
        }

        public override void Execute(Queue<WorkflowMessage> queue)
        {
            base.CheckStatus();
            base.JustCompleteCurrentWorkItem();
        }
    }
}

