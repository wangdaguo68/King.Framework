namespace King.Framework.WorkflowEngineCore.MessageHandler
{
    using King.Framework.EntityLibrary;
    using King.Framework.WorkflowEngineCore.InternalCore;
    using System;
    using System.Collections.Generic;

    internal class CompleteWorkItemSelfMessage : CompleteWorkItemMessage
    {
        public CompleteWorkItemSelfMessage(ProcessEngine engine, SysWorkItem wi) : base(engine, wi)
        {
        }

        public override void Execute(Queue<WorkflowMessage> queue)
        {
            if (!base.CurrentWorkItem.Status.HasValue)
            {
                base.CurrentWorkItem.Status = 0;
            }
            if (base.CurrentWorkItem.Status == 1)
            {
                throw new ApplicationException("工作项已经完成，不可重复完成");
            }
            if (base.CurrentWorkItem.Status != 0)
            {
                throw new ApplicationException("该工作项已被取消");
            }
            base.CurrentWorkItem.Status = 1;
            base.CurrentWorkItem.EndTime = new DateTime?(DateTime.Now);
            base.PICacheFactory.UpdateWorkItem(base.CurrentWorkItem, base.PI, base.AI);
        }
    }
}

