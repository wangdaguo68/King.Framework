namespace King.Framework.WorkflowEngineCore.WorkflowMessages
{
    using King.Framework.EntityLibrary;
    using King.Framework.WorkflowEngineCore.InternalCore;
    using System;
    using System.Runtime.CompilerServices;

    internal static class AsyncMessageHandlerHelper
    {
        public static AsyncMessageHandler CreateHandler(this WorkflowMessageTypeEnum msgType, ProcessEngine engine)
        {
            switch (msgType)
            {
                case WorkflowMessageTypeEnum.CompletingWorkItem:
                    return new AsyncMessageHandler4CompletingWorkItem(engine);

                case WorkflowMessageTypeEnum.CompletingApproveWorkItem:
                    return new AsyncMessageHandler4CompletingApproveWorkItem(engine);

                case WorkflowMessageTypeEnum.StartingProcess:
                    return new AsyncMessageHandler4StartingProcess(engine);
            }
            throw new ApplicationException("不支持的消息类型");
        }
    }
}

