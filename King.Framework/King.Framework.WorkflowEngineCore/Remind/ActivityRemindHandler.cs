namespace King.Framework.WorkflowEngineCore.Remind
{
    using King.Framework.EntityLibrary;
    using King.Framework.WorkflowEngineCore.Cache;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;

    internal class ActivityRemindHandler : ProcessRemindHandlerBase
    {
        private List<SysActivityRemind> _remindList;

        public ActivityRemindHandler(ProcessInstanceCacheFactory cache, SysProcessInstance pi, SysActivityInstance ai, ActivityRemindUseTimeType useTime, SysWorkItem wi = null, int? result = new int?()) : base(cache, pi, ai, (UseTimeType) useTime, wi, result)
        {
        }

        public override void Execute()
        {
            foreach (SysActivityRemind remind in this.RemindList)
            {
                foreach (SysActivityRemindParticipant participant in remind.RemindParticipants)
                {
                    base.RemindForParticipant(remind.RemindType, participant.Participant, participant.RemindTemplate);
                }
            }
        }

        public override void InitRemindList()
        {
            Func<SysActivityRemind, bool> predicate = null;
            this._remindList = this.Activity.ActivityReminds.Where<SysActivityRemind>(delegate (SysActivityRemind p) {
                int? useTimeType = p.UseTimeType;
                int useTime = (int) this.UseTime;
                return ((useTimeType.GetValueOrDefault() == useTime) && useTimeType.HasValue);
            }).ToList<SysActivityRemind>();
            if ((this.Activity.ActivityType == 6) && ((this.UseTime == ActivityRemindUseTimeType.ActivityEnd) || (this.UseTime == ActivityRemindUseTimeType.WorkItemFinished)))
            {
                if (predicate == null)
                {
                    predicate = delegate (SysActivityRemind p) {
                        int? resultType = p.ResultType;
                        int num = (int) this.ResultType;
                        return (resultType.GetValueOrDefault() == num) && resultType.HasValue;
                    };
                }
                this._remindList = this._remindList.Where<SysActivityRemind>(predicate).ToList<SysActivityRemind>();
            }
            if (this._remindList.Count > 0)
            {
                base.CreateModel();
            }
        }

        protected SysActivity Activity
        {
            get
            {
                return base.creator.Activity;
            }
        }

        public List<SysActivityRemind> RemindList
        {
            get
            {
                return this._remindList;
            }
        }

        protected ProcessTemplateResultType ResultType
        {
            get
            {
                return base.creator.ResultType;
            }
        }

        public ActivityRemindUseTimeType UseTime
        {
            get
            {
                return (ActivityRemindUseTimeType) base.creator.UseTime;
            }
        }
    }
}

