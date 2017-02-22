using System.Data.Common;

namespace King.Framework.WorkflowEngineCore.InternalCore
{
    using King.Framework.Common;
    using King.Framework.EntityLibrary;
    using King.Framework.Interfaces;
    using King.Framework.Linq;
    using King.Framework.WorkflowEngineCore.Cache;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.InteropServices;

    internal class InternalEngineProxy : IWorkflowEngine
    {
        private readonly DataContext _context;
        private readonly ProcessCacheFactory _processCache;

        public InternalEngineProxy(DataContext context)
        {
            if (context == null)
            {
                throw new ApplicationException("context");
            }
            this._context = context;
            this._processCache = new ProcessCacheFactory(context);
        }

        public void CancelProcess(int processInstanceId)
        {
            try
            {
                ProcessInstanceCacheFactory runtimeContext = new ProcessInstanceCacheFactory(this._context);
                SysProcessInstance processInstanceCache = runtimeContext.GetProcessInstanceCache(processInstanceId);
                new ProcessEngine(runtimeContext, processInstanceCache).CancelProcess(processInstanceId);
            }
            catch (Exception exception)
            {
                AppLogHelper.Error(string.Format("撤消流程实例[{0}]失败", processInstanceId), exception);
                throw;
            }
        }

        public void CompleteApproveWorkItem(int workItemId, ApproveResultEnum approveResult, string approveComment, bool addUser = false, int? addUserId = new int?())
        {
            try
            {
                ProcessInstanceCacheFactory runtimeContext = new ProcessInstanceCacheFactory(this._context);
                SysProcessInstance processInstanceCacheByWorkItem = runtimeContext.GetProcessInstanceCacheByWorkItem(workItemId);
                new ProcessEngine(runtimeContext, processInstanceCacheByWorkItem).CompleteApproveWorkItem(workItemId, approveResult, approveComment, addUser, addUserId);
            }
            catch (Exception exception)
            {
                AppLogHelper.Error(string.Format("完成【签核流程】的工作项[{0}]失败", workItemId), exception);
                throw exception;
            }
        }

        public void CompleteApproveWorkItem(int workItemId, ApproveResultEnum approveResult, string approveComment, List<IApproveUser> nextApproveUserList, bool addUser = false, int? addUserId = new int?())
        {
            try
            {
                ProcessInstanceCacheFactory runtimeContext = new ProcessInstanceCacheFactory(this._context);
                SysProcessInstance processInstanceCacheByWorkItem = runtimeContext.GetProcessInstanceCacheByWorkItem(workItemId);
                new ProcessEngine(runtimeContext, processInstanceCacheByWorkItem).CompleteApproveWorkItem(workItemId, approveResult, approveComment, nextApproveUserList, addUser, addUserId);
            }
            catch (Exception exception)
            {
                AppLogHelper.Error(string.Format("完成【签核流程】的工作项[{0}]失败", workItemId), exception);
                throw exception;
            }
        }

        public void CompleteApproveWorkItemAsync(int workItemId, ApproveResultEnum approveResult, string approveComment, List<IApproveUser> nextApproveUserList, bool addUser = false, int? addUserId = new int?())
        {
            try
            {
                this.UpdateWorkItemAsCompleting(workItemId);
                int num = this._context.GetNextIdentity_Int(false);
                SysWorkflowMessage message = new SysWorkflowMessage {
                    MessageId = num,
                    MessageType = WorkflowMessageTypeEnum.CompletingApproveWorkItem,
                    WorkItemId = workItemId,
                    State = SysWorkflowMessageStateEnum.Inited,
                    CreateTime = DateTime.Now,
                    ApproveResult = approveResult,
                    ApproveComment = approveComment,
                    AddUser = addUser,
                    AddUserId = addUserId
                };
                if ((nextApproveUserList != null) && (nextApproveUserList.Count > 0))
                {
                    foreach (IApproveUser user in nextApproveUserList)
                    {
                        MessageApproveUser user2 = new MessageApproveUser(user) {
                            WorkflowMessageId = num,
                            MessageApproveUserId = this._context.GetNextIdentity_Int(false)
                        };
                        this._context.Insert(user2);
                    }
                }
                this._context.Insert(message);
            }
            catch (Exception exception)
            {
                AppLogHelper.Error(exception, "完成签核工作项失败,workitem_id={0}", new object[] { workItemId });
                throw;
            }
        }

