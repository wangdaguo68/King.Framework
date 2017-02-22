namespace King.Framework.WorkflowEngineCore.MessageHandler
{
    using King.Framework.EntityLibrary;
    using King.Framework.WorkflowEngineCore.InternalCore;
    using System;
    using System.Collections.Generic;

    internal class RejectWorkItemWithNextActivityMessage_Approve : CompleteWorkItemMessage_Approve
    {
        private long _nextActivityId;

        public RejectWorkItemWithNextActivityMessage_Approve(ProcessEngine engine, SysWorkItem wi, long nextActivityId) : base(engine, wi)
        {
            this._nextActivityId = nextActivityId;
        }

        public override void Execute(Queue<WorkflowMessage> queue)
        {
            base.CheckStatus();
            base.JustCompleteCurrentWorkItem();
            SysWorkItemApproveGroup approveGroup = base.CurrentWorkItem.ApproveGroup;
            approveGroup.ApproveResult = 0;
            base.PICacheFactory.UpdateWorkItemGroup(approveGroup);
            base.CancelUncompletedWorkItems(approveGroup);
            base.AI.ApproveResult = 0;
            base.PICacheFactory.UpdateActiviyInstance(base.AI);
            base.CancelUncompletedWorkItems(base.AI);
            WorkflowMessage item = base.Engine.NewCompleteActivityWithNextActivityMessage(base.AI, this._nextActivityId);
            queue.Enqueue(item);
        }
    }
}

