namespace King.Framework.WorkflowEngineCore.InternalCore
{
    using King.Framework.Common;
    using King.Framework.EntityLibrary;
    using King.Framework.Interfaces;
    using King.Framework.Linq;
    using King.Framework.OrgLibrary;
    using King.Framework.WorkflowEngineCore.Cache;
    using King.Framework.WorkflowEngineCore.MessageHandler;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    internal class ProcessEngine : IDisposable
    {
        private readonly DataContext _context;
        private DynamicManager _manager;
        private SysProcessInstance _pi;
        private ProcessInstanceCacheFactory _piCacheFactory;
        private readonly SysProcess _process;
        private SysWorkItem _wi;

        internal ProcessEngine(ProcessInstanceCacheFactory runtimeContext, SysProcess process)
        {
            this._context = runtimeContext.Context;
            this._process = process;
            this._piCacheFactory = runtimeContext;
            this._manager = this._piCacheFactory.Manager;
            this._pi = null;
        }

        internal ProcessEngine(ProcessInstanceCacheFactory runtimeContext, SysProcessInstance processInstance)
        {
            if (processInstance.Process == null)
            {
                processInstance.Process = runtimeContext.PCacheFactory.GetProcessCache(processInstance.ProcessId.Value);
            }
            this._context = runtimeContext.Context;
            this._process = processInstance.Process;
            this._piCacheFactory = runtimeContext;
            this._manager = this._piCacheFactory.Manager;
            this._pi = processInstance;
        }

        internal ProcessEngine(ProcessInstanceCacheFactory runtimeContext, SysProcessInstance processInstance, SysWorkItem currentWorkItem) : this(runtimeContext, processInstance)
        {
            this._wi = currentWorkItem;
        }

        internal void AddInstanceData(SysWorkItem work_item, ApproveResultEnum approveResult, string approveComment)
        {
            SysApproveActivityData data = new SysApproveActivityData {
                DataId = work_item.WorkItemId,
                ActivityInstanceId = work_item.ActivityInstanceId,
                AddingUserId = work_item.AddingUserId,
                IsAdded = work_item.IsAdded,
                IsProxy = work_item.IsProxy,
                ProxyUserId = work_item.ProxyUserId,
                WorkItemId = new int?(work_item.WorkItemId),
                ApproveComment = approveComment,
                ApproveGroupId = work_item.ApproveGroupId,
                ApproveResult = new int?((int) approveResult),
                ApproveTime = new DateTime?(DateTime.Now),
                ApproveUserId = work_item.OwnerId
            };
            this._piCacheFactory.AddApproveData(data);
        }

        internal virtual void CancelProcess(int processInstanceId)
        {
            this._pi = this._piCacheFactory.GetProcessInstanceCache(processInstanceId);
            Queue<WorkflowMessage> queue = new Queue<WorkflowMessage>(10);
            WorkflowMessage item = this.NewCancelProcessMessage(this._pi);
            queue.Enqueue(item);
            while (queue.Count > 0)
            {
                queue.Dequeue().Execute(queue);
            }
            this._piCacheFactory.ClearCache(processInstanceId);
        }

        internal virtual void CompleteApproveWorkItem(int workItemId, ApproveResultEnum approveResult, string approveComment, bool addUser = false, int? addUserId = new int?())
        {
            SysWorkItem item;
            if (addUser && (!addUserId.HasValue || (addUserId <= 0)))
            {
                throw new ApplicationException("指定了加签，但未指定加签用户");
            }
            this._pi = this._piCacheFactory.GetProcessInstanceCacheByWorkItem(workItemId, out item);
            if (!item.Status.HasValue)
            {
                item.Status = 0;
            }
            if (item.Status.Value == 1)
            {
                throw new ApplicationException("该工作项已完成");
            }
            if (item.Status.Value == 3)
            {
                throw new ApplicationException("该工作项已被系统取消");
            }
            if (item.Status.Value != 0)
            {
                throw new ApplicationException("该工作项已完成或被取消");
            }
            this.AddInstanceData(item, approveResult, approveComment);
            if (addUser && (approveResult == ApproveResultEnum.Pass))
            {
                this.CreateAddingWorkItem(item, addUserId.Value);
            }
            Queue<WorkflowMessage> queue = new Queue<WorkflowMessage>(10);
            WorkflowMessage message = this.NewCompleteWorkItemApproveMessage(item);
            queue.Enqueue(message);
            while (queue.Count > 0)
            {
                queue.Dequeue().Execute(queue);
            }
            this._piCacheFactory.ClearCache(this._pi.ProcessInstanceId);
        }

        internal void CompleteApproveWorkItem(int workItemId, ApproveResultEnum approveResult, string approveComment, List<IApproveUser> nextApproveUserList, bool addUser = false, int? addUserId = new int?())
        {
            SysWorkItem item;
            if (addUser && (!addUserId.HasValue || (addUserId <= 0)))
            {
                throw new ApplicationException("指定了加签，但未指定加签用户");
            }
            this._pi = this._piCacheFactory.GetProcessInstanceCacheByWorkItem(workItemId, out item);
            if (!item.Status.HasValue)
            {
                item.Status = 0;
            }
            if (item.Status.Value == 1)
            {
                throw new ApplicationException("该工作项已完成");
            }
            if (item.Status.Value == 3)
            {
                throw new ApplicationException("该工作项已被系统取消");
            }
            if (item.Status.Value != 0)
            {
                throw new ApplicationException("该工作项已完成或被取消");
            }
            this.AddInstanceData(item, approveResult, approveComment);
            if (addUser && (approveResult == ApproveResultEnum.Pass))
            {
                this.CreateAddingWorkItem(item, addUserId.Value);
            }
            Queue<WorkflowMessage> queue = new Queue<WorkflowMessage>(10);
            WorkflowMessage message = this.NewCompleteWorkItemWithNextUsersApproveMessage(item, nextApproveUserList);
            queue.Enqueue(message);
            while (queue.Count > 0)
            {
                queue.Dequeue().Execute(queue);
            }
            this._piCacheFactory.ClearCache(this._pi.ProcessInstanceId);
        }

        internal void CompleteApproveWorkItemSelf(int workItemId, ApproveResultEnum approveResult, string approveComment, bool addUser = false, int? addUserId = new int?())
        {
            SysWorkItem item;
            if (addUser && (!addUserId.HasValue || (addUserId <= 0)))
            {
                throw new ApplicationException("指定了加签，但未指定加签用户");
            }
            this._pi = this._piCacheFactory.GetProcessInstanceCacheByWorkItem(workItemId, out item);
            if (!item.Status.HasValue)
            {
                item.Status = 0;
            }
            if (item.Status.Value == 1)
            {
                throw new ApplicationException("该工作项已完成");
            }
            if (item.Status.Value == 3)
            {
                throw new ApplicationException("该工作项已被系统取消");
            }
            if (item.Status.Value != 0)
            {
                throw new ApplicationException("该工作项已完成或被取消");
            }
            this.AddInstanceData(item, approveResult, approveComment);
            if (addUser && (approveResult == ApproveResultEnum.Pass))
            {
                this.CreateAddingWorkItem(item, addUserId.Value);
            }
            Queue<WorkflowMessage> queue = new Queue<WorkflowMessage>(10);
            WorkflowMessage message = this.NewCompleteWorkItemSelfApproveMessage(item);
            queue.Enqueue(message);
            while (queue.Count > 0)
            {
                queue.Dequeue().Execute(queue);
            }
            this._piCacheFactory.ClearCache(this._pi.ProcessInstanceId);
        }

        internal virtual void CompleteWorkItem(int workItemId)
        {
            SysWorkItem item;
            this._pi = this._piCacheFactory.GetProcessInstanceCacheByWorkItem(workItemId, out item);
            if (!item.Status.HasValue)
            {
                item.Status = 0;
            }
            if (item.Status.Value != 0)
            {
                throw new ApplicationException("该工作项已完成或被取消");
            }
            Queue<WorkflowMessage> queue = new Queue<WorkflowMessage>(10);
            WorkflowMessage message = this.NewCompleteWorkItemMessage(item);
            queue.Enqueue(message);
            while (queue.Count > 0)
            {
                queue.Dequeue().Execute(queue);
            }
            this._piCacheFactory.ClearCache(this._pi.ProcessInstanceId);
        }

        internal void CompleteWorkItem(int workItemId, List<IApproveUser> nextApproveUserList)
        {
            SysWorkItem item;
            this._pi = this._piCacheFactory.GetProcessInstanceCacheByWorkItem(workItemId, out item);
            if (!item.Status.HasValue)
            {
                item.Status = 0;
            }
            if (item.Status.Value != 0)
            {
                throw new ApplicationException("该工作项已完成或被取消");
            }
            Queue<WorkflowMessage> queue = new Queue<WorkflowMessage>(10);
            WorkflowMessage message = this.NewCompleteWorkItemWithNextUsersMessage(item, nextApproveUserList);
            queue.Enqueue(message);
            while (queue.Count > 0)
            {
                queue.Dequeue().Execute(queue);
            }
            this._piCacheFactory.ClearCache(this._pi.ProcessInstanceId);
        }

        internal void CompleteWorkItemSelf(int workItemId)
        {
            SysWorkItem item;
            this._pi = this._piCacheFactory.GetProcessInstanceCacheByWorkItem(workItemId, out item);
            if (!item.Status.HasValue)
            {
                item.Status = 0;
            }
            if (item.Status.Value != 0)
            {
                throw new ApplicationException("该工作项已完成或被取消");
            }
            Queue<WorkflowMessage> queue = new Queue<WorkflowMessage>(10);
            WorkflowMessage message = this.NewCompleteWorkItemSelfMessage(item);
            queue.Enqueue(message);
            while (queue.Count > 0)
            {
                queue.Dequeue().Execute(queue);
            }
            this._piCacheFactory.ClearCache(this._pi.ProcessInstanceId);
        }

        internal void CreateAddingWorkItem(SysWorkItem work_item, int addUserId)
        {
            SysWorkItem wi = new SysWorkItem {
                ProcessInstanceId = work_item.ProcessInstanceId,
                ActivityInstanceId = work_item.ActivityInstanceId,
                ActivityParticipantId = null,
                ParticipantId = null,
                IsAdded = true,
                ProxyUserId = work_item.OwnerId,
                ProxyWorkItemId = new int?(work_item.WorkItemId),
                AddingUserId = work_item.OwnerId,
                ApproveGroupId = work_item.ApproveGroupId,
                CreateTime = new DateTime?(DateTime.Now),
                EndTime = null,
                IsProxy = false,
                OwnerId = new int?(addUserId),
                RelativeObjectId = work_item.RelativeObjectId,
                Status = 0,
                WorkItemType = 0,
                WwfBookmarkId = null
            };
            this._piCacheFactory.AddWorkItem(wi, this.PI, work_item.ActivityInstance);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (this._pi != null)
            {
                this._piCacheFactory.ClearCache(this._pi.ProcessInstanceId);
                this._pi = null;
            }
        }

        ~ProcessEngine()
        {
            this.Dispose(false);
        }

        internal virtual WorkflowMessage NewActivityInstanceCreatedMessage(SysActivityInstance ai)
        {
            return new ActivityInstanceCreatedMessage(this, ai);
        }

        internal virtual WorkflowMessage NewCancelProcessMessage(SysProcessInstance pi)
        {
            return new CancelProcessMessage(this, pi);
        }

        internal virtual WorkflowMessage NewCompleteActivityMessage(SysActivityInstance ai)
        {
            return new CompleteActivityMessage(this, ai);
        }

        internal virtual WorkflowMessage NewCompleteActivityWithNextActivityMessage(SysActivityInstance ai, long nextActivityId)
        {
            return new CompleteActivityWithNextActivityMessage(this, ai, nextActivityId);
        }

        internal virtual WorkflowMessage NewCompleteWorkItemApproveMessage(SysWorkItem wi)
        {
            return new CompleteWorkItemMessage_Approve(this, wi);
        }

        internal virtual WorkflowMessage NewCompleteWorkItemMessage(SysWorkItem wi)
        {
            return new CompleteWorkItemMessage(this, wi);
        }

        internal virtual WorkflowMessage NewCompleteWorkItemSelfApproveMessage(SysWorkItem wi)
        {
            return new CompleteWorkItemSelfMessage_Approve(this, wi);
        }

        internal virtual WorkflowMessage NewCompleteWorkItemSelfMessage(SysWorkItem wi)
        {
            return new CompleteWorkItemSelfMessage(this, wi);
        }

        internal virtual WorkflowMessage NewCompleteWorkItemWithNextUsersApproveMessage(SysWorkItem wi, List<IApproveUser> approveUsers)
        {
            return new CompleteWorkItemWithNextUsersMessage_Approve(this, wi, approveUsers);
        }

        internal virtual WorkflowMessage NewCompleteWorkItemWithNextUsersMessage(SysWorkItem wi, List<IApproveUser> approveUsers)
        {
            return new CompleteWorkItemWithNextUsersMessage(this, wi, approveUsers);
        }

        internal virtual WorkflowMessage NewRejectWorkItemWithNextActivityApproveMessage(SysWorkItem wi, long nextActivityId)
        {
            return new RejectWorkItemWithNextActivityMessage_Approve(this, wi, nextActivityId);
        }

        internal virtual WorkflowMessage NewStartProcessMessage()
        {
            return new StartProcessMessage(this);
        }

        internal virtual WorkflowMessage NewStartProcessWithNextUsersMessage(List<IApproveUser> approveUsers)
        {
            return new StartProcessWithNextUsersMessage(this, approveUsers);
        }

        internal void RejectApproveWorkItem(int workItemId, string approveComment, long nextActivityId)
        {
            SysWorkItem item;
            this._pi = this._piCacheFactory.GetProcessInstanceCacheByWorkItem(workItemId, out item);
            if (!item.Status.HasValue)
            {
                item.Status = 0;
            }
            if (item.Status.Value == 1)
            {
                throw new ApplicationException("该工作项已完成");
            }
            if (item.Status.Value == 3)
            {
                throw new ApplicationException("该工作项已被系统取消");
            }
            if (item.Status.Value != 0)
            {
                throw new ApplicationException("该工作项已完成或被取消");
            }
            this.AddInstanceData(item, ApproveResultEnum.Reject, approveComment);
            Queue<WorkflowMessage> queue = new Queue<WorkflowMessage>(10);
            WorkflowMessage message = this.NewRejectWorkItemWithNextActivityApproveMessage(item, nextActivityId);
            queue.Enqueue(message);
            while (queue.Count > 0)
            {
                queue.Dequeue().Execute(queue);
            }
            this._piCacheFactory.ClearCache(this._pi.ProcessInstanceId);
        }

        internal virtual int StartFormProcess(int startUserId, int formInstanceId)
        {
            IOrgProxy proxy = OrgProxyFactory.GetProxy(this._context);
            int nextIdentity = this._manager.GetNextIdentity();
            IUser userById = proxy.GetUserById(startUserId);
            if (userById == null)
            {
                throw new ApplicationException("用户不存在");
            }
            if (!userById.Department_ID.HasValue)
            {
                throw new ApplicationException("未指定用户部门");
            }
            IDepartment departmentById = proxy.GetDepartmentById(userById.Department_ID.Value);
            if (departmentById == null)
            {
                throw new ApplicationException("用户部门无效");
            }
            SysFormInstance fi = this._context.FindById<SysFormInstance>(new object[] { formInstanceId });
            if (fi == null)
            {
                throw new ApplicationException("表单实例不存在");
            }
            if (!fi.ObjectId.HasValue)
            {
                throw new ApplicationException("表单实例未关联对象ID");
            }
            fi.State = 1;
            this.ProcessInstanceCache.UpdateFormInstance(fi);
            SysProcessInstance instance2 = new SysProcessInstance {
                StartTime = new DateTime?(DateTime.Now),
                EndTime = null,
                InstanceStatus = 0,
                ObjectId = fi.ObjectId.Value,
                FormInstanceId = new int?(fi.FormInstanceId),
                FormInstance = fi,
                ProcessId = new long?(this._process.ProcessId),
                ProcessInstanceId = nextIdentity,
                StartUserId = new int?(startUserId),
                StartDeptId = new int?(departmentById.Department_ID),
                WwfInstanceId = null
            };
            this._pi = instance2;
            this._pi.FormInstance = fi;
            this._piCacheFactory.AddProcessInstance(this._pi);
            Queue<WorkflowMessage> queue = new Queue<WorkflowMessage>(10);
            WorkflowMessage item = this.NewStartProcessMessage();
            queue.Enqueue(item);
            while (queue.Count > 0)
            {
                queue.Dequeue().Execute(queue);
            }
            this._piCacheFactory.ClearCache(nextIdentity);
            return nextIdentity;
        }

        internal virtual int StartProcess(int startUserId, int relativeObjectId)
        {
            IOrgProxy proxy = OrgProxyFactory.GetProxy(this._context);
            int nextIdentity = this._manager.GetNextIdentity();
            IUser userById = proxy.GetUserById(startUserId);
            if (userById == null)
            {
                throw new ApplicationException("用户不存在");
            }
            if (!userById.Department_ID.HasValue)
            {
                throw new ApplicationException("未指定用户部门");
            }
            IDepartment departmentById = proxy.GetDepartmentById(userById.Department_ID.Value);
            if (departmentById == null)
            {
                throw new ApplicationException("用户部门无效");
            }
            SysProcessInstance instance = new SysProcessInstance {
                StartTime = new DateTime?(DateTime.Now),
                EndTime = null,
                InstanceStatus = 0,
                ObjectId = relativeObjectId,
                ProcessId = new long?(this._process.ProcessId),
                ProcessInstanceId = nextIdentity,
                StartUserId = new int?(startUserId),
                StartDeptId = new int?(departmentById.Department_ID),
                WwfInstanceId = null
            };
            this._pi = instance;
            this._piCacheFactory.AddProcessInstance(this._pi);
            Queue<WorkflowMessage> queue = new Queue<WorkflowMessage>(10);
            WorkflowMessage item = this.NewStartProcessMessage();
            queue.Enqueue(item);
            while (queue.Count > 0)
            {
                queue.Dequeue().Execute(queue);
            }
            this._piCacheFactory.ClearCache(nextIdentity);
            return nextIdentity;
        }

        internal virtual int StartProcess(int startUserId, int relativeObjectId, List<IApproveUser> nextApproveUserList)
        {
            IOrgProxy proxy = OrgProxyFactory.GetProxy(this._context);
            int nextIdentity = this._manager.GetNextIdentity();
            IUser userById = proxy.GetUserById(startUserId);
            if (userById == null)
            {
                throw new ApplicationException("用户不存在");
            }
            if (!userById.Department_ID.HasValue)
            {
                throw new ApplicationException("未指定用户部门");
            }
            IDepartment departmentById = proxy.GetDepartmentById(userById.Department_ID.Value);
            if (departmentById == null)
            {
                throw new ApplicationException("用户部门无效");
            }
            SysProcessInstance instance = new SysProcessInstance {
                StartTime = new DateTime?(DateTime.Now),
                EndTime = null,
                InstanceStatus = 0,
                ObjectId = relativeObjectId,
                ProcessId = new long?(this._process.ProcessId),
                ProcessInstanceId = nextIdentity,
                StartUserId = new int?(startUserId),
                StartDeptId = new int?(departmentById.Department_ID),
                WwfInstanceId = null
            };
            this._pi = instance;
            this._piCacheFactory.AddProcessInstance(this._pi);
            Queue<WorkflowMessage> queue = new Queue<WorkflowMessage>(10);
            WorkflowMessage item = this.NewStartProcessWithNextUsersMessage(nextApproveUserList);
            queue.Enqueue(item);
            while (queue.Count > 0)
            {
                queue.Dequeue().Execute(queue);
            }
            this._piCacheFactory.ClearCache(nextIdentity);
            return nextIdentity;
        }

        internal virtual int StartProcessAsync(int startUserId, int relativeObjectId, List<IApproveUser> nextApproveUserList)
        {
            using (BizDataContext context = new BizDataContext(true))
            {
                SysProcessInstance instance = new SysProcessInstance {
                    StartTime = new DateTime?(DateTime.Now),
                    InstanceStatus = 0,
                    ObjectId = relativeObjectId,
                    ProcessId = new long?(this._process.ProcessId),
                    ProcessInstanceId = context.GetNextIdentity_Int(false),
                    StartUserId = new int?(startUserId)
                };
                this._pi = instance;
                SysWorkflowMessage message = new SysWorkflowMessage {
                    ProcessId = this._process.ProcessId,
                    ProcessInstanceId = this._pi.ProcessInstanceId,
                    CreateTime = DateTime.Now,
                    MessageId = context.GetNextIdentity_Int(false),
                    MessageType = WorkflowMessageTypeEnum.StartingProcess,
                    OperationUserId = startUserId,
                    State = SysWorkflowMessageStateEnum.Inited
                };
                foreach (IApproveUser user in nextApproveUserList)
                {
                    MessageApproveUser user2 = new MessageApproveUser(user) {
                        //WorkflowMessageId = message.MessageId,
                        WorkflowMessageId = context.GetNextIdentity_Int(false)
                    };
                    context.Insert(user2);
                }
                context.Insert(message);
                context.Insert(this._pi);
            }
            return this._pi.ProcessInstanceId;
        }

        internal DataContext Context
        {
            get
            {
                return this._context;
            }
        }

        internal DynamicManager Manager
        {
            get
            {
                return this._manager;
            }
        }

        internal SysProcessInstance PI
        {
            get
            {
                return this._pi;
            }
        }

        internal SysProcess Process
        {
            get
            {
                return this._process;
            }
        }

        internal ProcessCacheFactory ProcessCache
        {
            get
            {
                return this._piCacheFactory.PCacheFactory;
            }
        }

        internal ProcessInstanceCacheFactory ProcessInstanceCache
        {
            get
            {
                return this._piCacheFactory;
            }
        }

        internal SysWorkItem WI
        {
            get
            {
                return this._wi;
            }
        }
    }
}