        public void CompleteApproveWorkItemSelf(int workItemId, ApproveResultEnum approveResult, string approveComment, bool addUser = false, int? addUserId = new int?())
        {
            try
            {
                ProcessInstanceCacheFactory runtimeContext = new ProcessInstanceCacheFactory(this._context);
                SysProcessInstance processInstanceCacheByWorkItem = runtimeContext.GetProcessInstanceCacheByWorkItem(workItemId);
                new ProcessEngine(runtimeContext, processInstanceCacheByWorkItem).CompleteApproveWorkItemSelf(workItemId, approveResult, approveComment, addUser, addUserId);
            }
            catch (Exception exception)
            {
                AppLogHelper.Error(string.Format("完成【签核流程】的工作项，仅完成自身[{0}]失败", workItemId), exception);
                throw exception;
            }
        }

        public void CompleteWorkItem(int workItemId)
        {
            try
            {
                ProcessInstanceCacheFactory runtimeContext = new ProcessInstanceCacheFactory(this._context);
                SysProcessInstance processInstanceCacheByWorkItem = runtimeContext.GetProcessInstanceCacheByWorkItem(workItemId);
                new ProcessEngine(runtimeContext, processInstanceCacheByWorkItem).CompleteWorkItem(workItemId);
            }
            catch (Exception exception)
            {
                AppLogHelper.Error(string.Format("完成【普通流程】的工作项[{0}]失败", workItemId), exception);
                throw exception;
            }
        }

        public void CompleteWorkItem(int workItemId, List<IApproveUser> nextApproveUserList)
        {
            try
            {
                ProcessInstanceCacheFactory runtimeContext = new ProcessInstanceCacheFactory(this._context);
                SysProcessInstance processInstanceCacheByWorkItem = runtimeContext.GetProcessInstanceCacheByWorkItem(workItemId);
                new ProcessEngine(runtimeContext, processInstanceCacheByWorkItem).CompleteWorkItem(workItemId, nextApproveUserList);
            }
            catch (Exception exception)
            {
                AppLogHelper.Error(string.Format("完成【普通流程】的工作项[{0}]失败", workItemId), exception);
                throw exception;
            }
        }

        public void CompleteWorkItemAsync(int workItemId)
        {
            this.CompleteWorkItemAsync(workItemId, null);
        }

        public void CompleteWorkItemAsync(int workItemId, List<IApproveUser> nextApproveUserList)
        {
            try
            {
                this.UpdateWorkItemAsCompleting(workItemId);
                int num = this._context.GetNextIdentity_Int(false);
                SysWorkflowMessage msg = new SysWorkflowMessage {
                    MessageId = num,
                    MessageType = WorkflowMessageTypeEnum.CompletingWorkItem,
                    WorkItemId = workItemId,
                    State = SysWorkflowMessageStateEnum.Inited,
                    CreateTime = DateTime.Now
                };
                if ((nextApproveUserList != null) && (nextApproveUserList.Count > 0))
                {
                    msg.NextApproveUserList.AddRange(nextApproveUserList);
                }
                this.SaveWorkflowMessage(msg);
            }
            catch (Exception exception)
            {
                AppLogHelper.Error(exception, "完成工作项失败,workitem_id={0}", new object[] { workItemId });
                throw;
            }
        }

        public void CompleteWorkItemSelf(int workItemId)
        {
            try
            {
                ProcessInstanceCacheFactory runtimeContext = new ProcessInstanceCacheFactory(this._context);
                SysProcessInstance processInstanceCacheByWorkItem = runtimeContext.GetProcessInstanceCacheByWorkItem(workItemId);
                new ProcessEngine(runtimeContext, processInstanceCacheByWorkItem).CompleteWorkItemSelf(workItemId);
            }
            catch (Exception exception)
            {
                AppLogHelper.Error(string.Format("完成【普通流程】的工作项，仅完成自身[{0}]失败", workItemId), exception);
                throw exception;
            }
        }

