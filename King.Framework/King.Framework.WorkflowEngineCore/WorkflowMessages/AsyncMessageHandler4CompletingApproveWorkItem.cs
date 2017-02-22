namespace King.Framework.WorkflowEngineCore.WorkflowMessages
{
    using King.Framework.EntityLibrary;
    using King.Framework.WorkflowEngineCore.InternalCore;
    using King.Framework.WorkflowEngineCore.MessageHandler;
    using System;
    using System.Collections.Generic;

    internal class AsyncMessageHandler4CompletingApproveWorkItem : AsyncMessageHandler
    {
        private readonly ProcessEngine _engine;

        public AsyncMessageHandler4CompletingApproveWorkItem(ProcessEngine engine)
        {
            this._engine = engine;
        }

        public override void ProcessMessage(SysWorkflowMessage msg)
        {
            if (msg.AddUser && (!msg.AddUserId.HasValue || (msg.AddUserId <= 0)))
            {
                throw new ApplicationException("指定了加签，但未指定加签用户");
            }
            SysWorkItem wI = this._engine.WI;
            SysProcessInstance pI = this._engine.PI;
            if (!wI.Status.HasValue)
            {
                wI.Status = 0;
            }
            if (wI.Status.Value == 1)
            {
                throw new ApplicationException("该工作项已完成");
            }
            if (wI.Status.Value == 3)
            {
                throw new ApplicationException("该工作项已被系统取消");
            }
            if (wI.Status.Value != 0x63)
            {
                throw new ApplicationException("该工作项已完成或被取消");
            }
            this._engine.AddInstanceData(wI, msg.ApproveResult, msg.ApproveComment);
            if (msg.AddUser && (msg.ApproveResult == ApproveResultEnum.Pass))
            {
                this._engine.CreateAddingWorkItem(wI, msg.AddUserId.Value);
            }
            Queue<WorkflowMessage> queue = new Queue<WorkflowMessage>(10);
            if (msg.NextApproveUserList.Count > 0)
            {
                WorkflowMessage item = this._engine.NewCompleteWorkItemWithNextUsersApproveMessage(wI, msg.NextApproveUserList);
                queue.Enqueue(item);
            }
            else
            {
                WorkflowMessage message2 = this._engine.NewCompleteWorkItemApproveMessage(wI);
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

