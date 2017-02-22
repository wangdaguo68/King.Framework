namespace King.Framework.WorkflowEngineCore.MessageHandler
{
    using King.Framework.EntityLibrary;
    using King.Framework.WorkflowEngineCore.InternalCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class CompleteActivityWithNextActivityMessage : CompleteActivityMessage
    {
        private long _nextActivityId;

        public CompleteActivityWithNextActivityMessage(ProcessEngine engine, SysActivityInstance ai, long nextActivityId) : base(engine, ai)
        {
            this._nextActivityId = nextActivityId;
        }

        public override void Execute(Queue<WorkflowMessage> queue)
        {
            base.JustCompleteCurrentAI();
            SysActivity postActivity = base.PI.Process.Activities.FirstOrDefault<SysActivity>(p => p.ActivityId == this._nextActivityId);
            SysActivityInstance ai = base.CreatePostActivityInstance(base.PI, postActivity);
            WorkflowMessage item = base.Engine.NewActivityInstanceCreatedMessage(ai);
            queue.Enqueue(item);
        }
    }
}

