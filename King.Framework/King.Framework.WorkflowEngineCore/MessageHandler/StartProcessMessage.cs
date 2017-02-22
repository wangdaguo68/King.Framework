namespace King.Framework.WorkflowEngineCore.MessageHandler
{
    using King.Framework.EntityLibrary;
    using King.Framework.WorkflowEngineCore.InternalCore;
    using King.Framework.WorkflowEngineCore.Remind;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class StartProcessMessage : WorkflowMessage
    {
        public StartProcessMessage(ProcessEngine engine) : base(engine)
        {
        }

        protected SysActivityInstance CreateActivityInstance(SysProcessInstance pi, SysActivity act)
        {
            SysActivityInstance ai = new SysActivityInstance {
                Activity = act,
                ActivityId = new long?(act.ActivityId),
                ActivityInstanceId = base.Manager.GetNextIdentity(),
                EndTime = null,
                ExpressionValue = null,
                InstanceStatus = 0,
                ProcessInstance = pi,
                ProcessInstanceId = new int?(pi.ProcessInstanceId),
                StartTime = new DateTime?(DateTime.Now),
                WwfActivityInstanceId = null
            };
            base.PICacheFactory.AddActivityInstance(pi, ai);
            return ai;
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
            SysActivityInstance ai = this.CreateActivityInstance(base.PI, act);
            new ProcessRemindHandler(base.PICacheFactory, base.PI, ai, ProcessRemindUseTimeType.ProcessStart).Execute();
            WorkflowMessage item = base.Engine.NewActivityInstanceCreatedMessage(ai);
            queue.Enqueue(item);
        }
    }
}

