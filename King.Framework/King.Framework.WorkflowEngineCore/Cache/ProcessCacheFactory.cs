using System.Data.Common;

namespace King.Framework.WorkflowEngineCore.Cache
{
    using King.Framework.EntityLibrary;
    using King.Framework.Linq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;

    public class ProcessCacheFactory
    {
        private King.Framework.WorkflowEngineCore.Cache.ProcessCacheHelper _cacheHelper;
        private readonly DataContext _context;
        private static readonly Dictionary<long, SysEntity> _entityCacheDict = new Dictionary<long, SysEntity>();
        private static readonly Dictionary<long, SysField> _fieldCacheDict = new Dictionary<long, SysField>();
        private static readonly Dictionary<long, SysProcess> _processCacheDict = new Dictionary<long, SysProcess>();
        private static readonly Dictionary<long, SysOneMoreRelation> _relationCacheDict = new Dictionary<long, SysOneMoreRelation>();
        private static readonly object meta_locker = new object();

        public ProcessCacheFactory(DataContext ctx)
        {
            this._context = ctx;
            this._cacheHelper = new King.Framework.WorkflowEngineCore.Cache.ProcessCacheHelper(ctx, this);
            this.LoadEntityCache();
        }

        public void ClearCache(long processId)
        {
            lock (_processCacheDict)
            {
                if (_processCacheDict.ContainsKey(processId))
                {
                    _processCacheDict.Remove(processId);
                }
            }
        }

        public SysEntity GetEntityCache(long entityId)
        {
            SysEntity entity;
            lock (meta_locker)
            {
                if (!_entityCacheDict.TryGetValue(entityId, out entity))
                {
                    entity = null;
                }
            }
            return entity;
        }

        public SysField GetFieldCache(long fieldId)
        {
            SysField field;
            lock (meta_locker)
            {
                if (!_fieldCacheDict.TryGetValue(fieldId, out field))
                {
                    field = null;
                }
            }
            return field;
        }

        public SysProcess GetProcessCache(long processId)
        {
            SysProcess process;
            lock (_processCacheDict)
            {
                if (!_processCacheDict.TryGetValue(processId, out process))
                {
                    process = this._cacheHelper.LoadProcess(processId);
                    if (process != null)
                    {
                        _processCacheDict[processId] = process;
                    }
                }
            }
            return process;
        }

        public SysProcess GetProcessCache(string processName)
        {
            SysProcess startUsedProcessByName = this.GetStartUsedProcessByName(processName);
            return this.GetProcessCache(startUsedProcessByName.ProcessId);
        }

        public SysProcess GetProcessCacheByActivity(long activityId, out SysActivity activity)
        {
            Func<SysActivity, bool> predicate = null;
            SysActivity activity2 = this._context.FindById<SysActivity>(new object[] { activityId });
            if ((activity2 == null) || !activity2.ProcessId.HasValue)
            {
                throw new ApplicationException("活动ID不正确");
            }
            SysProcess processCache = this.GetProcessCache(activity2.ProcessId.Value);
            if (predicate == null)
            {
                predicate = p => p.ActivityId == activityId;
            }
            activity = processCache.Activities.FirstOrDefault<SysActivity>(predicate);
            return processCache;
        }

        public SysOneMoreRelation GetRelationCache(long relationId)
        {
            SysOneMoreRelation relation;
            lock (meta_locker)
            {
                if (!_relationCacheDict.TryGetValue(relationId, out relation))
                {
                    relation = null;
                }
            }
            return relation;
        }

        private SysProcess GetStartUsedProcessByName(string processName)
        {
            SysProcess process = this._context.FirstOrDefault<SysProcess>(i => (i.ProcessName == processName) && (i.ProcessStatus == 1));
            if (process == null)
            {
                throw new ApplicationException(string.Format("找不到名称为[{0}]的已启用流程", processName));
            }
            return process;
        }

