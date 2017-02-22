namespace King.Framework.WorkflowEngineCore.WorkflowMessages
{
    using King.Framework.EntityLibrary;
    using King.Framework.Interfaces;
    using King.Framework.Linq;
    using King.Framework.OrgLibrary;
    using King.Framework.WorkflowEngineCore.InternalCore;
    using King.Framework.WorkflowEngineCore.MessageHandler;
    using System;
    using System.Collections.Generic;

    internal class AsyncMessageHandler4StartingProcess : AsyncMessageHandler
    {
        private readonly ProcessEngine _engine;
        private readonly DataContext dbContext;

        public AsyncMessageHandler4StartingProcess(ProcessEngine engine)
        {
            this._engine = engine;
            this.dbContext = engine.Context;
        }

        public override void ProcessMessage(SysWorkflowMessage msg)
        {
            if (this._engine.ProcessCache.GetProcessCache(msg.ProcessId) == null)
            {
                throw new ApplicationException("process == null");
            }
            SysProcessInstance processInstance = this._engine.PI;
            if (processInstance == null)
            {
                throw new ApplicationException("processInstance == null");
            }
            IOrgProxy proxy = OrgProxyFactory.GetProxy(this.dbContext);
            if (!processInstance.StartUserId.HasValue)
            {
                throw new ApplicationException("processInstance.StartUserId == null");
            }
            IUser userById = proxy.GetUserById(processInstance.StartUserId.Value);
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
            int? startDeptId = processInstance.StartDeptId;
            if ((startDeptId.HasValue ? startDeptId.GetValueOrDefault() : -1) < 0)
            {
                processInstance.StartDeptId = new int?(departmentById.Department_ID);
                this.dbContext.UpdatePartial<SysProcessInstance>(processInstance, p => new { StartDeptId = processInstance.StartDeptId });
            }
            Queue<WorkflowMessage> queue = new Queue<WorkflowMessage>(10);
            WorkflowMessage item = this._engine.NewStartProcessWithNextUsersMessage(msg.NextApproveUserList);
            queue.Enqueue(item);
            while (queue.Count > 0)
            {
                queue.Dequeue().Execute(queue);
            }
            this._engine.ProcessInstanceCache.ClearCache(msg.ProcessInstanceId);
        }
    }
}