        public List<SysActivity> GetNextActivityList(int workItemId)
        {
            List<SysActivity> list2;
            try
            {
                SysWorkItem item;
                List<SysActivity> list = new List<SysActivity>();
                new ProcessInstanceCacheFactory(this._context).GetProcessInstanceCacheByWorkItem(workItemId, out item);
                SysActivity activity = item.ActivityInstance.Activity;
                if (activity != null)
                {
                    list = (from p in activity.FromTransitions select p.PostActivity).ToList<SysActivity>();
                }
                list2 = list;
            }
            catch (Exception exception)
            {
                AppLogHelper.Error(string.Format("GetNextActivityList,workItemId=[{0}]失败", workItemId), exception);
                throw exception;
            }
            return list2;
        }

        public List<SysActivity> GetNextActivityList(string processName)
        {
            List<SysActivity> list2;
            try
            {
                List<SysActivity> list = new List<SysActivity>();
                SysProcess processCache = this._processCache.GetProcessCache(processName);
                if (processCache == null)
                {
                    throw new ApplicationException(string.Format("流程名称[{0}]不正确", processName));
                }
                SysActivity activity = processCache.Activities.FirstOrDefault<SysActivity>(p => p.ActivityType == 1);
                if (activity != null)
                {
                    list = (from p in activity.FromTransitions select p.PostActivity).ToList<SysActivity>();
                }
                list2 = list;
            }
            catch (Exception exception)
            {
                AppLogHelper.Error(string.Format("GetNextActivityList,processName=[{0}]失败", processName), exception);
                throw exception;
            }
            return list2;
        }

        public List<IUser> GetParticipantUsers(long activityId, int? processInstanceId = new int?(), int? activityInstanctId = new int?())
        {
            Func<SysActivityInstance, bool> predicate = null;
            Func<SysActivityInstance, bool> func3 = null;
            List<IUser> list3;
            try
            {
                SysActivity activity;
                int? nullable = processInstanceId;
                if ((nullable.HasValue ? nullable.GetValueOrDefault() : -1) <= 0)
                {
                    processInstanceId = null;
                }
                int? nullable2 = activityInstanctId;
                if ((nullable2.HasValue ? nullable2.GetValueOrDefault() : -1) <= 0)
                {
                    activityInstanctId = null;
                }
                List<IUser> users = new List<IUser>();
                SysProcess processCacheByActivity = this._processCache.GetProcessCacheByActivity(activityId, out activity);
                SysProcessInstance pi = null;
                SysActivityInstance ai = null;
                if (processInstanceId.HasValue)
                {
                    pi = new ProcessInstanceCacheFactory(this._context).GetProcessInstanceCache(processInstanceId.Value);
                    if (pi == null)
                    {
                        throw new ApplicationException(string.Format("流程实例ID：{0}不正确", processInstanceId));
                    }
                    if (activityInstanctId.HasValue)
                    {
                        if (predicate == null)
                        {
                            predicate = p => p.ActivityInstanceId == activityInstanctId.Value;
                        }
                        ai = pi.ActivityInstances.FirstOrDefault<SysActivityInstance>(predicate);
                        if (ai == null)
                        {
                            throw new ApplicationException(string.Format("活动实例ID：{0}不正确", activityInstanctId));
                        }
                    }
                    else
                    {
                        if (func3 == null)
                        {
                            func3 = delegate (SysActivityInstance p) {
                                long? nullable1 = p.ActivityId;
                                long num = activityId;
                                return (nullable1.GetValueOrDefault() == num) && nullable1.HasValue;
                            };
                        }
                        ai = pi.ActivityInstances.FirstOrDefault<SysActivityInstance>(func3);
                    }
                }
                using (IEnumerator<SysActivityParticipant> enumerator = activity.ActivityParticipants.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        Func<SysProcessParticipant, bool> func = null;
                        SysActivityParticipant activity_part = enumerator.Current;
                        if (func == null)
                        {
                            func = p => p.ParticipantId == activity_part.ParticipantId.Value;
                        }
                        SysProcessParticipant part = processCacheByActivity.ProcessParticipants.FirstOrDefault<SysProcessParticipant>(func);
                        if (part == null)
                        {
                            throw new ApplicationException("参与人为空");
                        }
                        int? wiOwnerId = null;
                        List<IUser> collection = ParticipantHelper.GetUsers(this._context, part, pi, ai, wiOwnerId);
                        users.AddRange(collection);
                    }
                }
                RemoveRepeatedUsers(users);
                list3 = users;
            }
            catch (Exception exception)
            {
                AppLogHelper.Error(string.Format("GetParticipantUsers,activityId=[{0}]失败", activityId), exception);
                throw exception;
            }
            return list3;
        }

