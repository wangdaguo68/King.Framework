namespace King.Framework.WorkflowEngineCore.MessageHandler
{
    using King.Framework.EntityLibrary;
    using King.Framework.WorkflowEngineCore.InternalCore;
    using King.Framework.WorkflowEngineCore.Remind;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class StartProcessWithNextUsersMessage : StartProcessMessage
    {
        private List<IApproveUser> _approveUsers;

        public StartProcessWithNextUsersMessage(ProcessEngine engine, List<IApproveUser> nextApproveUsers) : base(engine)
        {
            this._approveUsers = nextApproveUsers;
        }

        public override void Execute(Queue<WorkflowMessage> queue)
        {
            SysProcess process = base.PI.Process;
            if ((process.Activities == null) || (process.Activities.Count == 0))
            {
                throw new ApplicationException("没有任何活动");
            }
            SysActivity act = process.Activities.FirstOrDefault<SysActivity>(a => a.ActivityType == 1);
            if (act == null)
            {
                throw new ApplicationException("没有开始活动");
            }
            SysActivityInstance firstAi = base.CreateActivityInstance(base.PI, act);
            this.SaveNextApproveUsers(firstAi);
            new ProcessRemindHandler(base.PICacheFactory, base.PI, firstAi, ProcessRemindUseTimeType.ProcessStart).Execute();
            WorkflowMessage item = base.Engine.NewActivityInstanceCreatedMessage(firstAi);
            queue.Enqueue(item);
        }

        private void SaveNextApproveUsers(SysActivityInstance firstAi)
        {
            base.PICacheFactory.AddNextApproveUsers(firstAi, this._approveUsers);
        }
    }
}

