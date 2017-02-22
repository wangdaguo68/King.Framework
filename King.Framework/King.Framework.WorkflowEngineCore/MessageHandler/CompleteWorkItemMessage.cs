namespace King.Framework.WorkflowEngineCore.MessageHandler
{
    using King.Framework.EntityLibrary;
    using King.Framework.WorkflowEngineCore.InternalCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class CompleteWorkItemMessage : WorkflowMessage
    {
        private SysActivityInstance _ai;
        private SysWorkItem _wi;

        public CompleteWorkItemMessage(ProcessEngine engine, SysWorkItem wi) : base(engine)
        {
            if (wi == null)
            {
                throw new ArgumentNullException("wi");
            }
            this._wi = wi;
            this._ai = wi.ActivityInstance;
        }

        public override void Execute(Queue<WorkflowMessage> queue)
        {
            if (!this.CurrentWorkItem.Status.HasValue)
            {
                this.CurrentWorkItem.Status = 0;
            }
            if (this.CurrentWorkItem.Status == 1)
            {
                throw new ApplicationException("工作项已经完成，不可重复完成");
            }
            if ((this.CurrentWorkItem.Status != 0) && (this.CurrentWorkItem.Status != 0x63))
            {
                throw new ApplicationException("该工作项已被取消");
            }
            this.CurrentWorkItem.Status = 1;
            this.CurrentWorkItem.EndTime = new DateTime?(DateTime.Now);
            base.PICacheFactory.UpdateWorkItem(this.CurrentWorkItem, base.PI, this.AI);
            foreach (SysWorkItem item in this.AI.WorkItems.ToList<SysWorkItem>())
            {
                if (!item.Status.HasValue)
                {
                    item.Status = 0;
                }
                if ((item.Status.Value == 0) && (item.WorkItemId != this.CurrentWorkItem.WorkItemId))
                {
                    item.Status = 3;
                    item.EndTime = new DateTime?(DateTime.Now);
                    base.PICacheFactory.UpdateWorkItem(item, base.PI, this.AI);
                }
            }
            WorkflowMessage message = base.Engine.NewCompleteActivityMessage(this.AI);
            queue.Enqueue(message);
        }

        internal SysActivityInstance AI
        {
            get
            {
                return this._ai;
            }
        }

        protected SysWorkItem CurrentWorkItem
        {
            get
            {
                return this._wi;
            }
        }
    }
}

