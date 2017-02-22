namespace King.Framework.WorkflowEngineCore.WorkflowMessages
{
    using King.Framework.EntityLibrary;
    using King.Framework.WorkflowEngineCore.InternalCore;
    using King.Framework.WorkflowEngineCore.MessageHandler;
    using System;
    using System.Collections.Generic;

    internal class AsyncMessageHandler4CompletingWorkItem : AsyncMessageHandler
    {
        private readonly ProcessEngine _engine;

        public AsyncMessageHandler4CompletingWorkItem(ProcessEngine engine)
        {
            this._engine = engine;
        }

        public override void ProcessMessage(SysWorkflowMessage msg)
        {
            SysWorkItem wI = this._engine.WI;
            SysProcessInstance pI = this._engine.PI;
            if (!wI.Status.HasValue)
            {
                wI.Status = 0;
            }
            if (wI.Status.Value != 0x63)
            {
                throw new ApplicationException("该工作项的状态已变更");
            }
            Queue<WorkflowMessage> queue = new Queue<WorkflowMessage>(10);
            if (msg.NextApproveUserList.Count > 0)
            {
                WorkflowMessage item = this._engine.NewCompleteWorkItemWithNextUsersMessage(wI, msg.NextApproveUserList);
                queue.Enqueue(item);
            }
            else
            {
                WorkflowMessage message2 = this._engine.NewCompleteWorkItemMessage(wI);
                queue.Enqueue(message2);
            }
            while (queue.Count > 0)
            {
                queue.Dequeue().Execute(queue);
            }
            this._engine.ProcessInstanceCache.ClearCache(pI.ProcessInstanceId);
        }
    }
}