        private void LoadActivity(SysActivity a, SysProcess p)
        {
            string condition = string.Format("activityId = {0}", a.ActivityId);
            a.Process = p;
            a.Expression = p.Expressions.FirstOrDefault<SysExpression>(i => i.ExpressionId == a.ExpressionId);
            a.ActivityParticipants = this._context.Where<SysActivityParticipant>(condition, new DbParameter[0]);
            a.ActivityReminds = this._context.Where<SysActivityRemind>(condition, new DbParameter[0]);
            a.ActivityOperations = this._context.Where<SysActivityOperation>(condition, new DbParameter[0]);
            a.Expressions = p.Expressions.Where<SysExpression>(delegate (SysExpression i) {
                long? activityId = i.ActivityId;
                long num = a.ActivityId;
                return ((activityId.GetValueOrDefault() == num) && activityId.HasValue);
            }).ToList<SysExpression>();
            a.FromTransitions = p.Transitions.Where<SysTransition>(delegate (SysTransition i) {
                long? preActivityId = i.PreActivityId;
                long activityId = a.ActivityId;
                return ((preActivityId.GetValueOrDefault() == activityId) && preActivityId.HasValue);
            }).ToList<SysTransition>();
            a.ToTransitions = p.Transitions.Where<SysTransition>(delegate (SysTransition i) {
                long? postActivityId = i.PostActivityId;
                long activityId = a.ActivityId;
                return ((postActivityId.GetValueOrDefault() == activityId) && postActivityId.HasValue);
            }).ToList<SysTransition>();
            foreach (SysActivityParticipant participant in a.ActivityParticipants)
            {
                this.LoadActivityParticipant(participant, a, p);
            }
            foreach (SysActivityRemind remind in a.ActivityReminds)
            {
                this.LoadActivityRemind(remind, a, p);
            }
            foreach (SysActivityOperation operation in a.ActivityOperations)
            {
                this.LoadActivityOperation(operation, a);
            }
        }

        private void LoadActivityOperation(SysActivityOperation ao, SysActivity a)
        {
            ao.Activity = a;
            string condition = string.Format("OperationId = {0}", ao.OperationId);
            ao.ActivitySteps = this._context.Where<SysActivityStep>(condition, new DbParameter[0]);
            foreach (SysActivityStep step in ao.ActivitySteps)
            {
                step.ActivityOperation = ao;
            }
        }

        private void LoadActivityParticipant(SysActivityParticipant ap, SysActivity a, SysProcess p)
        {
            ap.Activity = a;
            ap.ActivityApproveGroup = this._context.FindById<SysActivityApproveGroup>(new object[] { ap.ActivityApproveGroupId });
            if (ap.ActivityApproveGroup != null)
            {
                string condition = string.Format("ActivityApproveGroupId = {0}", ap.ActivityApproveGroup.ActivityApproveGroupId);
                ap.ActivityApproveGroup.Activity = a;
                ap.ActivityApproveGroup.ActivityParticipants = this._context.Where<SysActivityParticipant>(condition, new DbParameter[0]);
            }
            ap.ProcessParticipant = p.ProcessParticipants.FirstOrDefault<SysProcessParticipant>(i => i.ParticipantId == ap.ParticipantId);
        }

        private void LoadActivityRemind(SysActivityRemind ar, SysActivity a, SysProcess p)
        {
            ar.Activity = a;
            string condition = string.Format("RemindId = {0}", ar.RemindId);
            ar.RemindParticipants = this._context.Where<SysActivityRemindParticipant>(condition, new DbParameter[0]);
            using (IEnumerator<SysActivityRemindParticipant> enumerator = ar.RemindParticipants.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    Func<SysProcessParticipant, bool> predicate = null;
                    SysActivityRemindParticipant arp = enumerator.Current;
                    arp.Remind = ar;
                    if (predicate == null)
                    {
                        predicate = i => i.ParticipantId == arp.ParticipantId;
                    }
                    arp.Participant = p.ProcessParticipants.FirstOrDefault<SysProcessParticipant>(predicate);
                    arp.RemindTemplate = this._context.FindById<SysProcessRemindTemplate>(new object[] { arp.TemplateId });
                }
            }
        }

        private void LoadEntityCache()
        {
            lock (meta_locker)
            {
                if (_entityCacheDict.Count == 0)
                {
                    EntityCacheBase base2 = EntityCacheBase.LoadCache();
                    List<SysEntity> sysEntity = base2.SysEntity;
                    List<SysField> sysField = base2.SysField;
                    List<SysOneMoreRelation> sysOneMoreRelation = base2.SysOneMoreRelation;
                    foreach (SysField field in sysField)
                    {
                        _fieldCacheDict[field.FieldId] = field;
                    }
                    foreach (SysEntity entity in sysEntity)
                    {
                        _entityCacheDict[entity.EntityId] = entity;
                    }
                    foreach (SysOneMoreRelation relation in sysOneMoreRelation)
                    {
                        _relationCacheDict[relation.RelationId] = relation;
                    }
                }
            }
        }

        private void LoadExpression(SysExpression e, SysProcess p)
        {
            e.Process = p;
            e.Activity = p.Activities.FirstOrDefault<SysActivity>(i => i.ActivityId == e.ActivityId);
            if (e.FieldId.HasValue)
            {
                e.Field = this.GetFieldCache(e.FieldId.Value);
            }
            if (e.RelationId.HasValue)
            {
                e.Relation = this.GetRelationCache(e.RelationId.Value);
            }
        }

