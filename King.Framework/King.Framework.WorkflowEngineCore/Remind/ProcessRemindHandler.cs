namespace King.Framework.WorkflowEngineCore.Remind
{
    using King.Framework.EntityLibrary;
    using King.Framework.WorkflowEngineCore.Cache;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class ProcessRemindHandler : ProcessRemindHandlerBase
    {
        private List<SysProcessRemind> _remindList;

        public ProcessRemindHandler(ProcessInstanceCacheFactory cache, SysProcessInstance pi, SysActivityInstance ai, ProcessRemindUseTimeType useTime) : base(cache, pi, ai, (UseTimeType) useTime, null, null)
        {
        }

        public override void Execute()
        {
            foreach (SysProcessRemind remind in this.RemindList)
            {
                foreach (SysProcessRemindParticipant participant in remind.RemindParticipants)
                {
                    base.RemindForParticipant(remind.RemindType, participant.Participant, participant.RemindTemplate);
                }
            }
        }

        public override void InitRemindList()
        {
            this._remindList = this.Process.ProcessReminds.Where<SysProcessRemind>(delegate (SysProcessRemind p) {
                int? useTimeType = p.UseTimeType;
                int useTime = (int) this.UseTime;
                return ((useTimeType.GetValueOrDefault() == useTime) && useTimeType.HasValue);
            }).ToList<SysProcessRemind>();
            if (this._remindList.Count > 0)
            {
                base.CreateModel();
            }
        }

        public SysProcess Process
        {
            get
            {
                return base.creator.PI.Process;
            }
        }

        public List<SysProcessRemind> RemindList
        {
            get
            {
                return this._remindList;
            }
        }

        public ProcessRemindUseTimeType UseTime
        {
            get
            {
                return (ProcessRemindUseTimeType) base.creator.UseTime;
            }
        }
    }
}

