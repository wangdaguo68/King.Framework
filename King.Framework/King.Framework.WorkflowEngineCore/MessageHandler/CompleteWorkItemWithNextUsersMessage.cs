namespace King.Framework.WorkflowEngineCore.MessageHandler
{
    using King.Framework.EntityLibrary;
    using King.Framework.WorkflowEngineCore.InternalCore;
    using System;
    using System.Collections.Generic;

    internal class CompleteWorkItemWithNextUsersMessage : CompleteWorkItemMessage
    {
        private List<IApproveUser> _approveUsers;

        public CompleteWorkItemWithNextUsersMessage(ProcessEngine engine, SysWorkItem wi, List<IApproveUser> nextApproveUsers) : base(engine, wi)
        {
            this._approveUsers = nextApproveUsers;
        }

        public override void Execute(Queue<WorkflowMessage> queue)
        {
            this.SaveNextApproveUsers();
            base.Execute(queue);
        }

        private void SaveNextApproveUsers()
        {
            base.PICacheFactory.AddNextApproveUsers(base.CurrentWorkItem, base.AI, this._approveUsers);
        }
    }
}