        private SysProcess LoadProcess(long processId)
        {
            SysProcess p = this._context.FindById<SysProcess>(new object[] { processId });
            if (p == null)
            {
                throw new ApplicationException("流程ID不正确");
            }
            if (p.EntityId.HasValue)
            {
                p.ProcessEntity = this.GetEntityCache(p.EntityId.Value);
            }
            if (p.ActivityEntityId.HasValue)
            {
                p.ActivityEntity = this.GetEntityCache(p.ActivityEntityId.Value);
            }
            if (p.FormId.HasValue)
            {
                p.Form = this._context.FindById<SysForm>(new object[] { p.FormId });
            }
            string condition = string.Format("processId = {0}", processId);
            p.Activities = this._context.Where<SysActivity>(condition, new DbParameter[0]);
            p.ProcessProxies = this._context.Where<SysProcessProxy>(condition, new DbParameter[0]);
            p.ProcessParticipants = this._context.Where<SysProcessParticipant>(condition, new DbParameter[0]);
            p.ProcessReminds = this._context.Where<SysProcessRemind>(condition, new DbParameter[0]);
            p.Expressions = this._context.Where<SysExpression>(condition, new DbParameter[0]);
            p.Transitions = this._context.Where<SysTransition>(condition, new DbParameter[0]);
            foreach (SysActivity activity in p.Activities)
            {
                this.LoadActivity(activity, p);
            }
            foreach (SysProcessProxy proxy in p.ProcessProxies)
            {
                proxy.Process = p;
            }
            foreach (SysProcessParticipant participant in p.ProcessParticipants)
            {
                this.LoadProcessParticipant(participant, p);
            }
            foreach (SysProcessRemind remind in p.ProcessReminds)
            {
                this.LoadProcessRemind(remind, p);
            }
            foreach (SysExpression expression in p.Expressions)
            {
                this.LoadExpression(expression, p);
            }
            foreach (SysTransition transition in p.Transitions)
            {
                LoadTransition(transition, p);
            }
            return p;
        }

        private void LoadProcessParticipant(SysProcessParticipant pp, SysProcess p)
        {
            pp.Process = p;
            pp.Param_Participant = p.ProcessParticipants.FirstOrDefault<SysProcessParticipant>(i => i.ParticipantId == pp.Param_ParticipantId);
            string condition = string.Format("participantId = {0}", pp.ParticipantId);
            pp.ActivityParticipants = this._context.Where<SysActivityParticipant>(condition, new DbParameter[0]);
        }

        private void LoadProcessRemind(SysProcessRemind pr, SysProcess p)
        {
            pr.Process = p;
            string condition = string.Format("RemindId = {0}", pr.RemindId);
            pr.RemindParticipants = this._context.Where<SysProcessRemindParticipant>(condition, new DbParameter[0]);
            using (IEnumerator<SysProcessRemindParticipant> enumerator = pr.RemindParticipants.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    Func<SysProcessParticipant, bool> predicate = null;
                    SysProcessRemindParticipant rp = enumerator.Current;
                    rp.Remind = pr;
                    if (predicate == null)
                    {
                        predicate = i => i.ParticipantId == rp.ParticipantId;
                    }
                    rp.Participant = p.ProcessParticipants.FirstOrDefault<SysProcessParticipant>(predicate);
                    rp.RemindTemplate = this._context.FindById<SysProcessRemindTemplate>(new object[] { rp.TemplateId });
                }
            }
        }

        private static void LoadTransition(SysTransition t, SysProcess p)
        {
            t.Process = p;
            t.PreActivity = p.Activities.FirstOrDefault<SysActivity>(i => i.ActivityId == t.PreActivityId);
            t.PostActivity = p.Activities.FirstOrDefault<SysActivity>(i => i.ActivityId == t.PostActivityId);
            t.Expression = p.Expressions.FirstOrDefault<SysExpression>(i => i.ExpressionId == t.ExpressionId);
            t.Expressions = p.Expressions.Where<SysExpression>(delegate (SysExpression i) {
                long? transitionId = i.TransitionId;
                long num = t.TransitionId;
                return ((transitionId.GetValueOrDefault() == num) && transitionId.HasValue);
            }).ToList<SysExpression>();
        }

        internal King.Framework.WorkflowEngineCore.Cache.ProcessCacheHelper ProcessCacheHelper
        {
            get
            {
                return this._cacheHelper;
            }
        }
    }
}

