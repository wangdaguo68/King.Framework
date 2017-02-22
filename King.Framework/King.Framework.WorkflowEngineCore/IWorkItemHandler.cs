namespace King.Framework.WorkflowEngineCore
{
    using King.Framework.EntityLibrary;
    using System;

    public interface IWorkItemHandler
    {
        void OnWorkItemCompleted(int wiBaseId, int wiId, string Id);
        string OnWorkItemCreating(T_WorkItemBase wiBase, SysWorkItem wi);
    }
}

