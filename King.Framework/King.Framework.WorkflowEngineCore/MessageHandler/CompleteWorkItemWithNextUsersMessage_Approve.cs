namespace King.Framework.WorkflowEngineCore.MessageHandler
{
    using King.Framework.EntityLibrary;
    using King.Framework.WorkflowEngineCore.InternalCore;
    using System;
    using System.Collections.Generic;

    internal class CompleteWorkItemWithNextUsersMessage_Approve : CompleteWorkItemWithNextUsersMessage
    {
        public CompleteWorkItemWithNextUsersMessage_Approve(ProcessEngine engine, SysWorkItem wi, List<IApproveUser> nextApproveUsers) : base(engine, wi, nextApproveUsers)
        {
        }

        public override void Execute(Queue<WorkflowMessage> queue)
        {
            SysWorkItemApproveGroup approveGroup = base.CurrentWorkItem.ApproveGroup;
            approveGroup.ApproveResult = 1;
            base.PICacheFactory.UpdateWorkItemGroup(approveGroup);
            base.AI.ApproveResult = 1;
            base.PICacheFactory.UpdateActiviyInstance(base.AI);
            base.Execute(queue);
        }
    }
}