        public void RejectApproveWorkItem(int workItemId, string approveComment, long nextActivityId)
        {
            try
            {
                ProcessInstanceCacheFactory runtimeContext = new ProcessInstanceCacheFactory(this._context);
                SysProcessInstance processInstanceCacheByWorkItem = runtimeContext.GetProcessInstanceCacheByWorkItem(workItemId);
                new ProcessEngine(runtimeContext, processInstanceCacheByWorkItem).RejectApproveWorkItem(workItemId, approveComment, nextActivityId);
            }
            catch (Exception exception)
            {
                AppLogHelper.Error(string.Format("完成【签核流程】的工作项[{0}]失败", workItemId), exception);
                throw exception;
            }
        }

        private static void RemoveRepeatedUsers(List<IUser> users)
        {
            Dictionary<int, IUser> dictionary = new Dictionary<int, IUser>();
            for (int i = users.Count - 1; i >= 0; i--)
            {
                IUser user = users[i];
                if (dictionary.ContainsKey(user.User_ID))
                {
                    users.RemoveAt(i);
                }
                else
                {
                    dictionary.Add(user.User_ID, user);
                }
            }
        }

        private void SaveWorkflowMessage(SysWorkflowMessage msg)
        {
            Trace.Assert(msg.MessageId > 0);
            if (msg.NextApproveUserList.Count > 0)
            {
                foreach (IApproveUser user in msg.NextApproveUserList)
                {
                    MessageApproveUser user2 = new MessageApproveUser(user) {
                        WorkflowMessageId = msg.MessageId,
                        MessageApproveUserId = this._context.GetNextIdentity_Int(false)
                    };
                    this._context.Insert(user2);
                }
            }
            this._context.Insert(msg);
        }

        public int StartProcess(long processId, int startUserId, int relativeObjectId)
        {
            int num2;
            try
            {
                int num;
                ProcessInstanceCacheFactory runtimeContext = new ProcessInstanceCacheFactory(this._context);
                SysProcess processCache = runtimeContext.PCacheFactory.GetProcessCache(processId);
                ProcessEngine engine = new ProcessEngine(runtimeContext, processCache);
                if (processCache.ProcessCategory == 2)
                {
                    num = engine.StartFormProcess(startUserId, relativeObjectId);
                }
                else
                {
                    num = engine.StartProcess(startUserId, relativeObjectId);
                }
                num2 = num;
            }
            catch (Exception exception)
            {
                AppLogHelper.Error(string.Format("启动流程[{0}], obj={1}失败", processId, relativeObjectId), exception);
                throw exception;
            }
            return num2;
        }

        public int StartProcess(string processName, int startUserId, int relativeObjectId)
        {
            int num2;
            try
            {
                int num;
                ProcessInstanceCacheFactory runtimeContext = new ProcessInstanceCacheFactory(this._context);
                SysProcess processCache = runtimeContext.PCacheFactory.GetProcessCache(processName);
                ProcessEngine engine = new ProcessEngine(runtimeContext, processCache);
                if (processCache.ProcessCategory == 2)
                {
                    num = engine.StartFormProcess(startUserId, relativeObjectId);
                }
                else
                {
                    num = engine.StartProcess(startUserId, relativeObjectId);
                }
                num2 = num;
            }
            catch (Exception exception)
            {
                AppLogHelper.Error(string.Format("启动流程[{0}], obj={1}失败", processName, relativeObjectId), exception);
                throw exception;
            }
            return num2;
        }

