using System.Data.Common;

namespace King.Framework.WorkflowEngineCore.Cache
{
    using King.Framework.EntityLibrary;
    using King.Framework.Linq;
    using King.Framework.WorkflowEngineCore.InternalCore;
    using King.Framework.WorkflowEngineCore.MessageHandler;
    using King.Framework.WorkflowEngineCore.Remind;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;

    public class ProcessInstanceCacheFactory
    {
        private readonly DataContext _context;
        private readonly DynamicManager _manager;
        private readonly ProcessCacheFactory _processCache;

        public ProcessInstanceCacheFactory(DataContext ctx)
        {
            this._context = ctx;
            this._processCache = new ProcessCacheFactory(ctx);
            this._manager = new DynamicManager(ctx);
        }

        public void AddActivityInstance(SysProcessInstance pi, SysActivityInstance ai)
        {
            this._context.Insert(ai);
            ai.Activity = pi.Process.Activities.FirstOrDefault<SysActivity>(i => i.ActivityId == ai.ActivityId);
            ai.ProcessInstance = pi;
            pi.ActivityInstances.Add(ai);
            ai.WorkItems = new List<SysWorkItem>();
            ai.ApproveGroups = new List<SysWorkItemApproveGroup>();
            ai.FromTransitionInstances = new List<SysTransitionInstance>();
            ai.ToTransitionInstances = new List<SysTransitionInstance>();
            ai.UserDefinedApproveUsers = new List<SysActivityInstanceApproveUsers>();
        }

        public void AddApproveData(SysApproveActivityData data)
        {
            this._context.Insert(data);
        }

        public void AddNextApproveUsers(SysActivityInstance ai, List<IApproveUser> approveUsers)
        {
            foreach (IApproveUser user in approveUsers)
            {
                SysActivityInstanceApproveUsers users = new SysActivityInstanceApproveUsers {
                    Id = this._context.GetNextIdentity_Int(false),
                    CurrentActivityInstanceId = new int?(ai.ActivityInstanceId),
                    ProcessInstanceId = ai.ProcessInstanceId,
                    UserId = user.UserId,
                    IsMajor = user.IsMajor,
                    FlagString = user.FlagString,
                    FlagInt = user.FlagInt
                };
                this._context.Insert(users);
                users.CurrentActivityInstance = ai;
                ai.UserDefinedApproveUsers.Add(users);
            }
        }

        public void AddNextApproveUsers(SysWorkItem wi, SysActivityInstance ai, List<IApproveUser> approveUsers)
        {
            foreach (IApproveUser user in approveUsers)
            {
                SysActivityInstanceApproveUsers users = new SysActivityInstanceApproveUsers {
                    Id = this._context.GetNextIdentity_Int(false),
                    OriginalWorkItemId = new int?(wi.WorkItemId),
                    ProcessInstanceId = ai.ProcessInstanceId,
                    CurrentActivityInstanceId = new int?(ai.ActivityInstanceId),
                    UserId = user.UserId,
                    IsMajor = user.IsMajor,
                    FlagString = user.FlagString,
                    FlagInt = user.FlagInt
                };
                this._context.Insert(users);
                users.CurrentActivityInstance = ai;
                users.OriginalWorkItem = wi;
                ai.UserDefinedApproveUsers.Add(users);
            }
        }

        public void AddProcessInstance(SysProcessInstance pi)
        {
            this._context.Insert(pi);
            pi.Process = this._processCache.GetProcessCache(pi.ProcessId.Value);
            pi.ActivityInstances = new List<SysActivityInstance>();
        }

        public void AddProcessRemindToken(SysProcessRemindToken token)
        {
            this._context.Insert(token);
        }

