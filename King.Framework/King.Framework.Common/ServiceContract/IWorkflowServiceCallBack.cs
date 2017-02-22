namespace King.Framework.Common.ServiceContract
{
    using System;
    using System.ServiceModel;

    [ServiceContract]
    public interface IWorkflowServiceCallBack
    {
        [OperationContract(IsOneWay=true)]
        void Progress(WfMessage msg);
    }
}