        public int StartProcess(string processName, int startUserId, int relativeObjectId, List<IApproveUser> nextApproveUserList)
        {
            int num2;
            try
            {
                ProcessInstanceCacheFactory runtimeContext = new ProcessInstanceCacheFactory(this._context);
                SysProcess processCache = runtimeContext.PCacheFactory.GetProcessCache(processName);
                num2 = new ProcessEngine(runtimeContext, processCache).StartProcess(startUserId, relativeObjectId, nextApproveUserList);
            }
            catch (Exception exception)
            {
                AppLogHelper.Error(string.Format("启动流程[{0}], obj={1}失败", processName, relativeObjectId), exception);
                throw exception;
            }
            return num2;
        }

        public int StartProcessAsync(long processId, int startUserId, int relativeObjectId)
        {
            SysProcess processCache = this._processCache.GetProcessCache(processId);
            if (processCache == null)
            {
                string errorInfo = string.Format("未找到流程,id={0}", processId);
                AppLogHelper.Error(startUserId, errorInfo, relativeObjectId.ToString());
                throw new ApplicationException(errorInfo);
            }
            return this.StartProcessAsync(processCache, startUserId, relativeObjectId, null);
        }

        public int StartProcessAsync(string processName, int startUserId, int relativeObjectId)
        {
            return this.StartProcessAsync(processName, startUserId, relativeObjectId, null);
        }

        private int StartProcessAsync(SysProcess process, int startUserId, int relativeObjectId, List<IApproveUser> nextApproveUserList)
        {
            int num3;
            try
            {
                int num = this._context.GetNextIdentity_Int(false);
                SysProcessInstance instance = new SysProcessInstance {
                    StartTime = new DateTime?(DateTime.Now),
                    InstanceStatus = 0,
                    ObjectId = relativeObjectId,
                    ProcessId = new long?(process.ProcessId),
                    ProcessInstanceId = num,
                    StartUserId = new int?(startUserId)
                };
                if (process.ProcessCategory == 2)
                {
                    instance.FormInstanceId = new int?(relativeObjectId);
                    T_User user = this._context.FindById<T_User>(new object[] { startUserId });
                    if (user != null)
                    {
                        T_Department department = this._context.FindById<T_Department>(new object[] { user.Department_ID });
                        if (department != null)
                        {
                            instance.StartDeptId = new int?(department.Department_ID);
                        }
                    }
                }
                int num2 = this._context.GetNextIdentity_Int(false);
                SysWorkflowMessage msg = new SysWorkflowMessage {
                    ProcessId = process.ProcessId,
                    ProcessInstanceId = num,
                    CreateTime = DateTime.Now,
                    MessageId = num2,
                    MessageType = WorkflowMessageTypeEnum.StartingProcess,
                    OperationUserId = startUserId,
                    State = SysWorkflowMessageStateEnum.Inited
                };
                if ((nextApproveUserList != null) && (nextApproveUserList.Count > 0))
                {
                    msg.NextApproveUserList.AddRange(nextApproveUserList);
                }
                this.SaveWorkflowMessage(msg);
                this._context.Insert(instance);
                num3 = num;
            }
            catch (Exception exception)
            {
                AppLogHelper.Error(startUserId, exception, "启动流程[{0}]失败, 流程id={1}, 对象id={2}", new object[] { process.ProcessId, process.ProcessName, relativeObjectId });
                throw;
            }
            return num3;
        }

        public int StartProcessAsync(string processName, int startUserId, int relativeObjectId, List<IApproveUser> nextApproveUserList)
        {
            SysProcess processCache = this._processCache.GetProcessCache(processName);
            if (processCache == null)
            {
                string errorInfo = string.Format("未找到流程name={0}", processName);
                AppLogHelper.Error(startUserId, errorInfo, relativeObjectId.ToString());
                throw new ApplicationException(errorInfo);
            }
            return this.StartProcessAsync(processCache, startUserId, relativeObjectId, nextApproveUserList);
        }

        private void UpdateWorkItemAsCompleting(int workItemId)
        {
            string sql = string.Format("UPDATE SysWorkItem SET Status = {0} WHERE WorkItemId = {1} AND (Status = {2} OR Status IS NULL)", Convert.ToInt32(WorkItemStatus.Completing), workItemId, Convert.ToInt32(WorkItemStatus.Created));
            if (this._context.ExecuteNonQuery(sql, new DbParameter[0]) == 0)
            {
                throw new ApplicationException("工作项不存在或已完成.");
            }
        }
    }
}

