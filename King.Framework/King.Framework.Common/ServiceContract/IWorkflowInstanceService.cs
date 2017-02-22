namespace King.Framework.Common.ServiceContract
{
    using System;
    using System.ServiceModel;

    [ServiceContract]
    public interface IWorkflowInstanceService
    {
        [OperationContract]
        WfWorkflowInstanceData GetWorkflowInstance(int processInstanceId);
    }
}
