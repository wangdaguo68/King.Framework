namespace King.Framework.WorkflowEngineCore.MessageHandler
{
    using King.Framework.EntityLibrary;
    using King.Framework.WorkflowEngineCore.InternalCore;
    using King.Framework.WorkflowEngineCore.Remind;
    using System;
    using System.Collections.Generic;

    internal class CancelProcessMessage : WorkflowMessage
    {
        public CancelProcessMessage(ProcessEngine engine, SysProcessInstance pi) : base(engine)
        {
        }

        private bool CanCancel()
        {
            if (base.PI.InstanceStatus == 10)
            {
                return false;
            }
            foreach (SysActivityInstance instance in base.PI.ActivityInstances)
            {
                SysActivity activity = instance.Activity;
                if (activity == null)
                {
                    throw new ApplicationException("活动为空");
                }
                if ((activity.ActivityType != 2) && (instance.InstanceStatus == 10))
                {
                    if (activity.ExecType == 1)
                    {
                        if (!string.IsNullOrEmpty(activity.JScriptText))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public override void Execute(Queue<WorkflowMessage> queue)
        {
            if (!this.CanCancel())
            {
                throw new ApplicationException("工作流已经执行了至少一个非决策活动，不能撤消");
            }
            base.Manager.ExecutePython(base.PI, base.PI.Process.CancelScript);
            base.PI.InstanceStatus = 20;
            base.PICacheFactory.UpdateProcessInstance(base.PI);
            SysActivityInstance ai = null;
            foreach (SysActivityInstance instance2 in base.PI.ActivityInstances)
            {
                if (instance2.InstanceStatus != 10)
                {
                    instance2.InstanceStatus = 20;
                    base.PICacheFactory.UpdateActiviyInstance(instance2);
                    ai = instance2;
                }
                ICollection<SysWorkItem> workItems = instance2.WorkItems;
                if (workItems != null)
                {
                    foreach (SysWorkItem item in workItems)
                    {
                        item.Status = 2;
                        base.PICacheFactory.UpdateWorkItem(item, base.PI, instance2);
                    }
                }
            }
            new ProcessRemindHandler(base.PICacheFactory, base.PI, ai, ProcessRemindUseTimeType.ProcessCancel).Execute();
        }
    }
}