        public void AddTransitionInstance(SysTransitionInstance ti, SysActivityInstance preAI, SysActivityInstance postAI)
        {
            this._context.Insert(ti);
            ti.PreActivityInstance = preAI;
            ti.PostActivityInstance = postAI;
            preAI.FromTransitionInstances.Add(ti);
            postAI.ToTransitionInstances.Add(ti);
            ti.Transition = preAI.ProcessInstance.Process.Transitions.FirstOrDefault<SysTransition>(i => i.TransitionId == ti.TransitionId);
        }

        public void AddWorkItem(SysWorkItem wi, SysProcessInstance pi, SysActivityInstance ai)
        {
            T_WorkItemBase wiBase = this.InsertWorkItemBase(wi, pi, ai);
            string str = new CustomWorkItemHandler(this.Context).OnWorkItemCreating(wiBase, wi);
            wi.WwfBookmarkId = str;
            this.Context.Insert(wi);
            wi.ProcessInstance = pi;
            wi.ActivityInstance = ai;
            ai.WorkItems.Add(wi);
            wi.ActivityParticipant = ai.Activity.ActivityParticipants.FirstOrDefault<SysActivityParticipant>(i => i.ActivityParticipantId == wi.ActivityParticipantId);
            wi.ApproveGroup = ai.ApproveGroups.FirstOrDefault<SysWorkItemApproveGroup>(i => i.ApproveGroupId == wi.ApproveGroupId);
            wi.ApproveGroup.WorkItems.Add(wi);
            new ActivityRemindHandler(this, pi, ai, ActivityRemindUseTimeType.WorkItemCreate, wi, null).Execute();
        }

        public void AddWorkItemGroup(SysWorkItemApproveGroup group, SysProcessInstance pi, SysActivityInstance ai)
        {
            this._context.Insert(group);
            group.ActivityInstance = ai;
            ai.ApproveGroups.Add(group);
            group.WorkItems = new List<SysWorkItem>();
        }

        public void ClearApproveUsers(SysActivityInstance ai)
        {
            if (ai.UserDefinedApproveUsers.Count > 0)
            {
                foreach (SysActivityInstanceApproveUsers users in ai.UserDefinedApproveUsers)
                {
                    this._context.Delete(users);
                }
                ai.UserDefinedApproveUsers.Clear();
            }
        }

        public void ClearCache(int processInstanceId)
        {
        }

        public SysProcessInstance GetProcessInstanceCache(int processInstanceId)
        {
            return this.PCacheFactory.ProcessCacheHelper.LoadProcessInstance((long) processInstanceId);
        }

        public SysProcessInstance GetProcessInstanceCacheByWorkItem(int workItemId)
        {
            SysWorkItem item = this._context.FindById<SysWorkItem>(new object[] { workItemId });
            if (item == null)
            {
                throw new ApplicationException("工作项ID不正确");
            }
            SysActivityInstance instance = this._context.FindById<SysActivityInstance>(new object[] { item.ActivityInstanceId });
            if (instance == null)
            {
                throw new ApplicationException("工作项ID不正确");
            }
            return this.GetProcessInstanceCache(instance.ProcessInstanceId.Value);
        }

        public SysProcessInstance GetProcessInstanceCacheByWorkItem(int workItemId, out SysWorkItem work_item)
        {
            Func<SysWorkItem, bool> predicate = null;
            SysWorkItem item = this._context.FindById<SysWorkItem>(new object[] { workItemId });
            if (item == null)
            {
                throw new ApplicationException("工作项ID不正确");
            }
            Func<SysActivityInstance, bool> func = null;
            SysActivityInstance ai = this._context.FindById<SysActivityInstance>(new object[] { item.ActivityInstanceId });
            if (ai == null)
            {
                throw new ApplicationException("工作项ID不正确");
            }
            SysProcessInstance processInstanceCache = this.GetProcessInstanceCache(ai.ProcessInstanceId.Value);
            if (func == null)
            {
                func = i => i.ActivityInstanceId == ai.ActivityInstanceId;
            }
            SysActivityInstance instance2 = processInstanceCache.ActivityInstances.FirstOrDefault<SysActivityInstance>(func);
            if (predicate == null)
            {
                predicate = i => i.WorkItemId == workItemId;
            }
            work_item = instance2.WorkItems.FirstOrDefault<SysWorkItem>(predicate);
            return processInstanceCache;
        }

