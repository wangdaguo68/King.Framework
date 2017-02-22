namespace King.Framework.Common.ServiceContract
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;

    [ServiceContract]
    public interface IWorkflowService
    {
        [OperationContract]
        string Delete(long processId);
        [OperationContract]
        long GetCurrentIdentity(long count);
        [OperationContract]
        IEnumerable<WfDepartment> GetDepartmentList();
        [OperationContract]
        IEnumerable<WfEntity> GetEntityList();
        [OperationContract]
        IEnumerable<WfRemindTemplate> GetRemindTemplateList();
        [OperationContract]
        IEnumerable<WfRole> GetRoleList();
        [OperationContract]
        IEnumerable<WfUser> GetUserList();
        [OperationContract]
        WfWorkflowData GetWorkFlow(long processId);
        [OperationContract]
        IEnumerable<WfProcess> GetWorkFlowList();
        [OperationContract]
        string PublishWorkFlow(WfWorkflowData data, long? oldProcessId);
        [OperationContract]
        string SaveWorkFlow(WfWorkflowData data, long? oldProcessId);
    }
}
