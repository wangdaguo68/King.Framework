using System.Data.Common;

namespace King.Framework.WorkflowEngineCore.Cache
{
    using King.Framework.EntityLibrary;
    using King.Framework.Linq;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class ProcessCacheHelper
    {
        private ProcessCacheFactory _cacheFactory;
        private readonly DataContext _context;

        internal ProcessCacheHelper(DataContext ctx, ProcessCacheFactory _cache)
        {
            this._context = ctx;
            this._cacheFactory = _cache;
        }

        internal SysProcess LoadProcess(long processId)
        {
            SysProcess process = this._context.FindById<SysProcess>(new object[] { processId });
            if (process == null)
            {
                throw new ApplicationException("流程ID不正确");
            }
            if (process.EntityId.HasValue)
            {
                process.ProcessEntity = this._cacheFactory.GetEntityCache(process.EntityId.Value);
            }
            if (process.ActivityEntityId.HasValue)
            {
                process.ActivityEntity = this._cacheFactory.GetEntityCache(process.ActivityEntityId.Value);
            }
            if (process.FormId.HasValue)
            {
                process.Form = this._context.FindById<SysForm>(new object[] { process.FormId });
            }
            string condition = string.Format("processId = {0}", processId);
            List<SysProcessProxy> list = this._context.Where<SysProcessProxy>(condition, new DbParameter[0]);
            List<SysProcessParticipant> list2 = this._context.Where<SysProcessParticipant>(condition, new DbParameter[0]);
            List<SysExpression> list3 = this._context.Where<SysExpression>(condition, new DbParameter[0]);
            List<SysTransition> list4 = this._context.Where<SysTransition>(condition, new DbParameter[0]);
            List<SysProcessRemind> list5 = this._context.Where<SysProcessRemind>(condition, new DbParameter[0]);
            List<SysProcessRemindParticipant> source = this._context.Where<SysProcessRemindParticipant>(condition, new DbParameter[0]);
            List<SysActivity> list7 = this._context.Where<SysActivity>(condition, new DbParameter[0]);
            List<SysActivityParticipant> list8 = this._context.Where<SysActivityParticipant>(condition, new DbParameter[0]);
            List<SysActivityOperation> list9 = this._context.Where<SysActivityOperation>(condition, new DbParameter[0]);
            List<SysActivityStep> list10 = this._context.Where<SysActivityStep>(condition, new DbParameter[0]);
            List<SysActivityRemind> list11 = this._context.Where<SysActivityRemind>(condition, new DbParameter[0]);
            List<SysActivityRemindParticipant> list12 = this._context.Where<SysActivityRemindParticipant>(condition, new DbParameter[0]);
            List<SysActivityApproveGroup> list13 = this._context.Where<SysActivityApproveGroup>(condition, new DbParameter[0]);
            process.Activities = list7;
            process.ProcessProxies = list;
            process.ProcessParticipants = list2;
            process.ProcessReminds = list5;
            process.Expressions = list3;
            process.Transitions = list4;
            using (IEnumerator<SysActivity> enumerator = process.Activities.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    Func<SysExpression, bool> predicate = null;
                    Func<SysActivityParticipant, bool> func8 = null;
                    Func<SysActivityRemind, bool> func9 = null;
                    Func<SysActivityOperation, bool> func10 = null;
                    Func<SysExpression, bool> func11 = null;
                    Func<SysTransition, bool> func12 = null;
                    Func<SysTransition, bool> func13 = null;
                    SysActivity a = enumerator.Current;
                    a.Process = process;
                    if (predicate == null)
                    {
                        predicate = i => i.ExpressionId == a.ExpressionId;
                    }
                    a.Expression = process.Expressions.FirstOrDefault<SysExpression>(predicate);
                    if (func8 == null)
                    {
                        func8 = delegate (SysActivityParticipant i) {
                            long? activityId = i.ActivityId;
                            long num = a.ActivityId;
                            return (activityId.GetValueOrDefault() == num) && activityId.HasValue;
                        };
                    }
                    a.ActivityParticipants = list8.Where<SysActivityParticipant>(func8).ToList<SysActivityParticipant>();
                    if (func9 == null)
                    {
                        func9 = delegate (SysActivityRemind i) {
                            long? activityId = i.ActivityId;
                            long num = a.ActivityId;
                            return (activityId.GetValueOrDefault() == num) && activityId.HasValue;
                        };
                    }
                    a.ActivityReminds = list11.Where<SysActivityRemind>(func9).ToList<SysActivityRemind>();
                    if (func10 == null)
                    {
                        func10 = delegate (SysActivityOperation i) {
                            long? activityId = i.ActivityId;
                            long num = a.ActivityId;
                            return (activityId.GetValueOrDefault() == num) && activityId.HasValue;
                        };
                    }
                    a.ActivityOperations = list9.Where<SysActivityOperation>(func10).ToList<SysActivityOperation>();
                    if (func11 == null)
                    {
                        func11 = delegate (SysExpression i) {
                            long? activityId = i.ActivityId;
                            long num = a.ActivityId;
                            return (activityId.GetValueOrDefault() == num) && activityId.HasValue;
                        };
                    }
                    a.Expressions = process.Expressions.Where<SysExpression>(func11).ToList<SysExpression>();
                    if (func12 == null)
                    {
                        func12 = delegate (SysTransition i) {
                            long? preActivityId = i.PreActivityId;
                            long activityId = a.ActivityId;
                            return (preActivityId.GetValueOrDefault() == activityId) && preActivityId.HasValue;
                        };
                    }
                    a.FromTransitions = process.Transitions.Where<SysTransition>(func12).ToList<SysTransition>();
                    if (func13 == null)
                    {
                        func13 = delegate (SysTransition i) {
                            long? postActivityId = i.PostActivityId;
                            long activityId = a.ActivityId;
                            return (postActivityId.GetValueOrDefault() == activityId) && postActivityId.HasValue;
                        };
                    }
                    a.ToTransitions = process.Transitions.Where<SysTransition>(func13).ToList<SysTransition>();
                    using (IEnumerator<SysActivityParticipant> enumerator2 = a.ActivityParticipants.GetEnumerator())
                    {
                        while (enumerator2.MoveNext())
                        {
                            Func<SysActivityApproveGroup, bool> func = null;
                            Func<SysActivityParticipant, bool> func2 = null;
                            Func<SysProcessParticipant, bool> func3 = null;
                            SysActivityParticipant ap = enumerator2.Current;
                            ap.Activity = a;
                            if (func == null)
                            {
                                func = i => i.ActivityApproveGroupId == ap.ActivityApproveGroupId;
                            }
                            ap.ActivityApproveGroup = list13.FirstOrDefault<SysActivityApproveGroup>(func);
                            if (ap.ActivityApproveGroup != null)
                            {
                                ap.ActivityApproveGroup.Activity = a;
                                if (func2 == null)
                                {
                                    func2 = i => i.ActivityApproveGroupId == ap.ActivityApproveGroupId;
                                }
                                ap.ActivityApproveGroup.ActivityParticipants = list8.Where<SysActivityParticipant>(func2).ToList<SysActivityParticipant>();
                            }
                            if (func3 == null)
                            {
                                func3 = i => i.ParticipantId == ap.ParticipantId;
                            }
                            ap.ProcessParticipant = process.ProcessParticipants.FirstOrDefault<SysProcessParticipant>(func3);
                        }
                    }
                    using (IEnumerator<SysActivityRemind> enumerator3 = a.ActivityReminds.GetEnumerator())
                    {
                        while (enumerator3.MoveNext())
                        {
                            Func<SysActivityRemindParticipant, bool> func5 = null;
                            SysActivityRemind ar = enumerator3.Current;
                            ar.Activity = a;
                            if (func5 == null)
                            {
                                func5 = delegate (SysActivityRemindParticipant i) {
                                    long? remindId = i.RemindId;
                                    long num = ar.RemindId;
                                    return (remindId.GetValueOrDefault() == num) && remindId.HasValue;
                                };
                            }
                            ar.RemindParticipants = list12.Where<SysActivityRemindParticipant>(func5).ToList<SysActivityRemindParticipant>();
                            using (IEnumerator<SysActivityRemindParticipant> enumerator4 = ar.RemindParticipants.GetEnumerator())
                            {
                                while (enumerator4.MoveNext())
                                {
                                    Func<SysProcessParticipant, bool> func4 = null;
                                    SysActivityRemindParticipant arp = enumerator4.Current;
                                    arp.Remind = ar;
                                    if (func4 == null)
                                    {
                                        func4 = i => i.ParticipantId == arp.ParticipantId;
                                    }
                                    arp.Participant = process.ProcessParticipants.FirstOrDefault<SysProcessParticipant>(func4);
                                    arp.RemindTemplate = this._context.FindById<SysProcessRemindTemplate>(new object[] { arp.TemplateId });
                                }
                                continue;
                            }
                        }
                    }
                    using (IEnumerator<SysActivityOperation> enumerator5 = a.ActivityOperations.GetEnumerator())
                    {
                        while (enumerator5.MoveNext())
                        {
                            Func<SysActivityStep, bool> func6 = null;
                            SysActivityOperation ao = enumerator5.Current;
                            ao.Activity = a;
                            if (func6 == null)
                            {
                                func6 = delegate (SysActivityStep i) {
                                    long? operationId = i.OperationId;
                                    long num = ao.OperationId;
                                    return (operationId.GetValueOrDefault() == num) && operationId.HasValue;
                                };
                            }
                            ao.ActivitySteps = list10.Where<SysActivityStep>(func6).ToList<SysActivityStep>();
                            foreach (SysActivityStep step in ao.ActivitySteps)
                            {
                                step.ActivityOperation = ao;
                            }
                        }
                        continue;
                    }
                }
            }
            foreach (SysProcessProxy proxy in process.ProcessProxies)
            {
                proxy.Process = process;
            }
            using (IEnumerator<SysProcessParticipant> enumerator8 = process.ProcessParticipants.GetEnumerator())
            {
                while (enumerator8.MoveNext())
                {
                    Func<SysProcessParticipant, bool> func14 = null;
                    Func<SysActivityParticipant, bool> func15 = null;
                    SysProcessParticipant pp = enumerator8.Current;
                    pp.Process = process;
                    if (func14 == null)
                    {
                        func14 = i => i.ParticipantId == pp.Param_ParticipantId;
                    }
                    pp.Param_Participant = process.ProcessParticipants.FirstOrDefault<SysProcessParticipant>(func14);
                    if (func15 == null)
                    {
                        func15 = delegate (SysActivityParticipant i) {
                            long? participantId = i.ParticipantId;
                            long num = pp.ParticipantId;
                            return (participantId.GetValueOrDefault() == num) && participantId.HasValue;
                        };
                    }
                    pp.ActivityParticipants = list8.Where<SysActivityParticipant>(func15).ToList<SysActivityParticipant>();
                }
            }
            using (IEnumerator<SysProcessRemind> enumerator9 = process.ProcessReminds.GetEnumerator())
            {
                while (enumerator9.MoveNext())
                {
                    Func<SysProcessRemindParticipant, bool> func17 = null;
                    SysProcessRemind pr = enumerator9.Current;
                    pr.Process = process;
                    if (func17 == null)
                    {
                        func17 = delegate (SysProcessRemindParticipant i) {
                            long? remindId = i.RemindId;
                            long num = pr.RemindId;
                            return (remindId.GetValueOrDefault() == num) && remindId.HasValue;
                        };
                    }
                    pr.RemindParticipants = source.Where<SysProcessRemindParticipant>(func17).ToList<SysProcessRemindParticipant>();
                    using (IEnumerator<SysProcessRemindParticipant> enumerator10 = pr.RemindParticipants.GetEnumerator())
                    {
                        while (enumerator10.MoveNext())
                        {
                            Func<SysProcessParticipant, bool> func16 = null;
                            SysProcessRemindParticipant rp = enumerator10.Current;
                            rp.Remind = pr;
                            if (func16 == null)
                            {
                                func16 = i => i.ParticipantId == rp.ParticipantId;
                            }
                            rp.Participant = process.ProcessParticipants.FirstOrDefault<SysProcessParticipant>(func16);
                            rp.RemindTemplate = this._context.FindById<SysProcessRemindTemplate>(new object[] { rp.TemplateId });
                        }
                        continue;
                    }
                }
            }
            using (IEnumerator<SysExpression> enumerator11 = process.Expressions.GetEnumerator())
            {
                while (enumerator11.MoveNext())
                {
                    Func<SysActivity, bool> func18 = null;
                    SysExpression e = enumerator11.Current;
                    e.Process = process;
                    if (func18 == null)
                    {
                        func18 = i => i.ActivityId == e.ActivityId;
                    }
                    e.Activity = process.Activities.FirstOrDefault<SysActivity>(func18);
                    if (e.FieldId.HasValue)
                    {
                        e.Field = this._cacheFactory.GetFieldCache(e.FieldId.Value);
                    }
                    if (e.RelationId.HasValue)
                    {
                        e.Relation = this._cacheFactory.GetRelationCache(e.RelationId.Value);
                    }
                }
            }
            using (IEnumerator<SysTransition> enumerator12 = process.Transitions.GetEnumerator())
            {
                while (enumerator12.MoveNext())
                {
                    Func<SysActivity, bool> func19 = null;
                    Func<SysActivity, bool> func20 = null;
                    Func<SysExpression, bool> func21 = null;
                    Func<SysExpression, bool> func22 = null;
                    SysTransition t = enumerator12.Current;
                    t.Process = process;
                    if (func19 == null)
                    {
                        func19 = i => i.ActivityId == t.PreActivityId;
                    }
                    t.PreActivity = process.Activities.FirstOrDefault<SysActivity>(func19);
                    if (func20 == null)
                    {
                        func20 = i => i.ActivityId == t.PostActivityId;
                    }
                    t.PostActivity = process.Activities.FirstOrDefault<SysActivity>(func20);
                    if (func21 == null)
                    {
                        func21 = i => i.ExpressionId == t.ExpressionId;
                    }
                    t.Expression = process.Expressions.FirstOrDefault<SysExpression>(func21);
                    if (func22 == null)
                    {
                        func22 = delegate (SysExpression i) {
                            long? transitionId = i.TransitionId;
                            long num = t.TransitionId;
                            return (transitionId.GetValueOrDefault() == num) && transitionId.HasValue;
                        };
                    }
                    t.Expressions = process.Expressions.Where<SysExpression>(func22).ToList<SysExpression>();
                }
            }
            return process;
        }