        public string GetWorkItemCompletePageUrl(SysWorkItem wi, SysProcessInstance pi, SysActivityInstance ai, bool isRelative = true)
        {
            string str3;
            try
            {
                SysActivity activity = ai.Activity;
                if (activity == null)
                {
                    throw new ApplicationException(string.Format("活动实例:{0}找不到对应活动", ai.ActivityInstanceId));
                }
                if (pi.Process.ProcessCategory == 2)
                {
                    return string.Format("../FormWorkflow/FormInstanceApprove.aspx?id={0}", wi.WorkItemId);
                }
                long? pageid = activity.PageId;
                SysPage page = pi.Process.ProcessEntity.Pages.FirstOrDefault<SysPage>(p => p.ControlId == pageid);
                if (page == null)
                {
                    throw new ApplicationException(string.Format("活动:{0}找不到对应页面", activity.ActivityId));
                }
                if (!isRelative)
                {
                    return string.Format("{0}_{1}/{2}.aspx?id={3}", new object[] { page.OwnerEntity.OwnerModule.EntityCategory.CategoryName, page.OwnerEntity.OwnerModule.ModuleName, page.PageName, wi.WorkItemId });
                }
                str3 = string.Format("../{0}_{1}/{2}.aspx?id={3}", new object[] { page.OwnerEntity.OwnerModule.EntityCategory.CategoryName, page.OwnerEntity.OwnerModule.ModuleName, page.PageName, wi.WorkItemId });
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return str3;
        }

        public string GetWorkItemDisplayText(SysWorkItem wi, SysProcessInstance pi, SysActivityInstance ai)
        {
            string str6;
            try
            {
                SysActivity activity = ai.Activity;
                if (activity == null)
                {
                    throw new ApplicationException(string.Format("活动实例:{0}找不到对应活动", ai.ActivityInstanceId));
                }
                SysProcess process = pi.Process;
                if (process == null)
                {
                    throw new ApplicationException(string.Format("活动:{0}找不到对应流程", activity.ActivityId));
                }
                string activityName = activity.ActivityName;
                string processName = process.ProcessName;
                if (process.ProcessCategory == 2)
                {
                    return string.Format("[{0}][{1}]{2}", processName, activityName, pi.FormInstance.FormTitle);
                }
                EntityCache cache = new EntityCache(this._manager);
                SysEntity processEntity = process.ProcessEntity;
                string objectDisplayName = cache.GetObjectDisplayName(processEntity, wi.RelativeObjectId.Value);
                str6 = string.Format("[{0}][{1}]{2}", processName, activityName, objectDisplayName);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return str6;
        }

        public T_WorkItemBase InsertWorkItemBase(SysWorkItem wi, SysProcessInstance pi, SysActivityInstance ai)
        {
            T_WorkItemBase base4;
            try
            {
                int num = this._context.GetNextIdentity_Int(false);
                int num2 = this._context.GetNextIdentity_Int(false);
                wi.WorkItemId = num;
                wi.WorkItemBaseId = new int?(num2);
                string str = this.GetWorkItemDisplayText(wi, pi, ai);
                string str2 = this.GetWorkItemCompletePageUrl(wi, pi, ai, true);
                T_WorkItemBase base2 = new T_WorkItemBase {
                    WorkItemBase_Id = num2,
                    WorkItemBase_Name = str,
                    Title = str,
                    CompletePageUrl = str2,
                    CreateTime = new DateTime?(DateTime.Now),
                    StartTime = new DateTime?(DateTime.Now),
                    EndTime = null,
                    IsAllDay = true,
                    WorkItemId = num.ToString(),
                    OwnerId = wi.OwnerId,
                    State = wi.Status
                };
                this._context.Insert(base2);
                base4 = base2;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return base4;
        }

        private void LoadActivityInstance(SysActivityInstance ai, SysProcessInstance pi, SysProcess p)
        {
            ai.ProcessInstance = pi;
            ai.Activity = p.Activities.FirstOrDefault<SysActivity>(i => i.ActivityId == ai.ActivityId);
            string condition = string.Format("ActivityInstanceId = {0}", ai.ActivityInstanceId);
            ai.WorkItems = this._context.Where<SysWorkItem>(condition, new DbParameter[0]);
            ai.ApproveGroups = this._context.Where<SysWorkItemApproveGroup>(condition, new DbParameter[0]);
            ai.FromTransitionInstances = this._context.Where<SysTransitionInstance>(string.Format("PreActivityInstanceId = {0}", ai.ActivityInstanceId), new DbParameter[0]);
            ai.ToTransitionInstances = this._context.Where<SysTransitionInstance>(string.Format("PostActivityInstanceId = {0}", ai.ActivityInstanceId), new DbParameter[0]);
            ai.UserDefinedApproveUsers = this._context.Where<SysActivityInstanceApproveUsers>(string.Format("CurrentActivityInstanceId = {0}", ai.ActivityInstanceId), new DbParameter[0]);
            using (IEnumerator<SysWorkItem> enumerator = ai.WorkItems.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    Func<SysActivityParticipant, bool> predicate = null;
                    Func<SysWorkItemApproveGroup, bool> func2 = null;
                    SysWorkItem wi = enumerator.Current;
                    wi.ActivityInstance = ai;
                    if (predicate == null)
                    {
                        predicate = i => i.ActivityParticipantId == wi.ActivityParticipantId;
                    }
                    wi.ActivityParticipant = ai.Activity.ActivityParticipants.FirstOrDefault<SysActivityParticipant>(predicate);
                    if (func2 == null)
                    {
                        func2 = i => i.ApproveGroupId == wi.ApproveGroupId;
                    }
                    wi.ApproveGroup = ai.ApproveGroups.FirstOrDefault<SysWorkItemApproveGroup>(func2);
                }
            }
            using (IEnumerator<SysWorkItemApproveGroup> enumerator2 = ai.ApproveGroups.GetEnumerator())
            {
                while (enumerator2.MoveNext())
                {
                    Func<SysWorkItem, bool> func3 = null;
                    SysWorkItemApproveGroup ag = enumerator2.Current;
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
            using (IEnumerator<SysTransitionInstance> enumerator3 = ai.FromTransitionInstances.GetEnumerator())
            {
                while (enumerator3.MoveNext())
                {
                    Func<SysActivityInstance, bool> func4 = null;
                    SysTransitionInstance fti = enumerator3.Current;
                    if (func4 == null)
                    {
                        func4 = x => x.ActivityInstanceId == fti.PostActivityInstanceId;
                    }
                    fti.PostActivityInstance = pi.ActivityInstances.FirstOrDefault<SysActivityInstance>(func4);
                    fti.PreActivityInstance = ai;
                }
            }
            using (IEnumerator<SysTransitionInstance> enumerator4 = ai.ToTransitionInstances.GetEnumerator())
            {
                while (enumerator4.MoveNext())
                {
                    Func<SysActivityInstance, bool> func5 = null;
                    SysTransitionInstance tti = enumerator4.Current;
                    tti.PostActivityInstance = ai;
                    if (func5 == null)
                    {
                        func5 = x => x.ActivityInstanceId == tti.PreActivityInstanceId;
                    }
                    tti.PreActivityInstance = pi.ActivityInstances.FirstOrDefault<SysActivityInstance>(func5);
                }
            }
        }

        private SysProcessInstance LoadProcessInstance(int processInstanceId)
        {
            SysProcessInstance pi = this._context.FindById<SysProcessInstance>(new object[] { processInstanceId });
            if (pi == null)
            {
                throw new ApplicationException("流程实例ID不正确");
            }
            if (pi.FormInstanceId.HasValue)
            {
                pi.FormInstance = this._context.FindById<SysFormInstance>(new object[] { pi.FormInstanceId });
                if (pi.FormInstance == null)
                {
                    throw new ApplicationException("流程实例关联表单实例不存在");
                }
            }
            SysProcess processCache = this._processCache.GetProcessCache(pi.ProcessId.Value);
            pi.Process = processCache;
            string condition = string.Format("ProcessInstanceId = {0}", pi.ProcessInstanceId);
            pi.ActivityInstances = this._context.Where<SysActivityInstance>(condition, new DbParameter[0]);
            foreach (SysActivityInstance instance2 in pi.ActivityInstances)
            {
                this.LoadActivityInstance(instance2, pi, processCache);
            }
            return pi;
        }

        public void RedirectNextApproveUsers(SysActivityInstance preAi, SysActivityInstance postAi)
        {
            if (preAi.UserDefinedApproveUsers.Count > 0)
            {
                foreach (SysActivityInstanceApproveUsers users in preAi.UserDefinedApproveUsers)
                {
                    users.CurrentActivityInstanceId = new int?(postAi.ActivityInstanceId);
                    this._context.Update(users);
                    users.CurrentActivityInstance = postAi;
                    postAi.UserDefinedApproveUsers.Add(users);
                }
                preAi.UserDefinedApproveUsers.Clear();
            }
        }

        public void UpdateActiviyInstance(SysActivityInstance ai)
        {
            this._context.Update(ai);
        }

        public void UpdateFormInstance(SysFormInstance fi)
        {
            this._context.Update(fi);
        }

        public void UpdateProcessInstance(SysProcessInstance pi)
        {
            this._context.Update(pi);
        }

        public void UpdateWorkItem(SysWorkItem wi, SysProcessInstance pi, SysActivityInstance ai)
        {
            this.UpdateWorkItemBaseState(wi);
            this.Context.Update(wi);
            if (wi.Status == 1)
            {
                CustomWorkItemHandler handler = new CustomWorkItemHandler(this.Context);
                int wiBaseId = wi.WorkItemBaseId.Value;
                int workItemId = wi.WorkItemId;
                string wwfBookmarkId = wi.WwfBookmarkId;
                handler.OnWorkItemCompleted(wiBaseId, workItemId, wwfBookmarkId);
                if (ai.Activity.ActivityType == 6)
                {
                    WorkItemWrapper wrapper = new WorkItemWrapper(this, wi, pi);
                    int? result = new int?(CompleteWorkItemMessage_Approve.ConvertBoolToApproveResult(wrapper.ApproveResult.Value));
                    new ActivityRemindHandler(this, pi, ai, ActivityRemindUseTimeType.WorkItemFinished, wi, result).Execute();
                }
                else
                {
                    new ActivityRemindHandler(this, pi, ai, ActivityRemindUseTimeType.WorkItemFinished, wi, null).Execute();
                }
            }
        }

        public void UpdateWorkItemBaseState(SysWorkItem wi)
        {
            try
            {
                if (!wi.WorkItemBaseId.HasValue)
                {
                    throw new Exception(string.Format("SysWorkItem，Id:{0}没有对应的WorkItemBase", wi.WorkItemId));
                }
                string format = "update T_WorkItemBase set State = {0} where WorkItemBase_Id = {1}";
                string sql = string.Format(format, wi.Status, wi.WorkItemBaseId);
                this._context.ExecuteNonQuery(sql, new DbParameter[0]);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        public void UpdateWorkItemGroup(SysWorkItemApproveGroup group)
        {
            this._context.Update(group);
        }

        public DataContext Context
        {
            get
            {
                return this._context;
            }
        }

        public DynamicManager Manager
        {
            get
            {
                return this._manager;
            }
        }

        public ProcessCacheFactory PCacheFactory
        {
            get
            {
                return this._processCache;
            }
        }
    }
}