        internal SysProcessInstance LoadProcessInstance(long processInstanceId)
        {
            SysProcessInstance instance = this._context.FindById<SysProcessInstance>(new object[] { processInstanceId });
            if (instance == null)
            {
                throw new ApplicationException("流程实例ID不正确");
            }
            if (instance.FormInstanceId.HasValue)
            {
                instance.FormInstance = this._context.FindById<SysFormInstance>(new object[] { instance.FormInstanceId });
                if (instance.FormInstance == null)
                {
                    throw new ApplicationException("流程实例关联表单实例不存在");
                }
            }
            string condition = string.Format("ProcessInstanceId = {0}", instance.ProcessInstanceId);
            List<SysActivityInstance> list = this._context.Where<SysActivityInstance>(condition, new DbParameter[0]);
            List<SysWorkItem> source = this._context.Where<SysWorkItem>(condition, new DbParameter[0]);
            List<SysWorkItemApproveGroup> list3 = this._context.Where<SysWorkItemApproveGroup>(condition, new DbParameter[0]);
            List<SysTransitionInstance> list4 = this._context.Where<SysTransitionInstance>(condition, new DbParameter[0]);
            List<SysActivityInstanceApproveUsers> list5 = this._context.Where<SysActivityInstanceApproveUsers>(condition, new DbParameter[0]);
            instance.ActivityInstances = list;
            SysProcess processCache = this._cacheFactory.GetProcessCache(instance.ProcessId.Value);
            instance.Process = processCache;
            using (IEnumerator<SysActivityInstance> enumerator = instance.ActivityInstances.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    Func<SysActivity, bool> predicate = null;
                    Func<SysWorkItem, bool> func7 = null;
                    Func<SysWorkItemApproveGroup, bool> func8 = null;
                    Func<SysTransitionInstance, bool> func9 = null;
                    Func<SysTransitionInstance, bool> func10 = null;
                    Func<SysActivityInstanceApproveUsers, bool> func11 = null;
                    SysActivityInstance ai = enumerator.Current;
                    ai.ProcessInstance = instance;
                    if (predicate == null)
                    {
                        predicate = i => i.ActivityId == ai.ActivityId;
                    }
                    ai.Activity = processCache.Activities.FirstOrDefault<SysActivity>(predicate);
                    if (func7 == null)
                    {
                        func7 = delegate (SysWorkItem i) {
                            int? activityInstanceId = i.ActivityInstanceId;
                            int num = ai.ActivityInstanceId;
                            return (activityInstanceId.GetValueOrDefault() == num) && activityInstanceId.HasValue;
                        };
                    }
                    ai.WorkItems = source.Where<SysWorkItem>(func7).ToList<SysWorkItem>();
                    if (func8 == null)
                    {
                        func8 = delegate (SysWorkItemApproveGroup i) {
                            int? activityInstanceId = i.ActivityInstanceId;
                            int num = ai.ActivityInstanceId;
                            return (activityInstanceId.GetValueOrDefault() == num) && activityInstanceId.HasValue;
                        };
                    }
                    ai.ApproveGroups = list3.Where<SysWorkItemApproveGroup>(func8).ToList<SysWorkItemApproveGroup>();
                    if (func9 == null)
                    {
                        func9 = delegate (SysTransitionInstance i) {
                            int? preActivityInstanceId = i.PreActivityInstanceId;
                            int activityInstanceId = ai.ActivityInstanceId;
                            return (preActivityInstanceId.GetValueOrDefault() == activityInstanceId) && preActivityInstanceId.HasValue;
                        };
                    }
                    ai.FromTransitionInstances = list4.Where<SysTransitionInstance>(func9).ToList<SysTransitionInstance>();
                    if (func10 == null)
                    {
                        func10 = delegate (SysTransitionInstance i) {
                            int? postActivityInstanceId = i.PostActivityInstanceId;
                            int activityInstanceId = ai.ActivityInstanceId;
                            return (postActivityInstanceId.GetValueOrDefault() == activityInstanceId) && postActivityInstanceId.HasValue;
                        };
                    }
                    ai.ToTransitionInstances = list4.Where<SysTransitionInstance>(func10).ToList<SysTransitionInstance>();
                    if (func11 == null)
                    {
                        func11 = delegate (SysActivityInstanceApproveUsers i) {
                            int? currentActivityInstanceId = i.CurrentActivityInstanceId;
                            int activityInstanceId = ai.ActivityInstanceId;
                            return (currentActivityInstanceId.GetValueOrDefault() == activityInstanceId) && currentActivityInstanceId.HasValue;
                        };
                    }
                    ai.UserDefinedApproveUsers = list5.Where<SysActivityInstanceApproveUsers>(func11).ToList<SysActivityInstanceApproveUsers>();
                    using (IEnumerator<SysWorkItem> enumerator2 = ai.WorkItems.GetEnumerator())
                    {
                        while (enumerator2.MoveNext())
                        {
                            Func<SysActivityParticipant, bool> func = null;
                            Func<SysWorkItemApproveGroup, bool> func2 = null;
                            SysWorkItem wi = enumerator2.Current;
                            wi.ProcessInstance = instance;
                            wi.ActivityInstance = ai;
                            if (func == null)
                            {
                                func = i => i.ActivityParticipantId == wi.ActivityParticipantId;
                            }
                            wi.ActivityParticipant = ai.Activity.ActivityParticipants.FirstOrDefault<SysActivityParticipant>(func);
                            if (func2 == null)
                            {
                                func2 = i => i.ApproveGroupId == wi.ApproveGroupId;
                            }
                            wi.ApproveGroup = ai.ApproveGroups.FirstOrDefault<SysWorkItemApproveGroup>(func2);
                        }
                    }
                    using (IEnumerator<SysWorkItemApproveGroup> enumerator3 = ai.ApproveGroups.GetEnumerator())
                    {
                        while (enumerator3.MoveNext())
                        {
                            Func<SysWorkItem, bool> func3 = null;
                            SysWorkItemApproveGroup ag = enumerator3.Current;
                            ag.ActivityInstance = ai;
                            if (func3 == null)
                            {
                                func3 = delegate (SysWorkItem i) {
                                    int? approveGroupId = i.ApproveGroupId;
                                    int num = ag.ApproveGroupId;
                                    return (approveGroupId.GetValueOrDefault() == num) && approveGroupId.HasValue;
                                };
                            }
                            ag.WorkItems = ai.WorkItems.Where<SysWorkItem>(func3).ToList<SysWorkItem>();
                        }
                    }
                    using (IEnumerator<SysTransitionInstance> enumerator4 = ai.FromTransitionInstances.GetEnumerator())
                    {
                        while (enumerator4.MoveNext())
                        {
                            Func<SysActivityInstance, bool> func4 = null;
                            SysTransitionInstance fti = enumerator4.Current;
                            if (func4 == null)
                            {
                                func4 = x => x.ActivityInstanceId == fti.PostActivityInstanceId;
                            }
                            fti.PostActivityInstance = instance.ActivityInstances.FirstOrDefault<SysActivityInstance>(func4);
                            fti.PreActivityInstance = ai;
                        }
                    }
                    using (IEnumerator<SysTransitionInstance> enumerator5 = ai.ToTransitionInstances.GetEnumerator())
                    {
                        while (enumerator5.MoveNext())
                        {
                            Func<SysActivityInstance, bool> func5 = null;
                            SysTransitionInstance tti = enumerator5.Current;
                            tti.PostActivityInstance = ai;
                            if (func5 == null)
                            {
                                func5 = x => x.ActivityInstanceId == tti.PreActivityInstanceId;
                            }
                            tti.PreActivityInstance = instance.ActivityInstances.FirstOrDefault<SysActivityInstance>(func5);
                        }
                        continue;
                    }
                }
            }
            return instance;
        }
    }
}

